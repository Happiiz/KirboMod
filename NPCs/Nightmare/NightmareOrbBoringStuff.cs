using KirboMod.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
    [AutoloadBossHead]
    public partial class NightmareOrb : ModNPC
    {
        public static SoundStyle StarThrowSFX => new("KirboMod/Sounds/NPC/Nightmare/StarThrow5");
        public static SoundStyle StarSpreadShotSFX => new("KirboMod/Sounds/NPC/Nightmare/NightmareStarSpreadShot");
        public static SoundStyle SlashBeamSFX => new("KirboMod/Sounds/NPC/Nightmare/NightmareSlashShot5");
        public static SoundStyle DashSFX => new("KirboMod/Sounds/NPC/Nightmare/NightmareDash");
        public static SoundStyle FirstHitSFX => new("KirboMod/Sounds/NPC/Nightmare/NightmareOrbStart");
        public static SoundStyle DashChargeSFXSlow => new("KirboMod/Sounds/NPC/Nightmare/ChargeUpSoundSlow");
        public static SoundStyle DashChargeSFXMedium => new("KirboMod/Sounds/NPC/Nightmare/ChargeUpSoundMedium");
        public bool DoneFirstHit { get => NPC.soundDelay != 0; set => NPC.soundDelay = value ? int.MaxValue : 0; }
        enum NightmareOrbAtkType
        {
            DecideNext = -1,
            Spawn = 0,
            SingleStar = 5,
            SlashBeam = 1,
            TripleStar = 2,
            HomingStar = 3,
            Dash = 4
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Power Orb");
            Main.npcFrameCount[NPC.type] = 4;

            //Needed for multiplayer spawning to work
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true; //immune to not mess up movement
        }

        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 100;
            NPC.damage = 70; //initally
            NPC.noTileCollide = true;
            NPC.defense = 30;
            NPC.lifeMax = 12000;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14; //explosive metal
            NPC.value = 0f; // money it drops
            NPC.knockBackResist = 0f;
            Banner = 0;
            BannerItem = Item.BannerToItem(Banner);
            NPC.aiStyle = -1;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            Music = MusicID.Boss3;
            NPC.npcSlots = 6;
            NPC.dontTakeDamage = true;
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
				new FlavorTextBestiaryInfoElement("An ominous orb that appeared from the fountain after being attracted to it's magic. Acts as the shield for a mysterious yet diabolical sorcerer.")
            });
        }
        public override void FindFrame(int frameHeight) // animation
        {
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter < 4.0)
            {
                NPC.frame.Y = 0;
            }
            else if (NPC.frameCounter < 8.0)
            {
                NPC.frame.Y = frameHeight;
            }
            else if (NPC.frameCounter < 12.0)
            {
                NPC.frame.Y = frameHeight * 2;
            }
            else if (NPC.frameCounter < 16.0)
            {
                NPC.frame.Y = frameHeight * 3;
            }
            else
            {
                NPC.frameCounter = 0.0;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; // Makes it uneffected by light            
        }

        public override bool PreKill()
        {
            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (!DoneFirstHit)
            {
                DoneFirstHit = true;
                PlayFirstHitSFX();
                AttackType = NightmareOrbAtkType.DecideNext;
                EndAttack();
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 spawnVel = NightmareOrbFirstHitShine.GetSpawnVelocity(hit.HitDirection);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.width / -2 * hit.HitDirection, 0), spawnVel, ModContent.ProjectileType<NightmareOrbFirstHitShine>(), -1, 0);
                }
            }
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 10; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                {
                    Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
                    Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.NightStar>(), speed, Scale: 1.5f); //Makes dust in a messy circle
                    d.noGravity = true;
                }
                for (int i = 0; i < 10; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                {
                    Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                    Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, speed, Main.rand.Next(11, 13), Scale: 1.5f); //double jump smoke
                }
            }
        }
        public static void PlayStarThrowSFX(Vector2 pos)
        {
            SoundEngine.PlaySound(StarThrowSFX, pos);
        }
        void PlayStarThrowSFX()
        {
            SoundEngine.PlaySound(StarThrowSFX, NPC.Center);
        }
        public static void PlaySpreadShotSFX(Vector2 pos)
        {
            SoundEngine.PlaySound(StarSpreadShotSFX, pos);
        }
        void PlaySpreadShotSFX()
        {
            SoundEngine.PlaySound(StarSpreadShotSFX, NPC.Center);
        }
        void PlaySlashBeamSFX()
        {
            SoundEngine.PlaySound(SlashBeamSFX, NPC.Center);
        }
        void PlayDashSFX()
        {
            SoundEngine.PlaySound(DashSFX, NPC.Center);
        }
        void PlayFirstHitSFX()
        {
            SoundEngine.PlaySound(FirstHitSFX, NPC.Center);
        }
        void PlayDashChargeSFXMedium()
        {
            SoundEngine.PlaySound(DashChargeSFXMedium, NPC.Center);
        }
        void PlayDashChargeSFXSlow()
        {
            SoundEngine.PlaySound(DashChargeSFXSlow, NPC.Center, ChargeUpdateCallback);
        }    
        bool ChargeUpdateCallback(ActiveSound soundInstance)
        {
            if (Main.gamePaused)
            {
                soundInstance.Pause();
            }
            else
            {
                soundInstance.Resume();
            }

            //sound effect stops when dash starts
            return NPC.ai[0] < GetDashTime();
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return true;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return AttackType != NightmareOrbAtkType.Spawn;
        }
        public override bool CheckDead()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.SpawnBoss((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<NightmareWizard>(), Main.player[NPC.target].whoAmI); //different from SpawnOnPlayer()
            }

            return true;
        }
    }
}
