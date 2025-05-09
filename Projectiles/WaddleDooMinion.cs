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
	public class WaddleDooMinion : ModProjectile
	{
		ref float Attack => ref Projectile.ai[0];
        ref float JumpTimer => ref Projectile.ai[1];
        public static int AttackDuration => 25;
        static float Speed => 15;
        static float Inertia => 3;
        static float BeamRangeMult => 10f;
        public bool attacking = false; //checks if in attacking state
        bool spaceJumping = false; //determines if gonna warp
        float spaceJumpRotation = 0; //here for sprite rotation of space jump

        private List<float> Targetdistances = new List<float>(); //targeting
        public NPC aggroTarget = null; //target the minion is currently focused on

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Waddle Doo");
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[Projectile.type] = 10;
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
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

		public sealed override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;
			DrawOriginOffsetY = -8; 
            DrawOffsetX = -4;
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

		public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction; //face direction it's going

            JumpTimer--;

            Player player = Main.player[Projectile.owner];

			// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
			if (player.dead || !player.active)
			{
				player.ClearBuff(ModContent.BuffType<Buffs.MinionBuffs.WaddleDooBuff>());
			}
			if (player.HasBuff(ModContent.BuffType<Buffs.MinionBuffs.WaddleDooBuff>()))
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
                    if (Projectile.velocity.Y >= Speed)
                    {
                        Projectile.velocity.Y = Speed;
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

            if (aggroTarget == null || !aggroTarget.CanBeChasedBy()) //search target
            {
                aggroTarget = null;
                int targetIndex = -1;
                Projectile.Minion_FindTargetInRange(1500, ref targetIndex, true, null);
                attacking = false;
                if(targetIndex >= 0 && Main.maxNPCs >= targetIndex)
                {
                    aggroTarget = Main.npc[targetIndex];
                    attacking = true;
                }
            }

            if (aggroTarget != null && aggroTarget.CanBeChasedBy()) //ATTACK
			{
                Vector2 direction = aggroTarget.Center - Projectile.Center; //start - end

                if (direction.Length() < 30 * BeamRangeMult) //attack if close enough and not currently attacking
				{
                    attacking = true; //start beamin 
				}
				//else 
                {
					//Attack = 0;

                    if (direction.Y <= -100f && JumpTimer <= 0 && spaceJumping == false) //jump when below enemy and can jump again
                    {
                        Jump();
                    }

                    float speed = Speed; //walk speed
                    float inertia = Inertia; //turn speed
                    int pseudoDirection = 1;
                    if (direction.X < 0) //enemy is behind
                    {
                        pseudoDirection = -1; //change direction so it will go towards enemy
                    }
                    //we put this instead of player.Center so it will always be moving top speed instead of slowing down when enemy is near but unreachable
                    //A "carrot on a stick" if you will

                    Vector2 carrotDirection = new Vector2(pseudoDirection * speed, 0); //start - end 

                    if (MathF.Abs(direction.X) > 100)
                    {
                        //use .X so it only effects horizontal movement
                        Projectile.velocity.X = (Projectile.velocity.X * (inertia - 1) + carrotDirection.X) / inertia; //use .X so it only effects horizontal movement
                    }
                    else
                    {
                        Projectile.velocity.X *= 0.9f;
                    }
                    //animation
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter < 5.0)
                    {
                        Projectile.frame = 0;
                    }
                    else if (Projectile.frameCounter < 10.0)
                    {
                        Projectile.frame = 1;
                    }
                    else if (Projectile.frameCounter < 15.0)
                    {
                        Projectile.frame = 2;
                    }
                    else if (Projectile.frameCounter < 20.0)
                    {
                        Projectile.frame = 3;
                    }
                    else if (Projectile.frameCounter < 25.0)
                    {
                        Projectile.frame = 4;
                    }
                    else if (Projectile.frameCounter < 30.0)
                    {
                        Projectile.frame = 5;
                    }
                    else if (Projectile.frameCounter < 35.0)
                    {
                        Projectile.frame = 6;
                    }
                    else if (Projectile.frameCounter < 40.0)
                    {
                        Projectile.frame = 7;
                    }
                    else
                    {
                        Projectile.frameCounter = 0;
                    }
                }
                if (attacking == true) //checks if attacking
                {
                    AI_Attack();
                }
            }
			else //FOLLOW PLAYER
			{
                if (Projectile.velocity.X <= 0.1f & Projectile.velocity.X >= -0.1f) //barely moving
				{
					Projectile.frame = 1; //stand still
				}
				else //walk cycle
				{
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter < 5.0)
                    {
                        Projectile.frame = 0;
                    }
                    else if (Projectile.frameCounter < 10.0)
                    {
                        Projectile.frame = 1;
                    }
                    else if (Projectile.frameCounter < 15.0)
                    {
                        Projectile.frame = 2;
                    }
                    else if (Projectile.frameCounter < 20.0)
                    {
                        Projectile.frame = 3;
                    }
                    else if (Projectile.frameCounter < 25.0)
                    {
                        Projectile.frame = 4;
                    }
                    else if (Projectile.frameCounter < 30.0)
                    {
                        Projectile.frame = 5;
                    }
                    else if (Projectile.frameCounter < 35.0)
                    {
                        Projectile.frame = 6;
                    }
                    else if (Projectile.frameCounter < 40.0)
                    {
                        Projectile.frame = 7;
                    }
                    else
                    {
                        Projectile.frameCounter = 0;
                    }
                }

                Attack = 0;

				if (vectorToIdlePosition.Y <= -50f & JumpTimer <= 0 && spaceJumping == false) //jump (lower distance when following player)
                {
                    Jump();
                }
                float speed = Speed; //walk speed
                float inertia = Inertia; //turn speed

                if (Math.Abs(vectorToIdlePosition.X) < 10f) //near idle position
				{
					Projectile.velocity.X *= 0.8f; //slow
				}
				else if (distanceToIdlePosition <= 1000f) //walk within a certain range
				{
                    Vector2 direction = IdlePosition - Projectile.Center; //start - end

                    int pseudoDirection = 1;
                    if (direction.X < 0) //enemy is behind
                    {
                        pseudoDirection = -1; //change direction so it will go towards enemy
                    }
                    //we put this instead of player.Center so it will always be moving top speed instead of slowing down when enemy is near but unreachable
                    //A "carrot on a stick" if you will

                    Vector2 carrotDirection = new Vector2(pseudoDirection * speed, 0); 

                    //use .X so it only effects horizontal movement
                    Projectile.velocity.X = (Projectile.velocity.X * (inertia - 1) + carrotDirection.X) / inertia; //use .X so it only effects horizontal movement
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
                JumpTimer = 1; // hold till not space jumping
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

            if (JumpTimer > 0 && Attack == 0) //if jumping and not attacking
			{
				Projectile.frame = 9; //jump frame
            }
		}
        void SocialDistancing()
        {

            int owner = Projectile.owner;
            int type = Projectile.type;
            int index = Projectile.whoAmI;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if(proj.active && proj.owner == owner && type == proj.type && proj.whoAmI != index && proj.Distance(Projectile.Center) < 32f)
                {
                    proj.velocity -= proj.DirectionTo(Projectile.Center)*3;
                }
            }
        }
		private void AI_Attack()
        {
            if (aggroTarget == null)
                return;
            SocialDistancing();
            Projectile.velocity.X *= 0.8f;
			Attack++;

            Vector2 direction = aggroTarget.Center - Projectile.Center; //start - end 

            if (Attack >= AttackDuration) 
            {
                if (direction.Length() > 160 || !aggroTarget.CanBeChasedBy())
                {
                    attacking = false;
                    Attack = 0;
                }
            }

            Player player = Main.player[Projectile.owner];
            Vector2 toTarget = Projectile.DirectionTo(aggroTarget.Center * aggroTarget.velocity * AttackDuration * 0.5f);
            Projectile.spriteDirection = MathF.Sign(toTarget.X);
            if (Attack % AttackDuration == 0)
			{
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, toTarget * 10, 
                    ModContent.ProjectileType<MinionBeamSpread>(), Projectile.damage, Projectile.knockBack, player.whoAmI, Projectile.identity);
			}

            Projectile.frame = 8;
        }
        private void Jump()
        {
            Projectile.velocity.Y = -Speed; //velocityY boosts up 
            JumpTimer = 15;
            Projectile.frame = 9;
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