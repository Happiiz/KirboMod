using KirboMod.Items;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.DataStructures;

namespace KirboMod.NPCs
{
	public class Kabu : ModNPC
	{
		public ref float movement => ref NPC.ai[2];

		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Kabu");
			Main.npcFrameCount[NPC.type] = 4;

			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true; //immune to not mess up movement
        }

		public override void SetDefaults() {
			NPC.width = 30;
			NPC.height = 28;
			DrawOffsetY = -2; //make sprite line up with hitbox
			NPC.damage = 30;
			NPC.lifeMax = 40;
			NPC.defense = 5;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(0, 0, 0, 5);
			NPC.knockBackResist = 0f; //How much of the knockback it receives will actually apply
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.KabuBanner>();
			NPC.aiStyle = -1;
			NPC.noGravity = true;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) 
		{
			if (spawnInfo.Player.ZoneRockLayerHeight || spawnInfo.Player.ZoneDirtLayerHeight) //if player is within undergound and cave height
			{
				if (spawnInfo.Player.ZoneJungle)
				{
					return 0f; //no spawn rate
				}
				else if (spawnInfo.Player.ZoneSnow)
				{
					return 0f; //no spawn rate
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
				else if (spawnInfo.Player.ZoneDungeon) //don't spawn in dungeon
				{
					return 0f;
				}
				else //only forest
				{
					return spawnInfo.SpawnTileType == TileID.Stone || spawnInfo.SpawnTileType == TileID.Dirt ? .2f : 0f; //functions like a mini if else statement
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
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("A little wooden mound that slides around in caves. Is said to carry a lot of wisdom.")
            });
        }

        public override void AI() //constantly cycles each time
        {
			NPC.spriteDirection = NPC.direction;
			//switch movements(never twice in a row)
			if (NPC.ai[0] == 0)
			{
				int ranan = Main.rand.Next(1, 4);

				if (ranan == 3) //right
				{
					if (movement == 4) //if already 4
					{
						int subranan = Main.rand.Next(1, 3);
						if (subranan == 3)
						{
							movement = 3;
                        }
						else if (subranan == 2)
						{
							movement = 2;
                        }
						else
						{
							movement = 1;
                        }
					}
					else
					{
						movement = 4;
                    }
				}
				else if (ranan == 2) // left
				{
					if (movement == 3) //if already 3
					{
						int subranan = Main.rand.Next(1, 3);
						if (subranan == 3)
						{
							movement = 4;
                        }
						else if (subranan == 2)
						{
							movement = 2;
                        }
						else
						{
							movement = 1;
                        }
					}
					else
					{
						movement = 3;
                    }
				}
				else if (ranan == 1) //up
				{
					if (movement == 2) //if already 2
					{
						int subranan  = Main.rand.Next(1, 3);
						if (subranan == 3)
						{
							movement = 4;
                        }
						else if (subranan == 2)
						{
							movement = 3;
                        }
						else
						{
							movement = 1;
                        }
					}
					else
					{
						movement = 2;
                    }
				}
				else //down
				{
					if (movement == 1) //if already 1
					{
						int subranan  = Main.rand.Next(1, 3);
						if (subranan == 3)
						{
							movement = 4;
                        }
						else if (subranan == 2)
						{
							movement = 3;
                        }
						else
						{
							movement = 2;
                        }
					}
					else
					{
						movement = 1;
                    }
				}

                NPC.netUpdate = true;
            }

			//timer
			++NPC.ai[0];
			if (NPC.ai[0] >= 120)
            {
				NPC.ai[0] = 0f;
            }

			NPC.ai[1]--; //tile bounce delay timer

			//movement
			if (movement == 1) //down
			{
				if (NPC.collideY && NPC.ai[1] <= 0) //stopped and delay down
				{
					movement = 2; //opposite direction
					NPC.ai[1] = 30;
				}
				else
				{
					NPC.velocity.X = 0;
					NPC.velocity.Y = 2;
				}
			}
			if (movement == 2) //up
			{
				if (NPC.collideY && NPC.ai[1] <= 0) //stopped and delay down
                {
					movement = 1; //opposite direction
                    NPC.ai[1] = 30;
                }
				else
				{
					NPC.velocity.X = 0;
					NPC.velocity.Y = -2;
				}
			}
			if (movement == 3) //left
			{
				if (NPC.collideX && NPC.ai[1] <= 0) //stopped and delay down
                {
					movement = 4; //opposite direction
                    NPC.ai[1] = 30;
                }
				else
				{
					NPC.velocity.X = -2;
					NPC.velocity.Y = 0;
				}
			}
			if (movement == 4) //right
			{
				if (NPC.collideX && NPC.ai[1] <= 0) //stopped and delay down
                {
					movement = 3; //opposite direction
                    NPC.ai[1] = 30;
                }
				else
				{
					NPC.velocity.X = 2;
					NPC.velocity.Y = 0;
				}
			}

			//for stepping up tiles
			Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
			//for falling through tiles
			CheckPlatform();
		}

		private void CheckPlatform() //trust me this is totally unique and original code and definitely not stolen from Spirit Mod's public source code(thx so much btw you don't know the hell I went through with this)
		{
			bool onplatform = true;
			for (int i = (int)NPC.position.X; i < NPC.position.X + NPC.width; i += NPC.width / 4)
			{ //check tiles beneath the boss to see if they are all platforms
				Tile tile = Framing.GetTileSafely(new Point((int)NPC.position.X / 16, (int)(NPC.position.Y + NPC.height + 8) / 16));
				if (!TileID.Sets.Platforms[tile.TileType])
					onplatform = false;
			}
			if (onplatform) //if they are and the player is lower than the boss, temporarily let the boss ignore tiles to go through them
			{
				NPC.noTileCollide = true;
			}
			else
			{
				NPC.noTileCollide = false;
			}
		}

		public override void FindFrame(int frameHeight) // animation
        {
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter < 10.0)
            {
                NPC.frame.Y = 0; //facing screen
            }
            else if (NPC.frameCounter < 20.0)
            {
                NPC.frame.Y = frameHeight; //facing left
            }
            else if (NPC.frameCounter < 30.0)
            {
                NPC.frame.Y = frameHeight * 2; //back towards screen
            }
            else if (NPC.frameCounter < 40.0)
            {
                NPC.frame.Y = frameHeight * 3; //facing right
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
                if (NPC.life <= 0)
                {
                    for (int i = 0; i < 10; i++)
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

		/*public override void OnKill()
		{
			Item.NewItem(NPC.getRect(), ModContent.ItemType<Items.Starbit>(), Main.rand.Next(1, 2));
			Item.NewItem(NPC.getRect(), ItemID.Wood, Main.rand.Next(2, 4));
		}*/

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Starbit>(), 1, 1, 2));
            npcLoot.Add(ItemDropRule.Common(ItemID.Wood, 1, 2, 4));
        }
    }
}
