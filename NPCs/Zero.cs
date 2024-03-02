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
using System.Collections.Generic;

namespace KirboMod.NPCs
{
	[AutoloadBossHead]
	public class Zero : ModNPC
	{
        enum ZeroAttackType : byte
        {
            DecideNext,//0
            BloodShots,//1
            Dash,//2
            DarkMatterShots,//3
            Sparks,//4
            ThornTail,//5
            BackgroundShots,//6
        }

        private int animation = 1; //frame cycles

		private ZeroAttackType lastattacktype = ZeroAttackType.DecideNext; //sets last attack type
		private ZeroAttackType attacktype = ZeroAttackType.DecideNext; //decides the attack

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
            };
        }

        public override void SetDefaults()
		{
			NPC.width = 398; 
			NPC.height = 398;
			DrawOffsetY = 193;
			NPC.damage = 150; 
			NPC.noTileCollide = true;
			NPC.defense = 150;
			NPC.lifeMax = 280000;
			NPC.HitSound = SoundID.NPCHit1; //slime
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice( 0, 38, 18, 10); // money it drops
			NPC.knockBackResist = 0f;
			NPC.aiStyle = -1;
			NPC.npcSlots = 16;
			NPC.boss = true;
			NPC.noGravity = true;
			NPC.lavaImmune = true;
            NPC.dontTakeDamage = true; //only initally

            /*Music = MusicID.Boss2;*/
            if (!Main.dedServ)
            {
				//Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Zero");
				Music = MusicID.Boss2;
            }

            SceneEffectPriority = SceneEffectPriority.BossHigh; // By default, musicPriority is BossLow
			NPC.alpha = 255; //initally
        }

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */ //damage is automatically doubled in expert, use this to reduce it
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.75 * balance); //420,000 health in expert
			NPC.damage = (int)(NPC.damage * 1); //2x damage
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				new HyperzoneBackgroundProvider(), //I totally didn't reference the vanilla code what no way

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("The ultimate leader of all dark matter. Uses its leigon to blanket entire worlds in darkness.")
			}); 
        }

        public override void SendExtraAI(BinaryWriter writer) //for syncing non NPC.ai[] stuff
        {
            writer.Write(animation);

            writer.Write((byte)lastattacktype);
            writer.Write((byte)attacktype);

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

            lastattacktype = (ZeroAttackType)reader.ReadByte();
			attacktype = (ZeroAttackType)reader.ReadByte();

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

				if (NPC.ai[1] == 389) //fight start effect
                {
					for (int i = 0; i < 40; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
					{
						Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
						Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 20, Scale: 2); //Makes dust in a messy circle
						d.noGravity = true;
					}
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
            float playerSineDistanceExtra = attacktype == ZeroAttackType.BloodShots && NPC.ai[0] > 120 ?
				MathF.Sin((NPC.ai[0] / 60 % 3600) * 2) * 300 : 0;

            Vector2 playerRightDistance = player.Center + new Vector2(500 + backupoffset, playerSineDistanceExtra) - NPC.Center;
			Vector2 playerLeftDistance = player.Center + new Vector2(-500 - backupoffset, playerSineDistanceExtra) - NPC.Center;
			Vector2 playerAboveDistance = player.Center + new Vector2(0, -1000) - NPC.Center;

            if (NPC.ai[0] == 0) //restart stats
            {
				animation = 0;
				NPC.frameCounter = 0; //reset animation

				NPC.dontTakeDamage = false;

                backupoffset = 0;
            }

			NPC.ai[0]++;

			float timer = NPC.GetLifePercent() > 0.5 ? NPC.ai[0] : NPC.ai[0] + 60;

			float speed = 16; 
			float inertia = 20;

            //far away
            if (playerDistance.Length() > 5000)
            {
                speed += 10 + playerDistance.Length() - 5000; //make Zero move faster 
            }

            if (NPC.ai[0] <= 120)
			{
				NPC.TargetClosest(true); //face player before attacking

                DefaultMovement(playerDistance, playerRightDistance, playerLeftDistance, speed, inertia);

                if (NPC.ai[0] == 120)  
				{
					if (NPC.GetLifePercent() <= 0.25) //25%
					{
						//background attack is done every 3 attacks when zero's health gets low enough
						//(does it the next cycle upon initally dropping low enough)

						if (backgroundAttackCountDown <= 0) //equal or less than zero
						{
							attacktype = ZeroAttackType.BackgroundShots; //background attack
							lastattacktype = ZeroAttackType.BackgroundShots;
							backgroundAttackCountDown = 2; //restart
						}
						else //do regular attack
						{
							backgroundAttackCountDown--; //go down by 1
                            AttackDecideNext();
                        }
					}
					else
					{
                        AttackDecideNext();
                    }
				}
			}
			else //attacking
            {
				if (attacktype == ZeroAttackType.BloodShots) //blood shots
                {
                    NPC.TargetClosest(true); //face player during attack

                    speed += 8;

                    //blood effect for attack
                    if (NPC.ai[0] % 5 == 0) //every 5
					{
						int d = Dust.NewDust(NPC.position + new Vector2(NPC.direction == 1 ? NPC.width - 200 : 0, NPC.height / 4), 200, 200, ModContent.DustType<Dusts.Redsidue>());
						Main.dust[d].velocity *= 0;
					}

                    if (NPC.ai[0] % 10 == 0) //every 10 ticks
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Vector2 spread = Main.rand.NextVector2Circular(10f, 10f); //circle
                            Dust.NewDustPerfect(bloodlocation, ModContent.DustType<Dusts.Redsidue>(), spread, Scale: 1f); //Makes dust in a messy circle
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 bloodlocation = new Vector2(NPC.Center.X + NPC.width / 2 * NPC.direction, NPC.Center.Y);

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), bloodlocation, new Vector2(NPC.direction * 25, 0), ModContent.ProjectileType<ZeroBloodShot>(), 60 / 2, 6f, Main.myPlayer, NPC.whoAmI, 0);
                        }
                        SoundStyle SkinTear = new SoundStyle("KirboMod/Sounds/Item/SkinTear");
                        SoundEngine.PlaySound(SkinTear, NPC.Center);
                    }

                    DefaultMovement(playerDistance, playerRightDistance, playerLeftDistance, speed, inertia);

                    if (NPC.ai[0] >= 440)
                    {
                        NPC.ai[0] = 0;
                    }
                }


				if (attacktype == ZeroAttackType.Dash) //dash
				{
                    NPC.TargetClosest(false); //don't face player during attack

                    if (NPC.ai[0] <= 150) //backup
					{
						if (NPC.ai[0] == 121)
						{
							NPC.velocity *= 0.01f; //freeze to warn player (but not too much else it might disappear)
						}
						else
						{
							NPC.velocity.X -= NPC.direction * 0.5f; //back up
						}
					}
					else if (NPC.ai[0] <= 270) //dash
					{
						NPC.velocity.X += NPC.direction * 0.6f;
					}
					else //reset
					{
						NPC.ai[0] = 0;
						backupoffset = 0;
					}

					//go up or down
					if (player.Center.Y < NPC.Center.Y)
					{
						NPC.velocity.Y -= 0.5f;
					}
					else
					{
						NPC.velocity.Y += 0.5f;
					}

					//cap
					if (NPC.velocity.Y > 7.5f)
					{
						NPC.velocity.Y = 7.5f;
					}
					if (NPC.velocity.Y < -7.5f)
					{
						NPC.velocity.Y = -7.5f;
					}
				}

				if (attacktype == ZeroAttackType.DarkMatterShots) //dark matter shot
				{
                    NPC.TargetClosest(true); //face player during attack

					if (NPC.ai[0] % 80 == 0) //on multiples of 80
                    {
                        CreateDarkMatterFloorAndCeiling();

                        SoundEngine.PlaySound(SoundID.Item104, NPC.Center);
                    }

                    DefaultMovement(playerDistance, playerRightDistance, playerLeftDistance, speed, inertia);

                    if (NPC.ai[0] >= 520)
					{
						NPC.ai[0] = 0;
					}
				}

				if (attacktype == ZeroAttackType.Sparks) //sparks
                {
                    NPC.TargetClosest(true); //face player during attack

                    float attackRange = MathF.Sin((NPC.ai[0] / 60 % 3600) * 3) * 50;

                    for (int i = 0; i < 2; i++)
                    {
                        //rapid fire in a spread
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 150, 0),
                                Main.rand.NextVector2Unit(MathF.PI / 2 * -NPC.direction, MathF.PI * 0.85f) * (60 + attackRange),
                                ModContent.ProjectileType<ZeroSpark>(), 0, 0, Main.myPlayer, 0, 0);
                        }
                    }

                    if (NPC.ai[0] % 5 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item39.WithVolumeScale(1.4f), NPC.Center);
                    }

					backupoffset = 500;

                    DefaultMovement(playerDistance, playerRightDistance, playerLeftDistance, speed, inertia);

                    if (NPC.ai[0] >= 360)
					{
						NPC.ai[0] = 0;
					}
				}

				if (attacktype == ZeroAttackType.ThornTail) //thorns
                {
					if (NPC.ai[0] < 180)
					{
						animation = 2;
						NPC.velocity *= 0;

						if (NPC.ai[0] == 130)
                        {
							SoundEngine.PlaySound(SoundID.Item56.WithVolumeScale(1.6f).WithPitchOffset(-0.1f), NPC.Center); 
						}
					}
					else if (NPC.ai[0] < 480)
					{
						animation = 3;

                        speed += 8; //go slightly faster

                        playerAboveDistance.Normalize();
						playerAboveDistance *= speed;
						NPC.velocity = (NPC.velocity * (inertia - 1) + playerAboveDistance) / inertia; //fly towards player

						if (NPC.ai[0] % 3 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
						{ 
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, 290), 
								new Vector2(Main.rand.Next(-20, 20), 15), ModContent.ProjectileType<ZeroThornJuice>(), 60 / 2, 1f, Main.myPlayer, 0);
					    }
						if (NPC.ai[0] % 10 == 0)
						{
							SoundEngine.PlaySound(SoundID.Item17.WithVolumeScale(1.2f), NPC.Center); //stinger
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
				if (attacktype == ZeroAttackType.BackgroundShots) //background attack
				{
					NPC.velocity.Y *= 0.98f;

					if (NPC.ai[0] <= 180) //backup
					{
						NPC.TargetClosest(false); //don't face player during attack

						NPC.scale = 1 - ((float)Utils.GetLerpValue(120, 180, NPC.ai[0]) * 0.6f); //get smaller

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
							Vector2 BackgroundplayerDistance = player.Center + (player.velocity * 5) - NPC.Center;

							if (Main.netMode != NetmodeID.MultiplayerClient) 
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, BackgroundplayerDistance / 60, ModContent.ProjectileType<ZeroScreenBlood>(), 80 / 2, 1f, Main.myPlayer, 0, NPC.ai[0] % 2 == 0 ? 0 : 1);
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
        //call this on the frame you want to create the setup
        //probably snaps to the position too quickly, but idk how to calculate the numbers to fix it so it gets there in fewer frames
        void CreateDarkMatterFloorAndCeiling()
        {
            Player player = Main.player[NPC.target];
            //tweak these parameters to your liking for balancing
            float ySpacing = 1000;
            float xSpacing = 130;
            int mattersInWall = 20;

            for (int i = 0; i < mattersInWall; i++)
            {
                //if i is even, mod will result in 0, multiplied by 2 becomes 0, then subtract one to become -1
                //if i is odd, mod will result in 1, multiplied by 2 becomes 2, then subtract one to become 1
                int directionY = i % 2 * 2 - 1;
                Vector2 offset = default;
                offset.Y = directionY * ySpacing / 2;
                offset.X = Utils.Remap(i, 0, mattersInWall - 1, -xSpacing * mattersInWall / 2, xSpacing * mattersInWall / 2);
                DarkMatterShot.AccountForSpeed(ref offset, player);
                Vector2 from = NPC.Center + Main.rand.NextVector2Circular(NPC.width, NPC.height);
                DarkMatterShot.NewDarkMatterShot(NPC, offset + player.Center, from, 80 / 2, directionY, .3f);
            }
        }

        void AttackDecideNext()
        {
            List<ZeroAttackType> possibleAttacks = new() { ZeroAttackType.BloodShots, ZeroAttackType.Dash, ZeroAttackType.DarkMatterShots, ZeroAttackType.Sparks, ZeroAttackType.ThornTail };
            
            possibleAttacks.Remove(lastattacktype);

            if (NPC.GetLifePercent() >= 0.95f)
            {
                attacktype = ZeroAttackType.BloodShots;
            }
            else
            {
                attacktype = possibleAttacks[Main.rand.Next(possibleAttacks.Count)];
                NPC.netUpdate = true;
            }

            lastattacktype = attacktype;
		}

        void DefaultMovement(Vector2 playerDistance, Vector2 playerRightDistance, Vector2 playerLeftDistance, float speed, float inertia)
        {
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
        }

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
				return false; //no health bar
            }
			else //health bar
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
					for (int i = 0; i < 40; i++)
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
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<Dusts.Redsidue>());
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
			if (attacktype == ZeroAttackType.BackgroundShots & NPC.ai[0] > 120)
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

			//fade in stuff (fades in from black but it looks cool so I'm keeping it)
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
