using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
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

        private List<float> Targetdistances = new(); //targeting
        public NPC aggroTarget = null; //target the minion is currently focused on
        public static float Speed => 15f;
        public static float Inertia => 3f;
        public static int FireRate => 13;
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
            if (!spaceJumping)
            {
                Projectile.velocity.Y += 0.7f;
                if (Projectile.velocity.Y >= 10f)
                {
                    Projectile.velocity.Y = 10f;
                }

            }

            //for stepping up tiles
            if (spaceJumping == false)
            {
                Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
            }

            Vector2 IdlePosition = player.Center;
            float minionPositionOffsetX = (40 + Projectile.minionPos * 40) * -player.direction; //behind player depending on order summoned
            IdlePosition.X += minionPositionOffsetX;

            Vector2 vectorToIdlePosition = IdlePosition - Projectile.Center; //distance from idle
            float distanceToIdlePosition = vectorToIdlePosition.Length(); //aboslute distance from idle

            int targetIndex = -1;
            Projectile.Minion_FindTargetInRange(1000, ref targetIndex, true, null);
            aggroTarget = targetIndex == -1 ? null : Main.npc[targetIndex];
            if (distanceToIdlePosition > 1500f)
            {
                spaceJumping = true;
                attacking = false;
            }

            if (aggroTarget != null && aggroTarget.CanBeChasedBy()) //ATTACK
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

                    int pseudoDirection = 1;
                    if (direction.X < 0) //enemy is behind
                    {
                        pseudoDirection = -1; //change direction so it will go towards enemy
                    }
                    //we put this instead of player.Center so it will always be moving top speed instead of slowing down when enemy is near but unreachable
                    //A "carrot on a stick" if you will

                    Vector2 carrotDirection = new(pseudoDirection * Speed, 0);

                    //use .X so it only effects horizontal movement
                    Projectile.velocity.X = (Projectile.velocity.X * (Inertia - 1) + carrotDirection.X) / Inertia;

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
                if (attacking == true) //checks if attacking
                {
                    Attack();
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
                    carrotDirection *= Speed;

                    //use .X so it only effects horizontal movement
                    Projectile.velocity.X = (Projectile.velocity.X * (Inertia - 1) + carrotDirection.X) / Inertia;

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

            if (jumpTimer > 0) //if jumping
            {
                Projectile.frame = 8; //jump frame
            }
        }


        public virtual float fireType => 0; //determines if using prehardmode fire or hardmode fire

        public void Attack()
        {
            if (aggroTarget == null)
            {
                attacking = false;
                attack = 0;
                return;
            }

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

            if (attack >= FireRate) //if attacked once
            {
                float maxLength = 180;

                if (fireType == 1)
                {
                    maxLength = 360;
                }

                if (direction.Length() > maxLength || !aggroTarget.CanBeChasedBy())
                {
                    attacking = false;
                    attack = 0;
                    return;
                }
            }

            Player player = Main.player[Projectile.owner];
            attack++;
            if (attack % FireRate == 1 && Main.myPlayer == Projectile.owner)
            {
                float projSpeed = 30;
                int projType = ModContent.ProjectileType<MinionFireSpread>();
                Vector2 toTarget = Projectile.DirectionTo(aggroTarget.Center) * projSpeed;
                Utils.ChaseResults results = Utils.GetChaseResults(Projectile.Center, projSpeed / ContentSamples.ProjectilesByType[projType].MaxUpdates, aggroTarget.Center, aggroTarget.velocity);
                if (results.InterceptionHappens)
                {
                    toTarget = results.ChaserVelocity;
                }
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, toTarget,
                    projType, Projectile.damage, Projectile.knockBack, player.whoAmI, Projectile.identity, fireType);
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
            Projectile.velocity.Y = -Speed; //velocityY boosts up 
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

                Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
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
                    Vector2 drawOrigin2 = new(texture.Width / 2, texture.Height / 2);
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