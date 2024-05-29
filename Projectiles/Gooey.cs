using Humanizer;
using KirboMod.Projectiles.Lightnings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class Gooey : ModProjectile
	{
		int attack = 0;
		int attacktype;
        bool flying = false; //checks if flying
		int dashcooldown = 0; //checks if can dash if below 0
		bool groundcollide;
        bool spaceJumping = false; //determines if gonna warp
        float spaceJumpRotation = 0; //here for sprite rotation of space jump
        bool attacking = false; //checks if in attacking state

        private List<float> Targetdistances = new List<float>(); //targeting
        private NPC aggroTarget = null; //target the minion is currently focused on
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Gooey");
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[Projectile.type] = 12;
			// This is necessary for right-click targeting
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

			// These below are needed for a minion
			// Denotes that this projectile is a pet or minion
			Main.projPet[Projectile.type] = true;
			// This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			// Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;

            //for space jump trail
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

		public sealed override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;
            DrawOriginOffsetY = -59; //plus 1 to touch the ground
            DrawOffsetX = -44;
			Projectile.tileCollide = true;
            Projectile.netImportant = true;

            Projectile.friendly = true;
			Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1f;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
		}

		// Here you can decide if your minion breaks things like grass or pots
		public override bool? CanCutTiles()
		{
			return false;
		}

		// This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
		public override bool MinionContactDamage()
		{
			if (attacktype == 1 && attack > 0) //if dashing
            {
				return true;
            }
			return false;
		}

		public override void AI()
		{
			Projectile.spriteDirection = Projectile.direction;
			Player player = Main.player[Projectile.owner];
			dashcooldown--; //go down

            // This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
            if (player.dead || !player.active)
			{
				player.ClearBuff(ModContent.BuffType<Buffs.MinionBuffs.GooeyBuff>());
			}
			if (player.HasBuff(ModContent.BuffType<Buffs.MinionBuffs.GooeyBuff>()))
			{
				Projectile.timeLeft = 2;
			}

			//Gravity
			if (flying == false && spaceJumping == false) //not flying OR space jumping
			{
                Projectile.velocity.Y += 0.7f;
                if (Projectile.velocity.Y >= 10f)
                {
                    Projectile.velocity.Y = 10f;
                }
            }

            float distanceFromTarget = 1200f;

            Vector2 IdlePosition = player.Center;
            float minionPositionOffsetX = (40 + Projectile.minionPos * 40) * -player.direction; //behind player depending on order summoned
            IdlePosition.X += minionPositionOffsetX;

            Vector2 vectorToIdlePosition = IdlePosition - Projectile.Center; //distance from idle
            float distanceToIdlePosition = vectorToIdlePosition.Length(); //aboslute distance from idle

            if (player.HasMinionAttackTargetNPC)
			{
				NPC npc = Main.npc[player.MinionAttackTargetNPC];
				float distance = Vector2.Distance(npc.Center, Projectile.Center);
				// Reasonable distance away so it doesn't target across multiple screens
				if (distance < distanceFromTarget)
				{
                    aggroTarget = npc;
                }
			}

            if (aggroTarget == null || !aggroTarget.active || aggroTarget.dontTakeDamage) //search target
            {
                //start each number with a very big number so they can't be targeted if their npc doesn't exist
                Targetdistances = Enumerable.Repeat(999999f, Main.maxNPCs).ToList(); 

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    float distance = Vector2.Distance(Projectile.Center, npc.Center);

                    if (npc.CanBeChasedBy()) //checks if targetable
                    {
						Vector2 positionOffset = new Vector2(0, -5);
                        bool inView = Collision.CanHitLine(Projectile.position + positionOffset, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);

                        //close, hittable, hostile and can see target
                        if (inView && !npc.friendly && !npc.dontTakeDamage && !npc.dontCountMe && distance < distanceFromTarget && npc.active && spaceJumping == false)
                        {
                            Targetdistances.Insert(npc.whoAmI, (int)distance); //add to list of potential targets
                        }
                    }

                    if (i == Main.maxNPCs - 1)
                    {
                        int theTarget = -1;

                        //count up 'til reached maximum distance
                        for (float j = 0; j < distanceFromTarget; j++) 
                        {
                            int Aha = Targetdistances.FindIndex(a => a == j); //count up 'til a target is found in that range

                            if (Aha > -1) //found target
                            {
                                theTarget = Aha;

                                break;
                            }
                        }

                        if (theTarget > -1) //exists
                        {
                            NPC npc2 = Main.npc[theTarget];

                            if (npc2 != null) //exists
                            {
                                aggroTarget = npc2;
                            }
                        }
                        else
                        {
                            break; //just in case
                        }
                    }
                }
            }
            if (attacking == true) //checks if attacking
            {
                Attack();
            }
            else if (aggroTarget != null && aggroTarget.active && !aggroTarget.dontTakeDamage) //ATTACK
            {
				Vector2 direction = aggroTarget.Center - Projectile.Center; //start - end
				Vector2 absDirection = new Vector2(Math.Abs(direction.X), Math.Abs(direction.Y));

                bool canSeeEnemy = Collision.CanHitLine(aggroTarget.position, aggroTarget.width, aggroTarget.height, Projectile.position, Projectile.width, Projectile.height);

				bool inEnemyRangeX = direction.X <= 80f && direction.X >= 0f; //in range of the right

				if (Projectile.direction == -1) //facing left
				{
                    inEnemyRangeX = direction.X >= -80f && direction.X <= 0f; //in range of the left
                }

                bool inEnemyRangeFireX = direction.X <= 250f && direction.X >= 0f; //in range of the right (fire)

                if (Projectile.direction == -1) //facing left
                {
                    inEnemyRangeFireX = direction.X >= -250f && direction.X <= 0f; //in range of the left (fire)
                }

                //attack (if close enough to target center, touching target hitbox or already attacking)
                if (inEnemyRangeX && absDirection.Y <= 20f & spaceJumping == false && flying == false) //in close range of enemy
				{
					if (attack == 0)
					{
						attacktype = 0; //lick
					}
                    attacking = true;
				}

                //if npc is above and attack cycle is over
                else if (absDirection.X < 50 && direction.Y <= -20 && direction.Y > -80 && spaceJumping == false && flying == false) 
				{
					if (attack == 0)
					{
						attacktype = 2; //umbrella
					}
                    attacking = true;
                }

                //if dash cooldown is up, in fire range while flying, and can see enemy
                else if (inEnemyRangeFireX && absDirection.Y <= 20 && flying == true && spaceJumping == false && dashcooldown <= 0 && canSeeEnemy) 
                {
                    if (attack == 0) //if attack cycle restarted
					{
						attacktype = 1; //fire
					}
                    attacking = true;
                }
				else 
                {
					attack = 0;

					if ((absDirection.X > 200f || absDirection.Y > 200f) && attack == 0) //fly if far away and if not attacking
					{
						flying = true;
					}

                    //stop flying when close, can see the target, and the target is on the ground
                    if (flying == true && absDirection.X < 200f && absDirection.Y < 50f && canSeeEnemy && aggroTarget.velocity.Y == 0) 
					{
						flying = false;
					}

					if (flying == true)
                    {
						if (attack == 0) //if not attacking
						{
							float speed = 11f;
							float inertia = 6f;

                            Projectile.tileCollide = false;

                            direction.Normalize();
                            direction *= speed;
							Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;  //fly towards enemy

							//flying animation
							Projectile.frameCounter++;
							if (Projectile.frameCounter >= 10)
							{
								if (Projectile.frame == 0)
								{
									Projectile.frame = 9;
									Projectile.frameCounter = 0;
								}
								else
								{
									Projectile.frame = 8;
									Projectile.frameCounter = 0;
								}
							}

							//shoot
							Projectile.ai[1]++; //timer
							if (Projectile.ai[1] % 20 == 0)
							{
                                direction.Normalize();
                                direction *= 32; //speed
                                LightningProj.GetSpawningStats(direction, out float ai0, out float ai1);
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, direction, 
                                    ModContent.ProjectileType<GooeyDarkMatterLaser>(), Projectile.damage * 2, 3, Projectile.owner, ai0, ai1); //fire good laser
                            }
						}
					}
					else //walk
					{

						if (attack == 0) //if not attacking
						{
							float speed = 7f;
							float inertia = 6f;
                            int pseudoDirection = 1;
                            if (direction.X < 0) //enemy is behind
                            {
                                pseudoDirection = -1; //change direction so it will go towards enemy
                            }
                            //we put this instead of player.Center so it will always be moving top speed instead of slowing down when enemy is near but unreachable
                            //A "carrot on a stick" if you will

                            Vector2 carrotDirection = new Vector2(pseudoDirection, 0); //start - end 
                            carrotDirection.Normalize();
                            carrotDirection *= speed;

                            //use .X so it only effects horizontal movement
                            Projectile.velocity.X = (Projectile.velocity.X * (inertia - 1) + carrotDirection.X) / inertia; //use .X so it only effects horizontal movement		

                            //jump when a little close and no attack cycle and when touching ground
                            if (direction.Y < -100 & groundcollide == true & attack == 0)
                            {
                                Projectile.velocity.Y = -10f; //jump
                                groundcollide = false; //wait to touch floor again
                            }

                            Projectile.tileCollide = true;

							//animation
							Projectile.frameCounter++; //walking
							if (Projectile.frameCounter >= 5)
							{
								if (Projectile.frame == 0)
								{
									Projectile.frame = 1;
									Projectile.frameCounter = 0;
								}
								else
								{
									Projectile.frame = 0;
									Projectile.frameCounter = 0;
								}
							}
						}

                        //for stepping up tiles
                        if (spaceJumping == false)
                        {
                            Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
                        }
                    }																	 									 				
				}
			}

            //FOLLOW PLAYER
            else
            {
				attack = 0;
                float distance = Vector2.Distance(player.Center, Projectile.Center);

                if (distance <= 1000f) //move within this range
				{
					if (distance > 300f) //fly if far away 
					{
						flying = true;
					}

					//reverse Y values because Y negative is up and positive is down in Terraria
					bool closeToPlayerY = Projectile.Center.Y - player.Center.Y < 0 && Projectile.Center.Y - player.Center.Y > -100f;
                    bool closeToPlayerX = Math.Abs(player.Center.X - Projectile.Center.X) < 300;
                    //close to player X, not below player, close to the top of the player and player is not in air
                    if (flying == true && player.velocity.Y == 0 && closeToPlayerX && closeToPlayerY) //stop flying after 
                    {
                        flying = false;
                    }

                    if (flying == true)
					{
						float speed = 11f;
						float inertia = 6f;

						Projectile.tileCollide = false;

                        if (distance <= 60) //close to player
						{
							Projectile.velocity = Projectile.velocity;
						}
						else
						{
							Vector2 direction = player.Center - Projectile.Center; //start - end
							direction.Normalize();
							direction *= speed;
							Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;  //fly towards player
						}

						//flying animation
						Projectile.frameCounter++;
						if (Projectile.frameCounter >= 10)
						{
                            if (Projectile.frame == 8)
							{
								Projectile.frame = 9;
								Projectile.frameCounter = 0;
							}
							else if (Projectile.frame == 9)
                            {
								Projectile.frame = 10;
								Projectile.frameCounter = 0;
							}
                            else if (Projectile.frame == 10)
                            {
                                Projectile.frame = 11;
                                Projectile.frameCounter = 0;
                            }
                            else
                            {
                                Projectile.frame = 8;
                                Projectile.frameCounter = 0;
                            }
                        }
					}
					else //walk
					{
						float speed = 7f;
						float inertia = 6f;

						Projectile.tileCollide = true;

                        if (Math.Abs(vectorToIdlePosition.X) < 10f) //near idle position X
                        {
                            Projectile.velocity.X *= 0.8f; //slow
                        }
                        else //walk
						{
                            Vector2 direction = IdlePosition - Projectile.Center; //start - end

                            int pseudoDirection = 1;
                            if (direction.X < 0) //player is behind
                            {
                                pseudoDirection = -1; //change direction so it will go towards enemy
                            }
                            //we put this instead of player.Center so it will always be moving top speed instead of slowing down when enemy is near but unreachable
                            //A "carrot on a stick" if you will

                            Vector2 carrotDirection = new Vector2(pseudoDirection, 0); //start - end 
                            carrotDirection.Normalize();
                            carrotDirection *= speed;

                            //use .X so it only effects horizontal movement
                            Projectile.velocity.X = (Projectile.velocity.X * (inertia - 1) + carrotDirection.X) / inertia;
                        }

                        Projectile.frameCounter++; //walk cycle
						if (Projectile.frameCounter == 15)
						{
							if (Projectile.frame == 0)
							{
								Projectile.frame = 1; //bob down
								Projectile.frameCounter = 0;
							}
							else
							{
								Projectile.frame = 0; //bob up
								Projectile.frameCounter = 0;
							}
						}

                        //for stepping up tiles
                        if (spaceJumping == false)
                        {
                            Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
                        }
                    }
				}
				else //teleport
				{
                    spaceJumping = true;
                }
            }

            Vector2 direction2 = player.Center - Projectile.Center; //start - end

            if (spaceJumping == true) //if space jumping
            {
                Projectile.tileCollide = false;
                Projectile.ignoreWater = true;
                flying = false; // hold till not space jumping
                Projectile.alpha = 255; //hide projectile
                
                float speed = Math.Clamp(direction2.Length() / 30, 20f, float.MaxValue);
                Projectile.extraUpdates = 3; //run three extra ticks for space jump

                //fly toward player
                Projectile.velocity = Projectile.DirectionTo(player.Center) * speed;
            }
            else
            {
                Projectile.ignoreWater = false;
                Projectile.alpha = 0; //show projectile
                Projectile.extraUpdates = 0;
            }

            //space jump end
            if (direction2.Length() <= 20f && spaceJumping == true)
            {
                for (int i = 0; i < 20; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                    Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, DustID.Enchanted_Gold, speed, Scale: 1f); //Makes dust in a messy circle
                }
                for (int i = 0; i < 10; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                    Gore.NewGorePerfect(Projectile.GetSource_FromAI(), Projectile.Center, speed, Main.rand.Next(16, 18));
                }
                Projectile.velocity *= 0;
                SoundEngine.PlaySound(SoundID.Item10, Projectile.position); //impact
                spaceJumping = false;
            }
        }

		private void Attack()
        {
            Player player = Main.player[Projectile.owner];
			Projectile.tileCollide = true;
			
            if (attacktype == 0) //tounge
			{
				attack++;
				Projectile.velocity.X *= 0.8f;

				//attack 2 times
				if (attack == 3)
				{
					Projectile.frame = 2; //attacks
					Vector2 xoffset = new Vector2(Projectile.direction * 25, 0);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + xoffset, Projectile.velocity * 0, ModContent.ProjectileType<GooeyHitbox>(), Projectile.damage, 4, player.whoAmI);
				}
				if (attack == 6)
				{
					Projectile.frame = 3;
					Vector2 xoffset = new Vector2(Projectile.direction * 25, 0);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + xoffset, Projectile.velocity * 0, ModContent.ProjectileType<GooeyHitbox>(), Projectile.damage, 4, player.whoAmI);
				}
				if (attack > 8)
				{
                    attacking = false;
                    attack = 0;
				}
			}

			if (attacktype == 1) //fire dash
			{
				attack++;

                if (attack >= 1) //during attack
                {
                    Projectile.direction = Projectile.direction; //keep same direction
                    Projectile.velocity.X = Projectile.direction * 30;
                    Projectile.velocity.Y *= 0.01f;
                    Projectile.tileCollide = false;

                    Projectile.frame = 4; //fire
                    Lighting.AddLight(Projectile.Center, 0f, 0.5f, 0.8f); //light blue light
                    Dust.NewDust(Projectile.position, 32, 32, DustID.BlueTorch, 0f, 0f, 200, default, 1f);;

                    if (attack == 1)
                    {
                        Projectile.damage *= 3; //triple for fire attack
                    }

                    if (attack >= 15) //reset
                    {
                        Projectile.damage /= 3; //third it to return to normal
                        attacking = false;
						dashcooldown = 60;
                        attack = 0;
                    }
                }
            }

			if (attacktype == 2) //umbrella
            {
				attack++;
				Projectile.velocity.X *= 0.2f;

				if (attack == 1) //rise umbrella
				{
					Projectile.frame = 6; 
				}
				else if (attack >= 5 && attack % 5 == 0) //attack with umbrella (every 5th tick)
				{
					Projectile.frame = 7;
					Vector2 offset = new Vector2(Projectile.direction * 10, -50); //go up and back a bit from origin
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + offset, Projectile.velocity * 0, ModContent.ProjectileType<GooeyHitbox>(), Projectile.damage / 2, 2, player.whoAmI);
				}
				if (attack >= 30) //reset
                {
                    attacking = false;
                    attack = 0;
                }
			}
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			if (flying == false)
			{
				if (Projectile.oldVelocity.Y > 0f) //if was falling
				{
					groundcollide = true;
				}
			}
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (attacktype == 1 & attack != 0) //fire dash
			{
				target.AddBuff(BuffID.OnFire, 300);
			}
		}

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
			Player player = Main.player[Projectile.owner];

			if (aggroTarget != null && aggroTarget.active && !aggroTarget.dontTakeDamage) //fall to enemy
			{
				Vector2 toTarget = aggroTarget.Center - Projectile.Center;
				// Here we check if the NPC is below the minion and 300/16 = 18.25 tiles away horizontally
				if (toTarget.Y > 10 && Math.Abs(toTarget.X) < 300)
				{
					fallThrough = true;
				}
				else
				{
					fallThrough = false;
				}
			}
			else //fall to player
			{
                Vector2 toPlayer = player.Center - Projectile.Center;
				if (flying == true)
				{
					fallThrough = true;
				}
				else
                {
					if (toPlayer.Y > 10 && Math.Abs(toPlayer.X) < 300)
					{
						fallThrough = true;
					}
					else
					{
						fallThrough = false;
					}
				}
            }
			return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
		}

        public override Color? GetAlpha(Color lightColor)
        {
            if (attack > 0 && attacktype == 1) //using fire dash
			{
				return Color.White; //unaffected by light
			}
			else
			{
				return default; //return vanilla lighting
			}
        }

        //DRAWING SPACE JUMP

        public static Asset<Texture2D> JumpStar;

        public override bool PreDraw(ref Color lightColor)
        {
            if (spaceJumping == true)
            {
                Main.instance.LoadProjectile(Projectile.type);
                JumpStar = ModContent.Request<Texture2D>("KirboMod/Projectiles/TripleStarStar");
                Texture2D texture = JumpStar.Value;

                if (!Main.gamePaused)
                {
                    spaceJumpRotation--;
                }

                Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
                Vector2 drawPos = Projectile.position - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);

                Main.EntitySpriteDraw(texture, drawPos, null, Color.White, spaceJumpRotation, drawOrigin, 1, SpriteEffects.None, 0);

                int dustIndex = Dust.NewDust(Projectile.position, 50, 50, DustID.BlueTorch, Scale: 2f); //dust
                Main.dust[dustIndex].velocity *= 0.2f;
                Main.dust[dustIndex].noGravity = true;

                //change texture for afterimages
                JumpStar = ModContent.Request<Texture2D>("KirboMod/Projectiles/TripleStarStarAfterimage");
                texture = JumpStar.Value;

                for (int k = 1; k < Projectile.oldPos.Length; k++) //start at 1 so no ontop of actual star
                {
                    Vector2 drawOrigin2 = new Vector2(texture.Width / 2, texture.Height / 2);
                    Vector2 drawPos2 = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);

                    Color color = Color.DodgerBlue * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, drawPos2, null, color, spaceJumpRotation, drawOrigin2, 1, SpriteEffects.None, 0);
                }

                return false;
            }
            else
            {
                return true;
            }
        }
    }
}