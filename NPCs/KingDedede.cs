using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using KirboMod.Items.Zero;
using KirboMod.Items.KingDedede;
using System.Security.Policy;
using Terraria.GameContent.Personalities;
using KirboMod.Items.Armor.AirWalker;
using KirboMod.Bestiary;
using KirboMod.Systems;
using Terraria.DataStructures;
using Terraria.ModLoader.Config;
using System.IO;
using Terraria.GameContent.UI;

namespace KirboMod.NPCs
{

	[AutoloadBossHead]
	public class KingDedede : ModNPC
	{ 
		int attack = 0; //controls dedede's attack pattern

        int attacktype = 0; //which attack dedede will use 

        int lastattacktype = 0; //sets last attack type

		int phase = 1; //determines what phase is dedede on

        int repeathammer = 0; //for multi hammer swings

        int phasethreefasterer = 0; //speeds up attacks in phase 3

		int animation = 0; //selection of frames to cycle through
        public override void SetStaticDefaults() {
			// DisplayName.SetDefault("King Dedede");
			Main.npcFrameCount[NPC.type] = 25;

            // Add this in for bosses that have a summon item, requires corresponding code in the item
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
			{
				CustomTexturePath = "KirboMod/NPCs/BestiaryTextures/KingDededePortrait",
				PortraitScale = 1f, // Portrait refers to the full picture when clicking on the icon in the bestiary
				PortraitPositionYOverride = 30,
				Position = new Vector2(20, 70),
				
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true; //immune to not mess up movement
        }

		public override void SetDefaults() {
			NPC.width = 150;
			NPC.height = 150;
			DrawOffsetY = 52;
			NPC.damage = 50;
			NPC.noTileCollide = false;
			NPC.defense = 16; 
			NPC.lifeMax = 8000;
			NPC.HitSound = SoundID.NPCHit1; //organic
			NPC.DeathSound = SoundID.NPCDeath27; //tortoise
			NPC.value = Item.buyPrice(0, 3, 0, 0); // money it drops
			NPC.knockBackResist = 0f;
			NPC.aiStyle = -1;
			NPC.boss = true;
			NPC.noGravity = false;
			NPC.lavaImmune = true;
            //Music = MusicID.Boss5;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/KingDedede");
            }

            //bossBag/* tModPorter Note: Removed. Spawn the treasure bag alongside other loot via npcLoot.Add(ItemDropRule.//bossBag(type)) */ = ModContent.ItemType<Items.KingDedede.KingDededeBag>();
            NPC.friendly = false;
            NPC.buffImmune[BuffID.Confused] = true;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
			NPC.lifeMax = (int)(NPC.lifeMax * 0.7f * balance);
		    NPC.damage = (int)(NPC.damage * 1f);
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				new SurfaceBackgroundProvider(), //I totally didn't steal this code
				// Sets the spawning conditions of this NPC that is listed in the bestiary.

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("The mighty-yet-greedy ruler of a distant castle! He loves to snack and show those who mistreat him the boot! Especially people who take his precious life-saving brooches!"),
			});
		}

        public override void SendExtraAI(BinaryWriter writer) //for syncing non NPC.ai[] stuff
        {
            writer.Write(animation);
            writer.Write(lastattacktype);
            writer.Write(attacktype);
            writer.Write(phasethreefasterer);
            writer.Write(repeathammer);
            writer.Write(phase);
            writer.Write(attack);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            animation = reader.ReadInt32();
            lastattacktype = reader.ReadInt32();
            attacktype = reader.ReadInt32();
            phasethreefasterer = reader.ReadInt32();
            repeathammer = reader.ReadInt32();
            phase = reader.ReadInt32();
            attack = reader.ReadInt32();
        }

        public override void AI() //cycles through each tick
        {
			Player player = Main.player[NPC.target]; //the player the npc is targeting=
			NPC.spriteDirection = NPC.direction;

			//cap life
			if (NPC.life >= NPC.lifeMax)
            {
				NPC.life = NPC.lifeMax;
            }

			//Despawning
			if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active) //despawning
			{
				attack = 0;
				NPC.TargetClosest(false);
				NPC.noTileCollide = true;

				if (phase == 4)
                {
					NPC.velocity.Y -= 0.2f; //go up
                }

				if (NPC.timeLeft > 60)
				{
					NPC.timeLeft = 60;
					return;
				}
			}
			else
            {
				AttackPattern();

				if ((attack >= 60 - phasethreefasterer && attacktype == 4) == false && phase != 4) //not slamming or possessed
                {
					CheckPlatform(player);

					//for stepping up tiles
					Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
				}
			}
		}

		private void CheckPlatform(Player player) //trust me this is totally unique and original code and definitely not stolen from Spirit Mod's public source code(thx so much btw you don't know the hell I went through with this)
		{
			bool onplatform = true;
			for (int i = (int)NPC.position.X; i < NPC.position.X + NPC.width; i += NPC.width / 4)
			{ //check tiles beneath the boss to see if they are all platforms
				Tile tile = Framing.GetTileSafely(new Point((int)NPC.position.X / 16, (int)(NPC.position.Y + NPC.height + 8) / 16));
				if (!TileID.Sets.Platforms[tile.TileType])
					onplatform = false;
			}
			if (onplatform && (NPC.Center.Y < player.position.Y - 75)) //if they are and the player is lower than the boss, temporarily let the boss ignore tiles to go through them
			{
				NPC.noTileCollide = true;
			}
			else
			{
				NPC.noTileCollide = false;
			}
		}

		private void AttackPattern()
		{
			Player player = Main.player[NPC.target];
			Vector2 distance = player.Center - NPC.Center;

            //if low enough then change phases
            if (NPC.life <= NPC.lifeMax * (Main.expertMode ? 0.75 : 0.50) && attack == 0)
            {
				if (phase == 1)
                {
					phase = 2;
                }
            }
			if (NPC.life <= NPC.lifeMax * (Main.expertMode ? 0.50 : 0.25) && attack == 0)
			{
				if (phase == 2)
				{
					phase = 3;
					phasethreefasterer = 30; //speed up attacks
                }
			}

			//phase 4 change(can happen anytime)
			if (NPC.life <= NPC.lifeMax * 0.25 && Main.expertMode)
			{
				if (phase != 4)
				{
					attack = 0; //reset
					phase = 4;
                }
			}

			if (phase == 3 && NPC.life <= NPC.lifeMax * (Main.expertMode ? 0.35 : 0.10))
            {
				phasethreefasterer = 45; //speed up attacks even more
			}

			if (attack == 0 & phase >= 2) //for multihammer
            {
				repeathammer = 1;
            }

			attack++; //go up

			if (phase < 4) //not phase 4
			{
				if (attack < 60 - phasethreefasterer) //face player target
				{
					NPC.TargetClosest(true);
				}

				if (attack == 59 - phasethreefasterer) //random attack (30 on expert)
				{
					//make a line from dedede through player to see if no tiles
					bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
					float lineOfDistance = Vector2.Distance(player.Center, NPC.Center);

					bool stuck = false; //determines if stuck between two tiles

					for (float i = 0; i < NPC.width; i++) //counter for stuck
                    {
						Point abovenpc = new Vector2(NPC.position.X + i, NPC.position.Y).ToTileCoordinates(); //all tiles above npc
						Point belownpc = new Vector2(NPC.position.X + i, NPC.position.Y + NPC.height).ToTileCoordinates(); //all tiles below npc

						//head against celling
						if (WorldGen.SolidTile(abovenpc.X, abovenpc.Y) == true && Main.tile[belownpc.X, belownpc.Y].HasTile)
                        {
							stuck = true;
                        }
					}
					
					//too far, can't reach, or too low (don't ask why can't reach uses regular y system instead of inverted I do not know)
					if (lineOfSight == false || NPC.position.Y > (player.Center.Y - (player.height / 2) + 400) || lineOfDistance >= 1000 || stuck)
					{
						attacktype = 4; //slam
						lastattacktype = 4;
					}
					else //random attack
					{
						if (Main.netMode != NetmodeID.MultiplayerClient) //sync cause random
						{
							int randattack = Main.rand.Next(1, 4 + 1);

							if (randattack == 1) //dive
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
							else if (randattack == 2) //hammer
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
							else if (randattack == 3) //gordos
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
							else if (randattack == 4) //stomp
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

							NPC.netUpdate = true;  //sync cause random
						}
                    }
				}

				if (attacktype == 1) //DIVE
				{
					if (attack == 60 - phasethreefasterer)
					{
                        animation = 1; //run
                        NPC.velocity.Y = -4;
                    }
					if (attack >= 90 - phasethreefasterer && attack < 270 - phasethreefasterer)
					{
						if (attack % (phase == 3 ? 15 : 30) == 0) //multiple of 30(or 15)
						{
							NPC.TargetClosest(true); //target player

                            if (phase == 3) //faster
                            {
                                NPC.velocity.X = NPC.direction * 12; //go 6 in the direction of npc(facing player)

                            }
                            else
                            {
                                NPC.velocity.X = NPC.direction * 8; //go 6 in the direction of npc(facing player)

                            }
                        }
						else
						{
							NPC.TargetClosest(false); //don't face player
						}

						if (NPC.oldVelocity.X == 0 && NPC.velocity.Y == 0 && attack > 90) //jump if not moving X wise
						{
							NPC.velocity.Y = -10;
                            
                        }

						if (Math.Abs(distance.X) <= 200 && (distance.Y <= 100 && distance.Y >= -150)) //range
						{
							attack = 300 - phasethreefasterer;
						}
					}
					if (attack == 270 - phasethreefasterer) //3 seconds up
					{
                        NPC.velocity.X *= 0;
                        animation = 0; //stand	

                        attack = 0;
					}
					if (attack == 300 - phasethreefasterer) //dive
					{
						NPC.TargetClosest(false);

                        NPC.velocity.Y = -4;
                        animation = 2; //dive


                        if (phase == 3) //faster
                        {
                            NPC.velocity.X = NPC.direction * 16; //go 16 in the direction of npc(facing player)

                        }
                        else
                        {
                            NPC.velocity.X = NPC.direction * 12; //go 12 in the direction of npc(facing player)

                        }

                        SoundEngine.PlaySound(SoundID.NPCDeath8, NPC.Center); //beast grunt 
					}
					if (attack > 300 - phasethreefasterer)
					{
						if (NPC.velocity.Y == 0) //check if not in air
						{
							NPC.velocity.X *= 0.95f;
						}

						animation = 2; //dive
					}
					if (attack >= 360 - phasethreefasterer) //restart from dive
					{
                        NPC.velocity.X *= 0;
                        animation = 0; //stand
                        attack = 0;
					}
				}

				if (attacktype == 2) //HAMMER
				{
					if (attack == 60 - phasethreefasterer)
					{
                        NPC.velocity.Y = -4;
                        animation = 3; //draw hammer
                    }
					if (attack >= 90 - phasethreefasterer && attack < 270 - phasethreefasterer)
					{
                        animation = 4; //run with hammer	

                        if (attack % (phase == 3 ? 7 : 15) == 0) //multiple of 15(or 7)
						{
							NPC.TargetClosest(true); //target player

                            if (phase == 3) //faster
                            {
                                NPC.velocity.X = NPC.direction * 10; //go 6 in the direction of npc(facing player)

                            }
                            else
                            {
                                NPC.velocity.X = NPC.direction * 6; //go 6 in the direction of npc(facing player)

                            }
                        }
						else
						{
							NPC.TargetClosest(false); //don't face player
						}

						if (Math.Abs(distance.X) <= 150) //range
						{
							attack = 300 - phasethreefasterer;
						}
					}
					if (attack == 270 - phasethreefasterer) //3 seconds of running up
					{
                        NPC.velocity.X *= 0;
                        animation = 0; //stand	
                        attack = 0;
					}
					if (attack == 300 - phasethreefasterer) //charge swing
					{
						NPC.TargetClosest(false);
						SoundEngine.PlaySound(SoundID.NPCDeath8.WithPitchOffset(0.5f), NPC.Center); //beast grunt (high pitch)

                        NPC.velocity.X *= 0;
                        animation = 5; //ready swing   
                    }


					//IF ALREADY MULTISWUNG
					if (attack == (phase == 3 ? 307 : 315) - phasethreefasterer && phase >= 2 && repeathammer == 0) //jump if player is high and multiswing(quicker in phase 3)
					{
						if (distance.Y < -150) //too high
						{
                            NPC.velocity.Y = distance.Y / (phase == 3 ? 5 : 15); //jump depending on Y distance(quicker in phase 3)


                            //minimum
                            if (NPC.velocity.Y >= -15)
                            {
                                NPC.velocity.Y = -15;

                            }
                            //maximum
                            if (NPC.velocity.Y <= -30)
                            {
                                NPC.velocity.Y = -30;

                            }
                        }
					}

					if (attack == (phase == 3 ? 315 : 330) - phasethreefasterer && phase >= 2 && repeathammer == 0) //swing again after multiswinging
					{
						if (Main.netMode != NetmodeID.MultiplayerClient) //execute on server( or singleplayer) only
						{
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 120, 20), NPC.velocity *= 0, Mod.Find<ModProjectile>("BonkersSmash").Type, 60 / 2, 8f, Main.myPlayer, 0, 0);
						}
                        NPC.velocity.Y *= 0; // stop rising
                        animation = 6; //swing	

                        attack = (phase == 3 ? 371 : 401); //just after multiswing
					}
					//IF ALREADY MULTISWUNG


					if (attack == (phase == 3 ? 315 : 330) - phasethreefasterer) //jump if player is high(quicker in phase 3)
					{
						if (distance.Y < -150) //too high
						{
                            NPC.velocity.Y = distance.Y / (phase == 3 ? 5 : 15); //jump depending on Y distance(quicker in phase 3)


                            //minimum
                            if (NPC.velocity.Y >= -15)
                            {
                                NPC.velocity.Y = -15;

                            }
                            //maximum
                            if (NPC.velocity.Y <= -30)
                            {
                                NPC.velocity.Y = -30;

                            }
                        }
					}
					if (attack == (phase == 3 ? 330 : 360) - phasethreefasterer) //swing
					{
						if (Main.netMode != NetmodeID.MultiplayerClient) //execute on server( or singleplayer) only
						{
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 120, 20), NPC.velocity *= 0, Mod.Find<ModProjectile>("BonkersSmash").Type, 60 / 2, 8f, Main.myPlayer, 0, 0);
						}
                        NPC.velocity.Y *= 0; // stop rising
                        animation = 6; //swing	
                    }

					//multiswing
					if (phase == 2 || phase == 3) //only on phases 2 or 3
					{
						if (attack == (phase == 3 ? 340 : 370) - phasethreefasterer)
						{
							NPC.TargetClosest(false);

                            NPC.velocity.X *= 0;
                            animation = 5; //ready swing    
                        }
						if (attack == (phase == 3 ? 350 : 380) - phasethreefasterer)
						{
							if (Main.netMode != NetmodeID.MultiplayerClient) //execute on server( or singleplayer) only
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 120, 20), NPC.velocity *= 0, Mod.Find<ModProjectile>("BonkersSmash").Type, 60 / 2, 8f, Main.myPlayer, 0, 0);
							}
                            NPC.velocity.Y *= 0; // stop rising
                            animation = 6; //swing	
                        }
						if (attack == (phase == 3 ? 360 : 390) - phasethreefasterer)
						{
							NPC.TargetClosest(false);

                            NPC.velocity.X *= 0f;
                            animation = 5; //ready swing   
                        }
						if (attack == (phase == 3 ? 370 : 400) - phasethreefasterer)
						{
							if (Main.netMode != NetmodeID.MultiplayerClient) //execute on server( or singleplayer) only
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 120, 20), NPC.velocity *= 0, Mod.Find<ModProjectile>("BonkersSmash").Type, 60 / 2, 8f, Main.myPlayer, 0, 0);
							}
                            NPC.velocity.Y *= 0f; // stop rising
                            animation = 6; //swing	
                        }
					}

					if (attack == (phase == 3 ? 400 : 430) - phasethreefasterer && repeathammer == 1 && phase >= 2) //cycle back again
					{
						attack = 90;
						repeathammer = 0;
					}

					if (attack == 430 - phasethreefasterer && phase == 3) //restart from swing (phase 3)
					{
						attack = 0;

                        NPC.velocity.X *= 0f;
                        animation = 0; //stand    
                    }
					if (attack == 460 - phasethreefasterer && phase == 2) //restart from swing (phase 2)
					{
						attack = 0;

                        NPC.velocity.X *= 0f;
                        animation = 0; //stand    
                    }
					if (attack == 420 - phasethreefasterer && phase == 1) //restart from swing (phase 1)
					{
						attack = 0;

						NPC.velocity.X *= 0f;
                        animation = 0; //stand    
                    }
				}

                //GORDOS

                if (attacktype == 3) 
				{
					if (attack >= 60 - phasethreefasterer)
					{
						NPC.TargetClosest(true); //target player
					}

					if (attack == 60 - phasethreefasterer)
					{
						animation = 7; //ready gordo
                    }

					//low on health
					if (attack == 90 - phasethreefasterer && phase == 3)
					{
                        animation = 8; //swing gordo

                        Vector2 distanceahead = player.Center - (NPC.Center + new Vector2(NPC.direction * 100, 0));
						distanceahead.Normalize();
						distanceahead *= 15;
						if (Main.netMode != NetmodeID.MultiplayerClient) //execute on server( or singleplayer) only
						{
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 100, 0), distanceahead, Mod.Find<ModProjectile>("BouncyGordo").Type, 30 / 2, 4f, Main.myPlayer, 0, 0);
						}
						SoundEngine.PlaySound(SoundID.Item10.WithVolumeScale(2), NPC.Center); //impact
					}
					if (attack == 100 - phasethreefasterer && phase == 3)
					{
                        animation = 7; //ready gordo
                    }
					//low on health

					if (attack == 120 - phasethreefasterer)
					{
                        animation = 8; //swing gordo

                        Vector2 distanceahead = player.Center - (NPC.Center + new Vector2(NPC.direction * 100, 0));
                        distanceahead.Normalize();
						distanceahead *= 15;

						if (Main.netMode != NetmodeID.MultiplayerClient) //execute on server( or singleplayer) only
						{
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 100, 0), distanceahead, Mod.Find<ModProjectile>("BouncyGordo").Type, 30 / 2, 4f, Main.myPlayer, 0, 0);
						}
						SoundEngine.PlaySound(SoundID.Item10.WithVolumeScale(2), NPC.Center); //impact
					}
					if (attack == 130 - phasethreefasterer)
					{
                        animation = 7; //ready gordo
                    }

					//low on health
					if (attack == 150 - phasethreefasterer && (phase == 2 || phase == 3))
					{
                        animation = 8; //swing gordo

                        Vector2 distanceahead = player.Center - (NPC.Center + new Vector2(NPC.direction * 100, 0));
						distanceahead.Normalize();
						distanceahead *= 15;

                        if (Main.netMode != NetmodeID.MultiplayerClient) //execute on server( or singleplayer) only
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 100, 0), distanceahead, Mod.Find<ModProjectile>("BouncyGordo").Type, 30 / 2, 4f, Main.myPlayer, 0, 0);
                        }
                        SoundEngine.PlaySound(SoundID.Item10.WithVolumeScale(2), NPC.Center); //impact
					}
					if (attack == 160 - phasethreefasterer && (phase == 2 || phase == 3))
					{
                        animation = 7; //ready gordo
                    }
					//low on health

					if (attack == 180 - phasethreefasterer)
					{
                        animation = 8; //swing gordo
                        Vector2 distanceahead = player.Center - (NPC.Center + new Vector2(NPC.direction * 100, 0));
                        distanceahead.Normalize();
						distanceahead *= 15;

                        if (Main.netMode != NetmodeID.MultiplayerClient) //execute on server( or singleplayer) only
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 100, 0), distanceahead, Mod.Find<ModProjectile>("BouncyGordo").Type, 30 / 2, 4f, Main.myPlayer, 0, 0);
                        }
                        SoundEngine.PlaySound(SoundID.Item10.WithVolumeScale(2), NPC.Center); //impact
					}
					if (attack == 190 - phasethreefasterer)
					{
                        animation = 7; //ready gordo
                    }

					//low on health
					if (attack == 210 - phasethreefasterer && (phase == 2 || phase == 3))
					{
                        animation = 8; //swing gordo
                        Vector2 distanceahead = player.Center - (NPC.Center + new Vector2(NPC.direction * 100, 0));
                        distanceahead.Normalize();
						distanceahead *= 15;

                        if (Main.netMode != NetmodeID.MultiplayerClient) //execute on server( or singleplayer) only
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 100, 0), distanceahead, Mod.Find<ModProjectile>("BouncyGordo").Type, 30 / 2, 4f, Main.myPlayer, 0, 0);
                        }
                        SoundEngine.PlaySound(SoundID.Item10.WithVolumeScale(2), NPC.Center); //impact
					}
					if (attack == 220 - phasethreefasterer && (phase == 2 || phase == 3))
					{
                        animation = 7; //ready gordo
                    }
					//low on health

					if (attack == 240 - phasethreefasterer)
					{
                        animation = 8; //swing gordo
                        Vector2 distanceahead = player.Center - (NPC.Center + new Vector2(NPC.direction * 100, 0));
                        distanceahead.Normalize();
						distanceahead *= 15;

                        if (Main.netMode != NetmodeID.MultiplayerClient) //execute on server( or singleplayer) only
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 100, 0), distanceahead, Mod.Find<ModProjectile>("BouncyGordo").Type, 30 / 2, 4f, Main.myPlayer, 0, 0);
                        }
                        SoundEngine.PlaySound(SoundID.Item10.WithVolumeScale(2), NPC.Center); //impact
					}
					if (attack >= 300 - phasethreefasterer)
					{
                        animation = 0; //stand

                        attack = 0;
					}
				}

				if (attacktype == 4) //SLAM
				{
					if (attack == 60 - phasethreefasterer)
					{
                        animation = 9; //ready jump
                        NPC.TargetClosest(true); //face player
					}
					else
					{
						NPC.TargetClosest(false); //dont' face player
					}

					if (attack == (phase == 3 ? 90 : 120) - phasethreefasterer)
					{
						NPC.noTileCollide = true; //don't collide with tiles

                        NPC.velocity.X = distance.X / 120;
                        NPC.velocity.Y = distance.Y / 25;

                        animation = 10; //jump



                        //y caps
                        if (NPC.velocity.Y > -15) //less negative
                        {
                            NPC.velocity.Y = -15;

                        }
                        if (NPC.velocity.Y < -30)
                        {
                            NPC.velocity.Y = -30;

                        }

                        //x caps
                        if (NPC.velocity.X <= -15)
                        {
                            NPC.velocity.X = -15;

                        }
                        if (NPC.velocity.X >= 15)
                        {
                            NPC.velocity.X = 15;

                        }

                        SoundEngine.PlaySound(SoundID.NPCDeath8.WithPitchOffset(0.5f), NPC.Center); //beast grunt (high pitch)
					}
					if (attack > (phase == 3 ? 90 : 120) - phasethreefasterer && attack <= (phase == 3 ? 389 : 419) - phasethreefasterer) //fall for 5 seconds
					{
						NPC.wet = false; //Water collision I think???

						if (NPC.position.Y + NPC.height < player.position.Y || NPC.velocity.Y < 0) //feet higher than player or going up
						{
							NPC.noTileCollide = true; //don't collide with tiles
						}
						else
						{
							NPC.noTileCollide = false; //collide with tiles

							if (NPC.velocity.Y == 0 || NPC.oldVelocity.Y == 0) //oon ground or can't move anymore for some reason
							{
                                NPC.velocity.X *= 0;
                                animation = 9; //end jump

                                //slam
                                if (Main.netMode != NetmodeID.MultiplayerClient) //execute on server( or singleplayer) only
								{
									Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.position.Y + 150, 0, 0, Mod.Find<ModProjectile>("DededeSlam").Type, 40 / 2, 8f, Main.myPlayer, 0, 0);
								}
								SoundEngine.PlaySound(SoundID.Item14, NPC.Center); //bomb
								attack = (phase == 3 ? 390 : 420) - phasethreefasterer; //60 seconds before restart
							}
						}
					}
					if (attack >= (phase == 3 ? 450 : 480) - phasethreefasterer) //restart cycle
					{
						if (NPC.velocity.Y != 0 || NPC.oldVelocity.Y != 0 && animation != 9) //go back if not slam animation
						{
							attack = (phase == 3 ? 91 : 121);
						}
						else //continue
                        {
                            animation = 0; //stance

                            NPC.noTileCollide = false; //collide with tiles
							attack = 0;
                        }
					}
				}
			}

			//END OF NORMAL CYCLE //END OF PHASE 1 //END OF PHASE 1 //END OF PHASE 1 //END OF PHASE 1 //END OF PHASE 1 //END OF PHASE 1 //END OF PHASE 1

			else if (phase == 4)
            {
				//become boring
				NPC.noTileCollide = true;
				NPC.noGravity = true;

				if (attack < 240) //*casually gets possessed*
				{
                    animation = 11; //woooaaaahhh!
                    NPC.velocity *= 0.95f;

                    NPC.defense = 200; //increase defense for transformation
                    NPC.rotation += MathHelper.ToRadians(5); //degrees to radians

                    //dust
                    if (attack % 5 == 0)
					{
                        Vector2 speed = Main.rand.NextVector2Circular(20, 20);
                        Dust d = Dust.NewDustPerfect(NPC.Center + speed * 20, ModContent.DustType<Dusts.DarkResidue>(), -speed, 0); //Makes dust in a messy circle
                        d.noGravity = true;
                    }
				}
				else if (attack < 300) //rise dedede!
				{
					if (attack == 240)
					{
						for (int i = 0; i < 20; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
						{
							Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
							Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 10, 10); //Makes dust in a messy circle
							d.noGravity = true;
							SoundEngine.PlaySound(SoundID.Item81, NPC.Center); //spawn slime mount
						}

                        NPC.rotation = 0; //upright

                        animation = 12; //possessed fly
                        NPC.velocity.Y = -0.5f; //go up
                        lastattacktype = 2; //go to first attack
                    }
                }
				else if (attack >= 300) //START FROM HERE
				{
					if (attack == 301)
					{
						NPC.defense = NPC.defDefense; //original defense

                        animation = 12; //possessed fly
                    }

                    if (attack % 30 == 0) //every attack / 10 remainder of 0 
					{
						Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
						Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, Scale: 1f); //Makes dust in a messy circle
						d.noGravity = true;
					}

					if (attack < 540) 
					{
						NPC.TargetClosest(true);

						float speed = 8f;
						float inertia = 20;

						distance.Normalize();
						distance *= speed;

                        NPC.velocity = (NPC.velocity * (inertia - 1) + distance) / inertia; //fly towards player
                    }

					if (attack == 449)
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

					//no need for attack++ because it's oustide of this giant if statement

					if (attacktype == 1) //maw
                    {
						if (attack == 540)
                        {
                            NPC.TargetClosest(false);

                            NPC.frameCounter = 0; //restart animation
                            animation = 13; //open maw
                            NPC.velocity *= 0;
                        }

                        if (attack >= 600 && attack < 720)
						{
                            animation = 14; //chomp loop
                            float speed = NPC.life <= NPC.lifeMax * 0.10 ? 24 : 12; //faster if low on health
							float inertia = 30; //harder to turn

							distance.Normalize();
							distance *= speed;
                            NPC.velocity = (NPC.velocity * (inertia - 1) + distance) / inertia; //fly towards player
                            
                            NPC.TargetClosest(true);

                                
                        }
						if (attack == 720) //close
                        {
							NPC.frameCounter = 0; //restart animation

                            animation = 15; //close maw

                            NPC.TargetClosest(false);

                            NPC.velocity *= 0;

                              
                        }
						if (attack >= 780) //end
						{
                            animation = 12; //possessed fly
                            attack = 300; //restart from here

                               
                        }
					}

					if (attacktype == 2) //eye
					{

						if (attack == 540) //do this until closing
						{
                            NPC.TargetClosest(false);

                            NPC.frameCounter = 0; //restart animation
                            animation = 16; //open eye
                            NPC.velocity *= 0;
                        }

                        if (attack == 570)
						{
							if (Main.netMode != NetmodeID.MultiplayerClient) //execute on server( or singleplayer) only
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * 100, NPC.Center.Y, NPC.velocity.X * 0, NPC.velocity.Y * 0, ModContent.ProjectileType<Projectiles.DarkOrb>(), 50 / 2, 10f, Main.myPlayer, 0, NPC.target);
							}
							NPC.TargetClosest(false);
						}

						//low
						if (attack == 600 && NPC.life <= NPC.lifeMax * 0.10)
						{
							if (Main.netMode != NetmodeID.MultiplayerClient) //execute on server( or singleplayer) only
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * 100, NPC.Center.Y, NPC.velocity.X * 0, NPC.velocity.Y * 0, ModContent.ProjectileType<Projectiles.DarkOrb>(), 50 / 2, 10f, Main.myPlayer, 0, NPC.target);
							}
							NPC.TargetClosest(false);
						}
						//low

						if (attack == 630) //close
						{
                            NPC.TargetClosest(false);

                            NPC.frameCounter = 0; //restart animation
                            animation = 17; //close eye
                        }
						if (attack >= 660) //end
						{
                            animation = 12; //possessed fly
                            attack = 300; //restart from here
                        }
					}
				}
			}
		}
		public override void FindFrame(int frameHeight) // animation
        {
		   if (animation == 0) //stance
            {
				NPC.frameCounter++;
				if (NPC.frameCounter < 30)
				{
					NPC.frame.Y = 0;
				}
				else
				{
					NPC.frame.Y = frameHeight;
				}
				if (NPC.frameCounter >= 60)
                {
					NPC.frameCounter = 0;
                }
			}
		   if (animation == 1) //run
            {
				NPC.frameCounter++;
				if (NPC.frameCounter < 15)
                {
					NPC.frame.Y = frameHeight * 2; //right leg forward
				}
				else if (NPC.frameCounter < 30)
				{
					NPC.frame.Y = frameHeight * 3; //even out
				}
				else if (NPC.frameCounter < 45)
				{
					NPC.frame.Y = frameHeight * 4; //left leg forward
				}
				else if (NPC.frameCounter < 60)
				{
					NPC.frame.Y = frameHeight * 3; //even out
				}
				else
                {
					NPC.frameCounter = 0;
                }
			}
			if (animation == 2) //dive
			{
				NPC.frame.Y = frameHeight * 5; //faceplant
				NPC.frameCounter = 0;
			}
			if (animation == 3) //pull hammer
			{
				NPC.frame.Y = frameHeight * 6; //faceplant
				NPC.frameCounter = 0;
			}
			if (animation == 4) //run with hammer
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 15)
				{
					NPC.frame.Y = frameHeight * 7; //right leg forward
				}
				else if (NPC.frameCounter < 30)
				{
					NPC.frame.Y = frameHeight * 8; //even out
				}
				else if (NPC.frameCounter < 45)
				{
					NPC.frame.Y = frameHeight * 9; //left leg forward
				}
				else if (NPC.frameCounter < 60)
				{
					NPC.frame.Y = frameHeight * 8; //even out
				}
				else
				{
					NPC.frameCounter = 0;
				}
			}
			if (animation == 5) //ready swing
			{
				NPC.frame.Y = frameHeight * 10; //ready swing
				NPC.frameCounter = 0;
			}
			if (animation == 6) //swing
			{
				NPC.frame.Y = frameHeight * 11; //swing
				NPC.frameCounter = 0;
			}
			if (animation == 7) //ready swing gordo
			{
				NPC.frame.Y = frameHeight * 12; //ready swing gordo
				NPC.frameCounter = 0;
			}
			if (animation == 8) //swing gordo
			{
				NPC.frame.Y = frameHeight * 13; //swing gordo
				NPC.frameCounter = 0;
			}
			if (animation == 9) //slam
			{
				NPC.frame.Y = frameHeight * 14; //slam
				NPC.frameCounter = 0;
			}
			if (animation == 10) //rise
			{
				NPC.frame.Y = frameHeight * 15; //rise
				NPC.frameCounter = 0;
			}
			if (animation == 11) //ouch!
			{
				NPC.frame.Y = frameHeight * 16; //ouch!
				NPC.frameCounter = 0;
			}
			if (animation == 12) //possessed float
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 15)
				{
					NPC.frame.Y = frameHeight * 18; //inhale
				}
				else if (NPC.frameCounter < 30)
				{
					NPC.frame.Y = frameHeight * 17; //exhale
				}
				else
				{
					NPC.frameCounter = 0;
				}
			}
			if (animation == 13) //possessed open maw
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 5)
				{
					NPC.frame.Y = frameHeight * 19; //stance up
				}
				else if (NPC.frameCounter < 10)
				{
					NPC.frame.Y = frameHeight * 20; //reveal
				}
				else 
				{
					NPC.frame.Y = frameHeight * 21; //fully open
				}
			}
			if (animation == 14) //possessed chomp loop
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 5)
				{
					NPC.frame.Y = frameHeight * 20; //close
				}
				else if (NPC.frameCounter < 10)
				{
					NPC.frame.Y = frameHeight * 21; //open
				}
				else
				{
					NPC.frameCounter = 0;
				}
			}
			if (animation == 15) //possessed close maw
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 5)
				{
					NPC.frame.Y = frameHeight * 21; //fully open
				}
				else if (NPC.frameCounter < 10)
				{
					NPC.frame.Y = frameHeight * 20; //reveal
				}
				else 
				{
					NPC.frame.Y = frameHeight * 19; //normal
				}
			}
			if (animation == 16) //possessed open eye
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 5)
				{
					NPC.frame.Y = frameHeight * 22; //squint
				}
				else if (NPC.frameCounter < 10)
				{
					NPC.frame.Y = frameHeight * 23; //neutral
				}
				else 
				{
					NPC.frame.Y = frameHeight * 24; //big ol eyes
				}
			}
			if (animation == 17) //possessed close eye
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 5)
				{
					NPC.frame.Y = frameHeight * 23; //neutral
				}
				else if (NPC.frameCounter < 10)
				{
					NPC.frame.Y = frameHeight * 22; //squint
				}
				else 
				{
					NPC.frame.Y = frameHeight * 19; //normal
				}
			}
		}

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedKingDededeBoss, -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<KingDededeBag>())); //only drops in expert

            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert()); //checks if not expert
            LeadingConditionRule masterMode = new LeadingConditionRule(new Conditions.IsMasterMode()); //checks if master mode

            notExpertRule.OnSuccess(new OneFromRulesRule(1, ItemDropRule.Common(ModContent.ItemType<Items.Weapons.NewHammer>(), 1)
                , ItemDropRule.Common(ModContent.ItemType<Items.Weapons.Blado>(), 1, 300, 300)
                , ItemDropRule.Common(ModContent.ItemType<Items.Weapons.OrnateChest>(), 1)
                , ItemDropRule.Common(ModContent.ItemType<Items.Weapons.DooStaff>(), 1)));

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.KingDedede.KingDededeMask>(), 7));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.KingDedede.KingDededeTrophy>(), 10));

            //master mode stuff
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeables.BossRelics.KingDededeRelic>()));

            masterMode.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.KingDedede.KingDededePetItem>(), 4));

            // add the rules
            npcLoot.Add(notExpertRule);
            npcLoot.Add(masterMode);
        }

		public override void BossLoot(ref string name, ref int potionType)
		{
			name = "King Dedede";
			potionType = ItemID.HealingPotion;
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
					Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.BoldStar>(), speed, Scale: 2f); //Makes dust in a messy circle
					d.noGravity = true;
				}
				for (int i = 0; i < 20; i++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
                    Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, speed, Main.rand.Next(11, 13), Scale: 2f); //double jump smoke
                }

				if (NPC.direction == -1) //die left
				{
					Dust.NewDustPerfect(NPC.position, ModContent.DustType<Dusts.KingDededead>(), new Vector2(NPC.direction * -8, -8), 1);
				}
				else if (NPC.direction == 1)//die right
				{
					Dust.NewDustPerfect(NPC.position, ModContent.DustType<Dusts.KingDededeadRight>(), new Vector2(NPC.direction * -8, -8), 1);
				}
			}	
			else
            {
				if (phase != 4) //no dark residue
				{
					Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                    Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, speed, Main.rand.Next(11, 13), Scale: 0.5f); //double jump smoke
                }
				else
                {
					Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
					Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, Scale: 0.5f); //Makes dust in a messy circle
					d.noGravity = false;
				}
			}
        }

        public override Color? GetAlpha(Color drawColor)
        {
			Player player = Main.player[NPC.target]; //the player the npc is targeting

			//make a line from dedede through player to see if no tiles
			bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
			float lineOfDistance = Vector2.Distance(player.Center, NPC.Center);

			//not line of sight because it's annyoing
			if ((NPC.position.Y > (player.Center.Y - (player.height / 2) + 400) || lineOfDistance >= 1000) && phase != 4)
			{
				NPC.dontTakeDamage = true; //invunerable

				return new Color (255, 128, 128); //warning tint (light red)
			}
			NPC.dontTakeDamage = false; //vunerable

			return drawColor; 
		}

        public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
        {
            boundingBox = NPC.Hitbox;
        }
    }
}
