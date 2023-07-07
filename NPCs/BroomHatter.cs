using KirboMod.Items;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace KirboMod.NPCs
{
	public class BroomHatter : ModNPC
	{
		public ref float attackTimer => ref NPC.ai[0]; //Use NPC.ai[] as it makes your life easier with multiplayer
        public ref float ranan => ref NPC.ai[1]; 

        public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Broom Hatter");
			Main.npcFrameCount[NPC.type] = 9;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Direction = -1,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }

		public override void SetDefaults() {
			NPC.width = 28;
		    NPC.height = 28;
			NPC.damage = 1;
			NPC.defense = 3; 
			NPC.lifeMax = 20;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = 0f;
			NPC.knockBackResist = 1f;
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.BroomHatterBanner>();
			NPC.aiStyle = -1;
			NPC.noGravity = false;
            NPC.direction = Main.rand.Next(0, 1 + 1) == 1 ? 1 : -1; //determines whether to go left or right initally
        }

		public override float SpawnChance(NPCSpawnInfo spawnInfo) 
		{
            //if player is within surface height, daytime & windy day
            if (spawnInfo.Player.ZoneOverworldHeight && Main.dayTime && Main.IsItAHappyWindyDay) 
			{
				if (spawnInfo.Player.ZoneJungle) //don't spawn in jungle
                {
                    return 0f;
                }
				else if (spawnInfo.Player.ZoneSnow) //don't spawn in snow 
                {
                    return 0f;
                }
				else if (spawnInfo.Player.ZoneBeach) //don't spawn on beach
				{
					return 0f;
				}
				else if (spawnInfo.Player.ZoneDesert) //don't spawn in desert
				{
					return 0f;
				}
				else if (spawnInfo.Player.ZoneCorrupt) //don't spawn in corruption
				{
					return 0f;
				}
				else if (spawnInfo.Player.ZoneCrimson) //don't spawn in crimson
				{
					return 0f;
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
                else //only forest
				{
					return spawnInfo.SpawnTileType == TileID.Grass || spawnInfo.SpawnTileType == TileID.Dirt ? .3f : 0f; //functions like a mini if else statement
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
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.WindyDay,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Sweep, sweep, sweep! Must sweep the world clean! That's the monumental task this little guy puts on himself!")
            });
        }

        public override void AI() //constantly cycles each time
        {
			NPC.spriteDirection = NPC.direction;
			//movement

			if (attackTimer == 0) //switch directions
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
            
			++attackTimer;

			if (attackTimer >= 54) //end of animation
            {
				attackTimer = 0f;
            }

			//movement
			if (attackTimer == 24) //swing frames
			{
				NPC.velocity.X = 6 * NPC.direction; //use .X so it only effects horizontal movement

                if (Main.netMode != NetmodeID.MultiplayerClient) 
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + (NPC.direction * 25), NPC.Center.Y - 20, NPC.velocity.X, 0,
                        ModContent.ProjectileType<Projectiles.BroomHatterDustCloud>(), 6 / 2, 4, Main.myPlayer);
                }
            }
			else
			{
				NPC.velocity.X *= 0.9f;
			}

			//for stepping up tiles
			Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
		}

        public override void FindFrame(int frameHeight) // animation
        {
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter < 6.0)
            {
                NPC.frame.Y = 0;
            }
            else if (NPC.frameCounter < 12.0)
            {
                NPC.frame.Y = frameHeight;
            }
            else if (NPC.frameCounter < 18.0)
            {
                NPC.frame.Y = frameHeight * 2;
            }
            else if (NPC.frameCounter < 24.0)
            {
                NPC.frame.Y = frameHeight * 3;
            }
            else if (NPC.frameCounter < 30.0)
            {
                NPC.frame.Y = frameHeight * 4;
            }
            else if (NPC.frameCounter < 36.0)
            {
                NPC.frame.Y = frameHeight * 5;
            }
            else if (NPC.frameCounter < 42.0)
            {
                NPC.frame.Y = frameHeight * 6;
            }
            else if (NPC.frameCounter < 48.0)
            {
                NPC.frame.Y = frameHeight * 7;
            }
            else if (NPC.frameCounter < 54.0)
            {
                NPC.frame.Y = frameHeight * 8;
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
                    for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                    {
                        Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
                        Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.LilStar>(), speed * 5, Scale: 1f); //Makes dust in a messy circle
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                        Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, speed, Main.rand.Next(11, 13), Scale: 1f); //double jump smoke
                    }
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Items.Weapons.CleaningBroom>(), 20, 10)); // 1 in 20 (5%) chance in Normal. 1 in 10 (10%) chance in Expert
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Starbit>(), 1, 1, 2));
        }
    }
}
