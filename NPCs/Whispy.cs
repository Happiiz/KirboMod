using KirboMod.Items.DarkMatter;
using KirboMod.Items.WhispyWoods;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using static System.Text.StringBuilder;
using KirboMod.Systems;
using Terraria.DataStructures;

namespace KirboMod.NPCs
{ 

	[AutoloadBossHead]
	public class Whispy : ModNPC
	{ 
		ref float attack => ref NPC.localAI[0]; //controls whispy's attack pattern
		ref float projrand => ref NPC.ai[0]; //determines whether to use an apple or a gordo
		private string projtype; //determines if an apple or gordo is spawned
		private int projdamg; //determines the damage depending on the apple or gordo

		private int blowing = 0; //determines if whispy is blowing

		private int teleport = 0; //if more than 0 then whispy will go through it's teleportation to the player
		private int gravityTimer = 0; //if zero then whispy will have gravity

		private int animation = 0;
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Whispy Woods");
			Main.npcFrameCount[NPC.type] = 4;

            // Add this in for bosses that have a summon item, requires corresponding code in the item
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				PortraitScale = 1f, // Portrait refers to the full picture when clicking on the icon in the bestiary
				PortraitPositionYOverride = 40,
                PortraitPositionXOverride = 50,
                Position = new Vector2(40, 30),
				Scale = 0.8f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true; //immune to not mess up attack direction
        }

		public override void SetDefaults() {
			NPC.width = 150;
			NPC.height = 250;
			NPC.damage = 25;
			NPC.noTileCollide = false;
			NPC.defense = 10; 
			NPC.lifeMax = 1800;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = 1992f; // money it drops
			NPC.knockBackResist = 0f;
			Banner = 0;
			BannerItem = Item.BannerToItem(Banner);
			NPC.aiStyle = -1;
			NPC.boss = true;
			NPC.noGravity = true;
			NPC.lavaImmune = false;
			Music = MusicID.Boss5;
			NPC.timeLeft = 60;
			NPC.friendly = false;
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
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("A tree that gained the unatural ability of sentience, and now devoted it's life to guarding it's brethren.")
            });
        }

        public override void AI() //constantly cycles each time
        {
			blowing--; //if 0 then whispy will stop blowing animation

            gravityTimer--;

            //Gravity
            if (gravityTimer <= 1) //not teleporting
			{
				NPC.velocity.Y = NPC.velocity.Y + 0.2f;
			}
			else //suspend in air
            {
				if (gravityTimer % 2 == 0) //If about to fall after teleport
				{
					NPC.velocity *= 0;
					for (int i = 0; i < 1; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
					{
                        Vector2 speed = Main.rand.NextVector2Circular(2f, 2f); //circle
                        Gore.NewGore(NPC.GetSource_FromThis(), NPC.position + new Vector2(Main.rand.Next(0, NPC.width), 250), speed, 
							Main.rand.Next(61, 63), Scale: 1f); //smoke
                    }
				}
			}
			if (NPC.velocity.Y >= 6f) //cap fall speed
			{
				NPC.velocity.Y = 6f;
			}

			Player player = Main.player[NPC.target];
			Vector2 distance = player.Center - NPC.Center;
            
			AttackPattern();

			NPC.spriteDirection = NPC.direction;
			NPC.netAlways = true;
			NPC.TargetClosest(true);
			//cap life
			if (NPC.life >= NPC.lifeMax)
            {
				NPC.life = NPC.lifeMax;
            }

			//checks if whispy can "see" the player
			int tilesInFrontOfWhispy = 0;

            for (int j = 0; j < 10; j++) //10 tiles ahead
			{
				for (int i = 0; i < 28; i++) //28 tiles across
				{
					Point tileLocation = new Point(((int)NPC.Center.X) / 16 + j * NPC.direction, ((int)NPC.position.Y - 200) / 16 + i);

                    if (WorldGen.SolidOrSlopedTile(Main.tile[tileLocation]))
					{
						tilesInFrontOfWhispy++;
                    }
				}
            }

            //Despawning
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active) //despawning
			{
				NPC.TargetClosest(false);
				NPC.noTileCollide = true;
				if (NPC.timeLeft > 60)
				{
					NPC.timeLeft = 60;
					return;
				}
				teleport = 0;
				gravityTimer = 0;
			}
			else //keep falling from teleport until can see player
            {
                NPC.noTileCollide = false;
                CheckPlatform(player);
            }

			Vector2 distance2 = player.Center - NPC.Bottom;

            if ((distance2.Y >= 100 || distance.Y <= -100 || distance.X >= 1000 || distance.X <= -1000 || tilesInFrontOfWhispy > 10)
				&& gravityTimer <= 0) //teleporting to player when out of range or view
			{
				if (player.dead == false) //checking if player is not dead
				{
					teleport++;
				}
			}
			else
			{
				teleport = 0;
			}

			if (teleport > 0) //check if about to teleport
            {
				NPC.dontTakeDamage = true; //cant be hurt
            }
			else
            {
				NPC.dontTakeDamage = false; //can be hurt
			}

			if (teleport == 120)
            {
				gravityTimer = 60;
				TeleportToPlayer(new Vector2(-50f,-300f)); //positive y is down, dont forget it!
            }
		}
		private void AttackPattern()
		{
		    attack++;

			Vector2 projoffset1 = new Vector2(80f * NPC.direction, 60f);
            Vector2 projoffset2 = new Vector2(Main.rand.Next(-150, 150), -200f);

            if (attack < 500)
			{
				NPC.TargetClosest(true);
			}
			Player player = Main.player[NPC.target];

			//PROJECTILES

			//apple and gordo probability
			projrand = Main.rand.Next(1, 5); //if more than 2 then apple else gordo

			if (projrand > 2)
			{
				projtype = "Apple";
				projdamg = 10;
			}
			else
			{
				projtype = "Gordo";
				projdamg = 15;
			}

			//whisp trajectory
			Vector2 distance = new Vector2(NPC.direction * 4, 0.5f);

			//randomly select distance
			if (Main.rand.Next(1, 3) == 1) //down
			{
				distance = new Vector2(NPC.direction * 4, 1.5f);
			}
			else if (Main.rand.Next(1, 3) == 2)//up
			{
				distance = new Vector2(NPC.direction * 4, -1.5f);
			}
			else //mid
			{
				distance = new Vector2(NPC.direction * 4, 0.5f);
			}


			//Apples and Gordos


			if (attack == 240) 
			{
				projrand = Main.rand.Next(1, 5);

				//projoffset2 makes sure it spawns in whispy's trees
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + projoffset2, NPC.velocity * 0, Mod.Find<ModProjectile>(projtype).Type, projdamg / 2, 2f, Main.myPlayer, 0, NPC.target);
				}
				SoundEngine.PlaySound(SoundID.Grass, NPC.Center); //grass

				for (int i = 0; i < 20; i++) //dust
				{
					Vector2 circlespeed = Main.rand.NextVector2Circular(1f, 1f); //circle (put it inside loop so they do in different directions)

					Dust deeznuts = Dust.NewDustPerfect(NPC.Center + projoffset2, DustID.GrassBlades, circlespeed * 4);
					deeznuts.noGravity = false;
				}
				//Main.PlaySound(SoundID.Splash);
			}
            if (attack == 300)
			{
				projrand = Main.rand.Next(1, 5);

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + projoffset2, NPC.velocity * 0, Mod.Find<ModProjectile>(projtype).Type, projdamg / 2, 2f, Main.myPlayer, 0, NPC.target);
				}
				SoundEngine.PlaySound(SoundID.Grass, NPC.Center); //grass

				for (int i = 0; i < 20; i++) //dust
				{
					Vector2 circlespeed = Main.rand.NextVector2Circular(1f, 1f); //circle (put it inside loop so they do in different directions)

					Dust deeznuts = Dust.NewDustPerfect(NPC.Center + projoffset2, DustID.GrassBlades, circlespeed * 4);
					deeznuts.noGravity = false;
				}
				//Main.PlaySound(SoundID.Splash);
			}
			if (attack == 360)
			{
				projrand = Main.rand.Next(1, 5);

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + projoffset2, NPC.velocity * 0, Mod.Find<ModProjectile>(projtype).Type, projdamg / 2, 2f, Main.myPlayer, 0, NPC.target);
				}
				SoundEngine.PlaySound(SoundID.Grass, NPC.Center); //grass

				for (int i = 0; i < 20; i++) //dust
				{
					Vector2 circlespeed = Main.rand.NextVector2Circular(1f, 1f); //circle (put it inside loop so they do in different directions)

					Dust deeznuts = Dust.NewDustPerfect(NPC.Center + projoffset2, DustID.GrassBlades, circlespeed * 4);
					deeznuts.noGravity = false;
				}
				//Main.PlaySound(SoundID.Splash);
			}


			//expert tree items


			if (NPC.life <= NPC.lifeMax * 0.25f && Main.expertMode) //25% health and in expert mode
			{
				if (attack == 210)
				{
					projrand = Main.rand.Next(1, 5);

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + projoffset2, NPC.velocity * 0, Mod.Find<ModProjectile>(projtype).Type, projdamg / 2, 2f, Main.myPlayer, 0, NPC.target);
					}
					SoundEngine.PlaySound(SoundID.Grass, NPC.Center); //grass

					for (int i = 0; i < 20; i++) //dust
					{
						Vector2 circlespeed = Main.rand.NextVector2Circular(1f, 1f); //circle (put it inside loop so they do in different directions)

						Dust deeznuts = Dust.NewDustPerfect(NPC.Center + projoffset2, DustID.GrassBlades, circlespeed * 4);
						deeznuts.noGravity = false;
					}
				}
				if (attack == 270)
				{
					projrand = Main.rand.Next(1, 5);

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + projoffset2, NPC.velocity * 0, Mod.Find<ModProjectile>(projtype).Type, projdamg / 2, 2f, Main.myPlayer, 0, NPC.target);
					}
					SoundEngine.PlaySound(SoundID.Grass, NPC.Center); //grass

					for (int i = 0; i < 20; i++) //dust
					{
						Vector2 circlespeed = Main.rand.NextVector2Circular(1f, 1f); //circle (put it inside loop so they do in different directions)

						Dust deeznuts = Dust.NewDustPerfect(NPC.Center + projoffset2, DustID.GrassBlades, circlespeed * 4);
						deeznuts.noGravity = false;
					}
				}
				if (attack == 330)
				{
					projrand = Main.rand.Next(1, 5);

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + projoffset2, NPC.velocity * 0, Mod.Find<ModProjectile>(projtype).Type, projdamg / 2, 2f, Main.myPlayer, 0, NPC.target);
					}
					SoundEngine.PlaySound(SoundID.Grass, NPC.Center); //grass

					for (int i = 0; i < 20; i++) //dust
					{
						Vector2 circlespeed = Main.rand.NextVector2Circular(1f, 1f); //circle (put it inside loop so they do in different directions)

						Dust deeznuts = Dust.NewDustPerfect(NPC.Center + projoffset2, DustID.GrassBlades, circlespeed * 4);
						deeznuts.noGravity = false;
					}
				}
			}


            //First round of whisps


            if (attack == 540) 
			{
				NPC.frameCounter = 0;

				blowing = 30;

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + projoffset1, distance, Mod.Find<ModProjectile>("Whisp").Type, 10 / 2, 1f, Main.myPlayer, 0, 0); //projoffset is to make sure it shoots at whispy's lips
				}
				SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);
			}
			if (attack == 570)//second whisp
			{
				NPC.frameCounter = 0;

				blowing = 30;

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + projoffset1, distance, Mod.Find<ModProjectile>("Whisp").Type, 10 / 2, 1f, Main.myPlayer, 0, 0);
				}
				SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                if (NPC.life > NPC.lifeMax * (Main.expertMode ? 0.75f : 0.50f)) //above 50% health (75% in expert mode)
                {
					attack = 630; //go to second cycle
                }
			}
			if (attack == 600)//third whisp
			{
				NPC.frameCounter = 0;
				if (NPC.life <= NPC.lifeMax * (Main.expertMode ? 0.75f : 0.50f)) //50% health (75% in expert mode)
				{

					blowing = 30;

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + projoffset1, distance, Mod.Find<ModProjectile>("Whisp").Type, 10 / 2, 1f, Main.myPlayer, 0, 0);
					}
					SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                    if (NPC.life > NPC.lifeMax * (Main.expertMode ? 0.50f : 0.25f)) //25% health (50% in expert mode)
                    {
                        attack = 630; //go to second cycle
                    }
				}
			}
			if (attack == 630)//last whisp
			{
				NPC.frameCounter = 0;
				if (NPC.life <= NPC.lifeMax * (Main.expertMode ? 0.50f : 0.25f)) //25% health (50% in expert mode)
				{

					blowing = 30;

					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + projoffset1, distance, Mod.Find<ModProjectile>("Whisp").Type, 10 / 2, 1f, Main.myPlayer, 0, 0);
					SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);
				}
			}

			//second round of whisps

			if (attack == 690)//first whisp
			{
				NPC.frameCounter = 0;

				blowing = 30;

                if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + projoffset1, distance, Mod.Find<ModProjectile>("Whisp").Type, 10 / 2, 1f, Main.myPlayer, 0, 0);
				}
				SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);
            }
			if (attack == 720)//second whisp
			{
				NPC.frameCounter = 0;

				blowing = 30;

                if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + projoffset1, distance, Mod.Find<ModProjectile>("Whisp").Type, 10 / 2, 1f, Main.myPlayer, 0, 0);
				}
				SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                if (NPC.life > NPC.lifeMax * (Main.expertMode ? 0.75f : 0.50f)) //above 50% health (75% in expert mode)
                {
                    attack = 120; //end cycle
                }
            }
			if (attack == 750)
			{
				NPC.frameCounter = 0;
				if (NPC.life <= NPC.lifeMax * (Main.expertMode ? 0.75f : 0.50f)) //50% health (75% in expert mode)
				{
					blowing = 30;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + projoffset1, distance, Mod.Find<ModProjectile>("Whisp").Type, 10 / 2, 1f, Main.myPlayer, 0, 0);
					}
					SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                    if (NPC.life > NPC.lifeMax * (Main.expertMode ? 0.50f : 0.25f)) //above 25% health (50% in expert mode)
                    {
                        attack = 120; //end cycle
                    }
                }
			}
			if (attack == 780)//final whisp
			{
				NPC.frameCounter = 0;
				if (NPC.life <= NPC.lifeMax * (Main.expertMode ? 0.50f : 0.25f)) //25% health (50% in expert mode)
				{
					blowing = 30;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + projoffset1, distance, Mod.Find<ModProjectile>("Whisp").Type, 10 / 2, 1f, Main.myPlayer, 0, 0);
					}
					SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

					if (NPC.life > NPC.lifeMax * 0.25f || !Main.expertMode) //not 25% health or in expert mode
                    {
                        attack = 120; //end cycle
                    }
				}
			}

			if (attack == 840)
			{ 
			    NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + (NPC.direction * 100)), (int)(NPC.Center.Y + 275), Mod.Find<ModNPC>("WhispyRoot").Type, 0, 0, NPC.direction, 0, 0, 0);
			}
			if (attack == 845)
			{
				NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + (NPC.direction * 300)), (int)(NPC.Center.Y + 275), Mod.Find<ModNPC>("WhispyRoot").Type, 0, 0, NPC.direction, 0, 0, 0);
			}
			if (attack == 850)
			{
				NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + (NPC.direction * 500)), (int)(NPC.Center.Y + 275), Mod.Find<ModNPC>("WhispyRoot").Type, 0, 0, NPC.direction, 0, 0, 0);
			}
			if (attack == 855)
			{
				NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + (NPC.direction * 700)), (int)(NPC.Center.Y + 275), Mod.Find<ModNPC>("WhispyRoot").Type, 0, 0, NPC.direction, 0, 0, 0);
			}
			if (attack == 860)
			{
				NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + (NPC.direction * 900)), (int)(NPC.Center.Y + 275), Mod.Find<ModNPC>("WhispyRoot").Type, 0, 0, NPC.direction, 0, 0, 0);

				attack = 120; //end cycle definitely
			}


			if (blowing > 0) //declares if whispy is whispying
            {
				animation = 1; //blow
            }
			else
            {
				animation = 0; // 0o0
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
            if (onplatform && (NPC.Center.Y < player.position.Y - 100)) //if they are and the player is lower than the boss, temporarily let the boss ignore tiles to go through them
            {
                NPC.noTileCollide = true;
            }
            else
            {
                NPC.noTileCollide = false;
            }
        }

        public override void FindFrame(int frameHeight) // animation
        {
		   if (animation == 0) //open eye
            {
				NPC.frameCounter++;
				if (NPC.frameCounter < 180)
				{
					NPC.frame.Y = 0;
				}
				else
				{
					NPC.frame.Y = frameHeight;
				}
				if (NPC.frameCounter >= 190)
                {
					NPC.frameCounter = 0;
                }
			}
		   if (animation == 1) //blow
            {
				NPC.frameCounter++;
				if (NPC.frameCounter <= 15)
                {
					NPC.frame.Y = frameHeight * 2;
				}
				else if (NPC.frameCounter > 15 & NPC.frameCounter <= 30)
				{
					NPC.frame.Y = frameHeight * 3;
				}
				else
                {
					NPC.frameCounter = 0;
                }
			}
		}

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedWhispyBoss, -1);
        }

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<WhispyBag>())); //only drops in expert

            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert()); //checks if not expert
            LeadingConditionRule masterMode = new LeadingConditionRule(new Conditions.IsMasterMode()); //checks if master mode

            notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.Wood, 1, 20, 20)); //hell yeah

            notExpertRule.OnSuccess(new OneFromRulesRule(1, ItemDropRule.Common(ModContent.ItemType<Items.Weapons.SwishyTree>(), 1)
				, ItemDropRule.Common(ModContent.ItemType<Items.Weapons.GordoItem>(), 1, 300, 300)
				, ItemDropRule.Common(ModContent.ItemType<Items.Weapons.WindPipe>(), 1)
				, ItemDropRule.Common(ModContent.ItemType<Items.Weapons.SwayingBranch>(), 1)));

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<WhispyMask>(), 7));

			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WhispyTrophy>(), 10)); //drop trophy

			//master mode stuff
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeables.BossRelics.WhispyWoodsRelic>()));

            masterMode.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.WhispyWoods.WhispyPetItem>(), 4)); 

            // add the rules
            npcLoot.Add(notExpertRule);
			npcLoot.Add(masterMode);
        }

		public override void BossLoot(ref string name, ref int potionType)
		{
			name = "Whispy Woods";
			potionType = ItemID.LesserHealingPotion;
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
                for (int i = 0; i < 8; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                {
                    // go around in a octogonal pattern
                    Vector2 speed = new Vector2((float)Math.Cos(MathHelper.ToRadians(i * 45)) * 25, (float)Math.Sin(MathHelper.ToRadians(i * 45)) * 25);

                    Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.BoldStar>(), speed, Scale: 3f); //Makes dust in a messy circle
                    d.noGravity = true;
                }
				for (int i = 0; i < 20; i++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
                    Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, speed, Main.rand.Next(11, 13), Scale: 2f); //double jump smoke
                }
			}	
			else
            {
				for (int i = 0; i < 5; i++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(3f, 3f); //circle
					Dust d = Dust.NewDustPerfect(NPC.Center, DustID.Dirt, speed * 2, Scale: 1f); //Makes dust in a messy circle
					d.noGravity = false;
				}
			}
        }

		private void TeleportToPlayer(Vector2 teleoffset)
        {
			Player player = Main.player[NPC.target];
			NPC.position = player.Center + teleoffset; //above player
			teleport = 0;
			NPC.netUpdate = true;
		}

        public override Color? GetAlpha(Color drawColor)
        {
			if (teleport > 0) //if out of range
			{
				return new Color(255, 128, 128); //light red
			}
			else
            {
				return null;
            }
		}

        public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
        {
            boundingBox = NPC.Hitbox;
        }
    }
}
