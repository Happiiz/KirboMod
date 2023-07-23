using KirboMod.Items.DarkMatter;
using KirboMod.Items.Zero;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using KirboMod.Bestiary;
using KirboMod.Systems;
using Terraria.DataStructures;

namespace KirboMod.NPCs
{
	[AutoloadBossHead]
	public class DarkMatter : ModNPC
	{
		private int animation = 0; //frame cycles

		private float lunge = 20; // velocity of lunge when lunging

		private int phase = 1; //phase 1 = blade, phase 2 = transition, phase 3 = true form
		private int phase2special = 1; //for deciding which special is used in phase 2

		private int frenzydashamount = 2; //not 3 because the inital dash

		private bool frenzy = false;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dark Matter");
			Main.npcFrameCount[NPC.type] = 6;

            // Add this in for bosses that have a summon item, requires corresponding code in the item
            NPCID.Sets.MPAllowedEnemies[Type] = true; 

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                //CustomTexturePath = ,
                PortraitScale = 1, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 0,
                PortraitPositionXOverride = 20,
                Position = new Vector2(20, 0),
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] 
				{
                    BuffID.Confused, // Most NPCs have this
		            BuffID.Poisoned,
                    BuffID.Venom,
                    BuffID.OnFire,
                    BuffID.CursedInferno,
                    BuffID.ShadowFlame,
                }
            };
            NPCID.Sets.DebuffImmunitySets[Type] = debuffData;
        }

		public override void Load() //I use this to allow me to use the boss head
        {
            Mod.AddBossHeadTexture("KirboMod/NPCs/DarkMatter_Head_Boss2");
        }

		public override void SetDefaults()
		{
			NPC.width = 130;
			NPC.height = 130;
			NPC.damage = 100;
			NPC.noTileCollide = true;
			NPC.defense = 35;
			NPC.lifeMax = 52000;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice( 0, 19, 9, 5); // money it drops
			NPC.knockBackResist = 0f;
			NPC.aiStyle = -1;
			NPC.npcSlots = 12;
			NPC.boss = true;
			NPC.noGravity = true;
			NPC.lavaImmune = true;
			NPC.buffImmune[BuffID.Poisoned] = true;
			NPC.buffImmune[BuffID.Venom] = true;
			NPC.buffImmune[BuffID.OnFire] = true;
			NPC.buffImmune[BuffID.CursedInferno] = true;
			NPC.buffImmune[BuffID.ShadowFlame] = true;
			Music = MusicID.Boss4;
            NPC.buffImmune[BuffID.Confused] = true;
        }

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.7 * balance);
			NPC.damage = (int)(NPC.damage * 0.6);
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new SurfaceBackgroundProvider(), //I totally didn't steal this code
				// Sets the spawning conditions of this NPC that is listed in the bestiary.

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("A being of pure evil bent on spreading darkness. What could be the origin of this innate vile in it's heart?")
            });
        }

        public override void AI() //constantly cycles each time
		{
			Player playerstate = Main.player[NPC.target];

			//cap life
			if (NPC.life >= NPC.lifeMax)
			{
				NPC.life = NPC.lifeMax;
			}
			//DESPAWNING
			if (NPC.target < 0 || NPC.target == 255 || playerstate.dead || !playerstate.active)
			{
				NPC.TargetClosest(false);

				if (phase == 1 || phase == 2) //rise into the sky to never be seen again!
				{
					NPC.velocity.Y = NPC.velocity.Y - 0.4f;
				}
				else //rise phaster
                {
					NPC.velocity.Y = NPC.velocity.Y - 0.2f;
				}

				if (NPC.timeLeft > 60)
				{
					NPC.timeLeft = 60;
					return;
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
			Vector2 moveTo = player.Center; 
			Vector2 playerDistance = player.Center - NPC.Center;
			Vector2 move = player.Center - NPC.Center;

			if (phase == 1) //DARK MATTER BLADE
			{
		       
				NPC.spriteDirection = NPC.direction;
				NPC.rotation = 0.0f;

				if (NPC.ai[0] <= 360) //Movement before special attack (Alot of this code was taken from ExampleMod's capitive element(2)) so I don't fully understand this yet, but it makes my video game reference go smooth so I don't mind it
				{
					float minX = moveTo.X - 50f;
					float maxX = moveTo.X + 50f;
					float minY = moveTo.Y;
					float maxY = moveTo.Y;

					if (playerDistance.X <= 0) //if player is behind enemy
					{
						move = move + new Vector2(400f, -20); // go in front of player and slightly above
					}
					else
                    {
						move = move + new Vector2(-400f, -20); // go behind player and slightly above
					}

					if (NPC.Center.X >= minX && NPC.Center.X <= maxX && NPC.Center.Y >= minY && NPC.Center.Y <= maxY) //certain range
					{
						NPC.velocity *= 0.98f; //slow
					}
					else
					{
						float magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y);
						float speed = 14f; //speed I think
						if (magnitude > speed)
						{
							move *= speed / magnitude;
						}
						float inertia = 10f; //Ok so like I'm pretty sure this is supposed to be how wibbly wobbly you want the npc to be before it reaches its destination
						NPC.velocity = (inertia * NPC.velocity + move) / (inertia + 1);
						magnitude = (float)Math.Sqrt(NPC.velocity.X * NPC.velocity.X + NPC.velocity.Y + NPC.velocity.Y);
						if (magnitude > speed)
						{
							NPC.velocity *= speed / magnitude;
						}
					}
					NPC.TargetClosest(true);
				}

				animation = 0; //blade form

				if (NPC.ai[0] == 0) //checks if cycle has restarted
				{
					if (NPC.life <= NPC.lifeMax * 0.75 & Main.expertMode) //checks if npc has 3/4 of life and in expert mode
					{
						frenzy = true;
					}
					else
					{
						frenzy = false;
					}
				}

				NPC.ai[0]++; //phase 1 cycle

				//PROJECTILE
				Vector2 beamshoot = player.Center - NPC.Center;

				
				if (NPC.ai[0] == 60)
				{
					beamshoot.Normalize(); //reduces it to a value of 1
					beamshoot *= 20f; //inital projectile speed
					beamshoot.Y = 0f;
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 20f, beamshoot.X, beamshoot.Y, Mod.Find<ModProjectile>("DarkBeam").Type, (NPC.damage / 3) / 2, 8f, Main.myPlayer, 0, 0);
					}
					SoundEngine.PlaySound(SoundID.Item15); //phaseblade
				}
				if (NPC.ai[0] == 80)
				{
					beamshoot.Normalize(); //reduces it to a value of 1
					beamshoot *= 20f; //inital projectile speed
					beamshoot.Y = 0f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 20f, beamshoot.X, beamshoot.Y, Mod.Find<ModProjectile>("DarkBeam").Type, (NPC.damage / 3) / 2, 8f, Main.myPlayer, 0, 0);
                    }
                    SoundEngine.PlaySound(SoundID.Item15);
				}
				if (NPC.ai[0] == 100)
				{
					beamshoot.Normalize(); //reduces it to a value of 1
					beamshoot *= 20f; //inital projectile speed
					beamshoot.Y = 0f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 20f, beamshoot.X, beamshoot.Y, Mod.Find<ModProjectile>("DarkBeam").Type, (NPC.damage / 3) / 2, 8f, Main.myPlayer, 0, 0);
                    }
                    SoundEngine.PlaySound(SoundID.Item15);
				}


				if (frenzy == true) //angy
				{
					if (NPC.ai[0] == 120)
					{
						beamshoot.Normalize(); //reduces it to a value of 1
						beamshoot *= 20f; //inital projectile speed
						beamshoot.Y = 0f;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 20f, beamshoot.X, beamshoot.Y, Mod.Find<ModProjectile>("DarkBeam").Type, (NPC.damage / 3) / 2, 8f, Main.myPlayer, 0, 0);
                        }
                        SoundEngine.PlaySound(SoundID.Item15); //phaseblade
					}
					if (NPC.ai[0] == 140)
					{
						beamshoot.Normalize(); //reduces it to a value of 1
						beamshoot *= 20f; //inital projectile speed
						beamshoot.Y = 0f;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 20f, beamshoot.X, beamshoot.Y, Mod.Find<ModProjectile>("DarkBeam").Type, (NPC.damage / 3) / 2, 8f, Main.myPlayer, 0, 0);
                        }
                        SoundEngine.PlaySound(SoundID.Item15);
					}
				}


				if (NPC.ai[0] == 160)
				{
					beamshoot.Normalize(); //reduces it to a value of 1
					beamshoot *= 20f; //inital projectile speed
					beamshoot.Y = 0f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 20f, beamshoot.X, beamshoot.Y, Mod.Find<ModProjectile>("DarkBeam").Type, (NPC.damage / 3) / 2, 8f, Main.myPlayer, 0, 0);
                    }
                    SoundEngine.PlaySound(SoundID.Item15); //phaseblade
				}
				if (NPC.ai[0] == 180)
				{
					beamshoot.Normalize(); //reduces it to a value of 1
					beamshoot *= 20f; //inital projectile speed
					beamshoot.Y = 0f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 20f, beamshoot.X, beamshoot.Y, Mod.Find<ModProjectile>("DarkBeam").Type, (NPC.damage / 3) / 2, 8f, Main.myPlayer, 0, 0);
                    }
                    SoundEngine.PlaySound(SoundID.Item15);
				}
				if (NPC.ai[0] == 200)
				{
					beamshoot.Normalize(); //reduces it to a value of 1
					beamshoot *= 20f; //inital projectile speed
					beamshoot.Y = 0f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 20f, beamshoot.X, beamshoot.Y, Mod.Find<ModProjectile>("DarkBeam").Type, (NPC.damage / 3) / 2, 8f, Main.myPlayer, 0, 0);
                    }
                    SoundEngine.PlaySound(SoundID.Item15);
				}


				if (frenzy == true) //angy
				{
					if (NPC.ai[0] == 220)
					{
						beamshoot.Normalize(); //reduces it to a value of 1
						beamshoot *= 20f; //inital projectile speed
						beamshoot.Y = 0f;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 20f, beamshoot.X, beamshoot.Y, Mod.Find<ModProjectile>("DarkBeam").Type, (NPC.damage / 3) / 2, 8f, Main.myPlayer, 0, 0);
                        }
                        SoundEngine.PlaySound(SoundID.Item15); //phaseblade
					}
					if (NPC.ai[0] == 240)
					{
						beamshoot.Normalize(); //reduces it to a value of 1
						beamshoot *= 20f; //inital projectile speed
						beamshoot.Y = 0f;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 20f, beamshoot.X, beamshoot.Y, Mod.Find<ModProjectile>("DarkBeam").Type, (NPC.damage / 3) / 2, 8f, Main.myPlayer, 0, 0);
                        }
                        SoundEngine.PlaySound(SoundID.Item15);
					}
				}


				if (NPC.ai[0] == 260)
				{
					beamshoot.Normalize(); //reduces it to a value of 1
					beamshoot *= 20f; //inital projectile speed
					beamshoot.Y = 0f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 20f, beamshoot.X, beamshoot.Y, Mod.Find<ModProjectile>("DarkBeam").Type, (NPC.damage / 3) / 2, 8f, Main.myPlayer, 0, 0);
                    }
                    SoundEngine.PlaySound(SoundID.Item15);
				}
				if (NPC.ai[0] == 280)
				{
					beamshoot.Normalize(); //reduces it to a value of 1
					beamshoot *= 20f; //inital projectile speed
					beamshoot.Y = 0f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 20f, beamshoot.X, beamshoot.Y, Mod.Find<ModProjectile>("DarkBeam").Type, (NPC.damage / 3) / 2, 8f, Main.myPlayer, 0, 0);
                    }
                    SoundEngine.PlaySound(SoundID.Item15);
				}
				if (NPC.ai[0] == 300)
				{
					beamshoot.Normalize(); //reduces it to a value of 1
					beamshoot *= 20f; //inital projectile speed
					beamshoot.Y = 0f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 20f, beamshoot.X, beamshoot.Y, Mod.Find<ModProjectile>("DarkBeam").Type, (NPC.damage / 3) / 2, 8f, Main.myPlayer, 0, 0);
                    }
                    SoundEngine.PlaySound(SoundID.Item15);
				}

				if (NPC.ai[0] == 360) //Increase ai[1] to make dark orb attack happen next cycle
				{
					NPC.ai[1] += 1;
				}

				//LUNGE
				if (NPC.ai[0] >= 360 & NPC.ai[0] <= 380 )//back up
				{
					NPC.velocity.X += NPC.direction * -1.1f;

					NPC.velocity.Y *= 0;
				}

				if (NPC.ai[0] == 321) //declaring lunge values
				{
					if (player.Center.X > NPC.Center.X) //if player in front of npc
					{
						lunge = 30; //left
					}
					else
					{
						lunge = -30; //right
					}
					NPC.velocity.Y *= 0;
				}
				if (NPC.ai[0] >= 382 & NPC.ai[0] <= 391)//lunge
				{
					NPC.TargetClosest(false);
					NPC.velocity.X = lunge;
					NPC.velocity.Y *= 0;
				}

				if (frenzy == false) //not angy
				{
					if (NPC.ai[0] >= 392 & NPC.ai[0] <= 479)//slow
					{
						lunge *= 0.95f;
						NPC.TargetClosest(false);
						NPC.velocity.X = lunge;
						NPC.velocity.Y *= 0;
					}
				}
				else //angy
                {
					if (NPC.ai[0] >= 392 & NPC.ai[0] <= 412)//slow but shorter
					{
						lunge *= 0.95f;
						NPC.TargetClosest(false);
						NPC.velocity.X = lunge;
						NPC.velocity.Y *= 0;
					}

					if (NPC.ai[0] == 413)
                    {
						lunge = NPC.direction * -30; //backtrack

						NPC.TargetClosest(false);
						NPC.velocity.X = lunge;
						NPC.velocity.Y *= 0;
					}

					if (NPC.ai[0] > 414)
                    {
						lunge *= 0.95f;
						NPC.TargetClosest(false);
						NPC.velocity.X = lunge;
						NPC.velocity.Y *= 0;
					}
				}

				//DARK ORB
				if (NPC.ai[0] >= 360 & NPC.ai[1] == 2) //Stop movement for orb attack
				{
					NPC.velocity *= 0;
				}

				if (NPC.ai[0] == 370 & NPC.ai[1] == 2) //charge orb and attack
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X - 15, NPC.Center.Y + -90, NPC.velocity.X * 0, NPC.velocity.Y * 0, ModContent.ProjectileType<Projectiles.DarkOrb>(), NPC.damage / 2, 10f, Main.myPlayer, 0, NPC.target);
					}
					SoundEngine.PlaySound(SoundID.Item13, NPC.Center); //magic spray
					//(shoot sound is in dark orb)
				}

				//END OF PHASE 1 CYCLE

				if (NPC.ai[0] >= 425 && NPC.ai[1] == 2 && frenzy == true) //if enraged orb attack
                {
					NPC.ai[0] = 0; //recycle attack cycle timer
					if (NPC.ai[1] >= 2) //recycle special attack
					{
						NPC.ai[1] = 0;
					}
				}

				if (NPC.ai[0] >= 480)
				{
					NPC.ai[0] = 0; //recycle attack cycle timer
					if (NPC.ai[1] >= 2) //recycle special attack
					{
						NPC.ai[1] = 0;
					}
				}

				//NEXT PHASE
				if (NPC.life <= NPC.lifeMax / 2)
                {
					NPC.ai[0] = 0; //end of phase 1
					phase = 2; //transition
                }
			}

			if (phase == 2) //TRANSITION TO BALL
            {
				NPC.TargetClosest(false);
				NPC.rotation = 0.0f;
				NPC.velocity *= 0.95f; //slow
				NPC.ai[2]++; //go up by 1 each tick
				animation = 1; //phase transition
				NPC.defense = 2000;
				NPC.damage = 0;

			    if (NPC.ai[2] < 60) 
                {
					if (NPC.ai[2] % 10 == 0) //remainder
					{
						Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
						Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 3, 1); //Makes a crumb of dust
						d.noGravity = true;
					}
				}
				else if (NPC.ai[2] < 120) 
				{
					if (NPC.ai[2] % 5 == 0) //remainder
					{
						Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
						Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 3, 1); //Makes a crumb of dust
						d.noGravity = true;
					}
					
				}
				else
				{
					Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
					Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 3, 1); //Makes a crumb of dust
					d.noGravity = true;
				}

				//NEXT PHASE
				if (NPC.ai[2] >= 180) 
                {
					//dust-splosion
					for (int i = 0; i < 20; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
					{
						Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
						Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 10, 10); //Makes dust in a messy circle
						d.noGravity = true;
						SoundEngine.PlaySound(SoundID.Item81, NPC.Center); //spawn slime mount
					}
					phase = 3; //phase 2(?)
				}
			}

			if (phase == 3) //DARK MATTER'S TRUE FORM
			{
				
				if (NPC.ai[3] < 30) //rise up gang
				{
					NPC.velocity.Y = -3;
					NPC.ai[3]++;

					if (NPC.ai[3] == 29) //disable frenzy from last phase(not the last phase but phase 1)
					{
						frenzy = false;
					}
				}
				else if (NPC.ai[3] >= 30)//start cycle
				{

					if (NPC.ai[3] < 360) //Movement before special attack (Alot of this code was taken from ExampleMod's capitive element(2)) so I don't fully understand this yet, but it makes my video game reference go smooth so I don't mind it
                    {
						float minX = moveTo.X - 50f;
						float maxX = moveTo.X + 50f;
						float minY = moveTo.Y;
						float maxY = moveTo.Y;

						if (playerDistance.X <= 0) //if player is behind enemy
						{
							move = move + new Vector2(400f, 0); // go in front of player 
						}
						else
						{
							move = move + new Vector2(-400f, 0); // go behind player 
						}

						if (NPC.Center.X >= minX && NPC.Center.X <= maxX && NPC.Center.Y >= minY && NPC.Center.Y <= maxY) //certain range
						{
							NPC.velocity *= 0.98f; //slow
						}
						else
						{
							float magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y);
							float speed = 28; //speed I think
							if (magnitude > speed)
							{
								move *= speed / magnitude;
							}
							float inertia = 10f; //Ok so like I'm pretty sure this is supposed to be how wibbly wobbly you want the npc to be before it reaches its destination
							NPC.velocity = (inertia * NPC.velocity + move) / (inertia + 1);
							magnitude = (float)Math.Sqrt(NPC.velocity.X * NPC.velocity.X + NPC.velocity.Y + NPC.velocity.Y);
							if (magnitude > speed)
							{
								NPC.velocity *= speed / magnitude;
							}
						}
						NPC.TargetClosest(true);
					}

					if (!Main.expertMode) //if not Expert Mode
					{
						NPC.damage = 120; 
					}
                    else if (!Main.masterMode) //if not Master Mode
                    {
                        NPC.damage = (int)(360 * 0.6f);
                    }
                    else
                    {
						NPC.damage = (int)(240 * 0.6f); //put it this way because expert scaling
                    }

					NPC.defense = 35; //back to regular defense

					if (NPC.ai[3] == 30) //checks if cycle has restarted
					{
						if (NPC.life <= NPC.lifeMax * 0.25 & Main.expertMode) //checks if npc has 2/10 of life and in expert mode
						{
							frenzy = true;
						}
						else
						{
							frenzy = false;
						}
					}

					NPC.ai[3]++; //phase 2 cycle

					if (NPC.ai[3] >= 90 && NPC.ai[3] <= 120) //Matter Orb
                    {
						if (NPC.ai[3] == 90)
						{
							//up
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 0, NPC.Center.Y - 10, 0, -10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
							}
							SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

							for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
							{
								Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
								Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
								d.noGravity = true;
							}
						}
						else if (NPC.ai[3] == 100)
						{
							//down
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 0, NPC.Center.Y + 10, 0, 10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
							}
							SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

							for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
							{
								Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
								Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
								d.noGravity = true;
							}
						}
						else if (NPC.ai[3] == 110)
						{
							//diagonal up
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * -10, NPC.Center.Y + NPC.direction * -5, NPC.direction * -10, -10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
							}
							SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

							for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
							{
								Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
								Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
								d.noGravity = true;
							}
						}
						else if (NPC.ai[3] == 120)
						{
							//diagonal down
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * 10, NPC.Center.Y + NPC.direction * 5, NPC.direction * -10, 10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
							}
							SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

							for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
							{
								Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
								Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
								d.noGravity = true;
							}
						}
					}
					if (NPC.ai[3] >= 130 && NPC.ai[3] <= 170 && frenzy == true) //Matter Orb (enraged)
					{
						if (NPC.ai[3] == 130)
						{
							//up
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 0, NPC.Center.Y - 10, 0, -10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
							}
							SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

							for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
							{
								Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
								Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
								d.noGravity = true;
							}
						}
						else if (NPC.ai[3] == 140)
						{
							//down
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 0, NPC.Center.Y + 10, 0, 10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
							}
							SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

							for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
							{
								Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
								Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
								d.noGravity = true;
							}
						}
						else if (NPC.ai[3] == 150)
						{
							//diagonal up
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * -10, NPC.Center.Y + NPC.direction * -5, NPC.direction * -10, -10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
							}
							SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

							for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
							{
								Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
								Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
								d.noGravity = true;
							}
						}
						else if (NPC.ai[3] == 160)
						{
							//diagonal down
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * 10, NPC.Center.Y + NPC.direction * 5, NPC.direction * -10, 10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
							}
							SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

							for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
							{
								Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
								Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
								d.noGravity = true;
							}
						}
						else if (NPC.ai[3] == 170)
						{
							//center
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * 10, NPC.Center.Y + NPC.direction * 5, NPC.direction * -10, 0, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
							}
							SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

							for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
							{
								Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
								Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
								d.noGravity = true;
							}
						}
					}
					if (NPC.ai[3] >= 180 && NPC.ai[3] <= 210) //Matter Orb
					{
						if (NPC.ai[3] == 180)
						{
							//up
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 0, NPC.Center.Y - 10, 0, -10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
							}
							SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

							for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
							{
								Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
								Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
								d.noGravity = true;
							}
						}
						else if (NPC.ai[3] == 190)
						{
							//down
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 0, NPC.Center.Y + 10, 0, 10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
							}
							SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

							for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
							{
								Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
								Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
								d.noGravity = true;
							}
						}
						else if (NPC.ai[3] == 200)
						{
							//diagonal up
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * -10, NPC.Center.Y + NPC.direction * -5, NPC.direction * -10, -10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
							}
							SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

							for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
							{
								Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
								Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
								d.noGravity = true;
							}
						}
						else if (NPC.ai[3] == 210)
						{
							//diagonal down
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * 10, NPC.Center.Y + NPC.direction * 5, NPC.direction * -10, 10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
							}
							SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

							for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
							{
								Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
								Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
								d.noGravity = true;
							}
						}
					}
					if (NPC.ai[3] >= 220 && NPC.ai[3] <= 260 && frenzy == true) //Matter Orb (enraged)
					{
						if (NPC.ai[3] == 220)
						{
							//up
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 0, NPC.Center.Y - 10, 0, -10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
							}
							SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

							for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
							{
								Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
								Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
								d.noGravity = true;
							}
						}
						else if (NPC.ai[3] == 230)
						{
							//down
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 0, NPC.Center.Y + 10, 0, 10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
							}
							SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

							for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
							{
								Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
								Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
								d.noGravity = true;
							}
						}
						else if (NPC.ai[3] == 240)
						{
							//diagonal up
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * -10, NPC.Center.Y + NPC.direction * -5, NPC.direction * -10, -10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
							}
							SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

							for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
							{
								Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
								Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
								d.noGravity = true;
							}
						}
						else if (NPC.ai[3] == 250)
						{
							//diagonal down
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * 10, NPC.Center.Y + NPC.direction * 5, NPC.direction * -10, 10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
							}
							SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

							for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
							{
								Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
								Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
								d.noGravity = true;
							}
						}
						else if (NPC.ai[3] == 260)
						{
							//center
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * 10, NPC.Center.Y + NPC.direction * 5, NPC.direction * -10, 0, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
							}
							SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

							for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
							{
								Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
								Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
								d.noGravity = true;
							}
						}
					}
					if (NPC.ai[3] >= 270 && NPC.ai[3] <= 300) //Matter Orb
					{
						if (NPC.ai[3] == 270)
						{
							//up
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 0, NPC.Center.Y - 10, 0, -10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
							}
							SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

							for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
							{
								Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
								Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
								d.noGravity = true;
							}
						}
						else if (NPC.ai[3] == 280)
						{
							//down
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 0, NPC.Center.Y + 10, 0, 10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
							}
							SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

							for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
							{
								Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
								Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
								d.noGravity = true;
							}
						}
						else if (NPC.ai[3] == 290)
						{
							//diagonal up
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * -10, NPC.Center.Y + NPC.direction * -5, NPC.direction * -10, -10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
							}
							SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

							for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
							{
								Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
								Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
								d.noGravity = true;
							}
						}
						else if (NPC.ai[3] == 300)
						{
							//diagonal down
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * 10, NPC.Center.Y + NPC.direction * 5, NPC.direction * -10, 10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
							}
							SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

							for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
							{
								Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
								Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
								d.noGravity = true;
							}
						}
					}

					//SPECIAL ATTACKS
					if (NPC.ai[3] == 360 & phase2special == 3) //reset because there is no special 3
                    {
						phase2special = 1;
					}
					
					if (NPC.ai[3] > 360 & NPC.ai[3] < 420 & phase2special == 1) //DASH START //Backup
					{
						NPC.TargetClosest(true); //face player

						float speed = 28f;
						float inertia = 10f;

						Vector2 direction = player.Center - NPC.Center; //start - end

						if (playerDistance.X <= 0) //if player is behind enemy
						{
							direction.X += 400f + (NPC.ai[3] - 360) * 3; // go in front of player (and backup!)
						}
						else
						{
							direction.X -= 400f + (NPC.ai[3] - 360) * 3; // go behind player (and backup!)
						}

						direction.Y += player.velocity.Y * 30; //read player

						direction.Normalize();
						direction *= speed;
						NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;  //fly to area of aim
					}
					
					if (NPC.ai[3] >= 420 & phase2special == 1) //dash for 60 ticks
					{
						NPC.TargetClosest(false);
						NPC.velocity.X += NPC.direction * 1.04f;
						NPC.velocity.Y = 0;

						if (NPC.ai[3] % 2 == 0) //every multiple of 2 place a dust
						{
							Dust d = Dust.NewDustPerfect(NPC.Center + new Vector2(Main.rand.Next(-70, 70), Main.rand.Next(-70, 70)), ModContent.DustType<Dusts.DarkResidue>(), NPC.velocity * 0, 2); //Makes dust i
							d.noGravity = true;
						}

						if (NPC.ai[3] == 420) //upon dash start
                        {
							SoundEngine.PlaySound(SoundID.Roar, player.position); //do da roar
						}
					}
					
					if (NPC.ai[3] > 360 & NPC.ai[3] <= 540 & phase2special == 2) //DARK LASERS
                    {
						NPC.velocity *= 0;
					
						if (NPC.ai[3] % 60 == 0 & phase2special == 2) //shoots only if ai 3 is a multiple of 60
                        {
							NPC.TargetClosest(true);
							Vector2 lasershoot = player.Center - NPC.Center;
							lasershoot.Normalize(); //reduces it to a value of 1
							lasershoot *= 32f; //inital projectile speed

							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 5, 0), lasershoot, Mod.Find<ModProjectile>("DarkLaser").Type, 120 / 2, 10f, Main.myPlayer);
							}
							SoundEngine.PlaySound(SoundID.Item12);
						}
                    }

					if (NPC.ai[3] == 660 && phase2special == 2 & frenzy == true) //for spin beam
					{
						NPC.rotation = MathHelper.ToRadians(-90f); //face up
						NPC.direction = 1; //face correctly(idk which way but it's right*get it?*)
					}

					if (NPC.ai[3] > 660 & NPC.ai[3] <= 680 && phase2special == 2 & frenzy == true) // also for spin beam
                    {
						NPC.rotation += MathHelper.ToRadians(18);
                    }

					if (NPC.ai[3] >= 740 & NPC.ai[3] <= 980 & phase2special == 2 && frenzy == true) //SPIN BEAM (expert frenzy)
					{
						NPC.velocity *= 0; //stop

						if (NPC.ai[3] == 680) //face up
                        {
							NPC.rotation = MathHelper.ToRadians(-90f);
							NPC.direction = 1; //face correctly(idk which way but it's right*get it?*)
                        }
						
						if (NPC.ai[3] >= 740 & NPC.ai[3] < 980 & phase2special == 2) //4 seconds
						{
							NPC.rotation += MathHelper.ToRadians(3);
							NPC.TargetClosest(false); //don't face player

							if (NPC.ai[3] % 3 == 0)//only shoots if ai 3 is a multiple of 3
							{
								float spinshoot = 18f;

								if (Main.netMode != NetmodeID.MultiplayerClient)
								{
									Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 10 * (float)Math.Cos(NPC.rotation), NPC.Center.Y + 10 * (float)Math.Cos(NPC.rotation), spinshoot * (float)Math.Cos(NPC.rotation), spinshoot * (float)Math.Sin(NPC.rotation), Mod.Find<ModProjectile>("AngledDarkBeam").Type, 60 / 2, 10f, Main.myPlayer);
								}
								SoundEngine.PlaySound(SoundID.Item12);
							}
						}
					}


					if (NPC.ai[3] >= 480 & phase2special == 1) //dash end
                    {
						if (!frenzy) //not frenzying
						{
							NPC.ai[3] = 30; //don't make it zero or it will rise again
							phase2special += 1; //next attack
						}
						else //frenzying
                        {
							if (frenzydashamount > 0)
							{
								frenzydashamount -= 1;
								NPC.velocity *= 0; //cut velocity
								NPC.ai[3] = 361; //restart dash
							}
							else //end dash for real
                            {
								NPC.ai[3] = 30; //don't make it zero or it will rise again
								phase2special += 1; //next attack
								frenzydashamount = 2; //reset frenzy dash amount for next dash 
							}
                        }
					}

					if (NPC.ai[3] >= 660 & phase2special == 2 & frenzy == false) //laser end
					{
						NPC.ai[3] = 30; //don't make it zero or it will rise again
						phase2special += 1; //next attack
					}
					else if (NPC.ai[3] >= 1100 & phase2special == 2 & frenzy == true) //spin beam end
                    {
						NPC.ai[3] = 30; //don't make it zero or it will rise again
						phase2special += 1; //next attack
						NPC.rotation = 0;
					}
				}
				NPC.spriteDirection = NPC.direction;
				animation = 2; //true form
			}
		}	

		public override void FindFrame(int frameHeight) // animation
        {
			if (animation == 0) //phase 1
			{
				if (NPC.ai[0] >= 300 & NPC.ai[1] == 2) //dark orb attack
				{
					NPC.frame.Y = 0; //robe swish
					NPC.frameCounter = 0;
				}
				else //robe swing
				{
					NPC.frameCounter += 1.0;
					if (NPC.frameCounter < 15.0)
					{
						NPC.frame.Y = 0; //robe swish
					}
					else if (NPC.frameCounter < 30.0)
					{
						NPC.frame.Y = frameHeight; //robe swoosh
					}
					else
					{
						NPC.frameCounter = 0.0; //reset
					}
				}
			}
			if (animation == 1) //phase transition
            {
				NPC.frameCounter += 1.0;
				if (NPC.frameCounter < 15.0)
				{
					NPC.frame.Y = frameHeight * 2; //flap down
				}
				else if (NPC.frameCounter < 30.0)
				{
					NPC.frame.Y = frameHeight * 3; //flap up
				}
				else
				{
					NPC.frameCounter = 0.0; //reset
				}
			}
			if (animation == 2) //phase 2
			{
				NPC.frameCounter += 1.0;
				if (NPC.frameCounter < 8.0)
				{
					NPC.frame.Y = frameHeight * 4; //idle
				}
				else if (NPC.frameCounter < 16.0)
				{
					NPC.frame.Y = frameHeight * 5; //lil' stretch
				}
				else
				{
					NPC.frameCounter = 0.0; //reset
				}
			}
		}
		public override Color? GetAlpha(Color lightColor)
		{
			if (NPC.ai[3] > 360 & NPC.ai[3] < 540 & phase2special == 2 && phase == 3) //laser attack
			{
				if ((NPC.ai[3] % 60 <= 5) == false) //doesn't glow purple on the shot(or momentaraily after)
				{
					return Color.Purple; //make it ourple
				}
				else
				{
					return Color.White; //make it unaffected by light
				}
			}
			else
            {
				return Color.White; //make it unaffected by light
			}
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			scale = 1.5f;
			return true;
		}

		public override void BossLoot(ref string name, ref int potionType)
		{
			name = "Dark Matter"; //_ has been defeated!
			potionType = ItemID.GreaterHealingPotion; //potion it drops
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (phase == 1)
			{
				for (int i = 0; i < 1; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
					Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 3, 1); //Makes dust in a messy circle
					d.noGravity = true;
				}
			}
			if (phase == 3)
			{
				for (int i = 0; i < 5; i++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
					Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 2, 2); //Makes dust in a messy circle
					d.noGravity = false;
				}
			}
			if (NPC.life <= 0)
			{
				for (int i = 0; i < 40; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
					Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 4, 10); //Makes dust in a messy circle
					d.noGravity = true;
				}
			}
		}

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedDarkMatterBoss, -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<DarkMatterBag>())); //only drops in expert

            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert()); //checks if not expert
            LeadingConditionRule masterMode = new LeadingConditionRule(new Conditions.IsMasterMode()); //checks if master mode

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.DarkMaterial>(), 1, 30, 30));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<DarkMatterMask>(), 7));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DarkMatterTrophy>(), 10)); //drop trophy

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeables.BossRelics.DarkMatterRelic>()));

            masterMode.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.DarkMatter.DarkMatterPetItem>(), 4));

            // add the rules
            npcLoot.Add(notExpertRule);
            npcLoot.Add(masterMode);
        }

		// This npc uses additional textures for drawing
		public static Asset<Texture2D> DarkBladeLeft;
        public static Asset<Texture2D> DarkBladeRight;
        public static Asset<Texture2D> DarkBladeUp;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			DarkBladeLeft = ModContent.Request<Texture2D>("KirboMod/NPCs/DarkBlade");
            DarkBladeRight = ModContent.Request<Texture2D>("KirboMod/NPCs/DarkBlade2");
            DarkBladeUp = ModContent.Request<Texture2D>("KirboMod/NPCs/DarkBlade3");

            if (phase == 1) //sword
			{                                                                               
				if (NPC.ai[0] >= 300 & NPC.ai[1] == 2) //sword up bum bum bum  //
				{                                                             //
					Texture2D blade = DarkBladeUp.Value; //sword this way    o
					spriteBatch.Draw(blade, NPC.Center - Main.screenPosition + new Vector2(0, -30), null, new Color(255, 255, 255), 0, new Vector2(16, 16), 1f, SpriteEffects.None, 0f);
				}
				else //sword neutral
				{
					if (NPC.direction == 1)
					{
						Texture2D blade = DarkBladeRight.Value; //sword this way o->===>
						spriteBatch.Draw(blade, NPC.Center - Main.screenPosition + new Vector2(0, 30), null, new Color(255, 255, 255), 0, new Vector2(16, 16), 1f, SpriteEffects.None, 0f);
					}
					else
					{
						Texture2D blade = DarkBladeLeft.Value; //sword this way <===<-o 
						spriteBatch.Draw(blade, NPC.Center - Main.screenPosition + new Vector2(-60, 30), null, new Color(255, 255, 255), 0, new Vector2(16, 16), 1f, SpriteEffects.None, 0f);
					}
				}
			}
		}

        public override void BossHeadSlot(ref int index)
        {
            if (phase == 1 || phase == 2) //if in first phase or transitioning
			{
				index = ModContent.GetModBossHeadSlot("KirboMod/NPCs/DarkMatter_Head_Boss");
			}
			else //second phase
            {
				index = ModContent.GetModBossHeadSlot("KirboMod/NPCs/DarkMatter_Head_Boss2");
			}
        }
    }
}
