using KirboMod.Items.Zero;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using ReLogic.Content;
using KirboMod.Bestiary;
using KirboMod.Systems;
using Terraria.DataStructures;
using TextureAssets = Terraria.GameContent.TextureAssets;
using System.IO;
using KirboMod.Projectiles;
using KirboMod.Dusts;
using System.Reflection;

namespace KirboMod.NPCs
{
	[AutoloadBossHead]
	public class Zero : ModNPC
	{
		private int animation = 1; //frame cycles

		private int lastattacktype = 2; //sets last attack type
		private int attacktype = 1; //decides the attack

		private float backupoffset = 0; //offset goto position when backing up

		private Vector2 bloodlocation = new Vector2(0,0);
		private float bloodlocationx = 0;
		private float bloodlocationy = 0;

		private Vector2 bloodlocation2 = new Vector2(0, 0);
		private float bloodlocation2x = 0;
		private float bloodlocation2y = 0;

		private int deathcounter = 0; //for death animation

		private int backgroundAttackCountDown = 0; //cycles through background attack at 10% health every 3 attacks

		private int animationXframeOffset = 0; //changes the sprite column depending on what animation is playing

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Zero");
			Main.npcFrameCount[NPC.type] = 8; //kinda pointless as the drawing is done manually


            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
			{
				CustomTexturePath = "KirboMod/NPCs/BestiaryTextures/ZeroPortrait",
				PortraitScale = 1f, // Portrait refers to the full picture when clicking on the icon in the bestiary
				PortraitPositionYOverride = 0,
				PortraitPositionXOverride = 120,
                Position = new Vector2(100, 0),
                Scale = 0.75f,

            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                ImmuneToAllBuffsThatAreNotWhips = true,
                ImmuneToWhips = true
            };
        }

        public override void SetDefaults()
		{
			NPC.width = 398; 
			NPC.height = 398;
			DrawOffsetY = 193;
			NPC.damage = 120; //initally
			NPC.noTileCollide = true;
			NPC.defense = 70;
			NPC.lifeMax = 160000;
			NPC.HitSound = SoundID.NPCHit1; //slime
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice( 0, 38, 18, 10); // money it drops
			NPC.knockBackResist = 0f;
			NPC.aiStyle = -1;
			NPC.npcSlots = 16;
			NPC.boss = true;
			NPC.noGravity = true;
			NPC.lavaImmune = true;
			NPC.buffImmune[BuffID.Poisoned] = true;
			NPC.buffImmune[BuffID.Venom] = true;
			NPC.buffImmune[BuffID.OnFire] = true;
			NPC.buffImmune[BuffID.CursedInferno] = true;
			NPC.buffImmune[BuffID.ShadowFlame] = true;

            /*Music = MusicID.Boss2;*/
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Zero");
            }

            SceneEffectPriority = SceneEffectPriority.BossHigh; // By default, musicPriority is BossLow
			NPC.alpha = 255; //initally
        }

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */ //damage is automatically doubled in expert, use this to reduce it
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.75 * balance); //240,000 health in expert
			NPC.damage = (int)(NPC.damage * 1); //it scales by x2 automatically
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				new HyperzoneBackgroundProvider(), //I totally didn't reference the vanilla code what no way

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("The ultimate leader of all dark matter. Uses it's leigon to blanket entire worlds in darkness.")
			}); 
        }

        public override void SendExtraAI(BinaryWriter writer) //for syncing non NPC.ai[] stuff
        {
            writer.Write(animation);

            writer.Write(lastattacktype);
            writer.Write(attacktype);

            writer.Write(backupoffset);

			writer.WriteVector2(bloodlocation);
            writer.Write(bloodlocationx);
            writer.Write(bloodlocationy);

            writer.WriteVector2(bloodlocation2);
            writer.Write(bloodlocation2x);
            writer.Write(bloodlocation2y);

            writer.Write(deathcounter);

            writer.Write(backgroundAttackCountDown);

            writer.Write(animationXframeOffset);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            animation = reader.ReadInt32();

            lastattacktype = reader.ReadInt32();
            attacktype = reader.ReadInt32();

            backupoffset = reader.ReadSingle();

            bloodlocation = reader.ReadVector2();
            bloodlocationx = reader.ReadSingle();
            bloodlocationy = reader.ReadSingle();

            bloodlocation2 = reader.ReadVector2();
            bloodlocation2x = reader.ReadSingle();
            bloodlocation2y = reader.ReadSingle();

            deathcounter = reader.ReadInt32();

            backgroundAttackCountDown = reader.ReadInt32();

            animationXframeOffset = reader.ReadInt32();
        }

        public override void AI() //constantly cycles each time
		{
			Player playerstate = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction;

			//DESPAWNING
			if (NPC.target < 0 || NPC.target == 255 || playerstate.dead || !playerstate.active)
			{
				NPC.TargetClosest(false);

				NPC.velocity.Y -= 0.2f;

				if (NPC.timeLeft > 60)
				{
					NPC.timeLeft = 60;
					return;
				}
			}
			else if (deathcounter > 0)
			{
				DoDeathAnimation();
			}
			else if (NPC.ai[1] < 390) 
			{
				animation = 1;
				NPC.ai[1]++;

				if (NPC.ai[1] < 60) //rise up gang
				{
					NPC.velocity.Y = -2;
					NPC.alpha -= 5; //get visible
				}
				else
				{
					NPC.velocity *= 0;
				}

				if (NPC.ai[1] == 389) //regular stats
                {
					for (int i = 0; i < 40; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
					{
						Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
						Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 20, Scale: 2); //Makes dust in a messy circle
						d.noGravity = true;
					}

					NPC.dontTakeDamage = false;
                }
				else //invinicble
                {
					NPC.dontTakeDamage = true;
					NPC.damage = 0;
				}
            }
			else //regular attack
            {
                AttackPattern();
			}
		}
		private void AttackPattern()
		{
			Player player = Main.player[NPC.target];
			Vector2 playerDistance = player.Center - NPC.Center;
			Vector2 playerRightDistance = player.Center + new Vector2(500 + backupoffset, 0) - NPC.Center;
			Vector2 playerLeftDistance = player.Center + new Vector2(-500 - backupoffset, 0) - NPC.Center;
			Vector2 playerAboveDistance = player.Center + new Vector2(0, -500) - NPC.Center;

			if (NPC.ai[0] == 0) //restart stats
            {
				animation = 0;
				NPC.frameCounter = 0; //reset animation

				NPC.dontTakeDamage = false;

				if (Main.masterMode)
				{
					NPC.damage = 360;
				}
				else if (Main.expertMode)
				{
					NPC.damage = 240;
				}
				else
				{
					NPC.damage = 120;
				}
			}

			NPC.ai[0]++;

			float speed = 16; 
			float inertia = 15;

			if (NPC.ai[0] <= 120)
			{
				NPC.TargetClosest(true);

				if (playerDistance.X <= 0)
				{
					playerRightDistance.Normalize();
					playerRightDistance *= speed;
					NPC.velocity = (NPC.velocity * (inertia - 1) + playerRightDistance) / inertia; //fly towards player
				}
				else
				{
					playerLeftDistance.Normalize();
					playerLeftDistance *= speed;
					NPC.velocity = (NPC.velocity * (inertia - 1) + playerLeftDistance) / inertia; //fly towards player
				}


				//BEGINNING OF SUPER DUPER LONG LINE OF CODE RANDOMIZING ZERO'S ATTACKS




				if (NPC.ai[0] == 120)  
				{
					if (NPC.life <= NPC.lifeMax * 0.10) //10%
					{
						if (backgroundAttackCountDown <= 0) //equal or less than zero
						{
							attacktype = 6; //background attack
							lastattacktype = 6;
							backgroundAttackCountDown = 2; //restart
						}
						else //do regular attack
						{
							backgroundAttackCountDown--; //go down by 1

							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								int randattack = Main.rand.Next(1, 5 + 1);
								if (randattack == 1)
								{
									if (lastattacktype == 1)
									{
										int subrandattack = Main.rand.Next(1, 4 + 1);
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
										else if (subrandattack == 3)
										{
											attacktype = 4;
											lastattacktype = 4;
										}
										else
										{
											attacktype = 5;
											lastattacktype = 5;
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
										int subrandattack = Main.rand.Next(1, 4 + 1);
										if (subrandattack == 1)
										{
											attacktype = 1;
											lastattacktype = 1;
										}
										else if (subrandattack == 2)
										{
											attacktype = 4;
											lastattacktype = 4;
										}
										else if (subrandattack == 3)
										{
											attacktype = 3;
											lastattacktype = 3;
										}
										else
										{
											attacktype = 5;
											lastattacktype = 5;
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
										int subrandattack = Main.rand.Next(1, 4 + 1);
										if (subrandattack == 1)
										{
											attacktype = 2;
											lastattacktype = 2;
										}
										else if (subrandattack == 2)
										{
											attacktype = 1;
											lastattacktype = 1;
										}
										else if (subrandattack == 3)
										{
											attacktype = 5;
											lastattacktype = 5;
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
								else if (randattack == 4)
								{
									if (lastattacktype == 4)
									{
										int subrandattack = Main.rand.Next(1, 4 + 1);
										if (subrandattack == 1)
										{
											attacktype = 2;
											lastattacktype = 2;
										}
										else if (subrandattack == 2)
										{
											attacktype = 1;
											lastattacktype = 1;
										}
										else if (subrandattack == 3)
										{
											attacktype = 3;
											lastattacktype = 3;
										}
										else
										{
											attacktype = 5;
											lastattacktype = 5;
										}
									}
									else
									{
										attacktype = 4;
										lastattacktype = 4;
									}
								}
								else //5
								{
									if (lastattacktype == 5)
									{
										int subrandattack = Main.rand.Next(1, 4 + 1);
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
										else if (subrandattack == 3)
										{
											attacktype = 4;
											lastattacktype = 4;
										}
										else
										{
											attacktype = 1;
											lastattacktype = 1;
										}
									}
									else
									{
										attacktype = 5;
										lastattacktype = 5;
									}
								}
								NPC.netUpdate = true;
							}
						}
                    }
					else if (NPC.life <= NPC.lifeMax * 0.35) //35%
                    {
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							int randattack = Main.rand.Next(1, 5 + 1);
							if (randattack == 1)
							{
								if (lastattacktype == 1)
								{
									int subrandattack = Main.rand.Next(1, 4 + 1);
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
									else if (subrandattack == 3)
									{
										attacktype = 4;
										lastattacktype = 4;
									}
									else
									{
										attacktype = 5;
										lastattacktype = 5;
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
									int subrandattack = Main.rand.Next(1, 4 + 1);
									if (subrandattack == 1)
									{
										attacktype = 1;
										lastattacktype = 1;
									}
									else if (subrandattack == 2)
									{
										attacktype = 4;
										lastattacktype = 4;
									}
									else if (subrandattack == 3)
									{
										attacktype = 3;
										lastattacktype = 3;
									}
									else
									{
										attacktype = 5;
										lastattacktype = 5;
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
									int subrandattack = Main.rand.Next(1, 4 + 1);
									if (subrandattack == 1)
									{
										attacktype = 2;
										lastattacktype = 2;
									}
									else if (subrandattack == 2)
									{
										attacktype = 1;
										lastattacktype = 1;
									}
									else if (subrandattack == 3)
									{
										attacktype = 5;
										lastattacktype = 5;
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
							else if (randattack == 4)
							{
								if (lastattacktype == 4)
								{
									int subrandattack = Main.rand.Next(1, 4 + 1);
									if (subrandattack == 1)
									{
										attacktype = 2;
										lastattacktype = 2;
									}
									else if (subrandattack == 2)
									{
										attacktype = 1;
										lastattacktype = 1;
									}
									else if (subrandattack == 3)
									{
										attacktype = 3;
										lastattacktype = 3;
									}
									else
									{
										attacktype = 5;
										lastattacktype = 5;
									}
								}
								else
								{
									attacktype = 4;
									lastattacktype = 4;
								}
							}
							else //5
							{
								if (lastattacktype == 5)
								{
									int subrandattack = Main.rand.Next(1, 4 + 1);
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
									else if (subrandattack == 3)
									{
										attacktype = 4;
										lastattacktype = 4;
									}
									else
									{
										attacktype = 1;
										lastattacktype = 1;
									}
								}
								else
								{
									attacktype = 5;
									lastattacktype = 5;
								}
							}
							NPC.netUpdate = true;
						}
					}
					else if (NPC.life <= NPC.lifeMax * 0.50) //50%
                    {
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							int randattack = Main.rand.Next(1, 4 + 1);
							if (randattack == 1)
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
							else if (randattack == 2)
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
										attacktype = 4;
										lastattacktype = 4;
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
									int subrandattack = Main.rand.Next(1, 3 + 1);
									if (subrandattack == 1)
									{
										attacktype = 2;
										lastattacktype = 2;
									}
									else if (subrandattack == 2)
									{
										attacktype = 1;
										lastattacktype = 1;
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
							else //4
							{
								if (lastattacktype == 4)
								{
									int subrandattack = Main.rand.Next(1, 3 + 1);
									if (subrandattack == 1)
									{
										attacktype = 2;
										lastattacktype = 2;
									}
									else if (subrandattack == 2)
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
									attacktype = 4;
									lastattacktype = 4;
								}
							}
							NPC.netUpdate = true;
						}
					}
					else if (NPC.life <= NPC.lifeMax * 0.75) //75%
                    {
						if (Main.netMode != NetmodeID.MultiplayerClient)
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
							else //3
							{
								if (lastattacktype == 3)
								{
									int subrandattack = Main.rand.Next(1, 2 + 1);
									if (subrandattack == 1)
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
								else
								{
									attacktype = 3;
									lastattacktype = 3;
								}
							}
							NPC.netUpdate = true;
						}
					}
					else if (NPC.life <= NPC.lifeMax * 0.90) //90%
                    {
						if (Main.netMode != NetmodeID.MultiplayerClient)
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
							else //2
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
							NPC.netUpdate = true;
						}
					}
					else
                    {
						attacktype = 1;
						lastattacktype = 1;
					}
				}





				//END OF SUPER DUPER LONG LINE OF CODE RANDOMIZING ZERO'ATTACKS
			}
			else //attacking
            {
				if (attacktype == 1) //blood shots
                {
					//effect for entire attack
                    for (int i = 0; i < 1; i++)
                    {
                        Vector2 spread = Main.rand.NextVector2Circular(10f, 10f); //circle
						Vector2 offset = new Vector2(NPC.direction * 125, 0);
                        Dust.NewDustPerfect(NPC.Center + offset, ModContent.DustType<Dusts.Redsidue>(), spread, Scale: 1f); //Makes dust in a messy circle
                    }

                    //1st shot of round

                    if (NPC.ai[0] == 140 || NPC.ai[0] == 180 || NPC.ai[0] == 220 || NPC.ai[0] == 260 || NPC.ai[0] == 300 || NPC.ai[0] == 340)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            bloodlocationx = Main.rand.Next(0, 50);
                            bloodlocationy = Main.rand.Next(0, 199);
                            NPC.netUpdate = true;

                            if (NPC.direction == -1) //left
                            {
                                bloodlocation = new Vector2(NPC.position.X + bloodlocationx, NPC.position.Y + (NPC.height / 4) + bloodlocationy);
                            }
                            else //right
                            {
                                bloodlocation = new Vector2(NPC.position.X + NPC.width - bloodlocationx, NPC.position.Y + (NPC.height / 4) + bloodlocationy);
                            }

                            for (int i = 0; i < 5; i++)
                            {
                                Vector2 spread = Main.rand.NextVector2Circular(10f, 10f); //circle
                                Dust.NewDustPerfect(bloodlocation, ModContent.DustType<Dusts.Redsidue>(), spread, Scale: 1f); //Makes dust in a messy circle
                            }

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), bloodlocation, new Vector2(NPC.direction * 25, 0), ModContent.ProjectileType<ZeroBloodShot>(), 60 / 2, 6f, Main.myPlayer, NPC.whoAmI, 0);
                        }
                        SoundStyle SkinTear = new SoundStyle("KirboMod/Sounds/Item/SkinTear");
                        SoundEngine.PlaySound(SkinTear, NPC.Center);
                    }

                    //2nd shot of round

                    if (NPC.ai[0] == 150 || NPC.ai[0] == 190 || NPC.ai[0] == 230 || NPC.ai[0] == 270 || NPC.ai[0] == 310 || NPC.ai[0] == 350)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            bloodlocationx = Main.rand.Next(0, 50);
                            bloodlocationy = Main.rand.Next(0, 199);
                            NPC.netUpdate = true;

                            if (NPC.direction == -1) //left
                            {
                                bloodlocation = new Vector2(NPC.position.X + bloodlocationx, NPC.position.Y + (NPC.height / 4) + bloodlocationy);
                            }
                            else //right
                            {
                                bloodlocation = new Vector2(NPC.position.X + NPC.width - bloodlocationx, NPC.position.Y + (NPC.height / 4) + bloodlocationy);
                            }

                            for (int i = 0; i < 5; i++)
                            {
                                Vector2 spread = Main.rand.NextVector2Circular(10f, 10f); //circle
                                Dust.NewDustPerfect(bloodlocation, ModContent.DustType<Dusts.Redsidue>(), spread, Scale: 1f); //Makes dust in a messy circle
                            }

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), bloodlocation, new Vector2(NPC.direction * 25, 0), ModContent.ProjectileType<ZeroBloodShot>(), 60 / 2, 6f, Main.myPlayer, NPC.whoAmI, 0);
                        }
                        SoundStyle SkinTear = new SoundStyle("KirboMod/Sounds/Item/SkinTear");
                        SoundEngine.PlaySound(SkinTear, NPC.Center);
                    }

                    //3rd shot of round

                    if (NPC.ai[0] == 160 || NPC.ai[0] == 200 || NPC.ai[0] == 240 || NPC.ai[0] == 280 || NPC.ai[0] == 320 || NPC.ai[0] == 360)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            bloodlocationx = Main.rand.Next(0, 50);
                            bloodlocationy = Main.rand.Next(0, 199);
                            NPC.netUpdate = true;

                            if (NPC.direction == -1) //left
                            {
                                bloodlocation = new Vector2(NPC.position.X + bloodlocationx, NPC.position.Y + (NPC.height / 4) + bloodlocationy);
                            }
                            else //right
                            {
                                bloodlocation = new Vector2(NPC.position.X + NPC.width - bloodlocationx, NPC.position.Y + (NPC.height / 4) + bloodlocationy);
                            }

                            for (int i = 0; i < 5; i++)
                            {
                                Vector2 spread = Main.rand.NextVector2Circular(10f, 10f); //circle
                                Dust.NewDustPerfect(bloodlocation, ModContent.DustType<Dusts.Redsidue>(), spread, Scale: 1f); //Makes dust in a messy circle
                            }

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), bloodlocation, new Vector2(NPC.direction * 25, 0), ModContent.ProjectileType<ZeroBloodShot>(), 60 / 2, 6f, Main.myPlayer, NPC.whoAmI, 0);
                        }
                        SoundStyle SkinTear = new SoundStyle("KirboMod/Sounds/Item/SkinTear");
                        SoundEngine.PlaySound(SkinTear, NPC.Center);
                    }

                    if (playerDistance.X <= 0)
					{
						playerRightDistance.Normalize();
						playerRightDistance *= speed;
						NPC.velocity = (NPC.velocity * (inertia - 1) + playerRightDistance) / inertia; //fly towards player
					}
					else
					{
						playerLeftDistance.Normalize();
						playerLeftDistance *= speed;
						NPC.velocity = (NPC.velocity * (inertia - 1) + playerLeftDistance) / inertia; //fly towards player
					}

					if (NPC.ai[0] >= 440)
					{
						NPC.ai[0] = 0;
					}
				}


				if (attacktype == 2) //dash
				{
                    NPC.TargetClosest(false);

                    if (NPC.ai[0] <= 150) //backup
					{
						if (NPC.ai[0] == 121)
						{
							NPC.velocity *= 0.01f; //freeze to warn player (but not too much else it might disappear)
						}
						else
						{
							NPC.velocity.X -= NPC.direction * 0.2f; //back up
						}
					}
					else if (NPC.ai[0] <= 270) //dash
					{
						NPC.velocity.X += NPC.direction * 0.4f;
					}
					else //reset
					{
						NPC.ai[0] = 0;
						backupoffset = 0;
					}

					//go up or down
					if (player.Center.Y < NPC.Center.Y)
					{
						NPC.velocity.Y -= 0.4f;
					}
					else
					{
						NPC.velocity.Y += 0.4f;
					}

					//cap
					if (NPC.velocity.Y > 5f)
					{
						NPC.velocity.Y = 5f;
					}
					if (NPC.velocity.Y < -5f)
					{
						NPC.velocity.Y = -5f;
					}
				}



				if (attacktype == 3) //dark matter shot
				{
                    if (NPC.ai[0] % 40 == 0)
					{
                        Vector2 randomlocation = new Vector2(NPC.position.X + Main.rand.Next(0, NPC.width), NPC.position.Y + Main.rand.Next(20, NPC.height - 20));

                        for (int i = 0; i < 5; i++)
                        {
                            Dust d = Dust.NewDustPerfect(randomlocation, ModContent.DustType<DarkResidue>(), Main.rand.NextVector2Circular(10f, 10f), Scale: 1f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }

						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							Projectile.NewProjectile(NPC.GetSource_FromAI(), randomlocation, new Vector2(NPC.direction * 0.05f, 0), ModContent.ProjectileType<DarkMatterShot>(), 80 / 2, 8f, Main.myPlayer, 0, player.whoAmI);
						}
                        SoundEngine.PlaySound(SoundID.Item81, NPC.Center); //spawn slime mount
					}

					if (playerDistance.X <= 0)
					{
						playerRightDistance.Normalize();
						playerRightDistance *= speed;
						NPC.velocity = (NPC.velocity * (inertia - 1) + playerRightDistance) / inertia; //fly towards player
					}
					else
					{
						playerLeftDistance.Normalize();
						playerLeftDistance *= speed;
						NPC.velocity = (NPC.velocity * (inertia - 1) + playerLeftDistance) / inertia; //fly towards player
					}

					if (NPC.ai[0] >= 360)
					{
						NPC.ai[0] = 0;
					}
				}

				if (attacktype == 4) //sparks
				{
					//shoot 30 in intervals
					if (NPC.ai[0] > 120 && NPC.ai[0] <= 130 || NPC.ai[0] > 180 && NPC.ai[0] <= 190 || NPC.ai[0] > 240 && NPC.ai[0] <= 250 || NPC.ai[0] > 300 && NPC.ai[0] <= 310)
					{
						//shoot all at once
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 150, 0), new Vector2(NPC.direction * Main.rand.Next(1, 50), Main.rand.Next(-30, 30)), Mod.Find<ModProjectile>("ZeroSpark").Type, 0, 0, Main.myPlayer, 0, 0);
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 150, 0), new Vector2(NPC.direction * Main.rand.Next(1, 25), Main.rand.Next(-15, 15)), Mod.Find<ModProjectile>("ZeroSpark").Type, 0, 0, Main.myPlayer, 0, 0);
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 150, 0), new Vector2(NPC.direction * Main.rand.Next(1, 100), Main.rand.Next(-60, 60)), Mod.Find<ModProjectile>("ZeroSpark").Type, 0, 0, Main.myPlayer, 0, 0);
						}
                        SoundEngine.PlaySound(SoundID.Item5.WithVolumeScale(1.6f).WithPitchOffset(0.1f), NPC.Center); //bow shot
					}

					if (playerDistance.X <= 0)
					{
						playerRightDistance.Normalize();
						playerRightDistance *= speed;
						NPC.velocity = (NPC.velocity * (inertia - 1) + playerRightDistance) / inertia; //fly towards player
					}
					else
					{
						playerLeftDistance.Normalize();
						playerLeftDistance *= speed;
						NPC.velocity = (NPC.velocity * (inertia - 1) + playerLeftDistance) / inertia; //fly towards player
					}

					if (NPC.ai[0] >= 360)
					{
						NPC.ai[0] = 0;
					}
				}

				if (attacktype == 5) //thorns
                {
					if (NPC.ai[0] < 180)
					{
						animation = 2;
						NPC.velocity *= 0;

						if (NPC.ai[0] == 130)
                        {
							SoundEngine.PlaySound(SoundID.Item56.WithVolumeScale(1.6f).WithPitchOffset(-0.1f), NPC.Center); //grass
						}
					}
					else if (NPC.ai[0] < 480)
					{
						animation = 3;

						playerAboveDistance.Normalize();
						playerAboveDistance *= speed;
						NPC.velocity = (NPC.velocity * (inertia - 1) + playerAboveDistance) / inertia; //fly towards player

						if (NPC.ai[0] % 5 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
						{ 
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, 300), new Vector2(Main.rand.Next(-10, 10), -5), Mod.Find<ModProjectile>("ZeroThornJuice").Type, 60 / 2, 1f, Main.myPlayer, 0);
					    }
						if (NPC.ai[0] % 20 == 0)
						{
							SoundEngine.PlaySound(SoundID.Grass.WithVolumeScale(1.2f).WithPitchOffset(-0.1f), NPC.Center); //grass
						}
					}
					else if (NPC.ai[0] < 510)
					{
						if (NPC.ai[0] == 480)
						{
                            NPC.frameCounter = 0; //reset animation
                        }
						animation = 4;
						NPC.velocity *= 0;
					}
					else
                    {
						NPC.ai[0] = 0;
					}
				}
				if (attacktype == 6) //background attack
				{
					NPC.velocity.Y *= 0.98f;

					if (NPC.ai[0] <= 180) //backup
					{
						NPC.TargetClosest(false);

						NPC.scale -= 0.01f; //get smaller
						NPC.behindTiles = true;
						NPC.damage = 0;
						NPC.dontTakeDamage = true;

						if (NPC.ai[0] == 121)
						{
							NPC.velocity.X *= 0; //reset momentum
						}
						else
						{
							NPC.velocity.X -= NPC.direction * 0.2f; //back up
						}

						animation = 5;
					}
					else if (NPC.ai[0] <= 480) //zoom
					{
						NPC.TargetClosest(false);

						if (NPC.ai[0] % 9 == 0 && NPC.ai[0] < 420) //shoot projectiles before getting big again
                        {
							Vector2 BackgroundplayerDistance = player.Center - NPC.Center;

							if (Main.netMode != NetmodeID.MultiplayerClient) 
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, BackgroundplayerDistance / 60, Mod.Find<ModProjectile>("ZeroScreenBlood").Type, 80 / 2, 1f, Main.myPlayer, 0, NPC.ai[0] % 2 == 0 ? 0 : 1);
							}
							SoundEngine.PlaySound(SoundID.NPCHit9.WithVolumeScale(0.8f), NPC.Center); //leech hit

						}

						NPC.velocity.X += NPC.direction * 0.2f;

						if (NPC.velocity.X > 10)
                        {
							NPC.velocity.X = 10;
						}
						if (NPC.velocity.X < -10)
						{
							NPC.velocity.X = -10;
						}

						if (NPC.ai[0] >= 420)
						{
							NPC.scale += 0.01f; //get bigger
						}
					}
					else //reset
					{
						NPC.TargetClosest(true);
						NPC.ai[0] = 0;
						backupoffset = 0;
						NPC.scale = 1; //just in case
						NPC.behindTiles = false;
					}
				}
			}
		}

        /*public override void FindFrame(int frameHeight) // animation
        {
			int Yframe = 0; //Y determining which part of the animation

			if (animation == 0) //regular
			{
                NPC.frameCounter ++;
				if (NPC.frameCounter < 10)
				{
                    animationXframeOffset = 0; //attacks 
                    Yframe = 0;
                }
				else if (NPC.frameCounter < 20)
				{
                    animationXframeOffset = 0; //attacks 
                    Yframe = 800;
                }
				else if (NPC.frameCounter < 30)
				{
                    animationXframeOffset = 0; //attacks 
                    Yframe = 1600;
                }
				else if (NPC.frameCounter < 40)
				{
                    animationXframeOffset = 0; //attacks 
                    Yframe = 800;
                }
				else
				{
                    animationXframeOffset = 0; //attacks 
                    Yframe = 0;
                    NPC.frameCounter = 0; //reset
				}
			}
			if (animation == 1) //intro
            {
				NPC.frameCounter++;
				if (NPC.frameCounter < 60) //change texture
				{
                    animationXframeOffset = 1; //intro 1
                    Yframe = 0;
                }
				else if (NPC.frameCounter < 70)
				{
                    animationXframeOffset = 1; //intro 1
                    Yframe = 800;
                }
				else if (NPC.frameCounter < 80)
				{
                    animationXframeOffset = 1; //intro 1
                    Yframe = 1600;
                }
				else if (NPC.frameCounter < 90)
				{
                    animationXframeOffset = 1; //intro 1
                    Yframe = 2400;
                }
				else if (NPC.frameCounter < 150)
				{
                    animationXframeOffset = 1; //intro 1
                    Yframe = 3200; 
                }
				else if (NPC.frameCounter < 160)
				{
                    animationXframeOffset = 1; //intro 1
                    Yframe = 4000;
                }
				else if (NPC.frameCounter < 170)
				{
                    animationXframeOffset = 1; //intro 1
                    Yframe = 4800;
				}
				else if (NPC.frameCounter < 180) //change texture
                {
                    animationXframeOffset = 2; //intro 2
                    Yframe = 0;
                }
				else if (NPC.frameCounter < 240) 
                {
                    animationXframeOffset = 2; //intro 2
                    Yframe = 800; 
                }
				else if (NPC.frameCounter < 250)
				{
                    animationXframeOffset = 2; //intro 2
                    Yframe = 1600;
                }
				else if (NPC.frameCounter < 310)
				{
                    animationXframeOffset = 2; //intro 2
                    Yframe = 2400;
                }
				else if (NPC.frameCounter < 320)
				{
                    animationXframeOffset = 2; //intro 2
                    Yframe = 3200;
                }
				else 
				{
                    animationXframeOffset = 2; //intro 2
                    Yframe = 4000;
                }
			}
			if (animation == 2) //thorns open
			{
                NPC.frameCounter++;
				if (NPC.frameCounter < 10)
				{
                    animationXframeOffset = 0; //attacks 
                    Yframe = 2400;
                }
				else 
				{
                    animationXframeOffset = 0; //attacks 
                    Yframe = 3200;
                }
			}
			if (animation == 3) //thorns loop
			{
                animationXframeOffset = 0; //attacks 

                NPC.frameCounter++;
				if (NPC.frameCounter < 10)
				{
                    animationXframeOffset = 0; //attacks 
                    Yframe = 3200;
                }
				else if (NPC.frameCounter < 20)
				{
                    animationXframeOffset = 0; //attacks 
                    Yframe = 4000;
                }
				else if (NPC.frameCounter < 30)
				{
                    animationXframeOffset = 0; //attacks 
                    Yframe = 3200;
                }
				else if (NPC.frameCounter < 40)
				{
                    animationXframeOffset = 0; //attacks 
                    Yframe = 4800;
                }
				else
				{
                    Yframe = 3200;
                    NPC.frameCounter = 0; //reset
				}
			}
			if (animation == 4) //thorns close
			{
                NPC.frameCounter++;
				if (NPC.frameCounter < 10)
				{
                    animationXframeOffset = 0; //attacks 
                    Yframe = 2400;
                }
				else 
				{
                    animationXframeOffset = 0; //attacks 
                    Yframe = 5600;
                }
			}
			if (animation == 5) //front face
			{
                animationXframeOffset = 0; //attacks 
                Yframe = 5600;
            }

            NPC.frame = new Rectangle(animationXframeOffset * 800, Yframe, 800, 800);
        }*/

		public override Color? GetAlpha(Color lightColor)
		{
            int r;
            int g;
            int b;
            r = 255 - NPC.alpha;
            g = 255 - NPC.alpha;
            b = 255 - NPC.alpha;
            return new Color(r, g, b, 0); //fade in 
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			scale = 1.5f;

			position.Y = NPC.position.Y + NPC.height + 40;

			if (NPC.life == 1 && deathcounter > 0)
            {
				return false; //no healt bar
            }
			else //healt bar
			return true;
		}

        public override bool CheckDead()
        {
            if (deathcounter < 300)
            {
				NPC.active = true;
				NPC.life = 1;
				deathcounter += 1; //go up
				return false;
            }
			return true;
		}

		public override bool PreKill()
		{
			if (Main.expertMode == false)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private void DoDeathAnimation()
        {
			if (deathcounter > 0 && deathcounter < 300)
			{
				NPC.ai[0] = 0; //don't attack
				NPC.dontTakeDamage = true;
				deathcounter += 1; //go up
				NPC.damage = 0;
				NPC.active = true;
				NPC.velocity *= 0.95f;
				animation = 0; //regular

				if (Main.expertMode)
				{
					NPC.rotation = MathHelper.ToRadians(90); //up
					NPC.direction = -1; //to make sure it doesn't face down
				}
				else
				{
					NPC.rotation += MathHelper.ToRadians(5); //rotate
				}

				if (deathcounter % 5 == 0) //effects
				{
					int randomX = Main.rand.Next(0, NPC.width);
					int randomY = Main.rand.Next(0, NPC.height);
					SoundEngine.PlaySound(SoundID.NPCHit1, NPC.Center);

					for (int i = 0; i < 10; i++) //first section makes variable //second declares the conditional // third declares the loop
					{
						Vector2 speed = Main.rand.NextVector2Circular(8f, 8f); //circle
						Dust d = Dust.NewDustPerfect(NPC.position + new Vector2(randomX, randomY), ModContent.DustType<Dusts.Redsidue>(), speed, Scale: 2); //Makes dust in a messy circle
						d.noGravity = true;
					}
				}

				if ((deathcounter == 260 || deathcounter == 280) && !Main.expertMode) //only in normal mode
				{
					for (int i = 0; i < 40; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
					{
						Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
						Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.Redsidue>(), speed * 20, Scale: 4); //Makes dust in a messy circle
						d.noGravity = true;
					}

					SoundEngine.PlaySound(SoundID.NPCDeath1, NPC.Center);
				}
			}
			else if (deathcounter > 0)
			{
				NPC.dontTakeDamage = false;
                NPC.HideStrikeDamage = true;
                NPC.SimpleStrikeNPC(999999, 1, false, 0, null, false, 0, false);

				if (Main.expertMode)
				{
					for (int i = 0; i < 40; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				    {
					Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
					Dust d = Dust.NewDustPerfect(NPC.Center + new Vector2(0, -200), ModContent.DustType<Dusts.Redsidue>(), speed * 20, Scale: 4); //Makes dust in a messy circle
					d.noGravity = true;
				    }

					Dust.NewDustPerfect(NPC.position + new Vector2(-200, -200), ModContent.DustType<Dusts.ZeroEyeless>(), new Vector2(0, 5), 0);

                    if (Main.netMode != NetmodeID.MultiplayerClient) //don't use SpawnBoss() as we need the special status message
                    {
						int index = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y - 15, ModContent.NPCType<ZeroEye>());

                        if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
						{
                            NetMessage.SendData(MessageID.SyncNPC, number: index);
                        }

                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            Main.NewText("Zero's eye has ejected from it's body!", 175, 75);
                        }
                        else if (Main.netMode == NetmodeID.Server)
                        {
                            ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Zero's eye has ejected from it's body!"), new Color(175, 75, 255));
                        }
                    }
                }
			}
		}

		public override void BossLoot(ref string name, ref int potionType)
		{
			name = "Zero"; //_ has been defeated!
			potionType = ItemID.SuperHealingPotion; //potion it drops
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (deathcounter >= 300 && !Main.expertMode) //only in normal mode
			{
				for (int i = 0; i < 40; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
					Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.Redsidue>(), speed * 20, Scale: 4); //Makes dust in a messy circle
					d.noGravity = true;
				}

				SoundEngine.PlaySound(SoundID.NPCDeath1, NPC.Center);
			}
			else
			{
				for (int i = 0; i < 2; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
					Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.Redsidue>(), speed, 1); //Makes dust in a messy circle
					d.noGravity = true;
				}
			}
		}

        public override void OnKill()
        {
			if (!Main.expertMode && !Main.masterMode) //only register when not expert mode and master mode
			{
				NPC.SetEventFlagCleared(ref DownedBossSystem.downedZeroBoss, -1);
			}
        }

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<ZeroBag>())); //only drops in expert

            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert()); //checks if not expert
            LeadingConditionRule masterMode = new LeadingConditionRule(new Conditions.IsMasterMode()); //checks if master mode

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<ZeroMask>(), 7));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.MiracleMatter>(), 1, 2, 2));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Zero.ZeroTrophy>(), 10)); //drop trophy

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeables.BossRelics.ZeroRelic>()));

            masterMode.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.Zero.ZeroPetItem>(), 4));

            // add the rules
            npcLoot.Add(notExpertRule);
            npcLoot.Add(masterMode);
        }

		public override void DrawBehind(int index)
        {
			if (attacktype == 6 & NPC.ai[0] > 120)
			{
				NPC.hide = true;
				Main.instance.DrawCacheNPCsMoonMoon.Add(index);//be drawn behind things like moonlord(?)
			}
			else
			{
                NPC.hide = false;
            }
        }

        public override void ModifyHoverBoundingBox(ref Rectangle boundingBox) //box where NPC name and health is shown
        {
            boundingBox = NPC.Hitbox;
        }
		


		//MANUAL DRAWING INBOUND!




        public static Asset<Texture2D> ZeroSprite;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			ZeroSprite = ModContent.Request<Texture2D>("KirboMod/NPCs/Zero"); //get the texture

			Texture2D Zero = ZeroSprite.Value; //get the texture but actually

			//fade in stuff
            int r;
            int g;
            int b;
            r = 255 - NPC.alpha;
            g = 255 - NPC.alpha;
            b = 255 - NPC.alpha;

			SpriteEffects direction = SpriteEffects.FlipHorizontally; //face right
			if (NPC.direction == -1)
			{
                direction = SpriteEffects.None; //face left
            }

			//draw!
            spriteBatch.Draw(Zero, NPC.Center - Main.screenPosition + new Vector2(0, 0), Animation(), new Color(r, g, b), NPC.rotation, 
				new Vector2(400, 400), NPC.scale, direction, 0f);
            return false;
        }

		private Rectangle Animation() //make the dimensions for the frames
		{
			//inital
            int Xframe = 0;
            int Yframe = 0;

            if (animation == 0) //regular
            {
				if (!Main.gamePaused && Main.hasFocus) //not paused or tabbed out
				{
					NPC.frameCounter++;
				}
                if (NPC.frameCounter < 10)
                {
                    Xframe = 0; //attacks 
                    Yframe = 0;
                }
                else if (NPC.frameCounter < 20)
                {
                    Xframe = 0; //attacks 
                    Yframe = 800;
                }
                else if (NPC.frameCounter < 30)
                {
                    Xframe = 0; //attacks 
                    Yframe = 1600;
                }
                else if (NPC.frameCounter < 40)
                {
                    Xframe = 0; //attacks 
                    Yframe = 800;
                }
                else
                {
                    Xframe = 0; //attacks 
                    Yframe = 0;
                    NPC.frameCounter = 0; //reset
                }
            }
            if (animation == 1) //intro
            {
				if (!Main.gamePaused && Main.hasFocus) //not paused or tabbed out
				{
					NPC.frameCounter++;
				}
                if (NPC.frameCounter < 60) //change texture
                {
                    Xframe = 1; //intro 1
                    Yframe = 0;
                }
                else if (NPC.frameCounter < 70)
                {
                    Xframe = 1; //intro 1
                    Yframe = 800;
                }
                else if (NPC.frameCounter < 80)
                {
                    Xframe = 1; //intro 1
                    Yframe = 1600;
                }
                else if (NPC.frameCounter < 90)
                {
                    Xframe = 1; //intro 1
                    Yframe = 2400;
                }
                else if (NPC.frameCounter < 150)
                {
                    Xframe = 1; //intro 1
                    Yframe = 3200;
                }
                else if (NPC.frameCounter < 160)
                {
                    Xframe = 1; //intro 1
                    Yframe = 4000;
                }
                else if (NPC.frameCounter < 170)
                {
                    Xframe = 1; //intro 1
                    Yframe = 4800;
                }
                else if (NPC.frameCounter < 180) //change texture
                {
                    Xframe = 2; //intro 2
                    Yframe = 0;
                }
                else if (NPC.frameCounter < 240)
                {
                    Xframe = 2; //intro 2
                    Yframe = 800;
                }
                else if (NPC.frameCounter < 250)
                {
                    Xframe = 2; //intro 2
                    Yframe = 1600;
                }
                else if (NPC.frameCounter < 310)
                {
                    Xframe = 2; //intro 2
                    Yframe = 2400;
                }
                else if (NPC.frameCounter < 320)
                {
                    Xframe = 2; //intro 2
                    Yframe = 3200;
                }
                else
                {
                    Xframe = 2; //intro 2
                    Yframe = 4000;
                }
            }
            if (animation == 2) //thorns open
            {
				if (!Main.gamePaused && Main.hasFocus) //not paused or tabbed out
				{
					NPC.frameCounter++;
				}
                if (NPC.frameCounter < 10)
                {
                    Xframe = 0; //attacks 
                    Yframe = 2400;
                }
                else
                {
                    Xframe = 0; //attacks 
                    Yframe = 3200;
                }
            }
            if (animation == 3) //thorns loop
            {
                Xframe = 0; //attacks 
				if (!Main.gamePaused && Main.hasFocus) //not paused or tabbed out
				{
					NPC.frameCounter++;
				}
                if (NPC.frameCounter < 10)
                {
                    Xframe = 0; //attacks 
                    Yframe = 3200;
                }
                else if (NPC.frameCounter < 20)
                {
                    Xframe = 0; //attacks 
                    Yframe = 4000;
                }
                else if (NPC.frameCounter < 30)
                {
                    Xframe = 0; //attacks 
                    Yframe = 3200;
                }
                else if (NPC.frameCounter < 40)
                {
                    Xframe = 0; //attacks 
                    Yframe = 4800;
                }
                else
                {
                    Yframe = 3200;
                    NPC.frameCounter = 0; //reset
                }
            }
            if (animation == 4) //thorns close
            {
				if (!Main.gamePaused && Main.hasFocus) //not paused or tabbed out
				{
					NPC.frameCounter++;
				}
                if (NPC.frameCounter < 10)
                {
                    Xframe = 0; //attacks 
                    Yframe = 2400;
                }
                else
                {
                    Xframe = 0; //attacks 
                    Yframe = 5600;
                }
            }
            if (animation == 5) //front face
            {
                Xframe = 0; //attacks 
                Yframe = 5600;
            }

			return new Rectangle(Xframe * 800, Yframe, 800, 800);
        }
    }
}
