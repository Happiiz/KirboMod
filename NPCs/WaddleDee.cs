using KirboMod.Bestiary;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
	public class WaddleDee : ModNPC
	{
        public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Waddle Dee");
			Main.npcFrameCount[NPC.type] = 8;
		}

		public override void SetDefaults() {
			NPC.width = 36;
			NPC.height = 32;
			NPC.lifeMax = 20;
			NPC.damage = 8;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = 20; // money it drops
			NPC.knockBackResist = 1f; //How much of the knockback it receives will actually apply
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.WaddleDeeBanner>();
			NPC.aiStyle = -1;
			NPC.noGravity = false;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) 
		{
			if (spawnInfo.Player.ZoneOverworldHeight && Main.dayTime && !spawnInfo.Invasion && !Main.eclipse) //if player is within surface height & daytime
			{
				if (spawnInfo.Player.ZoneJungle)
				{
					return spawnInfo.SpawnTileType == TileID.JungleGrass || spawnInfo.SpawnTileType == TileID.Mud ? 0.075f : 0f; //functions like a mini if else statement
				}
				else if (spawnInfo.Player.ZoneSnow)
				{
					return spawnInfo.SpawnTileType == TileID.SnowBlock ? .15f : 0f; //functions like a mini if else statement
				}
				else if (spawnInfo.Player.ZoneForest) //if forest
                {
                    return spawnInfo.SpawnTileType == TileID.Grass || spawnInfo.SpawnTileType == TileID.Dirt ? 0.1f : 0f; //functions like a mini if else statement
                }
				else
				{
					return 0f; //no spawn rate
				}
			}
			else
			{
				return 0f; //no spawn rate
			}
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
			//uses AddRange to add multiple things instead of Add for simplicity
			bestiaryEntry.Info.AddRange(
            [
				//set spawning conditions of NPC in bestiary
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                new SurfaceBackgroundProvider(),

				//bestiary description
				new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.KirboMod.NPCs.Bestiary.WaddleDee"))
			]); 
        }

        public override void AI() //constantly cycles each time
        {
            NPC.TargetClosest(false);
            if (NPC.localAI[0] == 0)
            {
                if (NPC.HasValidTarget)
                {
                    NPC.direction = MathF.Sign(Main.player[NPC.target].Center.X - NPC.Center.X);
                }
                else
                {
                    NPC.direction = Main.rand.NextBool() ? 1 : -1;
                    NPC.netUpdate = true;
                }
                NPC.localAI[0] = 1;
            }
            NPC.spriteDirection = NPC.direction;
            //reroll direction
            ++NPC.ai[0];

            if (NPC.ai[0] >= 300)
            {
                if (NPC.ai[0] >= 300)
                {
                    int ranan = Main.rand.Next(0, 10);

                    if (ranan > 5)
                    {
                        NPC.direction = 1;
                    }
                    else
                    {
                        NPC.direction = -1;
                    }
                    NPC.netUpdate = true;

                    NPC.ai[0] = 0f;
                }

                NPC.ai[0] = 0f;
            }

            //turn around if touching wall for a while
            if (NPC.collideX && NPC.velocity.Y == 0) 
            {
				NPC.ai[1]++;

				if (NPC.ai[1] >= 180) //turn around
				{
                    NPC.direction *= -1;
					NPC.ai[1] = 0f;
                    NPC.ai[0] = 1f;
                }
            }
			else //reset
			{
				NPC.ai[1] = 0;
			}

            //movement
            float speed = 0.7f;
			float inertia = 20f;

			if (NPC.velocity.Y == 0) //on ground (so it doesn't interfere with knockback)
			{
                Helper.BasicEnemyWalk(ref NPC.velocity.X, speed, inertia, NPC.direction);
            }

			//for stepping up tiles
			Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
		}

        public override void FindFrame(int frameHeight) // animation
        {
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter < 10.0)
            {
                NPC.frame.Y = 0;
            }
            else if (NPC.frameCounter < 20.0)
            {
                NPC.frame.Y = frameHeight;
            }
            else if (NPC.frameCounter < 30.0)
            {
                NPC.frame.Y = frameHeight * 2;
            }
            else if (NPC.frameCounter < 40.0)
            {
                NPC.frame.Y = frameHeight * 3;
            }
            else if (NPC.frameCounter < 50.0)
            {
                NPC.frame.Y = frameHeight * 4;
            }
            else if (NPC.frameCounter < 60.0)
            {
                NPC.frame.Y = frameHeight * 5;
            }
            else if (NPC.frameCounter < 70.0)
            {
                NPC.frame.Y = frameHeight * 6;
            }
            else if (NPC.frameCounter < 80.0)
            {
                NPC.frame.Y = frameHeight * 7;
            }
            else
            {
                NPC.frameCounter = 0.0;
            }
        }

		public override void HitEffect(NPC.HitInfo hit)
		{
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 5; i++) //first section makes inital statement once //second declares the conditional they must follow // third declares the loop
                {
                    Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle edge
                    Gore.NewGorePerfect(NPC.GetSource_FromAI(), NPC.Center, speed, Main.rand.Next(16, 18));
                }
                for (int i = 0; i < 5; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                    Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, speed, Main.rand.Next(11, 13), Scale: 1f); //double jump smoke
                }
            }
        }
	}
}
