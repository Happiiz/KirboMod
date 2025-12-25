using KirboMod.Bestiary;
using KirboMod.Dusts.MarxSparks;
using KirboMod.Items;
using KirboMod.Items.Accesories.Wings;
using KirboMod.Systems;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace KirboMod.NPCs.Marx
{
    public partial class MarxBoss : ModNPC
    {
        //public override string HeadTexture => "KirboMod/NPCs/Nightmare/NightmareWizard_Head_Boss";

        public override string Texture => "KirboMod/NPCs/Marx/Marx";
        enum AttackType : byte
        {
            DecideNext,
            Teleport,
            Cutter,
            Vine,
            IceBomb,
            MassiveLaser,
            BlackHole,
            Intro,
            DashFromBelow,
            TeleportFrenzy,
        }

        enum Animation : byte
        {
            Idle,
            Rise,
            Charge,
            Spit,
            Blast,
            Cutter,
            TeleportOut,
            TeleportIn,
            Split,
            Intro,
            PuffUp,
            ShadowHole,
            Defeat
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 25;
            NPCID.Sets.NeedsExpertScaling[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new()
            {
                PortraitScale = 1f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                //PortraitPositionYOverride = 70f,
                //Position = new Vector2(0, 80),
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true; //immune to not mess up movement
        }
        public static int MaxHP => 56000;
        public static int Defense => 40;
        public override void SetDefaults()
        {
            NPC.width = 400;
            NPC.height = 120;
            DrawOffsetY = 54;
            NPC.damage = 70;
            NPC.noTileCollide = true;
            NPC.friendly = false;
            NPC.lifeMax = MaxHP;
            NPC.defense = Defense;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = Item.buyPrice(0, 15, 0, 0);
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.npcSlots = 8;
            if (!Main.dedServ)//if not dedicated server
            {
                int musicSlot = MusicLoader.GetMusicSlot("KirboMod/Music/DeathZ_Marx");
                Music = musicSlot;
                Main.musicFade[musicSlot] = 1;
                Main.musicNoCrossFade[musicSlot] = true;

            }
            MarxWingRenderer.Initialize();
            wingRenderer = new();
            MarxSparks.LoadTextureIfNeeded();
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            Helper.BossHpScalingForHigherDifficulty(ref NPC.lifeMax, balance);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(
            [
                new SurfaceBackgroundProvider(),
				new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.KirboMod.NPCs.Bestiary.MarxBoss"))
            ]);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((byte)attacktype);
            writer.Write((byte)lastattacktype);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            attacktype = (AttackType)reader.ReadByte();
            lastattacktype = (AttackType)reader.ReadByte();
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedMarxBoss, -1);

            MarxSpawningSystem.MarxActive = false; //Marx Townie can spawn again
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<Items.Marx.MarxBag>())); //only drops in expert

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert()); //checks if not expert
            LeadingConditionRule masterMode = new(new Conditions.IsMasterMode()); //checks if master mode

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SoulMatter>(), 1, 20, 20));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<MarxWings>(), 10));

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.Marx.MarxMask>(), 7));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Marx.MarxTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeables.BossRelics.MarxRelicItem>()));

            masterMode.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.Marx.MarxPetItem>(), 4));

            // add the rules
            npcLoot.Add(notExpertRule);
            npcLoot.Add(masterMode);
        }

        public override void BossLoot(ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion; //potion it drops
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return true;
        }

        public override void ModifyHoverBoundingBox(ref Rectangle boundingBox) 
        {
            if (attacktype == AttackType.BlackHole || (attacktype == AttackType.DashFromBelow 
                && AttackTimer < DashFromBelowChaseDuration + DashFromBelowTelegraphDuration))
            {
                boundingBox = Rectangle.Empty;
            }
            else
            {
                boundingBox = NPC.Hitbox;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    // go around in a octogonal pattern
                    Vector2 speed = new((float)Math.Cos(MathHelper.ToRadians(i * 45)) * 25, (float)Math.Sin(MathHelper.ToRadians(i * 45)) * 25);

                    Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.BoldStar>(), speed, Scale: 3f); //Makes dust in a messy circle
                    d.noGravity = true;
                }
                for (int i = 0; i < 20; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
                    Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, speed, Main.rand.Next(11, 13), Scale: 2f); //double jump smoke
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; // Makes it uneffected by light
        }
    }
}