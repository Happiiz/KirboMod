using KirboMod.Buffs.MinionBuffs;
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
	public class BurningLeoMinion : ModProjectile
	{
		int attack = 0;
		int jumpTimer = 0;
        public bool attacking = false; //checks if in attacking state
        bool spaceJumping = false; //determines if gonna warp
        float spaceJumpRotation = 0; //here for sprite rotation of space jump

        private List<float> Targetdistances = new List<float>(); //targeting
        public NPC aggroTarget = null; //target the minion is currently focused on

        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Burning Leo");
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[Projectile.type] = 9;
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

		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;
            DrawOriginOffsetY = -20;
            DrawOffsetX = -8;
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

		// This is mandatory if your minion deals contact damage
		public override bool MinionContactDamage()
		{
			return false;
        }

        //we make this so we can reference it in duo burning leo 
        public virtual string Buff => "BurningLeoBuff"; 

        public override void AI()
		{
			jumpTimer--;
			Projectile.spriteDirection = Projectile.direction;
			Player player = Main.player[Projectile.owner];

			// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
			if (player.dead || !player.active)
			{
				player.ClearBuff(Mod.Find<ModBuff>(Buff).Type);
			}
			if (player.HasBuff(Mod.Find<ModBuff>(Buff).Type))
			{
				Projectile.timeLeft = 2;
			}

            //Gravity
            if (spaceJumping == false)
            {
                Projectile.velocity.Y += 0.7f;

                if (attacking == false) //not attacking
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

            float distanceFromTarget = 1000f;

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

                if (direction.Length() < 120f & jumpTimer <= 0 & spaceJumping == false) //attack
				{
                    attacking = true;
                }
				else 
                {
					if (direction.Y <= -100f & jumpTimer <= 0) //jump when below enemy and can jump again
					{
                        Jump();
                    }

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
                    Projectile.velocity.X = (Projectile.velocity.X * (inertia - 1) + carrotDirection.X) / inertia;
                    
                    //animation
					Projectile.frameCounter++; //walking
					if (Projectile.frameCounter >= 5) //if this many frames pass
					{
                        if (Projectile.frame == 0) //if this frame
                        {
                            Projectile.frame = 1;
                            Projectile.frameCounter = 0;
                        }
                        else if (Projectile.frame == 1)
                        {
                            Projectile.frame = 2;
                            Projectile.frameCounter = 0;
                        }
                        else if (Projectile.frame == 2)
                        {
                            Projectile.frame = 3;
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
				if (Projectile.velocity.X <= 0.1f && Projectile.velocity.X >= -0.1f) //barely moving
				{
					Projectile.frame = 1; //stand still (closest to being so)
				}
				else //walk cycle
				{
					Projectile.frameCounter++;
					if (Projectile.frameCounter >= 5) //if this many frames pass
					{
						if (Projectile.frame == 0) //if this frame
						{
							Projectile.frame = 1;
							Projectile.frameCounter = 0;
						}
						else if (Projectile.frame == 1)
                        {
							Projectile.frame = 2;
							Projectile.frameCounter = 0;
						}
                        else if (Projectile.frame == 2)
                        {
                            Projectile.frame = 3;
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
                    float speed = 7f; //walk speed
                    float inertia = 6f; //turn speed
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

                float speed = direction2.Length() / 30;
                if (speed < 100) //don't go below 100
                {
                    speed = 100;
                }
                float inertia = 6f;

                Vector2 direction = player.Center - Projectile.Center; //start - end
                direction.Normalize();
                direction *= speed;
                Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;  //fly towards player
            }
            else
            {
                Projectile.tileCollide = true;
                Projectile.ignoreWater = false;
                Projectile.alpha = 0; //show projectile
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

            if (jumpTimer > 0) //if jumping
            {
				Projectile.frame = 8; //jump frame
            }
		}


        public virtual float fireType => 0; //determines if using prehardmode fire or hardmode fire


        public void Attack()
        {
			Projectile.velocity.X *= 0.8f;

			Vector2 direction = aggroTarget.Center - Projectile.Center; //start - end

            if (direction.X >= 0)
            {
                Projectile.direction = 1;
            }
            else
            {
                Projectile.direction = -1;
            }
            Projectile.spriteDirection = Projectile.direction;

            if (attack >= 20) //if over the lifespan of projectile
            {
                float maxLength = 180;

                if (fireType == 1)
                {
                    maxLength = 360;
                }

                if (direction.Length() > maxLength || !aggroTarget.active)
                {
                    attacking = false;
                    attack = 0;
                }
            }

            direction.Normalize();
            direction *= 10f;

            attack++;
			if (attack % 20 == 0)
			{
                Player player = Main.player[Projectile.owner];
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, direction.RotatedByRandom(MathHelper.ToRadians(50)),
                    ModContent.ProjectileType<MinionFireSpread>(), Projectile.damage, 1, player.whoAmI, Projectile.whoAmI, fireType);
            }

            //animation for burning
            Projectile.frameCounter++;
			if (Projectile.frameCounter >= 2)
			{
				if (Projectile.frame == 7) //if this frame
				{
					Projectile.frame = 6;
					Projectile.frameCounter = 0;
				}
				else
				{
					Projectile.frame = 7;
					Projectile.frameCounter = 0;
				}
			}
		}
        private void Jump()
        {
            Projectile.velocity.Y = -10f; //velocityY boosts up 
            jumpTimer = 15;
            Projectile.frame = 8;
        }

        //all of this for falling through tiles
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