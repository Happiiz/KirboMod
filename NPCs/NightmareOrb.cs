using KirboMod.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
	[AutoloadBossHead]
	public class NightmareOrb : ModNPC
	{
		private int attacktype = 0;

		public bool frenzy { get => NPC.ai[3] == 1f; set => NPC.ai[3] = value ? 1f : 0f; }

        public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Power Orb");
			Main.npcFrameCount[NPC.type] = 2;

			//Needed for multiplayer spawning to work
			NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }

		public override void SetDefaults() {
			NPC.width = 100;
			NPC.height = 100;
			NPC.damage = 0; //initally
			NPC.noTileCollide = true;
			NPC.defense = 20; 
			NPC.lifeMax = 15000;
			NPC.HitSound = SoundID.NPCHit4;
			NPC.DeathSound = SoundID.NPCDeath14; //explosive metal
			NPC.value = 0f; // money it drops
			NPC.knockBackResist = 0f;
			Banner = 0;
			BannerItem = Item.BannerToItem(Banner);
			NPC.aiStyle = -1;
			NPC.boss = true;
			NPC.noGravity = true;
			NPC.lavaImmune = true;
			Music = MusicID.Boss3;
			NPC.npcSlots = 6;
			NPC.alpha = 255; //only initally of course
		}

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */ 
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.75 * balance);
			NPC.damage = (int)(NPC.damage);
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("An ominous orb that appeared from the fountain after being attracted to it's magic. Acts as the shield for a mysterious yet diabolical sorcerer.")
            });
        }

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(attacktype);
        }

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			attacktype = reader.ReadInt32();
        }

        public override void AI() //constantly cycles each time
        {
			Player player = Main.player[NPC.target];

			NPC.TargetClosest(true);
			
			NPC.ai[2] = 60; //SKIP THIS PHASE FOR NOW AS IT's NOT NEEDED IN MULTIPLAYER

			if (NPC.ai[2] < 60) //rise
			{
				/*NPC.velocity.Y = -2; //rise

				if (NPC.alpha > 0)
				{
					NPC.alpha -= 5; //become visible
				}
				else
                {
					NPC.alpha = 0; //visible
				}

				if (NPC.ai[2] % 10 == 0) //sound
                {
					SoundEngine.PlaySound(SoundID.Run.WithVolumeScale(2f).WithPitchOffset(0.5f), NPC.Center);
				}

				if (NPC.ai[2] == 59)
                {
                    for (int i = 0; i < 60; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
					{
						Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
						Dust.NewDustPerfect(NPC.Center, DustID.Shadowflame, speed * 10, 1); //Makes dust in a ring
					}

                    NPC.dontTakeDamage = false;
                }*/
			}
			else //AFTER ENTRANCE
			{
				//set damage
                if (Main.masterMode)
                {
                    NPC.damage = 210;
                }
				else if (Main.expertMode)
				{
					NPC.damage = 140;
				}
				else
				{
					NPC.damage = 70;
				}

                //DESPAWNING
                if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active || Main.dayTime == true)
				{
					NPC.ai[0] = 0;
					
					NPC.velocity.Y = NPC.velocity.Y - 0.4f;
					if (NPC.timeLeft > 60)
					{
						NPC.timeLeft = 60;
						return;
					}
				}
				else //regular attack stuff
				{
					AttackPattern();

					//checks if rapid attacking in expert mode("phase 3" if you want) at the start of the cycle
					if (NPC.ai[1] == 0 && Main.expertMode && NPC.life <= NPC.lifeMax * 0.25)
                    {
						frenzy = true; //turn from false to true
                    }

					NPC.ai[1]++; //marks when to switch attacks

					if (Main.expertMode == false)
					{
						if (NPC.ai[1] == 300) //Tri stars
						{
							if (NPC.life >= NPC.lifeMax * 0.75) //dash if criteria met
							{
								NPC.ai[1] = 880; //point of dash
								NPC.ai[0] = 0;
							}
							else
							{
								attacktype = 2;
								NPC.ai[0] = 0; //set to 1 because it's one ahead
							}
						}

						if (NPC.ai[1] == 480) //Homing stars
						{
							if (NPC.life >= NPC.lifeMax * 0.5 & NPC.ai[0] == 0) //dash if criteria met
							{
								NPC.ai[1] = 880; //point of dash
								NPC.ai[0] = 0;
							}
							else
							{
								attacktype = 3;
								NPC.ai[0] = 0;
							}
						}

						if (NPC.ai[1] == 720) //Slash beams
						{
							if (NPC.life >= NPC.lifeMax * 0.25 & NPC.ai[0] == 0) //dash if criteria met
							{
								NPC.ai[1] = 880; //point of dash
								NPC.ai[0] = 0;
							}
							else
							{
								attacktype = 1;
								NPC.ai[0] = 0;
							}
						}

						if (NPC.ai[1] == 880) //start dashing
						{
							NPC.ai[0] = 0;
							attacktype = 4;
						}
					}
					else //expert Mode cycle
					{
						if (NPC.ai[1] == 300) //Tri stars
						{
							if (NPC.life >= NPC.lifeMax * 0.85) //dash if criteria met
							{
								NPC.ai[1] = 880; //point of dash
								NPC.ai[0] = 0;
							}
							else
							{
								attacktype = 2;
								NPC.ai[0] = 0;
							}
						}

						if (NPC.ai[1] == (frenzy ? 390 : 480)) //Homing stars(does it sooner if below 25%)
						{
							if (NPC.life >= NPC.lifeMax * 0.70 & NPC.ai[0] == 0) //dash if criteria met
							{
								NPC.ai[1] = 880; //point of dash
								NPC.ai[0] = 0;
							}
							else
							{
								attacktype = 3;
								NPC.ai[0] = 0;
							}
						}

						if (NPC.ai[1] == (frenzy ? 510 : 720)) //Slash beams(does it sooner if below 25%)
						{
							if (NPC.life >= NPC.lifeMax * 0.55 & NPC.ai[0] == 0) //dash if criteria met
							{
								NPC.ai[1] = 880; //point of dash
								NPC.ai[0] = 0;
							}
							else
							{
								attacktype = 1;
								NPC.ai[0] = 0;
							}
						}

						if (NPC.ai[1] == (frenzy ? 590 : 880)) //start dashing(does it sooner if below 25%)
						{
							NPC.ai[0] = 0;
							attacktype = 4;
						}
					}
				}
			}
		}
		private void AttackPattern()
		{
            Player player = Main.player[NPC.target];

            NPC.ai[0]++;

            if (attacktype != 4)
			{
                Vector2 moveTo = player.Center; //I don't know if I even need this
                Vector2 playerDistance = player.Center - NPC.Center;
                Vector2 move = player.Center - NPC.Center;

                float minX = moveTo.X - 50f;
                float maxX = moveTo.X + 50f;
                float minY = moveTo.Y;
                float maxY = moveTo.Y;

                if (playerDistance.X <= 0) //if player is behind enemy
                {
                    move += new Vector2(400f, 0); // go in front of player 
                }
                else
                {
                    move += new Vector2(-400f, 0); // go behind player
                }

                if (NPC.Center.X >= minX && NPC.Center.X <= maxX && NPC.Center.Y >= minY && NPC.Center.Y <= maxY) //certain range
                {
                    NPC.velocity *= 0.98f; //slow
                }
                else
                {
                    float magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y);
                    float speed = 15f; //speed I think
                    if (magnitude > speed)
                    {
                        move *= speed / magnitude;
                    }
                    float inertia = 20f; //Ok so like I'm pretty sure this is supposed to be how wibbly wobbly you want the npc to be before it reaches its destination
                    NPC.velocity = (inertia * NPC.velocity + move) / (inertia + 1);
                    magnitude = (float)Math.Sqrt(NPC.velocity.X * NPC.velocity.X + NPC.velocity.Y + NPC.velocity.Y);
                    if (magnitude > speed)
                    {
                        NPC.velocity *= speed / magnitude;
                    }
                }
            }

			if (attacktype == 0)//stars
			{
				//PROJECTILE
				Vector2 projshoot = player.Center - NPC.Center;
				projshoot.Normalize(); //reduces it to a value of 1
				projshoot *= 15f; //projectile speed

				if (NPC.ai[0] == 49 && frenzy) //expert mode below 25%
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projshoot, ModContent.ProjectileType<BadStar>(), 20 / 2, 0f, Main.myPlayer, 0, 0);
					}
					SoundEngine.PlaySound(SoundID.MaxMana, NPC.Center);
				}
				if (NPC.ai[0] == 59)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projshoot, ModContent.ProjectileType<BadStar>(), 20 / 2, 0f, Main.myPlayer, 0, 0);
					}
					SoundEngine.PlaySound(SoundID.MaxMana, NPC.Center);
				}
				if (NPC.ai[0] == 69 && frenzy) //expert mode below 25%
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projshoot, ModContent.ProjectileType<BadStar>(), 20 / 2, 0f, Main.myPlayer, 0, 0);
					}
					SoundEngine.PlaySound(SoundID.MaxMana, NPC.Center);
				}
				if (NPC.ai[0] == 79)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projshoot, ModContent.ProjectileType<BadStar>(), 20 / 2, 0f, Main.myPlayer, 0, 0);
					}
					SoundEngine.PlaySound(SoundID.MaxMana , NPC.Center);
				}
				if (NPC.ai[0] == 89 && frenzy) //expert mode below 25%
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projshoot, ModContent.ProjectileType<BadStar>(), 20 / 2, 0f, Main.myPlayer, 0, 0);
					}
					SoundEngine.PlaySound(SoundID.MaxMana, NPC.Center);
				}
				if (NPC.ai[0] == 99)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projshoot, ModContent.ProjectileType<BadStar>(), 20 / 2, 0f, Main.myPlayer, 0, 0);
					}
					SoundEngine.PlaySound(SoundID.MaxMana, NPC.Center);
				}
				if (NPC.ai[0] == 100)
                {
					NPC.ai[0] = 0; //end cycle
				}
			}

			else if (attacktype == 1) //slashbeams
			{
                Vector2 playerDistance = player.Center - NPC.Center;
				float xOffset = 400;

                if (playerDistance.X <= 0) //if player is behind enemy
                {
                    xOffset = 400; // go in front of player 
                }
                else
                {
                    xOffset = -400; // go behind player
                }

                Vector2 playerXOffest = player.Center + new Vector2(xOffset, 0f); //go in front of player
				Vector2 move = playerXOffest - NPC.Center;

				if (!frenzy) //not expert mode or above 25%(has to be both to be expert attack)
				{
					if (NPC.ai[0] >= 40) //shoot
					{
						move *= 0f;
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, NPC.direction * 18, 0, ModContent.ProjectileType<NightSlash>(), 40 / 2, 0f, Main.myPlayer, 0, 0);
						}
						SoundEngine.PlaySound(SoundID.Item15, NPC.Center); //phasesaber
						NPC.ai[0] = 0;
					}
					else if (NPC.ai[0] >= 20) //jump to location
					{
						move /= 5f;
					}
					else if (NPC.ai[0] <= 20) //freeze up
					{
						move *= 0f;
					}
				}
				else //expert cycle
				{
					if (NPC.ai[0] >= 20) //shoot
					{
						move *= 0f;
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, NPC.direction * 18, 0, ModContent.ProjectileType<NightSlash>(), 40 / 2, 0f, Main.myPlayer, 0, 0);
						}
						SoundEngine.PlaySound(SoundID.Item15, NPC.Center); //phasesaber
						NPC.ai[0] = 0;
					}
					else if (NPC.ai[0] >= 10) //jump to location
					{
						move /= 5f;
					}
					else if (NPC.ai[0] <= 10) //freeze up
					{
						move *= 0f;
					}
				}

				NPC.velocity = move;
			}

			else if (attacktype == 2)//star barrage
			{
				//PROJECTILE
				if (frenzy == false) //not expert mode or above 25%(has to be both to be expert attack)
				{
					if (NPC.ai[0] == 60 || NPC.ai[0] == 120 || NPC.ai[0] == 180)
					{
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
                            //Straight
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, NPC.direction * 15, 0, ModContent.ProjectileType<BadStar>(), 20 / 2, 0f, Main.myPlayer, 0, 0);
						
						    //Down
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, NPC.direction * 10, 5, ModContent.ProjectileType<BadStar>(), 20 / 2, 0f, Main.myPlayer, 0, 0);
						
						    //Up
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, NPC.direction * 10, -5, ModContent.ProjectileType<BadStar>(), 20 / 2, 0f, Main.myPlayer, 0, 0);
						}
						SoundEngine.PlaySound(SoundID.MaxMana, NPC.Center);
					}

					if (NPC.ai[0] >= 180)
					{
						NPC.ai[0] = 0; //end cycle
					}
				}
				else //expert cycle
				{
					if (NPC.ai[0] == 30 || NPC.ai[0] == 60 || NPC.ai[0] == 90)
					{
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
                            //Straight
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, NPC.direction * 15, 0, ModContent.ProjectileType<BadStar>(), 20 / 2, 0f, Main.myPlayer, 0, 0);
						
						    //Down
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, NPC.direction * 10, 5, ModContent.ProjectileType<BadStar>(), 20 / 2, 0f, Main.myPlayer, 0, 0);
						
						    //Up
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, NPC.direction * 10, -5, ModContent.ProjectileType<BadStar>(), 20 / 2, 0f, Main.myPlayer, 0, 0);
						}
						SoundEngine.PlaySound(SoundID.MaxMana, NPC.Center);
					}

					if (NPC.ai[0] >= 90)
					{
						NPC.ai[0] = 0; //end cycle
					}
				}
			}

			else if (attacktype == 3)//homing stars
			{
				//PROJECTILE
				if (frenzy == false) //not expert mode or above 25%(has to be both to be expert attack)
				{
					if (NPC.ai[0] == 60 || NPC.ai[0] == 120 || NPC.ai[0] == 180) //homing stars go behind it
					{
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
                            //low star
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, NPC.direction * -20, 10, ModContent.ProjectileType<HomingNightStar>(), 20 / 2, 0f, Main.myPlayer, 0, 0);

							//high star
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, NPC.direction * -20, -10, ModContent.ProjectileType<HomingNightStar>(), 20 / 2, 0f, Main.myPlayer, 0, 0);

							//lower star
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, NPC.direction * -20, 20, ModContent.ProjectileType<HomingNightStar>(), 20 / 2, 0f, Main.myPlayer, 0, 0);

							//higher star
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, NPC.direction * -20, -20, ModContent.ProjectileType<HomingNightStar>(), 20 / 2, 0f, Main.myPlayer, 0, 0);

							//center star
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, NPC.direction * -20, -20, ModContent.ProjectileType<HomingNightStar>(), 20 / 2, 0f, Main.myPlayer, 0, 0);
						}
						SoundEngine.PlaySound(SoundID.MaxMana, NPC.Center);
					}

					if (NPC.ai[0] >= 240)
					{
						NPC.ai[0] = 0; //end cycle
					}
				}
				else //expert cycle
				{
					if (NPC.ai[0] == 30 || NPC.ai[0] == 60 || NPC.ai[0] == 90)
					{
						//low star
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, NPC.direction * -20, 10, ModContent.ProjectileType<HomingNightStar>(), 20 / 2, 0f, Main.myPlayer, 0, 0);
							//high star
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, NPC.direction * -20, -10, ModContent.ProjectileType<HomingNightStar>(), 20 / 2, 0f, Main.myPlayer, 0, 0);
							//lower star
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, NPC.direction * -20, 20, ModContent.ProjectileType<HomingNightStar>(), 20 / 2, 0f, Main.myPlayer, 0, 0);
							//higher star
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, NPC.direction * -20, -20, ModContent.ProjectileType<HomingNightStar>(), 20 / 2, 0f, Main.myPlayer, 0, 0);
							//center star
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, NPC.direction * -20, -20, ModContent.ProjectileType<HomingNightStar>(), 20 / 2, 0f, Main.myPlayer, 0, 0);
						}
						SoundEngine.PlaySound(SoundID.MaxMana, NPC.Center);
					}

					if (NPC.ai[0] >= 120)
					{
						NPC.ai[0] = 0; //end cycle
					}
				}
			}

			else if (attacktype == 4)//dash
			{
				//MOVEMENT
				if (NPC.ai[0] == 1)
                {
					NPC.velocity.Y = 0;
					NPC.velocity.X = 0;
				}
				if (NPC.ai[0] > 1 & NPC.ai[0] < 30) //back up
                {
					NPC.velocity.Y = 0;
					NPC.velocity.X += NPC.direction * -0.5f;
                }
				if (NPC.ai[0] == 30) //charge
                {
					NPC.velocity.X = NPC.direction * 80;
                }
				if (NPC.ai[0] > 30) //slow move
                {
					NPC.velocity.X *= 0.92f;
                }

				if (NPC.ai[0] >= 210)
				{
					attacktype = 0; //recycle to single stars
					NPC.ai[1] = 0; //reset entire attack cycle
					NPC.ai[0] = 0; //end cycle
				}
			}
		}
		public override void FindFrame(int frameHeight) // animation
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
            else
            {
                NPC.frameCounter = 0.0;
            }
        }

		public override Color? GetAlpha(Color lightColor)
		{
			if (NPC.ai[2] >= 60) //if done phaseing in
			{
				return Color.White; // Makes it uneffected by light
			}
			else
            {
				return null;
            }
		}

		public override bool PreKill()
		{
			return false;
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life <= 0)
			{
				for (int i = 0; i < 10; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
					Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.NightStar>(), speed, Scale: 1.5f); //Makes dust in a messy circle
					d.noGravity = true;
				}
				for (int i = 0; i < 10; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                    Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, speed, Main.rand.Next(11, 13), Scale: 1.5f); //double jump smoke
                }
			}
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			scale = 1.5f;
			return true;
		}

        public override bool CheckDead()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.SpawnBoss((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<NightmareWizard>(), Main.player[NPC.target].whoAmI); //different from SpawnOnPlayer()
            }

            return true;
        }
	}
}
