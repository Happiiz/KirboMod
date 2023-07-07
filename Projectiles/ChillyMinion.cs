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
	public class ChillyMinion : ModProjectile
	{
		int attack = 0;
		bool attacking = false; //checks if in attacking state
		int jumpTimer = 0;
        bool spaceJumping = false; //determines if gonna warp
        float spaceJumpRotation = 0; //here for sprite rotation of space jump

        private List<float> Targetdistances = new List<float>(); //targeting
        private NPC aggroTarget = null; //target the minion is currently focused on
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Chilly");
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

		public sealed override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;
            DrawOriginOffsetY = -8; 
            //checks if minion can collide with tiles
            Projectile.tileCollide = true;

			// These below are needed for a minion weapon
			// Only controls if it deals damage to enemies on contact (more on that later)
			Projectile.friendly = true;
			// Only determines the damage type
			Projectile.minion = true;
			// Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			Projectile.minionSlots = 1f;
			// Needed so the minion doesn't despawn on collision with enemies or tiles
			Projectile.penetrate = -1;
			// local immunity makes it wait for it's own cooldown
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
			jumpTimer--;
			Projectile.spriteDirection = Projectile.direction;
			Player player = Main.player[Projectile.owner];

			// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
			if (player.dead || !player.active)
			{
				player.ClearBuff(ModContent.BuffType<Buffs.MinionBuffs.ChillyBuff>());
			}
			if (player.HasBuff(ModContent.BuffType<Buffs.MinionBuffs.ChillyBuff>()))
			{
				Projectile.timeLeft = 2;
			}

            //Gravity
            if (spaceJumping == false)
            {
                if (attack <= 0) //not attacking
                {
                    Projectile.velocity.Y += 0.7f;
                    if (Projectile.velocity.Y >= 10f)
                    {
                        Projectile.velocity.Y = 10f;
                    }
                }
                else //fall slowly at a constant rate 
                {
                    Projectile.velocity.Y = 1f;
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
                Vector2 absDirection = new Vector2(Math.Abs(direction.X), Math.Abs(direction.Y));

                if (((absDirection.X < 60f && absDirection.Y < 60f) || aggroTarget.Hitbox.Intersects(Projectile.Hitbox)) 
					&& jumpTimer <= 0 & spaceJumping == false) //attack when criteria met
				{
					attacking = true;
				}
				else
				{
					//attack = 0;

					if (direction.Y <= -100f & jumpTimer <= 0) //jump when below enemy and can jump again
					{
						Projectile.velocity.Y = -10f; //velocityY boosts up when attacking enemy
						jumpTimer = 25;
						Projectile.frame = 8;
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
					Projectile.frame = 1; //stand still
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

				if (vectorToIdlePosition.Y <= -50f & jumpTimer <= 0 && spaceJumping == false) //jump
				{
						Projectile.velocity.Y = -10f; //velocityY boosts up when following player
						jumpTimer = 25;
						Projectile.frame = 8;
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
                if (speed < 40) //don't go below 40
                {
                    speed = 40;
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
                for (int i = 0; i < 10; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
                    Dust d = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, ModContent.DustType<Dusts.LilStar>(), speed * 6, Scale: 1f); //Makes dust in a messy circle
                }
                Projectile.velocity *= 0;
                SoundEngine.PlaySound(SoundID.Item10, Projectile.position); //impact
                spaceJumping = false;
            }

            if (jumpTimer > 0 && attack == 0) //if jumping and not attacking
            {
				Projectile.frame = 8; //jump frame
            }
		}

		private void Attack()
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

			Vector2 projshoot = aggroTarget.Center - Projectile.Center;
			projshoot.Normalize();
			projshoot *= 10f;

            attack++;

			if (attack == 1)
			{
                Player player = Main.player[Projectile.owner];
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0.01f * Projectile.direction, 1), 
					Mod.Find<ModProjectile>("ChillyMinionFreeze").Type, Projectile.damage, 1, player.whoAmI, 0, Projectile.whoAmI);
			}
			if (attack >= 10) //reset
			{
				attack = 0;
				attacking = false;
			}

            //animation for freezing
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

        public override void PostDraw(Color lightColor)
        {
            if (spaceJumping == true)
            {
                JumpStar = ModContent.Request<Texture2D>("KirboMod/Projectiles/Star");
                Texture2D texture = JumpStar.Value;

                spaceJumpRotation--;

                Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
                Vector2 drawPos = (Projectile.position - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);

                Main.EntitySpriteDraw(texture, drawPos, null,
                    Color.White, spaceJumpRotation, drawOrigin, 1, SpriteEffects.None, 1);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (spaceJumping == true)
            {
                Main.instance.LoadProjectile(Projectile.type);
                JumpStar = ModContent.Request<Texture2D>("KirboMod/Projectiles/Star");
                Texture2D texture = JumpStar.Value;

                for (int k = 1; k < Projectile.oldPos.Length; k++) //rotation already going down btw (also start at 1 so no ontop of actual star)
                {
                    Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);

                    Color color = Color.White * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, drawPos, null, color, spaceJumpRotation, drawOrigin, 1, SpriteEffects.None, 0);
                }
            }
            return true;
        }
    }
}