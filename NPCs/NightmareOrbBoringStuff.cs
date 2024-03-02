using KirboMod.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
	[AutoloadBossHead]
	public partial class NightmareOrb : ModNPC
	{
		enum NightmareOrbAtkType
        {
			DecideNext = -1,
			SingleStar = 0,
			SlashBeam = 1,
			TripleStar = 2,
			HomingStar = 3,
			Dash = 4
        }
        public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Power Orb");
			Main.npcFrameCount[NPC.type] = 4;

			//Needed for multiplayer spawning to work
			NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true; //immune to not mess up movement
        }

		public override void SetDefaults() {
			NPC.width = 100;
			NPC.height = 100;
			NPC.damage = 70; //initally
			NPC.noTileCollide = true;
			NPC.defense = 25; 
			NPC.lifeMax = 24000;
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

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			scale = 1.5f;
			return true;
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
