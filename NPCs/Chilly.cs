using KirboMod.Items.Weapons;
using KirboMod.Items;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace KirboMod.NPCs
{
	public class Chilly : ModNPC
	{
		private int attacktype = 0;
		private int attackTimer; 
		private int attackStart = 70;
		private int attackDuration = 90;
		private int TotalAttackCycleDuration { get => attackStart + attackDuration; }
		private bool attacking = false;
        private bool jumped = false;

        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Chilly");
			Main.npcFrameCount[NPC.type] = 8;
		}
		public override void SetDefaults()
		{
			NPC.width = 28;
			NPC.height = 42;
			NPC.damage = 16;
			NPC.defense = 7;
			NPC.lifeMax = 70;
			NPC.HitSound = SoundID.NPCHit11;
			NPC.DeathSound = SoundID.NPCDeath15;
			NPC.value = Item.buyPrice(0, 0, 0, 10);
			NPC.knockBackResist = .8f;
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.ChillyBanner>();
			NPC.aiStyle = -1;
			NPC.friendly = false;
			NPC.noGravity = false;
			NPC.coldDamage = true;
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
            //if player is in snow biome and daytime or underground and not in water
            if (spawnInfo.Player.ZoneTowerVortex || spawnInfo.Player.ZoneTowerSolar
                || spawnInfo.Player.ZoneTowerNebula || spawnInfo.Player.ZoneTowerStardust)
            {
                return 0f;
            }
            else if (spawnInfo.Player.ZoneSnow && (Main.dayTime || spawnInfo.Player.ZoneRockLayerHeight) && !spawnInfo.Water && !spawnInfo.Sky
                && !Main.eclipse)
			{
				return spawnInfo.SpawnTileType == TileID.SnowBlock || spawnInfo.SpawnTileType == TileID.IceBlock ? .2f : 0f; //functions like a mini if else statement
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
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Chillies found comfort in the cold tundra, and now they freeze anyone who touches their land!")
            });
        }
        public override void AI() //constantly cycles each time
		{
			NPC.spriteDirection = NPC.direction;
			Player player = Main.player[NPC.target];
			Vector2 distance = player.Center - NPC.Center;
			float range = 120;
			bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);

			if (distance.X < range & distance.X > -range & distance.Y > -range & distance.Y < range && lineOfSight && !player.dead) //checks if chills is in range
			{
				attacking = true; //now attack
			}
			if (attacking == false) //if not attack
			{
				attacktype = 0; //walk
			}
			else
            {
				attacktype = 1; //attack
            }
			//declaring attacktype values
			if (attacktype == 0)
			{
				Walk();
			}
			if (attacktype == 1)
			{
				Freeze();
			}

			//for stepping up tiles
			Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
		}

		public override void FindFrame(int frameHeight) // animation
		{
			if (attacktype == 0) //walking
            {
                NPC.frameCounter++;
                if (NPC.frameCounter < 8.0)
                {
                    NPC.frame.Y = 0;
                }
                else if (NPC.frameCounter < 16.0)
                {
                    NPC.frame.Y = frameHeight;
                }
                else if (NPC.frameCounter < 24.0)
                {
                    NPC.frame.Y = frameHeight * 2;
                }
                else if (NPC.frameCounter < 32.0)
                {
                    NPC.frame.Y = frameHeight * 3;
                }
                else
                {
                    NPC.frameCounter = 0.0;
                }
            }
			if (attacktype == 1) //freezing
			{
                NPC.frameCounter++;

                if (attackTimer < attackStart) //getting ready
				{
                    if (NPC.frameCounter < 2.0)
                    {
                        NPC.frame.Y = frameHeight * 4;
                    }
                    else if (NPC.frameCounter < 4.0)
                    {
                        NPC.frame.Y = frameHeight * 5;
                    }
                    else
                    {
                        NPC.frameCounter = 0.0;
                    }
                }
				else //FREEEEEZE
				{
                    if (NPC.frameCounter < 3.0)
                    {
                        NPC.frame.Y = frameHeight * 6;
                    }
                    else if (NPC.frameCounter < 6.0)
                    {
                        NPC.frame.Y = frameHeight * 7;
                    }
                    else
                    {
                        NPC.frameCounter = 0.0;
                    }
                }
			}
		}

		private void Walk() //walk towards player
		{
			Player player = Main.player[NPC.target];
			NPC.TargetClosest(true);

			float speed = 1f; //top speed
			float inertia = 10f; //acceleration and decceleration speed

			Vector2 direction = NPC.Center + new Vector2(NPC.direction * 50, 0) - NPC.Center; //start - end 
			//we put this instead of player.Center so it will always be moving top speed instead of slowing down when player is near

			direction.Normalize();
			direction *= speed;
			if (NPC.velocity.Y == 0 || jumped == true) //walking/jumping (so it doesn't interfere with knockback)
            {
				NPC.velocity.X = (NPC.velocity.X * (inertia - 1) + direction.X) / inertia; //use .X so it only effects horizontal movement
			}

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
		private void Freeze()
        {
			Player player = Main.player[NPC.target];
			Vector2 projshoot = player.Center - NPC.Center;
			projshoot.Normalize();
			projshoot *= 8;//this is the max range
			if (Main.expertMode)
				projshoot *= 1.2f;//expert mode range multiplier
			projshoot *= Easings.EaseInOutSine(Utils.GetLerpValue(attackStart, TotalAttackCycleDuration, attackTimer + attackDuration / 3f, true));

			NPC.velocity.X *= 0.9f; //slow
			attackTimer++; //goes up by 1 each tick
			NPC.TargetClosest(true);

			if (attackTimer > attackStart) //attack for 60 ticks
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					if(Main.expertMode)
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projshoot.RotatedByRandom(MathF.Tau), ModContent.ProjectileType<Projectiles.BadIce>(), 20 / 2, 1, Main.myPlayer, 0, 0);
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projshoot.RotatedByRandom(MathF.Tau), ModContent.ProjectileType<Projectiles.BadIce>(), 20 / 2, 1, Main.myPlayer, 0, 0);
				}
				Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(16, 16), DustID.WhiteTorch, -Vector2.UnitY);
				if (attackTimer % 10 == 0) //every 10th tick
                {
					Particles.Ring.EmitRing(NPC.Center + Main.rand.NextVector2Circular(16, 16), Color.SkyBlue);
					SoundEngine.PlaySound(SoundID.Item24 with { MaxInstances = 0, Volume = 2, Pitch = 1}, NPC.Center); //spectre boots
                }
            }
			if (attackTimer >= TotalAttackCycleDuration)
			{
				attackTimer = 0; //ready next attack
				attacking = false; //stop attacking
			}
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Items.Weapons.Ice>(), 20, 10)); // 1 in 20 (5%) chance in Normal. 1 in 10 (10%) chance in Expert

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
