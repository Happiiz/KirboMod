using KirboMod.Items.Zero;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using KirboMod.Items.Kracko;
using KirboMod.Items.Armor.AirWalker;
using KirboMod.Items.Accesories;
using ReLogic.Content;
using KirboMod.Systems;
using Terraria.DataStructures;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters;

namespace KirboMod.NPCs
{ 
	[AutoloadBossHead]
	public class Kracko : ModNPC
	{
		private int animation = 0;

		private int attacktype = 1; //decides the attack
		private int doodelay = 0;
		private int beambigdirection = 1;

		private float sweepheight = 0;
		private float sweepX = 0;
		private float sweepY = 0;

		private int sideOfAttack = 2;

		private int lastattacktype = 2; //sets last attack type

		private bool transitioning = false; //checks if going through expert mode exclusive phase

		private bool frenzy = false; //checks if going in frenzy mode in expert mode
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Kracko");
			Main.npcFrameCount[NPC.type] = 2;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "KirboMod/NPCs/BestiaryTextures/KrackoPortrait",
                PortraitScale = 0.75f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 0f,
                Position = new Vector2(2, 6), //Center the eye
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Confused, // Most NPCs have this
                }
            };
            NPCID.Sets.DebuffImmunitySets[Type] = debuffData;
        }

		public override void SetDefaults() {
			NPC.width = 219; //hitbox less than sprite
			NPC.height = 150; //hitbox less than sprite
			DrawOffsetY = 29; //line up with middle
			NPC.damage = 30;
			NPC.defense = 12;
			NPC.noTileCollide = true;
			NPC.lifeMax = 4000;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(0, 5, 0, 0); // money it drops
			NPC.knockBackResist = 0f;
			NPC.aiStyle = -1;
			NPC.boss = true;
			NPC.noGravity = true;
			NPC.lavaImmune = true;
            Music = MusicID.Boss1;
            //bossBag/* tModPorter Note: Removed. Spawn the treasure bag alongside other loot via npcLoot.Add(ItemDropRule.//bossBag(type)) */ = ModContent.ItemType<Items.Kracko.KrackoBag>(); //the expert mode treasure bag it drops
            NPC.friendly = false;
			NPC.npcSlots = 4;
			NPC.buffImmune[BuffID.Confused] = true;
		}

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */ 
        {
			NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance);
		    NPC.damage = (int)(NPC.damage * 0.6f);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Kracko is a living cloud that has lingered in the sky since ancient times, and thus has mastered the art of thunder!")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(attacktype); //send non NPC.ai array info to servers
            writer.Write(doodelay); //send non NPC.ai array info to servers
            writer.Write(beambigdirection); //send non NPC.ai array info to servers
            writer.Write(sideOfAttack); //send non NPC.ai array info to servers
            writer.Write(lastattacktype); //send non NPC.ai array info to servers
            writer.Write(sweepheight); //send non NPC.ai array info to servers
            writer.Write(sweepX); //send non NPC.ai array info to servers
            writer.Write(sweepY); //send non NPC.ai array info to servers
            writer.Write(transitioning); //send non NPC.ai array info to servers
            writer.Write(frenzy); //send non NPC.ai array info to servers
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            attacktype = reader.ReadInt32(); //sync in multiplayer
            doodelay = reader.ReadInt32(); //sync in multiplayer
            beambigdirection = reader.ReadInt32(); //sync in multiplayer
            sideOfAttack = reader.ReadInt32(); //sync in multiplayer
            lastattacktype = reader.ReadInt32(); //sync in multiplayer
			sweepheight = reader.ReadSingle(); //sync in multiplayer
            sweepX = reader.ReadSingle(); //sync in multiplayer
            sweepY = reader.ReadSingle(); //sync in multiplayer
            transitioning = reader.ReadBoolean(); //sync in multiplayer
            frenzy = reader.ReadBoolean(); //sync in multiplayer
        }

        public override void AI() //constantly cycles each time
		{
			Player player = Main.player[NPC.target];

			if (NPC.ai[1] <= 60 || (NPC.ai[0] >= 60 && NPC.ai[0] < 90 && frenzy == true)) //be harmless upon spawn (or when moving during frenzy
            {
				NPC.damage = 0;
            }
			else
            {
				if (Main.expertMode == false)
                {
					NPC.damage = 30;
				}
				else
                {
					NPC.damage = 60;
				}
            }
			NPC.ai[1]++;

			if (NPC.life <= NPC.lifeMax * 0.25 && Main.expertMode == true) //transition
            {
				transitioning = true;
            }
			//DESPAWNING
			if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
			{
				NPC.TargetClosest(false);

				NPC.velocity.Y = NPC.velocity.Y - 0.2f;
				NPC.ai[0] = 0;

				if (NPC.timeLeft > 60)
				{
					NPC.timeLeft = 60;
					return;
				}
			}
			else if (transitioning == true && (NPC.ai[2] > 180) == false) //TRANSITION TO PHASE 2
            {
				NPC.ai[2]++;
                NPC.velocity *= 0.0001f; //stop

                if (NPC.ai[2] < 180)
				{
					NPC.rotation += MathHelper.ToRadians(4f); //rotate (in degrees)

					if (NPC.ai[2] % 5 == 0) //every multiple of 5
					{
						for (int i = 0; i < 2; i++)
						{
							Vector2 speed = Main.rand.NextVector2Circular(20f, 20f); //circle
							Dust d = Dust.NewDustPerfect(NPC.Center + speed, DustID.Cloud, speed, Scale: 2f); //Makes dust in a messy circle
							d.noGravity = true;
						}
					}
				}
				else
				{
					NPC.rotation = 0;
					transitioning = false;
					frenzy = true;
				}

                NPC.TargetClosest(true);
                NPC.ai[0] = 0; //no attack pattern cycle
            }
			else //regular attack
			{
                NPC.TargetClosest(true);
                AttackPattern();
			}
		}

		private void AttackPattern()
		{
			Player player = Main.player[NPC.target];
			Vector2 distance = player.Center - NPC.Center; //start - end
			Vector2 distanceOverPlayer = player.Center + new Vector2( 0, -200) - NPC.Center;
			Vector2 distanceBelowPlayer = player.Center + new Vector2(0, 200) - NPC.Center;
			
			Vector2 distanceDiagonalLeftOfPlayer = player.Center + new Vector2(-400, -400) - NPC.Center;
			Vector2 distanceDiagonalRightOfPlayer = player.Center + new Vector2(400, -400) - NPC.Center;

			Vector2 distanceLeftOfPlayer = player.Center + new Vector2(-400, 0) - NPC.Center;
			Vector2 distanceRightOfPlayer = player.Center + new Vector2(400, 0) - NPC.Center;

			NPC.ai[0]++;

			if (NPC.ai[0] == (!frenzy ? 15 : 60)) //changes depending or not in frenzy
			{
				if (doodelay == 1) //summon (multiplayer syncing stuff is because spawning npc)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						int index = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, Mod.Find<ModNPC>("WaddleDoo").Type, 0, 0, 1, 0, 0, NPC.target);

						if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
						{
							NetMessage.SendData(MessageID.SyncNPC, number: index);
						}
					}
                    doodelay = 0; //reset
				}
				else
				{
					doodelay += 1; //next turn will summon 
				}
			}

			if (NPC.ai[0] == (!frenzy ? 120 : 30) && Main.netMode != NetmodeID.MultiplayerClient) //choose attack randomly(faster is frenzying)
			{
				sideOfAttack = Main.rand.Next(1, 3); //right or left

				if ((Main.expertMode && NPC.life <= NPC.lifeMax * 0.50) || NPC.life <= NPC.lifeMax * 0.33) //expert mode below 50% or normal mode below 33%
                {
					int randattack = Main.rand.Next(1, 4 + 1); 

					if (randattack == 1) //beams
					{
						if (lastattacktype == 1)
						{
							int subrandattack = Main.rand.Next(1, 3 + 1);
							if (subrandattack == 1)
							{
								attacktype = 2;
								lastattacktype = 2;
							}
							else if (subrandattack == 2)
							{
								attacktype = 3;
								lastattacktype = 3;
							}
							else
							{
								attacktype = 4;
								lastattacktype = 4;
							}
						}
						else
						{
							attacktype = 1;
							lastattacktype = 1;
						}
					}
					else if (randattack == 2) //sweep
					{
						if (lastattacktype == 2)
						{
							int subrandattack = Main.rand.Next(1, 3 + 1);
							if (subrandattack == 1)
							{
								attacktype = 1;
								lastattacktype = 1;
							}
							else if (subrandattack == 2)
							{
								attacktype = 3;
								lastattacktype = 3;
							}
							else
							{
								attacktype = 4;
								lastattacktype = 4;
							}
						}
						else
						{
							attacktype = 2;
							lastattacktype = 2;
						}
					}
					else if (randattack == 3) //dash
					{
						if (lastattacktype == 3)
						{
							int subrandattack = Main.rand.Next(1, 3 + 1);
							if (subrandattack == 1)
							{
								attacktype = 1;
								lastattacktype = 1;
							}
							else if (subrandattack == 2)
							{
								attacktype = 2;
								lastattacktype = 2;
							}
							else
							{
								attacktype = 4;
								lastattacktype = 4;
							}
						}
						else
						{
							attacktype = 3;
							lastattacktype = 3;
						}
					}
					else if (randattack == 4) //lightning
					{
						if (lastattacktype == 4)
						{
							int subrandattack = Main.rand.Next(1, 3 + 1);
							if (subrandattack == 1)
							{
								attacktype = 1;
								lastattacktype = 1;
							}
							else if (subrandattack == 2)
							{
								attacktype = 2;
								lastattacktype = 2;
							}
							else
							{
								attacktype = 3;
								lastattacktype = 3;
							}
						}
						else
						{
							attacktype = 4;
							lastattacktype = 4;
						}
					}
				}//before lines is expert, after is normal
				if ((Main.expertMode && NPC.life <= NPC.lifeMax * 0.75 && NPC.life > NPC.lifeMax * 0.50) || !Main.expertMode && NPC.life <= NPC.lifeMax * 0.66 && NPC.life > NPC.lifeMax * 0.33) //npc below 66% and above 33% (and not expert mode)
				{
					int randattack = Main.rand.Next(1, 3 + 1);
					if (randattack == 1)
					{
						if (lastattacktype == 1)
						{
							int subrandattack = Main.rand.Next(1, 2 + 1);
							if (subrandattack == 1)
							{
								attacktype = 2;
								lastattacktype = 2;
							}
							else
							{
								attacktype = 3;
								lastattacktype = 3;
							}
						}
						else
						{
							attacktype = 1;
							lastattacktype = 1;
						}
					}
					else if (randattack == 2)
					{
						if (lastattacktype == 2)
						{
							int subrandattack = Main.rand.Next(1, 2 + 1);
							if (subrandattack == 1)
							{
								attacktype = 1;
								lastattacktype = 1;
							}
							else
							{
								attacktype = 3;
								lastattacktype = 3;
							}
						}
						else
						{
							attacktype = 2;
							lastattacktype = 2;
						}
					}
					else if (randattack == 3)
					{
						if (lastattacktype == 3)
						{
							int subrandattack = Main.rand.Next(1, 2 + 1);
							if (subrandattack == 1)
							{
								attacktype = 1;
								lastattacktype = 1;
							}
							else
							{
								attacktype = 2;
								lastattacktype = 2;
							}
						}
						else
						{
							attacktype = 3;
							lastattacktype = 3;
						}
					}
				}
				if ((Main.expertMode && NPC.life > NPC.lifeMax * 0.75) || !Main.expertMode && NPC.life > NPC.lifeMax * 0.66) //npc above 66% in normal and above 75% in expert
				{
					int randattack = Main.rand.Next(1, 2 + 1);
					if (randattack == 1)
					{
						if (lastattacktype == 1)
						{
							attacktype = 2;
							lastattacktype = 2;
						}
						else
						{
							attacktype = 1;
							lastattacktype = 1;
						}
					}
					else if (randattack == 2)
					{
						if (lastattacktype == 2)
						{
							attacktype = 1;
							lastattacktype = 1;
						}
						else
						{
							attacktype = 2;
							lastattacktype = 2;
						}
					}
				}

				NPC.netUpdate = true;
			}

			if (frenzy == false) //normal cycle
			{
				if (attacktype == 1) //beams
				{
					if (NPC.ai[0] >= 120 && NPC.ai[0] < 180)
					{
						if (NPC.ai[0] >= 179) //stop
						{
                            NPC.velocity *= 0.0001f;
                        }
						else //move
						{
							float speed = 15f;
							float inertia = 5f;

							distanceOverPlayer.Normalize();
							distanceOverPlayer *= speed;
							NPC.velocity = (NPC.velocity * (inertia - 1) + distanceOverPlayer) / inertia; //go above player
						}
					}

					if (NPC.ai[0] >= 240 && NPC.ai[0] < 360) //120 ticks 
					{
						if (NPC.ai[0] == 240 && Main.netMode != NetmodeID.MultiplayerClient) //inital
                        {
							for (int i = 0; i < 6; i++) //i decides the offset
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 0, 0, 
									Mod.Find<ModProjectile>("BeamBig").Type, 20 / 2, 8f, Main.myPlayer, NPC.whoAmI, i);
							}
                        }

						//sound
						if (NPC.ai[0] == 240 || NPC.ai[0] == 300)
						{
							SoundEngine.PlaySound(SoundID.Item93, NPC.Center);
						}
					}
					if (NPC.ai[0] >= 360)
					{
						NPC.ai[0] = 0;
					}
				}

				if (attacktype == 2) //sweep
				{
					if (NPC.ai[0] >= 120 && NPC.ai[0] < 180) //go to top left or right
					{
						if (NPC.ai[0] >= 179) //stop
						{
                            NPC.velocity *= 0.0001f;
                        }
						else //move
						{
							float speed = 20f;
							float inertia = 5f;

							if (sideOfAttack == 2) //left
							{
								distanceDiagonalLeftOfPlayer.Normalize();
								distanceDiagonalLeftOfPlayer *= speed;
								NPC.velocity = (NPC.velocity * (inertia - 1) + distanceDiagonalLeftOfPlayer) / inertia; //go above left of player
							}
							else if (sideOfAttack == 1) //right
							{
								distanceDiagonalRightOfPlayer.Normalize();
								distanceDiagonalRightOfPlayer *= speed;
								NPC.velocity = (NPC.velocity * (inertia - 1) + distanceDiagonalRightOfPlayer) / inertia; //go above right of player
							}
						}
					}

					if (NPC.ai[0] >= 200 && NPC.ai[0] < 230) //30 ticks
					{
						NPC.velocity.Y -= 0.2f;

						if (NPC.ai[0] == 200)
						{
							sweepX = distanceBelowPlayer.X / 30;
							sweepY = distanceBelowPlayer.Y / 30;
						}
					}
					if (NPC.ai[0] == 230)
					{
						//target below to give player chance to jump over
						NPC.velocity.X = sweepX;

						NPC.velocity.Y = sweepY;

						//caps
						if (NPC.velocity.X > 30)
						{
							NPC.velocity.X = 30;
						}
						if (NPC.velocity.X < -30)
						{
							NPC.velocity.X = -30;
						}
						if (NPC.velocity.Y < 5)
						{
							NPC.velocity.Y = 5;
						}
						if (NPC.velocity.Y > 30)
						{
							NPC.velocity.Y = 30;
						}

						sweepheight = NPC.velocity.Y;
					}

					if (NPC.ai[0] > 230) //sweep for 75 ticks
					{
						NPC.velocity.Y -= sweepheight / 40;
					}

					if (NPC.ai[0] >= 305)
					{
						NPC.velocity *= 0;
						NPC.ai[0] = 0;
					}
				}

				if (attacktype == 3) //dash
				{
					if (NPC.ai[0] >= 120 && NPC.ai[0] < 180) //go to right
					{
						if (NPC.ai[0] >= 179) //stop
						{
                            NPC.velocity *= 0.0001f;
                        }
						else //move
						{
							float speed = 20f;
							float inertia = 5f;

							if (sideOfAttack == 2) //left
							{
								distanceLeftOfPlayer.Normalize();
								distanceLeftOfPlayer *= speed;
								NPC.velocity = (NPC.velocity * (inertia - 1) + distanceLeftOfPlayer) / inertia; //go of player
							}
							else if (sideOfAttack == 1) //right
							{
								distanceRightOfPlayer.Normalize();
								distanceRightOfPlayer *= speed;
								NPC.velocity = (NPC.velocity * (inertia - 1) + distanceRightOfPlayer) / inertia; //go right of player
							}
						}
					}
					if (NPC.ai[0] >= 200 && NPC.ai[0] < 230) //30 ticks
					{
						if (sideOfAttack == 2) //left of player
						{
							NPC.velocity.X -= 0.2f; //backup left
						}
						else if (sideOfAttack == 1) //right of player
						{
							NPC.velocity.X += 0.2f; //backup right
						}
					}
					if (NPC.ai[0] == 230)
					{
						//choose direction to dash
						if (sideOfAttack == 2) //left of player
						{
							NPC.velocity.X = 40; //dash right
						}
						else if (sideOfAttack == 1) //right of player
						{
							NPC.velocity.X = -40; //dash left
						}

						NPC.velocity.Y = 0;
					}

					if (NPC.ai[0] > 230) //dash for 75 ticks
					{
						NPC.velocity.X *= 0.95f;
					}

					if (NPC.ai[0] >= 305)
					{
                        NPC.velocity *= 0.0001f;
                        NPC.ai[0] = 0;
					}
				}
				if (attacktype == 4) //thunder
				{
					if (NPC.ai[0] % 5 == 0 && NPC.ai[0] >= 120 && NPC.ai[0] < 240) //multiple of 5
					{
						for (int i = 0; i < 6; i++) //make bursts of electricity for warning
						{
							Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
							Dust d = Dust.NewDustPerfect(NPC.position + new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(0, NPC.height)), DustID.Electric, speed, Scale: 1f); //Makes dust in a messy circle
							d.noGravity = true;
						}
					}

					if (NPC.ai[0] >= 120 && NPC.ai[0] < 180)
					{
						if (NPC.ai[0] >= 179) //stop
						{
                            NPC.velocity *= 0.0001f;
                        }
						else //move
						{
							float speed = 15f;
							float inertia = 5f;

							if (sideOfAttack == 2) //left
							{
								distanceDiagonalLeftOfPlayer.Normalize();
								distanceDiagonalLeftOfPlayer *= speed;
								NPC.velocity = (NPC.velocity * (inertia - 1) + distanceDiagonalLeftOfPlayer) / inertia; //go above left of player
							}
							else if (sideOfAttack == 1) //right
							{
								distanceDiagonalRightOfPlayer.Normalize();
								distanceDiagonalRightOfPlayer *= speed;
								NPC.velocity = (NPC.velocity * (inertia - 1) + distanceDiagonalRightOfPlayer) / inertia; //go above right of player
							}
						}
					}
					if (NPC.ai[0] >= 240) //for 120 ticks dash across screen(if on it)
					{
						//choose direction to dash
						if (sideOfAttack == 2) //left of player
						{
							NPC.velocity.X = 10; //move right
						}
						else if (sideOfAttack == 1) //right of player
						{
							NPC.velocity.X = -10; //move left
						}

						if (NPC.ai[0] % 12 == 0 && Main.netMode != NetmodeID.MultiplayerClient) 
						{
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 0, 5.5f, Mod.Find<ModProjectile>("KrackoLightning").Type, 25 / 2, 2f, Main.myPlayer, 0, 0);
							SoundEngine.PlaySound(SoundID.Item122, NPC.Center);
						}
					}
					if (NPC.ai[0] >= 360) //end
					{
						NPC.velocity *= 0;
						NPC.ai[0] = 0;
					}
				}
			}




			else if (frenzy == true) //hardened cycle
            {
				if (attacktype == 1) //beams
				{
					if (NPC.ai[0] >= 60 && NPC.ai[0] < 90)
					{
						if (NPC.ai[0] >= 89) //stop
						{
							NPC.velocity *= 0.0001f;
						}
						else //move
						{
							NPC.velocity = distanceOverPlayer / 10;
						}
					}

					if (NPC.ai[0] >= 120 && NPC.ai[0] < 180) //60 ticks
					{
                        if (NPC.ai[0] == 120 && Main.netMode != NetmodeID.MultiplayerClient) //inital
                        {
                            for (int i = 0; i < 6; i++) //i decides the offset
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 0, 0,
                                    Mod.Find<ModProjectile>("BeamBig").Type, 20 / 2, 8f, Main.myPlayer, NPC.whoAmI, i);
                            }
                        }

                        //sound
                        if (NPC.ai[0] == 120 || NPC.ai[0] == 150)
						{
							SoundEngine.PlaySound(SoundID.Item93, NPC.Center);
						}
					}
					if (NPC.ai[0] >= 180)
					{
						NPC.ai[0] = 0;
					}
				}

				if (attacktype == 2) //sweep
				{
					if (NPC.ai[0] >= 60 && NPC.ai[0] < 90) //go to top left or right
					{
						if (NPC.ai[0] >= 89) //stop
						{
                            NPC.velocity *= 0.0001f;
                        }
						else //move
						{
							if (sideOfAttack == 2) //left
							{
								NPC.velocity = distanceDiagonalLeftOfPlayer / 10;
							}
							else if (sideOfAttack == 1) //right
							{
								NPC.velocity = distanceDiagonalRightOfPlayer / 10;
							}
						}
					}

					if (NPC.ai[0] >= 90 && NPC.ai[0] < 105) //15 ticks
					{
						NPC.velocity.Y -= 0.2f;

						if (NPC.ai[0] == 90)
						{
							sweepX = distanceBelowPlayer.X / 20;
							sweepY = distanceBelowPlayer.Y / 20;
						}
					}
					if (NPC.ai[0] == 105)
					{
						//target below to give player chance to jump over
						NPC.velocity.X = sweepX;

						NPC.velocity.Y = sweepY;

						//caps(most doubled)
						if (NPC.velocity.X > 60)
						{
							NPC.velocity.X = 60;
						}
						if (NPC.velocity.X < -60)
						{
							NPC.velocity.X = -60;
						}
						if (NPC.velocity.Y < 5)
						{
							NPC.velocity.Y = 5;
						}
						if (NPC.velocity.Y > 60)
						{
							NPC.velocity.Y = 60;
						}

						sweepheight = NPC.velocity.Y;
					}

					if (NPC.ai[0] > 105) //sweep for 50 ticks
					{
						NPC.velocity.Y -= sweepheight / 25;
					}

					if (NPC.ai[0] >= 155)
					{
						NPC.velocity *= 0;
						NPC.ai[0] = 0;
					}
				}

				if (attacktype == 3) //dash
				{
					if (NPC.ai[0] >= 60 && NPC.ai[0] < 90) //30 ticks
					{
						if (NPC.ai[0] >= 89) //stop
						{
                            NPC.velocity *= 0.0001f;
                        }
						else //move
						{
							if (sideOfAttack == 2) //left
							{
								NPC.velocity = distanceLeftOfPlayer / 10;
							}
							else if (sideOfAttack == 1) //right
							{
								NPC.velocity = distanceRightOfPlayer / 10;
							}
						}
					}
					if (NPC.ai[0] >= 90 && NPC.ai[0] < 105) //15 ticks
					{
						if (sideOfAttack == 2) //left of player
						{
							NPC.velocity.X -= 0.2f; //backup left
						}
						else if (sideOfAttack == 1) //right of player
						{
							NPC.velocity.X += 0.2f; //backup right
						}
					}
					if (NPC.ai[0] == 105)
					{
						//choose direction to dash
						if (sideOfAttack == 2) //left of player
						{
							NPC.velocity.X = 60; //dash right
						}
						else if (sideOfAttack == 1) //right of player
						{
							NPC.velocity.X = -60; //dash left
						}

						NPC.velocity.Y = 0;
					}

					if (NPC.ai[0] > 105) //dash for 50 ticks
					{
						NPC.velocity.X *= 0.92f;
					}

					if (NPC.ai[0] >= 155)
					{
                        NPC.velocity *= 0.0001f;
                        NPC.ai[0] = 0;
					}
				}
				if (attacktype == 4) //thunder
				{
					if (NPC.ai[0] % 5 == 0 && NPC.ai[0] >= 60 && NPC.ai[0] < 120) //multiple of 5
					{
						for (int i = 0; i < 6; i++) //make bursts of electricity for warning
						{
							Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
							Dust d = Dust.NewDustPerfect(NPC.position + new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(0, NPC.height)), DustID.Electric, speed, Scale: 1f); //Makes dust in a messy circle
							d.noGravity = true;
						}
					}

					if (NPC.ai[0] >= 60 && NPC.ai[0] < 90)
					{
						if (NPC.ai[0] >= 89) //stop
						{
                            NPC.velocity *= 0.0001f;
                        }
						else //move
						{
							if (sideOfAttack == 2) //left
							{
								NPC.velocity = distanceDiagonalLeftOfPlayer / 10;
							}
							else if (sideOfAttack == 1) //right
							{
								NPC.velocity = distanceDiagonalRightOfPlayer / 10;
							}
						}
					}
					if (NPC.ai[0] >= 120) //for 30 ticks dash across screen(if on it)
					{
						//choose direction to dash
						if (sideOfAttack == 2) //left of player
						{
							NPC.velocity.X = 20; //move right
						}
						else if (sideOfAttack == 1) //right of player
						{
							NPC.velocity.X = -20; //move left
						}

						if (NPC.ai[0] % 6 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
						{
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 0, 5.5f, Mod.Find<ModProjectile>("KrackoLightning").Type, 25 / 2, 2f, Main.myPlayer, 0, 0);
							SoundEngine.PlaySound(SoundID.Item122, NPC.Center);
						}
					}
					if (NPC.ai[0] >= 150) //end
					{
                        NPC.velocity *= 0.0001f;
                        NPC.ai[0] = 0;
					}
				}
			}
		}

		public override void FindFrame(int frameHeight) // animation
        {
			if (animation == 0) //slow
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 12)
				{
					NPC.frame.Y = 0; //normal
				}
				else
				{
					NPC.frame.Y = frameHeight; //pulse
				}
				if (NPC.frameCounter >= 24)
				{
					NPC.frameCounter = 0;
				}
			}
			if (animation == 1) //fast
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 4)
				{
					NPC.frame.Y = 0; //normal
				}
				else
				{
					NPC.frame.Y = frameHeight; //pulse
				}
				if (NPC.frameCounter >= 8)
				{
					NPC.frameCounter = 0;
				}
			}
		}

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedKrackoBoss, -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<KrackoBag>())); //only drops in expert

            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert()); //checks if not expert
            LeadingConditionRule masterMode = new LeadingConditionRule(new Conditions.IsMasterMode()); //checks if master mode

            notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.Cloud, 1, 50, 50)); //50 clouds

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<KrackoMask>(), 7));

            // Drop one of these 3 items with 100% chance
            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<AirWalkerHelmet>(), ModContent.ItemType<AirWalkerBreastplate>(), ModContent.ItemType<AirWalkerLeggings>()));

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<PeeWeePole>(), 4));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<KrackoTrophy>(), 10)); //drop trophy

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeables.BossRelics.KrackoRelic>()));

            masterMode.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.Kracko.KrackoPetItem>(), 4));

            // add the rules
            npcLoot.Add(notExpertRule);
            npcLoot.Add(masterMode);
        }

		public override void BossLoot(ref string name, ref int potionType)
		{
			name = "Kracko"; //_ has been defeated!
			potionType = ItemID.LesserHealingPotion; //potion it drops
		}
		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
			scale = 1.5f;
			return true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
				for (int i = 0; i < 20; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(20f, 20f); //circle
					Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.LilStar>(), speed, Scale: 2f); //Makes dust in a messy circle
					d.noGravity = true;
				}
				for (int i = 0; i < 15; i++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
                    Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, speed, Main.rand.Next(11, 13), Scale: 2f); //double jump smoke
                }
			}	
			else
            {
				for (int i = 0; i < 2; i++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(3f, 3f); //circle
					Dust d = Dust.NewDustPerfect(NPC.position + new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(0, NPC.height)), DustID.Cloud, speed, Scale: 1f); //Makes dust in a messy circle
					d.noGravity = true;
				}
			}
        }

		public override Color? GetAlpha(Color lightColor)
		{
			if (attacktype == 4 && NPC.ai[0] >= 120 && !frenzy) //no frenzy
			{
				int darkness = 255 - (((int)NPC.ai[0] - 120) * 9); //subtract 9 from the value each time

				if (darkness < 64) //minimum
				{
					darkness = 64;
				}

				return new Color(darkness, darkness, darkness); //darken for thunder
			}
            else if (attacktype == 4 && NPC.ai[0] >= 60 && frenzy) //frenzy
            {
                int darkness = 255 - (((int)NPC.ai[0] - 60) * 18); //subtract 18 from the value each time

                if (darkness < 64) //minimum
                {
                    darkness = 64;
                }

                return new Color(darkness, darkness, darkness); //darken for thunder
            }
            else
			{
				return Color.White; //make it unaffected by light
			}
		}

        // This npc uses an additional texture for drawing
        public static Asset<Texture2D> EyeTexture;

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            EyeTexture = ModContent.Request<Texture2D>("KirboMod/NPCs/KrackoEye");

			Player player = Main.player[NPC.target];

			//rotato
			// First, calculate a Vector pointing towards what you want to look at
			Vector2 distance = player.Center - NPC.Center;
			// Second, use the ToRotation method to turn that Vector2 into a float representing a rotation in radians.
			float desiredRotation = distance.ToRotation();

			if ((transitioning == true && NPC.ai[2] > 0 && NPC.ai[2] <= 180))
            {
				desiredRotation = MathHelper.ToRadians(90); //face down when transforming
                EyeTexture = ModContent.Request<Texture2D>("KirboMod/NPCs/KrackoEyeAngry"); //get mad
            }

			if (frenzy == true)
            {
                EyeTexture = ModContent.Request<Texture2D>("KirboMod/NPCs/KrackoEyeAngry"); //get mad
            }

			/*if (frenzy == true)
            {
				eye = Mod.GetTexture("NPCs/KrackoEyeAngry"); //get mad
			}*/

			//Draw eye(look at player)
			Texture2D eye = EyeTexture.Value;
			spriteBatch.Draw(eye, NPC.Center - Main.screenPosition, null, new Color(255, 255, 255), desiredRotation, new Vector2(29, 29), 1f, SpriteEffects.None, 0f);
        }
    }
}
