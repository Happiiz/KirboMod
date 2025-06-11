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
        public ref float AttackTimer => ref NPC.ai[0]; //Use NPC.ai[] as it makes your life easier with multiplayer
        public ref float Ranan => ref NPC.ai[1]; 
        public static float MoveSpeed { get => Main.expertMode ? 11 : 6; } 
        public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Broom Hatter");
			Main.npcFrameCount[NPC.type] = 9;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Direction = -1,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }

		public override void SetDefaults()
        {
			NPC.width = 28;
		    NPC.height = 28;
			NPC.damage = 10;
			NPC.defense = 6; 
			NPC.lifeMax = 30;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = 0f;
			NPC.knockBackResist = .5f;
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.BroomHatterBanner>();
			NPC.aiStyle = -1;
			NPC.noGravity = false;
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
					return spawnInfo.SpawnTileType == TileID.Grass || spawnInfo.SpawnTileType == TileID.Dirt ? .15f : 0f; //functions like a mini if else statement
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
                }
                NPC.localAI[0] = 1;
            }
            NPC.spriteDirection = NPC.direction;
            //movement

            if (AttackTimer == 0) //switch directions
			{
                Ranan = Main.rand.Next(0, 10);
				NPC.netUpdate = true;
            }

			if (Ranan > 5)
            {
				NPC.direction = 1;
			}
            else
            {
				NPC.direction = -1;
			}
            
			++AttackTimer;

			if (AttackTimer >= 54) //end of animation
            {
				AttackTimer = 0f;
            }

			//movement
			if (AttackTimer == 24) //swing frames
			{
				NPC.velocity.X = MoveSpeed * NPC.direction; //use .X so it only effects horizontal movement

                if (Main.netMode != NetmodeID.MultiplayerClient) 
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + (NPC.direction * 25), NPC.Center.Y - 20, NPC.velocity.X, 0,
                        ModContent.ProjectileType<Projectiles.BroomHatterDustCloud>(), 20 / 2, 4, Main.myPlayer);
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

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Items.Weapons.CleaningBroom>(), 20, 10)); // 1 in 20 (5%) chance in Normal. 1 in 10 (10%) chance in Expert
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Starbit>(), 1, 1, 2));
        }
    }
}
