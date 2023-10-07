using KirboMod.Bestiary;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
	public class WaddleDee : ModNPC
	{
        public ref float ranan => ref NPC.ai[2];

        public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Waddle Dee");
			Main.npcFrameCount[NPC.type] = 8;
		}

		public override void SetDefaults() {
			NPC.width = 36;
			NPC.height = 32;
			NPC.lifeMax = 20;
			NPC.damage = 5;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = 0f; // money it drops
			NPC.knockBackResist = 1f; //How much of the knockback it receives will actually apply
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.WaddleDeeBanner>();
			NPC.aiStyle = -1;
			NPC.noGravity = false;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) 
		{
			if (spawnInfo.Player.ZoneOverworldHeight && Main.dayTime) //if player is within surface height & daytime
			{
				if (spawnInfo.Player.ZoneJungle)
				{
					return spawnInfo.SpawnTileType == TileID.JungleGrass || spawnInfo.SpawnTileType == TileID.Mud ? .25f : 0f; //functions like a mini if else statement
				}
				else if (spawnInfo.Player.ZoneSnow)
				{
					return spawnInfo.SpawnTileType == TileID.SnowBlock ? .25f : 0f; //functions like a mini if else statement
				}
				else if (spawnInfo.Player.ZoneBeach) //don't spawn on beach
				{
					return 0f; //no spawn rate
				}
				else if (spawnInfo.Player.ZoneDesert) //don't spawn on beach
				{
					return 0f; //no spawn rate
				}
				else if (spawnInfo.Player.ZoneCorrupt) //don't spawn on beach
				{
					return 0f; //no spawn rate
				}
				else if (spawnInfo.Player.ZoneCrimson) //don't spawn on beach
				{
					return 0f; //no spawn rate
				}
				else if (spawnInfo.Invasion) //don't spawn during invasions
				{
					return 0f;
				}
				else if (spawnInfo.Player.ZoneMeteor) //don't spawn on meteor
				{
					return 0f;
				}
				else if (spawnInfo.Player.ZoneDungeon) //don't spawn in dungeon
				{
					return 0f;
				}
				else if (spawnInfo.Water) //don't spawn in water
				{
					return 0f;
				}
                else if (spawnInfo.Sky) //don't spawn in space
                {
                    return 0f;
                }
                else if (Main.eclipse) //don't spawn during eclipse
                {
                    return 0f;
                }
                else if (spawnInfo.Player.ZoneTowerVortex)
				{
					return 0f;
				}
				else if (spawnInfo.Player.ZoneTowerSolar)
				{
					return 0f;
				}
				else if (spawnInfo.Player.ZoneTowerNebula)
				{
					return 0f;
				}
				else if (spawnInfo.Player.ZoneTowerStardust)
				{
					return 0f;
				}
				else //only forest
				{
					return spawnInfo.SpawnTileType == TileID.Grass || spawnInfo.SpawnTileType == TileID.Dirt ? .5f : 0f; //functions like a mini if else statement
				}
			}
			else
			{
				return 0f; //no spawn rate
			}
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                new SurfaceBackgroundProvider(),

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Simple minded wanderers that have no desire to fight. Although they can be quite the nuisance when you're in a hurry.")
			}); 
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ranan); //send non NPC.ai array info to servers
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ranan = reader.ReadInt32(); //sync in multiplayer
        }

        public override void AI() //constantly cycles each time
        {
			NPC.spriteDirection = NPC.direction;

            if (NPC.ai[0] == 0)
            {
                ranan = Main.rand.Next(0, 10);
                NPC.netUpdate = true;
            }

            if (ranan > 5)
            {
                NPC.direction = 1;
            }
            else
            {
                NPC.direction = -1;
            }

            //reroll direction
            ++NPC.ai[0];

            if (NPC.ai[0] >= 300)
            {
                NPC.ai[0] = 0f;
            }

            //movement
            float speed = 0.7f;
			float inertia = 20f;

			Vector2 moveTo = NPC.Center + new Vector2(NPC.direction * 200, 0);
			Vector2 direction = moveTo - NPC.Center; //start - end
			direction.Normalize();
			direction *= speed;
			if (NPC.velocity.Y == 0) //on ground (so it doesn't interfere with knockback)
			{
				NPC.velocity.X = (NPC.velocity.X * (inertia - 1) + direction.X) / inertia; //use .X so it only effects horizontal movement
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
