using KirboMod.Items;
using KirboMod.Items.Nightmare;
using KirboMod.Projectiles.NightmareLightningOrb;
using KirboMod.Systems;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
    public partial class NightmareWizard : ModNPC
    {
        public override string HeadTexture => "KirboMod/NPCs/Nightmare/NightmareWizard_Head_Boss";
        public override string Texture => "KirboMod/NPCs/Nightmare/NightmareWizard";
        public static SoundStyle BodyStarSFX => new SoundStyle("KirboMod/Sounds/NPC/Nightmare/NightmareBodyStar").WithVolumeScale(0.7f);
        public static SoundStyle TeleportSFX => new("KirboMod/Sounds/NPC/Nightmare/NightmareTeleport");
        public static SoundStyle DashSFX => new("KirboMod/Sounds/NPC/Nightmare/NightmareWizardDash2");
        public static SoundStyle DashStartSFX => new("KirboMod/Sounds/NPC/Nightmare/NightmareWizardDashStart2");
        public static SoundStyle StarShotSFX => new("KirboMod/Sounds/NPC/Nightmare/WizardStarShot");
        public static SoundStyle OrbSpawnSFX => new("KirboMod/Sounds/NPC/Nightmare/NightmareSpawnOrb");
        enum NightmareAttackType : byte
        {
            SpreadStars,//1
            RingStars,//2
            Swoop,//3
            Tornado,//4
            Stoop,//5
            LightningOrbsPentagon,
            LightningOrbsHoming
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Nightmare");
            Main.npcFrameCount[NPC.type] = 25;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new()
            {
                PortraitScale = 1f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 70f,
                Position = new Vector2(0, 80),
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true; //immune to not mess up movement

        }

        public override void SetDefaults()
        {
            NPC.width = 114;
            NPC.height = 114;
            DrawOffsetY = 20;
            NPC.damage = 70;
            NPC.noTileCollide = true;
            NPC.lifeMax = 13000;
            NPC.defense = 20;
            NPC.HitSound = SoundID.NPCHit2; //bone
            NPC.DeathSound = SoundID.NPCDeath2; //undead
            NPC.value = Item.buyPrice(0, 5, 0, 0); // money it drops
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.friendly = false;
            NPC.dontTakeDamage = true;
            NPC.npcSlots = 6;
            NPC.buffImmune[BuffID.Confused] = true;
            if (!Main.dedServ)//if not dedicated server
            {
                int musicSlot = MusicLoader.GetMusicSlot("KirboMod/Music/NightmareWizardWithLoopMetadata");
                Music = musicSlot;
                Main.musicFade[musicSlot] = 1;
                Main.musicNoCrossFade[musicSlot] = true;

                musicSlot = MusicLoader.GetMusicSlot("KirboMod/Music/Photonic0_NightmareOrb");
                Main.musicFade[musicSlot] = 0;

            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            Helper.BossHpScalingForHigherDifficulty(ref NPC.lifeMax, balance);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("An evil wizard bent on spreading nightmares to all that can dream. Its magic can twist the mind and body of those who are cursed with it.")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(animation);
            writer.Write(attack);
            writer.Write(despawntimer);
            writer.Write((byte)attacktype);
            writer.Write((byte)lastattacktype);

            writer.Write((byte)phase);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            animation = reader.ReadInt32();
            attack = reader.ReadInt32();
            despawntimer = reader.ReadInt32();
            attacktype = (NightmareAttackType)reader.ReadByte();
            lastattacktype = (NightmareAttackType)reader.ReadByte();

            phase = reader.ReadByte();
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedNightmareBoss, -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<NightmareBag>())); //only drops in expert

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert()); //checks if not expert
            LeadingConditionRule masterMode = new(new Conditions.IsMasterMode()); //checks if master mode

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<NightCloth>(), 1, 15, 15));

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<NightmareMask>(), 7));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<NightmareTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeables.BossRelics.NightmareRelic>()));

            masterMode.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.Nightmare.NightmarePetItem>(), 4));

            // add the rules
            npcLoot.Add(notExpertRule);
            npcLoot.Add(masterMode);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "Nightmare"; //_ has been defeated!
            potionType = ItemID.GreaterHealingPotion; //potion it drops
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                int lightningOrb1 = ModContent.ProjectileType<NightmareLightningOrb>();
                int lightningOrb2 = ModContent.ProjectileType<NightmareLightningOrbHoming>();
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile projectile = Main.projectile[i];
                    if (projectile.active && (projectile.type == lightningOrb2 || projectile.type == lightningOrb1))
                    {
                        projectile.Kill();
                    }
                }
            }
            if (DeathCounter >= DeathAnimDuration && NPC.life <= 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    // go around in a octogonal pattern
                    Vector2 speed = new((float)Math.Cos(MathHelper.ToRadians(i * 45)) * 20, (float)Math.Sin(MathHelper.ToRadians(i * 45)) * 20);

                    Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.NightStar>(), speed, Scale: 3f); //Makes dust in an octogonal formation
                    d.noGravity = true;
                }

                for (int i = 0; i < 20; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
                    Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, speed, Main.rand.Next(11, 13), Scale: 2f); //double jump smoke
                }
            }
        }
        public static void PlayTeleportSoundEffect(Vector2 pos)
        {
            SoundEngine.PlaySound(TeleportSFX, pos);
        }
        void PlayTeleportSoundEffect()
        {
            SoundEngine.PlaySound(TeleportSFX, NPC.Center);
        }
        public static void PlayBodyStarSoundEffect(Vector2 pos)
        {
            SoundEngine.PlaySound(BodyStarSFX, pos);
        }
        void PlayBodyStarSoundEffect()
        {
            SoundEngine.PlaySound(BodyStarSFX, NPC.Center);
        }
        void PlayStarShotSoundEffect()
        {
            SoundEngine.PlaySound(StarShotSFX.WithVolumeScale(0.7f), NPC.Center);
        }
        void PlayOrbSpawnSoundEffect()
        {
            SoundEngine.PlaySound(OrbSpawnSFX with { MaxInstances = 0 }, NPC.Center);
        }
        void PlayDashStartSFX()
        {
            SoundEngine.PlaySound(DashStartSFX, NPC.Center);
        }
        void PlayDashSFX()
        {
            SoundEngine.PlaySound(DashSFX, NPC.Center);
        }  
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; // Makes it uneffected by light
        }
    }
}