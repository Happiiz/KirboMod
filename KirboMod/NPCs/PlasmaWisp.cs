using KirboMod.Items.Weapons;
using KirboMod.Items;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using SoundEngine = Terraria.Audio.SoundEngine;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.DataStructures;

namespace KirboMod.NPCs
{
	public class PlasmaWisp : ModNPC
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Plasma Wisp");
			Main.npcFrameCount[NPC.type] = 6;

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Confused, // Most NPCs have this
                }
            };
            NPCID.Sets.DebuffImmunitySets[Type] = debuffData;
        }

		public override void SetDefaults()
		{
			NPC.width = 82;
			NPC.height = 70;
			DrawOffsetY = -2; //make sprite line up with hitbox
			NPC.damage = 60;
			NPC.defense = 25;
			NPC.lifeMax = 500;
			NPC.HitSound = SoundID.NPCHit5; //pixie
			NPC.DeathSound = SoundID.NPCDeath7; //pixie
			NPC.value = Item.buyPrice(0, 0, 4, 0); // money it drops
			NPC.knockBackResist = 0f; //how much knockback is applied
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.PlasmaWispBanner>();
			NPC.aiStyle = -1; 
			NPC.noGravity = true; //not effected by gravity
			NPC.noTileCollide = true;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneDirtLayerHeight & Main.hardMode || spawnInfo.Player.ZoneRockLayerHeight & Main.hardMode) //if player is within cave height
			{
				return spawnInfo.SpawnTileType == TileID.Dirt || spawnInfo.SpawnTileType == TileID.Stone ? .03f : 0f; //functions like a mini if else statement
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
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("A wisp sparking high amounts of electricity and light. Tries to sniff out intruders of its domain.")
            });
        }

        public override void AI() //constantly cycles each time
		{
			NPC.spriteDirection = NPC.direction;
			Player player = Main.player[NPC.target];
			NPC.TargetClosest(true);

			//passive effects
			if (Main.rand.NextBool(4)) //1/3 chance
			{
				int dustnumber = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TerraBlade, 0f, -20f, 200, default, 1.5f); //dust
				Main.dust[dustnumber].velocity *= 0.3f;
				Main.dust[dustnumber].noGravity = true;
			}

			float speed = 1.2f; //top speed
			float inertia = 10f; //acceleration and decceleration speed

			if (player.dead == false) //player is alive
			{
				Vector2 direction = player.Center - NPC.Center; //start - end 

				direction.Normalize();
				direction *= speed;
				NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia; //move
			}
			else //player is dead
			{
                Vector2 direction = NPC.Center + new Vector2(0, 50) - NPC.Center; //start - end 

                direction.Normalize();
                direction *= speed;
                NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia; //go down
            }

			Vector2 direction2 = player.Center - NPC.Center; //start - end 
			bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);

			//checks if the plasmama is in range (or if already attacking)
			if ((Math.Abs(direction2.X) < 1280 && direction2.Y < 640 & direction2.Y > -720) || NPC.ai[0] >= 300)
			{
				if (NPC.ai[0] == 299) //almost at 300
                {
					if (lineOfSight && player.dead == false) //if in line of sight and player is alive
                    {
						NPC.ai[0]++; //go to 300
					}
					else
                    {
						NPC.ai[0] = 299; //freeze until in line of sight
                    }
				}
				else
                {
					NPC.ai[0]++;
				}
			}

			if (NPC.ai[0] >= 300) //attack phase
            {
				NPC.velocity *= 0f;
				if (NPC.ai[0] == 375 || NPC.ai[0] == 450 || NPC.ai[0] == 525)
                {
					//setting projectiles
					int proj = ModContent.ProjectileType<Projectiles.BadPlasmaZap>();
					int projdamage = 45;

					if (Math.Abs(direction2.X) < 320 && direction2.Y < 320 && direction2.Y > -180) //closest (168 because its 56% of 300 and 720 is 56% of 1280)
                    {
						proj = ModContent.ProjectileType<Projectiles.BadPlasmaZap>();
						projdamage = 30;

						direction2.Normalize(); //make into 1
						direction2 *= 20;
					}
					else if (Math.Abs(direction2.X) < 640 && direction2.Y < 480 && direction2.Y > -360) //mid
                    {
						proj = ModContent.ProjectileType<Projectiles.BadPlasmaLaser>();
						projdamage = 60;

						direction2.Normalize(); //make into 1
						direction2 *= 40;
					}
					else //furthest
                    {
						proj = ModContent.ProjectileType<Projectiles.BadPlasmaBlast>();
						projdamage = 120;

						direction2.Normalize(); //make into 1
						direction2 *= 8;
					}
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction2, proj, projdamage / 2, 10, Main.myPlayer);
					}

					if (proj == ModContent.ProjectileType<Projectiles.BadPlasmaZap>())
                    {
						SoundEngine.PlaySound(SoundID.Item12, NPC.Center); //laser beam
					}
					else if (proj == ModContent.ProjectileType<Projectiles.BadPlasmaLaser>())
					{
						SoundEngine.PlaySound(SoundID.Item33, NPC.Center); //boss laser beam
					}
					else if (proj == ModContent.ProjectileType<Projectiles.BadPlasmaBlast>())
					{
						SoundEngine.PlaySound(SoundID.Item117, NPC.Center); //conjure arcanum
					}
				}
            }
			if (NPC.ai[0] >= 600) //limit
            {
				NPC.ai[0] = 0;
            }
		}

		public override void FindFrame(int frameHeight) 
		{
			if (NPC.ai[0] < 300) //float
			{
				NPC.frameCounter += 1.0;
				if (NPC.frameCounter < 7.0)
				{
					NPC.frame.Y = 0;
				}
				else if (NPC.frameCounter < 14.0)
				{
					NPC.frame.Y = frameHeight;
				}
				else if (NPC.frameCounter < 21.0)
				{
					NPC.frame.Y = frameHeight * 2;
				}
                else if (NPC.frameCounter < 28.0)
                {
                    NPC.frame.Y = frameHeight * 3;
                }
                else
				{
                    NPC.frameCounter = 0.0;
				}
			}
			else //attack
			{
                NPC.frameCounter += 1.0;
                if (NPC.frameCounter < 5.0)
                {
                    NPC.frame.Y = frameHeight * 4;
                }
                else if (NPC.frameCounter < 10.0)
                {
                    NPC.frame.Y = frameHeight * 5;
                }
                else
                {
                    NPC.frameCounter = 0.0;
                }
            }
		}
		/*public override void OnKill()
		{
			if (Main.rand.NextBool(Main.expertMode ? 10 : 20))
			{
				Item.NewItem(NPC.getRect(), ModContent.ItemType<Items.Weapons.Plasma>());
			}
			Item.NewItem(NPC.getRect(), ModContent.ItemType<Items.DreamEssence>(), Main.rand.Next(5, 10));
		}*/

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // 1 in 20 (5%) chance in Normal. 1 in 10 (10%) chance in Expert
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Items.Weapons.Plasma>(), 20, 10)); // 1 in 20 (5%) chance in Normal. 1 in 10 (10%) chance in Expert

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DreamEssence>(), 1, 4, 8));
        }

        public override void HitEffect(NPC.HitInfo hit)
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

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}
