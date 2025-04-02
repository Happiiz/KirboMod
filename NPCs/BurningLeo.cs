using KirboMod.Items;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using KirboMod.Projectiles.Flames;

namespace KirboMod.NPCs
{
    public class BurningLeo : ModNPC
	{
		private int attacktype = 0;
		private int attack = -60; //0 is attack point
		private bool attacking = false;
        private bool jumped = false;
		float RangeMultiplier { get => Main.expertMode ? 1.45f : 1; }
		float ConfusedMultiplier { get => NPC.confused ? -1 : 1; }
		float MoveSpeedMultiplier { get 
			{ float result = Main.expertMode ? 1.35f : 1;
				return NPC.confused ? -result : result; } }
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Burning Leo");
			Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;
        }

		public override void SetDefaults()
		{
			NPC.width = 50;
			NPC.height = 46;
			NPC.damage = 15;
			NPC.defense = 10;
			NPC.lifeMax = 70;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(0, 0, 1, 50);
			NPC.knockBackResist = .5f;
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.BurningLeoBanner>();
			NPC.aiStyle = -1;
			NPC.friendly = false;
			NPC.noGravity = false;

        }

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			//if player is in jungle biome and daytime or underground and not in water
			if (spawnInfo.Player.ZoneTowerVortex || spawnInfo.Player.ZoneTowerSolar 
				|| spawnInfo.Player.ZoneTowerNebula || spawnInfo.Player.ZoneTowerStardust)
			{
				return 0f;
			}
			else if (spawnInfo.Player.ZoneJungle && (Main.dayTime || spawnInfo.Player.ZoneRockLayerHeight) && !spawnInfo.Water && !spawnInfo.Sky
				&& !Main.eclipse) 
			{
				return spawnInfo.SpawnTileType == TileID.JungleGrass || spawnInfo.SpawnTileType == TileID.Mud ? .1f : 0f; //functions like a mini if else statement
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
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Burning Leos found comfort in the warm jungle, and now they scorch anyone who touches their land!")
            });
        }

        public override void AI() //constantly cycles each time
		{
			NPC.spriteDirection = NPC.direction;
			Player player = Main.player[NPC.target];
			Vector2 distance = player.Center - NPC.Center;

			bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
			float range = 160 * ConfusedMultiplier * RangeMultiplier;
            bool inPlayerRangeX = distance.X <= range && distance.X >= 0f; //in range of the right
			Dust d = Dust.NewDustDirect(NPC.position - new Vector2(0, 8), NPC.width, NPC.height / 2, DustID.Torch, 0, 0, 200, Scale: 2);
			d.noGravity = true;
			d.velocity.Y -= 1;
            if (NPC.direction == -1) //facing left
            {
                inPlayerRangeX = distance.X >= -range && distance.X <= 0f; //in range of the left
            }

            if (inPlayerRangeX && distance.Y > -range && distance.Y < 50 && lineOfSight && !player.dead) //checks if the leo is in range
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
				Burn();
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
            if (attacktype == 1) //burning
            {
                NPC.frameCounter++;

                if (attack <= 0) //getting ready
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
                else //BUUURRRRN
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

			float speed = MoveSpeedMultiplier * 1.5f * ConfusedMultiplier; //top speed
			float inertia = 10f; //acceleration and decceleration speed

			Vector2 direction = NPC.Center + new Vector2(NPC.direction *  50, 0) - NPC.Center; //start - end 
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
		private void Burn()
        {
			Player player = Main.player[NPC.target];
			Vector2 shotOrigin = NPC.Center + new Vector2(0, 10);
			float shootSpeed = 10 * RangeMultiplier;
            float shootAngle = NPC.spriteDirection == 1 ? 0 : MathF.PI;
            if (Main.expertMode)
			{
				shootAngle = Utils.GetChaseResults(shotOrigin, shootSpeed, player.Center, player.velocity).ChaserVelocity.ToRotation();

                if (NPC.spriteDirection >= 0)
				{
					if (shootAngle < -MathF.PI / 4f)	
						shootAngle = -MathF.PI / 4;
					if (shootAngle > MathF.PI / 4)
						shootAngle = MathF.PI / 4;
				}
				else
				{
					float maxAngle = 3f * MathF.PI / 4;
                    if (shootAngle > -maxAngle && shootAngle < 0)
						shootAngle = -maxAngle;
					if (shootAngle < maxAngle && shootAngle > 0)
						shootAngle = maxAngle;
				}
			}
			Vector2 projshoot = shootAngle.ToRotationVector2() * shootSpeed * ConfusedMultiplier; //  NPC.Center + new Vector2(NPC.direction * 10, 0) - NPC.Center; //straight ahead
	
			NPC.velocity.X *= 0.9f; //slow

			if (attack == -60) //point of where attack begins (for some reason)
			{
				NPC.TargetClosest(true); //face target
			}

            attack++; //goes up by 1 each tick (1/60 of a second)

			if (attack >= 0 & attack <= 60) //attack for 60 ticks
			{
				if (attack % 5 == 0) //every 5th tick
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						float spread = .4f;
						if(Main.expertMode)
						{
							spread = .25f;
						}
						Projectile.NewProjectile(NPC.GetSource_FromAI(), shotOrigin, projshoot.RotatedByRandom(spread),
						ModContent.ProjectileType<BadFire>(), 30 / 2, 1, Main.myPlayer, 0, 0);
					}
				}

				if (attack % 26 == 0) //every 26th tick
				{
                    SoundEngine.PlaySound(SoundID.Item34 with { MaxInstances = 0 }, NPC.Center); //flamethrower
                }
			}
			if (attack >= 120) //pause for 60 ticks
			{
				attack = -60; //cooldown for 30 ticks
				attacking = false; //can walk if out of range
			}
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Items.Weapons.Fire>(), 20, 10)); // 1 in 20 (5%) chance in Normal. 1 in 10 (10%) chance in Expert

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
