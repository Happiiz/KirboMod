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
using System.IO;

namespace KirboMod.NPCs
{
	public class KnuckleJoe : ModNPC
	{
		private int attacktype = 0;

        int attackTimer = 0;

        int walkTimer = 0;
        int walkDirection = 1; //determines whether the enemy will walk forward or backward

        private bool jumped = false;

        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Knuckle Joe");
			Main.npcFrameCount[NPC.type] = 11;
		}

		public override void SetDefaults()
		{
			NPC.width = 44;
			NPC.height = 44;
			NPC.damage = 20;
			NPC.defense = 12;
			NPC.lifeMax = 60;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(0, 0, 0, 10);
			NPC.knockBackResist = 0f; //how much knockback applies
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.KnuckleJoeBanner>();
			NPC.aiStyle = -1; 
			NPC.friendly = false;
			NPC.noGravity = false;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneRockLayerHeight) //if player is within cave height
			{
				if (spawnInfo.Player.ZoneJungle)
                {
					return 0f;
				}
				else if (spawnInfo.Player.ZoneSnow)
				{
					return 0f;
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
					return spawnInfo.SpawnTileType == TileID.Stone || spawnInfo.SpawnTileType == TileID.Dirt ? .06f : 0f; //functions like a mini if else statement
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
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Watch out! Knuckle Joe wants a challenge and you seem perfectly fit! They'll bombard you with a flurry of vulcan jabs and smash punches 'til you're down!")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
			writer.Write(attacktype); //send non NPC.ai array info to servers
            writer.Write(attackTimer);
            writer.Write(attackTimer);
            writer.Write(walkDirection);
        }

		public override void ReceiveExtraAI(BinaryReader reader)
		{
            attacktype = reader.ReadInt32(); //sync in multiplayer
            attackTimer = reader.ReadInt32();
            attackTimer = reader.ReadInt32();
            walkDirection = reader.ReadInt32();
        }

        public override void AI() //constantly cycles each time
		{
            NPC.spriteDirection = NPC.direction;
            Player player = Main.player[NPC.target];
            Vector2 distance = player.Center - NPC.Center;

            bool inRange = Math.Abs(distance.X) < 400 && Math.Abs(distance.Y) < 200 && !player.dead;

            if (attacktype > 1 || inRange) //attacking or in range
            {
                attackTimer++; //attack timer
            }

            if (inRange) //checks if the joe is in range
            {
                walkTimer = 0; //reset walk timer

                if (attackTimer < 120) //not attacking
                {
                    attacktype = 1; //side stepping
                }
                else if (attackTimer == 120) //times up!
                {
                    bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);

                    if (Math.Abs(distance.X) < 150) //close enough
                    {
                        attacktype = 2; //vulcan jab
                    }
                    else if (lineOfSight) //can see player
                    {
                        attacktype = 3; //smash punch 
                    }
                    else //wait until can see player within range
                    {
                        attacktype = 1;
                        attackTimer = 119; //reset attack timer to before time's up
                    }
                }
            }
            else if (attacktype == 1) //sidestepping when out of range
            {
                attacktype = 0; //walk
                attackTimer = 0; //reset attack timer
            }

            //declaring attacktype values
            if (attacktype == 0)
			{
				Walk();
			}
			if (attacktype == 1)
			{
				Sidestep();
			}
			if (attacktype == 2)
			{
				RapidPunch();
			}
			if (attacktype == 3)
            {
				ChargeBlast();
			}

			//for stepping up tiles
			Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
		}

		public override void FindFrame(int frameHeight) // animation
		{
			if (attacktype == 0 || attacktype == 1) //sidestep
			{
				NPC.frameCounter += 1;
				if (NPC.frameCounter < 5)
				{
					NPC.frame.Y = 0; 
				}
				else if (NPC.frameCounter < 10)
				{
					NPC.frame.Y = frameHeight; 
				}
                else if (NPC.frameCounter < 15)
                {
                    NPC.frame.Y = frameHeight * 2; 
                }
                else if (NPC.frameCounter < 20)
                {
                    NPC.frame.Y = frameHeight; 
                }
                else
				{
                    NPC.frameCounter = 0;
				}
			}
			if (attacktype == 2) //vulcan jabs
			{
                NPC.frameCounter += 1;

				if (attackTimer >= 60 + 120) //attacking
				{
					if (NPC.frameCounter < 3)
					{
						NPC.frame.Y = frameHeight * 5; //punch
					}
					else
					{
						NPC.frame.Y = frameHeight * 6; //punch
					}
					if (NPC.frameCounter > 6)
                    {
                        NPC.frameCounter = 0;
                    }
				}
				else if (attackTimer >= 5 + 120) //ready punch
                {
					NPC.frame.Y = frameHeight * 4; 
				}
				else
				{
                    NPC.frame.Y = frameHeight * 3; 
                }
			}
			if (attacktype == 3) //smash punch
			{
                NPC.frameCounter += 1;

                if (attackTimer >= 60 + 120) //blast
                {
                    NPC.frame.Y = frameHeight * 9;

                    if (attackTimer >= 65 + 120)
                    {
                        NPC.frame.Y = frameHeight * 10;
                    }
                    else
                    {
                        NPC.frame.Y = frameHeight * 9;
                    }
                }
                else //charge
                {
                    NPC.frameCounter += 1;

                    if (NPC.frameCounter < 5) 
                    {
                        NPC.frame.Y = frameHeight * 8;
                    }
                    else if (NPC.frameCounter < 10)
                    {
                        NPC.frame.Y = frameHeight * 7;
                    }
                    else
                    {
                        NPC.frameCounter = 0;
                    }
                }
            }
		}

		private void Walk() //walk towards player
		{
            Player player = Main.player[NPC.target];
            Vector2 distance = player.Center - NPC.Center;

            walkTimer++;

            if (walkTimer % 10 == 0) //turn towards player every 10 ticks
            {
                NPC.TargetClosest(true);
            }
            NPC.velocity.X = NPC.direction * 1.6f;

            if (distance.X > 0) //player is ahead
            {
                walkDirection = 1; //walk forward
            }
            else //player is behind
            {
                walkDirection = -1; //walk forward
            }

            Jump();
        }

		private void Sidestep() //back up or move forward randomly
        {
            NPC.TargetClosest(true);

            Player player = Main.player[NPC.target];
            Vector2 distance = player.Center - NPC.Center;

            if (attackTimer % 10 == 0)
            {
                if (distance.X > 0) //player is ahead
                {
                    if (distance.X > 200) //far enough
                    {
                        walkDirection = 1; //walk forward
                    }
                    if (distance.X < 100) //close enough
                    {
                        walkDirection = -1; //walk backward
                    }
                }
                else //player is behind
                {
                    if (distance.X < -200) //far enough
                    {
                        walkDirection = -1; //walk forward (reversed)
                    }
                    if (distance.X > -100) //close enough
                    {
                        walkDirection = 1; //walk backward (reversed)
                    }
                }
            }

            Jump();

            NPC.velocity.X = walkDirection * 1.6f;
        }

        private void Jump()
        {
            if (NPC.collideX && NPC.velocity.Y == 0) //hop if touching wall
            {
                NPC.velocity.Y = -5;
                jumped = true;
            }

            if (NPC.velocity.Y == 0) //on ground
            {
                jumped = false;
            }
        }

		private void RapidPunch() //fires punches
        {
            if (attackTimer < 120 + 60) //stance (add 120 as that's where it starts from)
            {
                NPC.TargetClosest(true); //face player
                NPC.velocity.X *= 0.5f; //slow
            }
            else //punch
            {
                Player player = Main.player[NPC.target];
                Vector2 projshoot = NPC.Center + new Vector2(NPC.direction * 200, 0) - NPC.Center; //200 in the direction it's facing(doesn't have to be 200 because we normalize it)
                projshoot.Normalize();
                projshoot *= 10f;

                NPC.velocity.X *= 0.5f; //slow
                if (attackTimer >= 120 + 60) //unleash punch
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 20, 0), projshoot.RotatedByRandom(MathHelper.ToRadians(45)), Mod.Find<ModProjectile>("VulcanPunch").Type, 30 / 2, 0.1f, Main.myPlayer, 0, 0);
                    }

                    if (attackTimer % 5 == 0) //every 5 ticks
                    {
                        SoundEngine.PlaySound(SoundID.Item1, NPC.Center); //we dont define the stuff after coordinates because legacy sound style
                    }
                }
                if (attackTimer >= 120 + 180) //restart after 2 seconds
                {
                    attacktype = 1;
                    attackTimer = 0;
                }
            }
        }

		private void ChargeBlast() //blast 
		{
            if (attackTimer < 120 + 60)
            {
                NPC.TargetClosest(true);
                NPC.velocity.X *= 0.5f; //slow
            }
            Player player = Main.player[NPC.target];
            Vector2 projshoot = NPC.Center + new Vector2(NPC.direction * 200, 0) - NPC.Center; //200 in the direction it's facing(doesn't have to be 200 because we normalize it)
            projshoot.Normalize();
            projshoot *= 10f;

            if (attackTimer == 120 + 60) //climax
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projshoot, Mod.Find<ModProjectile>("JoeBlast").Type, 30 / 2, 8, Main.myPlayer, 0, 0); //actual damage has to be divided by 2
                }
                SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
            }
            if (attackTimer >= 120 + 120) //restart
            {
                attacktype = 1;
                attackTimer = 0;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Items.Weapons.FighterGlove>(), 20, 10)); // 1 in 20 (5%) chance in Normal. 1 in 10 (10%) chance in Expert
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Starbit>(), 1, 2, 4));
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
	}
}
