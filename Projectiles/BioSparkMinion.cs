using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;

namespace KirboMod.Projectiles
{
	public class BioSparkMinion : ModProjectile
	{
		int attack = 0;
		int attacktype;
		int jumpTimer = 0;
		int daggerCoolDown = 0; 
        bool attacking = false; //checks if in attacking state
        bool spaceJumping = false; //determines if gonna warp
        float spaceJumpRotation = 0; //here for sprite rotation of space jump

        private List<float> Targetdistances = new List<float>(); //targeting
        private NPC aggroTarget = null; //target the minion is currently focused on
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Bio Spark");
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[Projectile.type] = 17;
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
			DrawOriginOffsetY = -14;
            DrawOffsetX = -32;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;

            Projectile.friendly = true;
			Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1f;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 24;
		}

		// Here you can decide if your minion breaks things like grass or pots
		public override bool? CanCutTiles()
		{
			return false;
		}

		// This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
		public override bool MinionContactDamage()
		{
			return false;
		}

		public override void AI()
		{
			//continously go down
			jumpTimer--;
			daggerCoolDown--;

			Projectile.spriteDirection = Projectile.direction;
			Player player = Main.player[Projectile.owner];

			// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
			if (player.dead || !player.active)
			{
				player.ClearBuff(ModContent.BuffType<Buffs.MinionBuffs.BioSparkBuff>());
			}
			if (player.HasBuff(ModContent.BuffType<Buffs.MinionBuffs.BioSparkBuff>()))
			{
				Projectile.timeLeft = 2;
			}

            //Gravity
            if (spaceJumping == false)
            {
                Projectile.velocity.Y += 0.7f;

                if (attack <= 0) //not attacking
                {
                    if (Projectile.velocity.Y >= 10f)
                    {
                        Projectile.velocity.Y = 10f;
                    }
                }
                else //fall slower
                {
                    if (Projectile.velocity.Y >= 1f)
                    {
                        Projectile.velocity.Y = 1f;
                    }
                }
            }

            //for stepping up tiles
            if (spaceJumping == false)
            {
                Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
            }

            //Important stuff for targeting
			float distanceFromTarget = 1200f;

            Vector2 IdlePosition = player.Center;
            float minionPositionOffsetX = (40 + Projectile.minionPos * 40) * -player.direction; //behind player depending on order summoned
            IdlePosition.X += minionPositionOffsetX;

            Vector2 vectorToIdlePosition = IdlePosition - Projectile.Center; //distance from idle
            float distanceToIdlePosition = vectorToIdlePosition.Length(); //aboslute distance from idle

            //player selected targeting
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

                bool inEnemyRangeX = direction.X <= 400f && direction.X >= 0f; //in range of the right

                if (Projectile.direction == -1) //facing left
                {
                    inEnemyRangeX = direction.X >= -400f && direction.X <= 0f; //in range of the left
                }

                //attack (if close enough to target center, touching target hitbox or already attacking)

                if ((inEnemyRangeX && absDirection.Y <= 50 || 
					aggroTarget.Hitbox.Intersects(Projectile.Hitbox)) && spaceJumping == false) //attack if within range or intersecting hitbox
				{
					if (attack == 0) //if attack cycle restarted
					{
						attacktype = 0;
					}
                    attacking = true;
                }
                //attack if cooldown is done and within range
                else if ((absDirection.X <= 400 || absDirection.Y <= 400) && daggerCoolDown <= 0 && spaceJumping == false) 
                {
					if (attack == 0)
					{
						attacktype = 1;
					}
                    attacking = true;
                }
				else 
                {
					if (direction.Y <= -50f & jumpTimer <= 0 & attack == 0) //jump when below enemy, can jump again and not attacking
					{
                        Jump();
                    }

					//walking
					float speed = 7f; //walk speed
					float inertia = 6f; //turn speed
                    int pseudoDirection = 1;
                    if (direction.X < 0) //enemy is behind
                    {
                        pseudoDirection = -1; //change direction so it will go towards enemy
                    }
                    //we put this instead of player.Center so it will always be moving top speed instead of slowing down when enemy is near but unreachable
                    //A "carrot on a stick" if you will

                    Vector2 carrotDirection = Projectile.Center + new Vector2(pseudoDirection * 50, 0) - Projectile.Center; //start - end 
                    carrotDirection.Normalize();
                    carrotDirection *= speed;

                    //use .X so it only effects horizontal movement
                    Projectile.velocity.X = (Projectile.velocity.X * (inertia - 1) + carrotDirection.X) / inertia; //use .X so it only effects horizontal movement		

                    Projectile.frameCounter++; //walking
					if (Projectile.frameCounter >= 5)
					{
                        if (Projectile.frame < 3) //less than 4th frame
						{
							Projectile.frame++; //go up
							Projectile.frameCounter = 0;
						}
                        else
						{
							Projectile.frame = 0;
							Projectile.frameCounter = 0;
						}
					}
				}
			}
			else //FOLLOW PLAYER
			{
				if (Projectile.velocity.X <= 0.1f & Projectile.velocity.X >= -0.1f) //barely moving
				{
					Projectile.frame = 3; //stand still frame
                    Projectile.frameCounter = 0; //reset
                }
				else //walk cycle
				{
                    Projectile.frameCounter++; //walking
                    if (Projectile.frameCounter >= 5)
                    {
                        if (Projectile.frame < 3) //less than 4th frame
                        {
                            Projectile.frame++; //go up
                            Projectile.frameCounter = 0;
                        }
                        else
                        {
                            Projectile.frame = 0;
                            Projectile.frameCounter = 0;
                        }
                    }
                }

				if (vectorToIdlePosition.Y <= -50f & jumpTimer <= 0 && spaceJumping == false) //jump (lower distance when following player)
				{
                    Jump();
                }

				if (Math.Abs(vectorToIdlePosition.X) < 10f) //near idle position
				{
					Projectile.velocity.X *= 0.8f; //slow
				}
				else if (distanceToIdlePosition <= 1000f) //walk within a certain range
				{
					float speed = 7f;
					float inertia = 6f;
                    Vector2 direction = IdlePosition - Projectile.Center; //start - end
                    int pseudoDirection = 1;
                    if (direction.X < 0) //enemy is behind
                    {
                        pseudoDirection = -1; //change direction so it will go towards enemy
                    }
                    //we put this instead of player.Center so it will always be moving top speed instead of slowing down when enemy is near but unreachable
                    //A "carrot on a stick" if you will

                    Vector2 carrotDirection = Projectile.Center + new Vector2(pseudoDirection * 50, 0) - Projectile.Center; //start - end 
                    carrotDirection.Normalize();
                    carrotDirection *= speed;

                    //use .X so it only effects horizontal movement
                    Projectile.velocity.X = (Projectile.velocity.X * (inertia - 1) + carrotDirection.X) / inertia;
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
                jumpTimer = 1; // hold till not space jumping
                Projectile.alpha = 255; //hide projectile

                float speed = Math.Clamp(direction2.Length() / 30, 20f, float.MaxValue);
                Projectile.extraUpdates = 3; //run three extra ticks for space jump

                //fly toward player
                Projectile.velocity = Projectile.DirectionTo(player.Center) * speed;
            }
            else
            {
                Projectile.tileCollide = true;
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

            if (jumpTimer > 0 && attack <= 0) //jump frame time
			{
				Projectile.frame = 16; //jump frame
            }
		}

		private void Attack()
		{
            Player player = Main.player[Projectile.owner];
            //SLASH
            if (attacktype == 0)
			{
				attack++; //starts at 1
				if (attack == 1) //inital slash
				{
					Projectile.frame = 7; //slash frame
					Projectile.velocity.X = 40 * Projectile.direction; //20 times 1(right) or -1(left)
					Projectile.velocity.Y = 0;
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BioMinionSlashHitbox>(), Projectile.damage * 3, 8, player.whoAmI, Projectile.whoAmI);
					SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
				}
				else if (attack > 1) //slow down
				{
					Projectile.velocity.X *= 0.92f;
                    Projectile.velocity.Y = 0;
                }

                if (attack >= 1) //slash animation
                {
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter >= 4)
                    {
                        if (Projectile.frame < 15) //less than 16th frame
                        {
                            Projectile.frame++; //go up
                            Projectile.frameCounter = 0;
                        }
                    }
                }

				if (attack >= 21) //reset
				{
					attacking = false;
					attack = 0;
				}
			}

			if (attacktype == 1) //Kunai flurry
			{
			    attack++;

				Projectile.velocity.X *= 0.8f;
				Projectile.frame = 4; //ready kunai frame

                if (attack > 10 && attack < 20)
                {
                    if (attack % 3 == 0) //remiander is 0 (multiple of 3)
                    {
                        //set direction for each kunai (also slightly predict movement)
                        Vector2 Kunaidirection = aggroTarget.Center + aggroTarget.velocity - Projectile.Center;
                        Kunaidirection.Normalize();
                        Kunaidirection *= 20f;

                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Kunaidirection, ModContent.ProjectileType<GoodBioDagger>(), 
                            Projectile.damage, 4, player.whoAmI); //not dividing damage due to expert scaling btw
						SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
					}

                    Projectile.frame = 5; //throw kunai frame
                }

				if (attack >= 40) //reset
                {
                    attacking = false;
                    daggerCoolDown = 120;
					attack = 0;
                }
			}
		}

        private void Jump()
        {
            Projectile.velocity.Y = -10f; //velocityY boosts up 
            jumpTimer = 15;
            Projectile.frame = 16; //jump frame
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

				if (toPlayer.Y > 10 && Math.Abs(toPlayer.X) < 300)
				{
					fallThrough = true;
				}
				else
				{
					fallThrough = false;
				}
			}
			return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
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