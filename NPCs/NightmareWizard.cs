using KirboMod.Items.Zero;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using KirboMod.Items;
using KirboMod.Items.Nightmare;
using KirboMod.Systems;
using System.IO;

namespace KirboMod.NPCs
{
	[AutoloadBossHead]
	public class NightmareWizard : ModNPC
	{
		private int animation = 0; //which frames to cycle through
		private int attack = 0; //timer that counts through attack cycle
		private int despawntimer = 0; //for despawning
		private int attacktype = 0; //sets attack type
		private int lastattacktype = 1; //sets last attack type

		private int attackpatterntype = 1; //basic attack pattern
		private bool wizardspecialattack = false; //decides if it's going to do an expert mode exclusive attack
		private int wizardspecialattacktype = 1; //decides which expert mode exclusive attack

		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Nightmare");
			Main.npcFrameCount[NPC.type] = 19;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                PortraitScale = 1f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 90f,
                Position = new Vector2(0, 80),
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

        }

		public override void SetDefaults() {
			NPC.width = 152;
			NPC.height = 152;
			DrawOffsetY = -20;
			NPC.damage = 70; //just temporary for expert scaling
			NPC.noTileCollide = true;
			NPC.defense = 6;
			NPC.lifeMax = 10000;
			NPC.HitSound = SoundID.NPCHit2; //bone
			NPC.DeathSound = SoundID.NPCDeath2; //undead
			NPC.value = Item.buyPrice(0, 5, 0, 0); // money it drops
			NPC.knockBackResist = 0f;
			NPC.aiStyle = -1;
			NPC.boss = true;
			NPC.noGravity = true;
			NPC.lavaImmune = true;
			Music = MusicID.Boss1;
			NPC.friendly = false;
			NPC.dontTakeDamage = true;
			NPC.npcSlots = 6;
            NPC.buffImmune[BuffID.Confused] = true;
        }

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */ 
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.8 * balance);
			NPC.damage = (int)(NPC.damage * 0.6f);
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("An evil wizard bent on spreading nightmares to all that can dream. Its magic can twist the mind and body of those who are cursed with it.")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(animation);
            writer.Write(attack);
            writer.Write(despawntimer);
            writer.Write(attacktype);
            writer.Write(lastattacktype);

            writer.Write(attackpatterntype);
            writer.Write(wizardspecialattack);
            writer.Write(wizardspecialattacktype);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            animation = reader.ReadInt32();
            attack = reader.ReadInt32();
            despawntimer = reader.ReadInt32();
            attacktype = reader.ReadInt32();
            lastattacktype = reader.ReadInt32();

            attackpatterntype = reader.ReadInt32();
			wizardspecialattack = reader.ReadBoolean();
            wizardspecialattacktype = reader.ReadInt32();
        }

        public override void AI() //constantly cycles each time
		{
			Player player = Main.player[NPC.target];
			NPC.netAlways = true;

			//Despawn
			if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active || Main.dayTime) //Despawn
			{
				NPC.TargetClosest(false);
				animation = 2; //teleport
				despawntimer++;
				NPC.velocity *= 0;
				if (despawntimer > 8)//teleport away
				{
					NPC.Center += new Vector2( 0, -2000);
                }

				if (NPC.timeLeft > 8)
				{
					NPC.timeLeft = 8;
					return;
				}
			}
			else //regular attack
			{
				AttackCycle();
				despawntimer = 0;
			}
		}

		private void AttackCycle()
	    {
			Player player = Main.player[NPC.target];

			if (attack == 0)
            {
				if (Main.expertMode)
				{
					if (NPC.life <= NPC.lifeMax * 0.75 && NPC.life > NPC.lifeMax * 0.45) //between 75% and 45% health(because of attackpatterntype 3)
					{
						attackpatterntype = 2;
					}
					else if (NPC.life <= NPC.lifeMax * 0.45) //below 45% health
					{
						attackpatterntype = 3;
					}
				}
				else //not expertmode
                {
					if (NPC.life <= NPC.lifeMax * 0.50) //below 50% health
					{
						attackpatterntype = 2;
					}
				}

				NPC.damage = 0; //reset
            }

			attack++;

			if (attackpatterntype == 1) //regular cycle
			{
				if (attack == 1)
				{
					animation = 0;
				}
				if (attack == 120) //teleport
				{
					animation = 2;
					SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
				}
				if (attack == 129)
				{
					NPC.Center = new Vector2(player.Center.X, player.Center.Y - 150f);
				}
				if (attack == 138)
				{
					animation = 0;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int randattack = Main.rand.Next(1, 3);
						if (randattack == 1)
						{
							if (lastattacktype == 1)
							{
								int subrandattack = Main.rand.Next(1, 2);
								if (subrandattack == 1)
								{
									attacktype = 2;
								}
								else
								{
									attacktype = 0;
								}
							}
							else
							{
								attacktype = 1;
							}
						}
						else if (randattack == 2)
						{
							if (lastattacktype == 0)
							{
								int subrandattack = Main.rand.Next(1, 2);
								if (subrandattack == 1)
								{
									attacktype = 2;
								}
								else
								{
									attacktype = 1;
								}
							}
							else
							{
								attacktype = 0;
							}
						}
						else if (randattack == 3)
						{
							if (lastattacktype == 2)
							{
								int subrandattack = Main.rand.Next(1, 2);
								if (subrandattack == 1)
								{
									attacktype = 1;
								}
								else
								{
									attacktype = 0;
								}
							}
							else
							{
								attacktype = 2;
							}
						}
						NPC.netUpdate = true;
					}
				}

				if (attack < 198)
				{
					NPC.TargetClosest(true);
				}

				int stardamage = 20; //this damage is about doubled
				if (Main.expertMode)
				{
					stardamage = (int)(stardamage / 1.15);
				}

				if (attacktype == 0) //slide to open robe
				{

					if (attack == 198)
					{
						lastattacktype = 0;
						animation = 1; //slide
						NPC.velocity.X = -10 * NPC.direction; //go reverse of direction facing
						NPC.spriteDirection = NPC.direction;
					}
					if (attack == 258)
					{
						NPC.velocity.X *= 0.2f;
						animation = 3;
						NPC.dontTakeDamage = false;
					}
					if (attack == 258 || attack == 298 || attack == 338 || attack == 278 || attack == 318 || attack == 358) //stars (not in numerical order)
					{
						Vector2 projshoot = player.Center - NPC.Center;
						projshoot.Normalize(); //reduce into 1
						projshoot *= 16;

						//Where do these shots aim? (facing right)


						Vector2 shootoffset1; //= new Vector2(projshoot.X, projshoot.Y).RotatedBy(MathHelper.ToRadians(-30)); //-30 degree offset
						Vector2 shootoffset2; //= new Vector2(projshoot.X, projshoot.Y).RotatedBy(MathHelper.ToRadians(30)); //30 degree offset
						
						shootoffset1 = new Vector2(projshoot.X, projshoot.Y).RotatedBy(MathHelper.ToRadians(-30)); //-30 degree offset
						shootoffset2 = new Vector2(projshoot.X, projshoot.Y).RotatedBy(MathHelper.ToRadians(30)); //30 degree offset

						if (Main.netMode != NetmodeID.MultiplayerClient) //wrap this aronud any NPC spawned projectile. Not the sounds though!
						{
							//Straight at player
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, projshoot.X, projshoot.Y, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);

							//Below player
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, shootoffset1.X, shootoffset1.Y, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);

							//Above player
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, shootoffset2.X, shootoffset2.Y, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
						}
						SoundEngine.PlaySound(SoundID.MaxMana, NPC.Center);
					}

					if (attack == 450)
					{
						animation = 2; //teleport
						NPC.velocity *= 0;
						SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
					}
					if (attack == 459 && Main.netMode != NetmodeID.MultiplayerClient)
					{
						NPC.Center = new Vector2(player.Center.X + Main.rand.Next(-300, 300), player.Center.Y - Main.rand.Next(100, 200));
						NPC.netUpdate = true;
					}
					if (attack >= 468)
					{
						animation = 0; //idle
						NPC.dontTakeDamage = true;
						attack = 0; //reset
					}
				}

				if (attacktype == 1) //dive at player
				{
					if (attack == 198)
					{
						lastattacktype = 1;
						animation = 4;
						Vector2 distance = player.Center - NPC.Center;
						float Yspeed = distance.Y / 50;
						NPC.velocity.Y = Yspeed;
					}
					if (attack > 198 & attack < 218) //Back up
					{
						//npc.TargetClosest(false);
						if (NPC.direction == 1)
						{
							NPC.direction = 1;
						}
						else
						{
							NPC.direction = -1;
						}
						NPC.TargetClosest(false);

						NPC.spriteDirection = NPC.direction; //face direction it's pointing at
						NPC.velocity.X += NPC.direction * -1.2f;
					}

					if (attack == 239) //dash sound
					{
						SoundEngine.PlaySound(SoundID.Roar, NPC.Center); //roar
					}
					if (attack >= 219 & attack < 319) //Charge
					{
						//npc.TargetClosest(false);
						if (NPC.direction == 1)
						{
							NPC.direction = 1;
						}
						else
						{
							NPC.direction = -1;
						}

						NPC.spriteDirection = NPC.direction; //face direction it's pointing at
						NPC.velocity.X += NPC.direction * 1.3f;

						if (Main.expertMode)
						{
							NPC.damage = 120;
						}
						else
						{
							NPC.damage = 60;
						}

						NPC.TargetClosest(false);
						NPC.velocity.Y *= 0.99f;
					}
					if (attack == 319)
					{
						animation = 2; //teleport
						NPC.velocity *= 0;
						NPC.damage = 0;
						SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
					}
					if (attack == 328 && Main.netMode != NetmodeID.MultiplayerClient)
					{
						NPC.Center = new Vector2(player.Center.X + Main.rand.Next(-300, 300), player.Center.Y - Main.rand.Next(100, 200));
						NPC.netUpdate = true;
					}
					if (attack >= 337)
					{
						animation = 0; //idle
						NPC.dontTakeDamage = true;
						attack = 0; //reset
					}
				}

				if (attacktype == 2) //open robe and shoot stars
				{
					if (attack == 198)
					{
						lastattacktype = 2;
						animation = 5; //open robe
						NPC.dontTakeDamage = false;
					}

					if (attack == 258 || attack == 378) //stars 
					{
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							//down right
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 7, 7, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//right
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 10, 0, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//up right
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 7, -7, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//up
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 0, -10, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//up left
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, -7, -7, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//left
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, -10, 0, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//down left
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, -7, 7, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//down
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 0, 10, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
						}
							SoundEngine.PlaySound(SoundID.Item4, NPC.Center); //life crystal

					}
					if (attack == 318)
                    {
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							//down right
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 3.5f, 8.5f, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//down left
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, -3.5f, 8.5f, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//left up
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, -8.5f, -3.5f, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//left down
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, -8.5f, 3.5f, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//up left
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 3.5f, -8.5f, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//up right
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, -3.5f, -8.5f, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//right up
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 8.5f, -3.5f, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//right down
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 8.5f, 3.5f, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
						}
						SoundEngine.PlaySound(SoundID.Item4, NPC.Center); //life crystal
					}

					if (attack == 420)
					{
						animation = 2; //teleport
						NPC.velocity *= 0;
						SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
					}
					if (attack == 429 && Main.netMode != NetmodeID.MultiplayerClient)
					{
						NPC.Center = new Vector2(player.Center.X + Main.rand.Next(-300, 300), player.Center.Y - Main.rand.Next(100, 200));
						NPC.netUpdate = true;
					}
					if (attack >= 438)
					{
						animation = 0; //idle
						NPC.dontTakeDamage = true;
						attack = 0; //reset
					}
				}
			}


			else if (attackpatterntype == 2) //harder cycle
            {
				if (attack == 1)
				{
					animation = 0;
				}
				if (attack == 60) //teleport (halve of normal mode)
				{
					animation = 2;
					SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
				}
				if (attack == 69) //funny number haha
				{
					NPC.Center = new Vector2(player.Center.X, player.Center.Y - 150f);
				}
				if (attack == 78)
				{
					animation = 0;
				}

				if (attack == 137 && Main.netMode != NetmodeID.MultiplayerClient) //right before attacking
                {
                    int randattack = Main.rand.Next(1, 3);
					if (randattack == 1)
					{
						if (lastattacktype == 1)
						{
							int subrandattack = Main.rand.Next(1, 2);
							if (subrandattack == 1)
							{
								attacktype = 2;
							}
							else
							{
								attacktype = 0;
							}
						}
						else
						{
							attacktype = 1;
						}
					}
					else if (randattack == 2)
					{
						if (lastattacktype == 0)
						{
							int subrandattack = Main.rand.Next(1, 2);
							if (subrandattack == 1)
							{
								attacktype = 2;
							}
							else
							{
								attacktype = 1;
							}
						}
						else
						{
							attacktype = 0;
						}
					}
					else if (randattack == 3)
					{
						if (lastattacktype == 2)
						{
							int subrandattack = Main.rand.Next(1, 2);
							if (subrandattack == 1)
							{
								attacktype = 1;
							}
							else
							{
								attacktype = 0;
							}
						}
						else
						{
							attacktype = 2;
						}
					}
					NPC.netUpdate = true;
				}

				if (attack < 138)
				{
					NPC.TargetClosest(true);
				}

				int stardamage = 20; //this damage is about doubled

				if (attacktype == 0) //slide to open robe
				{

					if (attack == 138)
					{
						lastattacktype = 0;
						animation = 1; //slide
						NPC.velocity.X = -15 * NPC.direction; //go reverse of direction facing (1.5x faster than normal)
						NPC.spriteDirection = NPC.direction;
					}
					if (attack == 178)
					{
						NPC.velocity.X *= 0.2f;
						animation = 3;
						NPC.immortal = false;
						NPC.dontTakeDamage = false;
					}
					if (attack == 178 || attack == 198 || attack == 218 || attack == 238 || attack == 258 || attack == 278) //stars 
					{
						Vector2 projshoot = player.Center - NPC.Center;
						projshoot.Normalize(); //reduce into 1
						projshoot *= 16;

						//Where do these shots aim? (facing right)


						Vector2 shootoffset1; //= new Vector2(projshoot.X, projshoot.Y).RotatedBy(MathHelper.ToRadians(-30)); //-30 degree offset
						Vector2 shootoffset2; //= new Vector2(projshoot.X, projshoot.Y).RotatedBy(MathHelper.ToRadians(30)); //30 degree offset
						
						shootoffset1 = new Vector2(projshoot.X, projshoot.Y).RotatedBy(MathHelper.ToRadians(-30)); //-30 degree offset
						shootoffset2 = new Vector2(projshoot.X, projshoot.Y).RotatedBy(MathHelper.ToRadians(30)); //30 degree offset

						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							//Straight at player
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, projshoot.X, projshoot.Y, Mod.Find<ModProjectile>("BadStar").Type, stardamage / 2, 0f, Main.myPlayer, 0, 0);
							//Below player
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, shootoffset1.X, shootoffset1.Y, Mod.Find<ModProjectile>("BadStar").Type, stardamage / 2, 0f, Main.myPlayer, 0, 0);
							//Above player
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, shootoffset2.X, shootoffset2.Y, Mod.Find<ModProjectile>("BadStar").Type, stardamage / 2, 0f, Main.myPlayer, 0, 0);
						}
						SoundEngine.PlaySound(SoundID.MaxMana, NPC.Center);
					}

					if (attack == 350)
					{
						animation = 2; //teleport
						NPC.velocity *= 0;
						SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
					}
					if (attack == 359 && Main.netMode != NetmodeID.MultiplayerClient) //sync cause randomness
					{
                        NPC.Center = new Vector2(player.Center.X + Main.rand.Next(-300, 300), player.Center.Y - Main.rand.Next(100, 200));
						NPC.netUpdate = true;
					}
					if (attack >= 368)
					{
						animation = 0; //idle
						NPC.dontTakeDamage = true;
						attack = 0; //reset
					}
				}

				if (attacktype == 1) //dive at player
				{
					if (attack == 138)
					{
						lastattacktype = 1;
						animation = 4;
						Vector2 distance = player.Center - NPC.Center;
						float Yspeed = distance.Y / 50;
						NPC.velocity.Y = Yspeed;
					}
					if (attack > 138 & attack < 148) //Back up
					{
						//npc.TargetClosest(false);
						if (NPC.direction == 1)
						{
							NPC.direction = 1;
						}
						else
						{
							NPC.direction = -1;
						}
						NPC.TargetClosest(false);

						NPC.spriteDirection = NPC.direction; //face direction it's pointing at									
						NPC.velocity.X += NPC.direction * -1.4f;
					}

					if (attack == 169) //dash sound
					{
						SoundEngine.PlaySound(SoundID.Roar, NPC.Center); //roar
					}
					if (attack >= 149 & attack < 259) //Charge (longer duration)
					{
						//npc.TargetClosest(false);
						if (NPC.direction == 1)
						{
							NPC.direction = 1;
						}
						else
						{
							NPC.direction = -1;
						}

						NPC.spriteDirection = NPC.direction; //face direction it's pointing at
						NPC.velocity.X += NPC.direction * 1.4f;

						if (Main.expertMode)
						{
							NPC.damage = 120;
						}
						else
						{
							NPC.damage = 60;
						}

						NPC.TargetClosest(false);
						NPC.velocity.Y *= 0.99f;
					}
					if (attack == 259)
					{
						animation = 2; //teleport
						NPC.velocity *= 0;
						NPC.damage = 0;
						SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
					}
					if (attack == 268 && Main.netMode != NetmodeID.MultiplayerClient) //randomness means sync
					{
						NPC.Center = new Vector2(player.Center.X + Main.rand.Next(-300, 300), player.Center.Y - Main.rand.Next(100, 200));
						NPC.netUpdate = true;
					}
					if (attack >= 277)
					{
						animation = 0; //idle
						NPC.dontTakeDamage = true;
						attack = 0; //reset
					}
				}

				if (attacktype == 2) //open robe and shoot stars
				{
					if (attack == 138)
					{
						lastattacktype = 2;
						animation = 5; //open robe
						NPC.dontTakeDamage = false;
					}

					if (attack == 168 || attack == 228) //stars (one more cycle)(primary batch)
					{
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							//down right
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 7, 7, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//right
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 10, 0, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//up right
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 7, -7, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//up
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 0, -10, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//up left
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, -7, -7, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//left
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, -10, 0, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//down left
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, -7, 7, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//down
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 0, 10, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
						}
						SoundEngine.PlaySound(SoundID.Item4, NPC.Center); //life crystal
					}

					if (attack == 198 || attack == 258) //stars (one more cycle)(secondary batch)
					{
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							//down right
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 3.5f, 8.5f, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//down left
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, -3.5f, 8.5f, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//left up
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, -8.5f, -3.5f, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//left down
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, -8.5f, 3.5f, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//up left
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 3.5f, -8.5f, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//up right
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, -3.5f, -8.5f, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//right up
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 8.5f, -3.5f, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//right down
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 8.5f, 3.5f, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
						}
						SoundEngine.PlaySound(SoundID.Item4, NPC.Center); //life crystal
					}

					if (attack == 350)
					{
						animation = 2; //teleport
						NPC.velocity *= 0;
						SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
					}
					if (attack == 359 && Main.netMode != NetmodeID.MultiplayerClient)
					{
						NPC.Center = new Vector2(player.Center.X + Main.rand.Next(-300, 300), player.Center.Y - Main.rand.Next(100, 200));
						NPC.netUpdate = true;
					}
					if (attack >= 368)
					{
						animation = 0; //idle
						NPC.dontTakeDamage = true;
						attack = 0; //reset
					}
				}
			}

			else if (attackpatterntype == 3) //harder cycle with extra attacks
			{
				if (attack == 1) //start
				{
					animation = 0;
				}
				if (attack == 60) //teleport (halve of normal mode)
				{
					animation = 2;
					SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
				}
				if (attack == 69 && Main.netMode != NetmodeID.MultiplayerClient) //spawn on top of player(funny number haha)
				{
					NPC.Center = new Vector2(player.Center.X, player.Center.Y - 150f);
					NPC.netUpdate = true;
				}
				if (attack == 78) //teleportation animation end
				{
					animation = 0;

					if (wizardspecialattack == true & wizardspecialattacktype == 2) //time for the drop
                    {
						//jump to drop attack
						attacktype = 4;
						attack = 138;

						wizardspecialattacktype = 1; //tornado attack next time
						wizardspecialattack = false; //do regular attack instead
                    }
				}

				if (attack == 137 && Main.netMode != NetmodeID.MultiplayerClient) //right before attacking
				{
					if (wizardspecialattack == false) //not expert mode attack
					{
						int randattack = Main.rand.Next(1, 3);
						if (randattack == 1)
						{
							if (lastattacktype == 1)
							{
								int subrandattack = Main.rand.Next(1, 2);
								if (subrandattack == 1)
								{
									attacktype = 2;
								}
								else
								{
									attacktype = 0;
								}
							}
							else
							{
								attacktype = 1;
							}
						}
						else if (randattack == 2)
						{
							if (lastattacktype == 0)
							{
								int subrandattack = Main.rand.Next(1, 2);
								if (subrandattack == 1)
								{
									attacktype = 2;
								}
								else
								{
									attacktype = 1;
								}
							}
							else
							{
								attacktype = 0;
							}
						}
						else if (randattack == 3)
						{
							if (lastattacktype == 2)
							{
								int subrandattack = Main.rand.Next(1, 2);
								if (subrandattack == 1)
								{
									attacktype = 1;
								}
								else
								{
									attacktype = 0;
								}
							}
							else
							{
								attacktype = 2;
							}
						}

						wizardspecialattack = true;
					}
					else
                    {
						if (wizardspecialattacktype == 1) //tornado attack
                        {
							attacktype = 3;
							wizardspecialattacktype = 2; //drop attack
							wizardspecialattack = false; //switch off


						}
						else //drop attack
                        {
							attacktype = 4;
							wizardspecialattacktype = 1; //tornado attack
							wizardspecialattack = false; //switch off
						}
                    }
					NPC.netUpdate = true;
				}

				if (attack < 138)
				{
					NPC.TargetClosest(true);
				}

				int stardamage = 20; //this damage is about doubled
				if (Main.expertMode)
				{
					stardamage = (int)(stardamage / 1.15);
				}

				if (attacktype == 0) //slide to open robe
				{

					if (attack == 138)
					{
						lastattacktype = 0;
						animation = 1; //slide
						NPC.velocity.X = -15 * NPC.direction; //go reverse of direction facing (1.5x faster than normal)
						NPC.spriteDirection = NPC.direction;
					}
					if (attack == 178)
					{
						NPC.velocity.X *= 0.2f;
						animation = 3;
						NPC.immortal = false;
						NPC.dontTakeDamage = false;
					}
					if (attack == 178 || attack == 198 || attack == 218 || attack == 238 || attack == 258 || attack == 278) //stars 
					{
						Vector2 projshoot = player.Center - NPC.Center;
						projshoot.Normalize(); //reduce into 1
						projshoot *= 16;

						//Where do these shots aim? (facing right)


						Vector2 shootoffset1; //= new Vector2(projshoot.X, projshoot.Y).RotatedBy(MathHelper.ToRadians(-30)); //-30 degree offset
						Vector2 shootoffset2; //= new Vector2(projshoot.X, projshoot.Y).RotatedBy(MathHelper.ToRadians(30)); //30 degree offset

						shootoffset1 = new Vector2(projshoot.X, projshoot.Y).RotatedBy(MathHelper.ToRadians(-30)); //-30 degree offset
						shootoffset2 = new Vector2(projshoot.X, projshoot.Y).RotatedBy(MathHelper.ToRadians(30)); //30 degree offset
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							//Straight at player
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, projshoot.X, projshoot.Y, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//Below player
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, shootoffset1.X, shootoffset1.Y, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//Above player
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, shootoffset2.X, shootoffset2.Y, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
						}
						SoundEngine.PlaySound(SoundID.MaxMana, NPC.Center);
					}

					if (attack == 350)
					{
						animation = 2; //teleport
						NPC.velocity *= 0;
						SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
					}
					if (attack == 359 && Main.netMode != NetmodeID.MultiplayerClient)
					{
						NPC.Center = new Vector2(player.Center.X + Main.rand.Next(-300, 300), player.Center.Y - Main.rand.Next(100, 200));
						NPC.netUpdate = true;
					}
					if (attack >= 368)
					{
						animation = 0; //idle
						NPC.dontTakeDamage = true;
						attack = 0; //reset
					}
				}

				if (attacktype == 1) //dive at player
				{
					if (attack == 138)
					{
						lastattacktype = 1;
						animation = 4;
						Vector2 distance = player.Center - NPC.Center;
						float Yspeed = distance.Y / 50;
						NPC.velocity.Y = Yspeed;
					}
					if (attack > 138 & attack < 148) //Back up
					{
						//npc.TargetClosest(false);
						if (NPC.direction == 1)
						{
							NPC.direction = 1;
						}
						else
						{
							NPC.direction = -1;
						}
						NPC.TargetClosest(false);

						NPC.spriteDirection = NPC.direction; //face direction it's pointing at									
						NPC.velocity.X += NPC.direction * -1.4f;
					}

					if (attack == 169) //dash sound
					{
						SoundEngine.PlaySound(SoundID.Roar, NPC.Center); //roar
					}
					if (attack >= 149 & attack < 259) //Charge (longer duration)
					{
						//npc.TargetClosest(false);
						if (NPC.direction == 1)
						{
							NPC.direction = 1;
						}
						else
						{
							NPC.direction = -1;
						}

						NPC.spriteDirection = NPC.direction; //face direction it's pointing at
						NPC.velocity.X += NPC.direction * 1.4f;

						if (Main.expertMode)
						{
							NPC.damage = 120;
						}
						else
						{
							NPC.damage = 60;
						}

						NPC.TargetClosest(false);
						NPC.velocity.Y *= 0.99f;
					}
					if (attack == 259)
					{
						animation = 2; //teleport
						NPC.velocity *= 0;
						NPC.damage = 0;
						SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
					}
					if (attack == 268 && Main.netMode != NetmodeID.MultiplayerClient)
					{
						NPC.Center = new Vector2(player.Center.X + Main.rand.Next(-300, 300), player.Center.Y - Main.rand.Next(100, 200));
						NPC.netUpdate = true;
					}
					if (attack >= 277)
					{
						animation = 0; //idle
						NPC.dontTakeDamage = true;
						attack = 0; //reset
					}
				}

				if (attacktype == 2) //open robe and shoot stars
				{
					if (attack == 138)
					{
						lastattacktype = 2;
						animation = 5; //open robe
						NPC.dontTakeDamage = false;
					}

					if (attack == 168 || attack == 228) //stars (one more cycle)(primary batch)
					{
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							//down right
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 7, 7, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//right
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 10, 0, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//up right
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 7, -7, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//up
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 0, -10, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//up left
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, -7, -7, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//left
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, -10, 0, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//down left
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, -7, 7, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//down
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 0, 10, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
						}
						SoundEngine.PlaySound(SoundID.Item4, NPC.Center); //life crystal
					}

					if (attack == 198 || attack == 258) //stars (one more cycle)(secondary batch)
					{
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							//down right
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 3.5f, 8.5f, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//down left
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, -3.5f, 8.5f, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//left up
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, -8.5f, -3.5f, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//left down
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, -8.5f, 3.5f, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//up left
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 3.5f, -8.5f, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//up right
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, -3.5f, -8.5f, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//right up
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 8.5f, -3.5f, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
							//right down
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 8.5f, 3.5f, Mod.Find<ModProjectile>("BadStar").Type, stardamage, 0f, Main.myPlayer, 0, 0);
						}
						SoundEngine.PlaySound(SoundID.Item4, NPC.Center); //life crystal
					}

					if (attack == 350)
					{
						animation = 2; //teleport
						NPC.velocity *= 0;
						SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
					}
					if (attack == 359 && Main.netMode != NetmodeID.MultiplayerClient)
					{
						NPC.Center = new Vector2(player.Center.X + Main.rand.Next(-300, 300), player.Center.Y - Main.rand.Next(100, 200));
						NPC.netUpdate = true;
					}
					if (attack >= 368)
					{
						animation = 0; //idle
						NPC.dontTakeDamage = true;
						attack = 0; //reset
					}
				}

				if (attacktype == 3)
                {
					if (attack >= 138 && attack < 350) //tornado for a certain amount of time
                    {
						Vector2 direction =  player.Center - NPC.Center; //start - end
						float speed = 0.5f + (attack - 138) / 2; //move speed plus attack minus 138
						float inertia = speed * 2; //"turn" speed

						direction.Normalize();
						direction *= speed;
						NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;  //fly towards enemy

						if (Main.expertMode)
						{
							NPC.damage = 120;
						}
						else
						{
							NPC.damage = 60;
						}

						NPC.dontTakeDamage = false; //vunerable
						animation = 6; //tornado
					}
					if (attack == 350) //teleport
					{
						animation = 2; //teleport
						NPC.velocity *= 0;
						SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
					}
					if (attack == 359)
					{
						NPC.Center = new Vector2(player.Center.X + Main.rand.Next(-300, 300), player.Center.Y - Main.rand.Next(100, 200));
					}
					if (attack >= 368)
					{
						animation = 0; //idle
						NPC.dontTakeDamage = true;
						attack = 0; //reset
					}
				}
				if (attacktype == 4)
				{
					if (attack >= 138 && attack < 168) //back up
                    {
						NPC.velocity.Y -= 0.2f; //go up

						//extra
						NPC.velocity.X = 0;
						animation = 7; //drop attack
						NPC.dontTakeDamage = false; //vunerable
					}
					if (attack == 168) //drop
                    {
						NPC.velocity.Y = 30; //drop

						if (Main.expertMode) //deal damage(more in expert mode)
						{
							NPC.damage = 120;
						}
						else
						{
							NPC.damage = 60;
						}

						//extra
						NPC.velocity.X = 0;
						animation = 7; //drop attack
						NPC.dontTakeDamage = false; //vunerable
					}
					if (attack > 168 && attack < 228) //slow for 60 ticks(1 second)
                    {
						NPC.velocity.Y *= 0.9f; //slow 

						if (Main.expertMode) //deal damage(more in expert mode)
						{
							NPC.damage = 120;
						}
						else
						{
							NPC.damage = 60;
						}

						//extra
						NPC.velocity.X = 0;
						animation = 7; //drop attack
						NPC.dontTakeDamage = false; //vunerable
					}
					if (attack == 228) //teleport
					{
						animation = 2; //teleport
						NPC.velocity *= 0;
						SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
					}
					if (attack == 237 && Main.netMode != NetmodeID.MultiplayerClient)
					{
						NPC.Center = new Vector2(player.Center.X + Main.rand.Next(-300, 300), player.Center.Y - Main.rand.Next(100, 200));
						NPC.netUpdate = true;
					}
					if (attack >= 246)
					{
						animation = 0; //idle
						NPC.dontTakeDamage = true;
						attack = 0; //reset
					}
				}
			}
		}
		public override void FindFrame(int frameHeight) // animation
        {
			if (animation == 0) //idle
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 12)
				{
					NPC.frame.Y = 0; //idle 1
				}
				else
				{
					NPC.frame.Y = frameHeight; //idle 2
				}
				if (NPC.frameCounter >= 24)
				{
					NPC.frameCounter = 0;
				}
			}
			if (animation == 1) //moving side
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 7)
				{
					NPC.frame.Y = frameHeight * 2; //side 1
				}
				else
				{
					NPC.frame.Y = frameHeight * 3; //side 2
				}
				if (NPC.frameCounter >= 14)
				{
					NPC.frameCounter = 0;
				}
			}

			if (animation == 2) //teleport
            {
				NPC.frameCounter++;
				if (NPC.frameCounter < 3)
				{
					NPC.frame.Y = frameHeight * 10; //teleport 1
				}
				else if (NPC.frameCounter < 6)
				{
					NPC.frame.Y = frameHeight * 11; //teleport 2
				}
				else if (NPC.frameCounter < 9)
				{
					NPC.frame.Y = frameHeight * 12; //teleport 3
				}
				else if (NPC.frameCounter < 12)
				{
					NPC.frame.Y = frameHeight * 12; //teleport 3
				}
				else if (NPC.frameCounter < 15)
				{
					NPC.frame.Y = frameHeight * 11; //teleport 2
				}
				else
				{
					NPC.frame.Y = frameHeight * 10; //teleport 1
				}
				if (NPC.frameCounter >= 18)
				{
					NPC.frameCounter = 0;
				}
			}

			if (animation == 3) //open robe one side
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 5)
				{
					NPC.frame.Y = frameHeight * 4; //open robe 1
				}
				else
				{
					NPC.frame.Y = frameHeight * 5; //open robe 2
				}
				if (NPC.frameCounter >= 10)
				{
					NPC.frameCounter = 0;
				}
			}

			if (animation == 4) //dive with hand out
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 5)
				{
					NPC.frame.Y = frameHeight * 6; //dive 1
				}
				else
				{
					NPC.frame.Y = frameHeight * 7; //dive 2
				}
				if (NPC.frameCounter >= 10)
				{
					NPC.frameCounter = 0;
				}
			}

			if (animation == 5) //dive with hand out
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 5)
				{
					NPC.frame.Y = frameHeight * 8; //dive 1
				}
				else
				{
					NPC.frame.Y = frameHeight * 9; //dive 2
				}
				if (NPC.frameCounter >= 10)
				{
					NPC.frameCounter = 0;
				}
			}

			if (animation == 6) //tornado attack(expert mode)
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 3)
				{
					NPC.frame.Y = frameHeight * 13; //left swing
				}
				else if (NPC.frameCounter < 6)
				{
					NPC.frame.Y = frameHeight * 14; //front swing
				}
				else if (NPC.frameCounter < 9)
				{
					NPC.frame.Y = frameHeight * 15; //right swing
				}
				else
				{
					NPC.frame.Y = frameHeight * 16; //back swing
				}
				if (NPC.frameCounter >= 12)
				{
					NPC.frameCounter = 0;
				}
			}

			if (animation == 7) // vetical drop attack
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 5)
				{
					NPC.frame.Y = frameHeight * 17; //drop 1
				}
				else
				{
					NPC.frame.Y = frameHeight * 18; //drop 2
				}
				if (NPC.frameCounter >= 10)
				{
					NPC.frameCounter = 0;
				}
			}
		}

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedNightmareBoss, -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<NightmareBag>())); //only drops in expert

            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert()); //checks if not expert
            LeadingConditionRule masterMode = new LeadingConditionRule(new Conditions.IsMasterMode()); //checks if master mode

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<NightCloth>(), 1, 15, 15));

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<NightmareMask>(), 7));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<NightmareTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeables.BossRelics.NightmareRelic>()));

            masterMode.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.Nightmare.NightmarePetItem>(), 4));

            // add the rules
            npcLoot.Add(notExpertRule);
            npcLoot.Add(masterMode);
        }

		public override void BossLoot(ref string name, ref int potionType)
		{
			name = "Nightmare"; //_ has been defeated!
			potionType = ItemID.HealingPotion; //potion it drops
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
					Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.NightStar>(), speed, Scale: 2f); //Makes dust in a messy circle
					d.noGravity = true;
				}
				for (int i = 0; i < 20; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
                    Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, speed, Main.rand.Next(11, 13), Scale: 2f); //double jump smoke
                }
			}	
        }

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}