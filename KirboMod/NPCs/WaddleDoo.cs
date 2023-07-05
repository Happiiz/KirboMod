using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using KirboMod.Items;
using KirboMod.ItemDropRules.DropConditions;
using KirboMod.Bestiary;

namespace KirboMod.NPCs
{
	public class WaddleDoo : ModNPC
	{
		private bool attacking = false; //controls if in attacking state

		public ref float ranan => ref NPC.ai[2];

		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Waddle Doo");
			Main.npcFrameCount[NPC.type] = 11; //frames something has
		}

		public override void SetDefaults() 
		{
			NPC.width = 36;
			NPC.height = 34;
			NPC.damage = 5;
			NPC.defense = 0;
			NPC.lifeMax = 40;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(0, 0, 0, 5); // money it drops
			NPC.knockBackResist = 1f;
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.WaddleDooBanner>();
			NPC.aiStyle = -1;
			NPC.friendly = false;
			NPC.noGravity = false;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneOverworldHeight && Main.dayTime) //if player is within surface height & daytime
			{
				if (spawnInfo.Player.ZoneJungle)
				{
					return spawnInfo.SpawnTileType == TileID.JungleGrass || spawnInfo.SpawnTileType == TileID.Mud ? .15f : 0f; //functions like a mini if else statement
				}
				else if (spawnInfo.Player.ZoneSnow)
				{
					return spawnInfo.SpawnTileType == TileID.SnowBlock ? .15f : 0f; //functions like a mini if else statement
				}
				else if (spawnInfo.Player.ZoneBeach) //don't spawn on beach
				{
					return 0f;
				}
				else if (spawnInfo.Player.ZoneDesert) //don't spawn on beach
				{
					return 0f;
				}
				else if (spawnInfo.Player.ZoneCorrupt) //don't spawn on beach
				{
					return 0f;
				}
				else if (spawnInfo.Player.ZoneCrimson) //don't spawn on beach
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
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                new SurfaceBackgroundProvider(),

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("The more dangerous cousin of the Waddle Dee. Shoots out electric beams whenever it feels threatened.")
            });
        }

        public override void AI() //constantly cycles each time
		{
			NPC.spriteDirection = NPC.direction;
			Player player = Main.player[NPC.target];
			Vector2 distance = player.Center - NPC.Center;

			if (NPC.ai[1] == 1) //kracko doo
			{
                NPC.value = Item.buyPrice(0, 0, 0, 0); // money it dro- oh wait!
            }

            bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);

            if (distance.X < 120 & distance.X > -120 & distance.Y > -120 & distance.Y < 120 && lineOfSight && attacking == false) //checks if da doo is in range
			{
				NPC.ai[0] = 0; 
                attacking = true;
			}
			
			//keep attacking if started
			if (attacking == true)
            {
				Beam();
			}
			else
            {
				Walk();
			}

			//for stepping up tiles
			Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
		}

		public override void FindFrame(int frameHeight) // animation
		{
			if (attacking == false) //walking
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
			else //BeAm AttAck
            {
                if (NPC.ai[0] < 60) //charge
                {
                    NPC.frameCounter += 1.0;

                    if (NPC.frameCounter < 5.0)
                    {
                        NPC.frame.Y = frameHeight * 8;
                    }
                    else if (NPC.frameCounter < 10.0)
                    {
                        NPC.frame.Y = frameHeight * 9;
                    }
                    else
                    {
                        NPC.frameCounter = 0.0;
                    }
                }
                else //shoot
                {
                    NPC.frame.Y = frameHeight * 10; 
                }
			}
		}

        public override bool PreKill() //check if Kracko Doo so it won't drop anything, not even if told to from outside mods
        {
            if (NPC.ai[1] == 1)
			{
				return false;
			}
			return true;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
            WaddleDooDropCondition WaddleDooDropCondition = new WaddleDooDropCondition();
            IItemDropRule NotKrackoDoo = new LeadingConditionRule(WaddleDooDropCondition);

			//checks if npcai[1] != 1
            NotKrackoDoo.OnSuccess(ItemDropRule.NormalvsExpert(ModContent.ItemType<Items.Weapons.BeamStaff>(), 40, 20)); // 1 in 40 (2.5%) chance in Normal. 1 in 20 (5%) chance in Expert
            NotKrackoDoo.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Starbit>(), 1, 1, 2));

			npcLoot.Add(NotKrackoDoo);
        }

		private void Beam()
        {
			Player player = Main.player[NPC.target];

			if (NPC.ai[0] < 60) //target before attacking
			{
				NPC.TargetClosest(true);
			}

            NPC.ai[0]++;

			NPC.velocity.X *= 0.9f;

			Vector2 projshoot = new Vector2(NPC.direction * 8, (float)Math.Sin(45 - (NPC.ai[0] - 60) / 22.5f) * -5); //up and down starting from 30

			Vector2 startOffset = new Vector2(NPC.direction * 8, 0);

            if (NPC.ai[0] >= 60 & NPC.ai[0] <= 180) //attack window
			{
				if (NPC.ai[0] % 3 == 0) //every multiple of 3
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + startOffset, projshoot, ModContent.ProjectileType<Projectiles.BeamBad>(), NPC.damage / 2, 1, Main.myPlayer, 0, 0);
					}
                }

                if (NPC.ai[0] == 60) //sound
                {
                    SoundEngine.PlaySound(SoundID.Item93, NPC.Center); //plays electro zap
                }
            }
			if (NPC.ai[0] >= 240)
            {
                NPC.ai[0] = 0f;
                attacking = false;
            }
        }

		private void Walk()
        {
			if (NPC.ai[1] == 1) //hostile ai, used for kracko
			{
                NPC.TargetClosest(true);

                float speed = 0.7f; //top speed
                float inertia = 20f; //acceleration and decceleration speed

                //we put this instead of player.Center so it will always be moving top speed instead of slowing down when player is near
                Vector2 direction = NPC.Center + new Vector2(NPC.direction * 50, 0) - NPC.Center; //start - end 
                                                                                                  

                direction.Normalize();
                direction *= speed;
				if (NPC.velocity.Y == 0) //on ground (so it doesn't interfere with knockback)
				{
					NPC.velocity.X = (NPC.velocity.X * (inertia - 1) + direction.X) / inertia; //use .X so it only effects horizontal movement
				}
				NPC.ai[0] = 0;
            }
			else
			{
                NPC.TargetClosest(false);

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
			}
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life <= 0)
			{
				if (NPC.ai[1] != 1)
				{
                    for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                    {
                        Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
                        Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.LilStar>(), speed * 5, Scale: 1f); //Makes dust in a messy circle
                    }
                }
				for (int i = 0; i < 5; i++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(2.5f, 2.5f); //circle
                    Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, speed, Main.rand.Next(11, 13), Scale: 1f); //double jump smoke
                }
			}
		}
	}
}
