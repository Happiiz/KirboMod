using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.BandanaDee
{
    public class BandanaWaddleDee : ModProjectile
    {
        public int attack = 0; //timer for attack
        public int attacktype;
        int jumpTimer = 0;
        public bool attacking = false; //checks if in attacking state
        bool spaceJumping = false; //determines if gonna warp
        float spaceJumpRotation = 0; //here for sprite rotation of space jump

        private List<float> Targetdistances = new(); //targeting
        public NPC aggroTarget = null; //target the minion is currently focused on

        bool reverseRun = false; //for running animation loop

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 14;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;

            //for space jump trail
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            DrawOriginOffsetY = -20;
            DrawOffsetX = -16;
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
            Projectile.tileCollide = true;
            //continously go down
            jumpTimer--;

            Projectile.spriteDirection = Projectile.direction;
            Player player = Main.player[Projectile.owner];

            // This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<Buffs.MinionBuffs.BandanaDeeBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<Buffs.MinionBuffs.BandanaDeeBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            //Gravity
            if (spaceJumping == false)
            {
                Projectile.velocity.Y += 0.7f;
                if (Projectile.velocity.Y >= 20f)
                {
                    Projectile.velocity.Y = 20f;
                }
            }

            //for stepping up tiles
            if (spaceJumping == false)
            {
                Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
            }

            //Important stuff for targeting

            Vector2 IdlePosition = player.Center;
            float minionPositionOffsetX = (40 + Projectile.minionPos * 40) * -player.direction; //behind player depending on order summoned
            IdlePosition.X += minionPositionOffsetX;

            Vector2 vectorToIdlePosition = IdlePosition - Projectile.Center; //distance from idle
            float distanceToIdlePosition = vectorToIdlePosition.Length(); //aboslute distance from idle

            int targetIndex = -1;
            Projectile.Minion_FindTargetInRange(12000, ref targetIndex, true, null);
            aggroTarget = targetIndex == -1 ? null : Main.npc[targetIndex];

            //if (distanceToIdlePosition > 1200f)
            //{
            //    spaceJumping = true;
            //    attacking = false;
            //}

            if (attacking == true && aggroTarget != null && aggroTarget.active) //checks if attacking
            {
                Attack();
            }
            else if (aggroTarget != null && aggroTarget.active && aggroTarget.CanBeChasedBy()) //ATTACK
            {
                Vector2 direction = aggroTarget.Center - Projectile.Center; //start - end
                Vector2 absDirection = new(Math.Abs(direction.X), Math.Abs(direction.Y));

                //attack if within range or intersecting hitbox
                if ((absDirection.Y <= 80 ||
                    aggroTarget.Hitbox.Intersects(Projectile.Hitbox)) && spaceJumping == false)
                {
                    if (attack == 0) //if attack cycle restarted
                    {
                        attacktype = 0;
                    }
                    attacking = true;
                }
                //attack if within range
                else if ((absDirection.X <= 800 || absDirection.Y <= 800) && spaceJumping == false)
                {
                    if (attack == 0)
                    {
                        attacktype = 1;
                    }
                    attacking = true;
                }

                if (direction.Y <= -50f && jumpTimer <= 0) //jump when below enemy and can jump again
                {
                    Jump();
                }

                //movement
                float speed = 30f; //walk speed
                float inertia = 4f; //turn speed
                int pseudoDirection = 1;
                if (direction.X < 0) //enemy is behind
                {
                    pseudoDirection = -1; //change direction so it will go towards enemy
                }

                Vector2 carrotDirection = new(pseudoDirection * speed, 0);

                Projectile.velocity.X = (Projectile.velocity.X * (inertia - 1) + carrotDirection.X) / inertia;

                if (attacking == false && jumpTimer > 0)
                {
                    RunAnimation();
                }
            }
            else //FOLLOW PLAYER
            {
                attacking = false;
                attack = 0;

                if (Projectile.velocity.X <= 0.1f & Projectile.velocity.X >= -0.1f) //idle
                {
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter >= 10)
                    {
                        if (Projectile.frame < 1)
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
                else //run
                {
                    RunAnimation();
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
                    float speed = 11f;
                    float inertia = 6f;
                    Vector2 direction = IdlePosition - Projectile.Center; //start - end
                    int pseudoDirection = 1;
                    if (direction.X < 0) //enemy is behind
                    {
                        pseudoDirection = -1; //change direction so it will go towards enemy
                    }

                    Vector2 carrotDirection = new(pseudoDirection * speed, 0); //start - end 

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
                Projectile.frame = 11; //jump frame
            }
        }

        private void Attack()
        {
            Player player = Main.player[Projectile.owner];

            if (attacktype == 0) //rapid jab
            {
                attack++; //starts at 1

                Vector2 direction = aggroTarget.Center - Projectile.Center; //start - end

                //summon spear to jab enemies if there is no spear

                bool foundASpear = false;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile spear = Main.projectile[i];

                    //my spear proj
                    if (spear.active && spear.owner == Projectile.owner && spear.type == ModContent.ProjectileType<BandanaDeeSpearHeld>() && spear.ai[1] == Projectile.identity)
                    {
                        foundASpear = true;
                        spear.rotation = direction.ToRotation(); //constantly rotate towards target
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (!foundASpear)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, direction, ModContent.ProjectileType<BandanaDeeSpearHeld>(), Projectile.damage, Projectile.knockBack, player.whoAmI, ai1: Projectile.whoAmI);
                }
                if (attack >= 1) //jab animation
                {
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter >= 2)
                    {
                        if (Projectile.frame > 7 && Projectile.frame < 10) //inbetween jab frames
                        {
                            Projectile.frame++; //go up
                        }
                        else
                        {
                            Projectile.frame = 8;
                        }
                        Projectile.frameCounter = 0;
                    }
                }

                if (attack >= 10) //reset
                {
                    attacking = false;
                    attack = 0;
                }
            }

            if (attacktype == 1) //throw spear
            {

                attack++;

                int attackTime1 = 5;
                int attackTime2 = -100;
                int framesBeforeShootingToSetReadySpearFrame = 2;
                if (attack == (attackTime1 - framesBeforeShootingToSetReadySpearFrame) || attack == (attackTime2 - framesBeforeShootingToSetReadySpearFrame))
                {
                    Projectile.frame = 12; //ready spear frame
                }

                if (attack == attackTime1 || attack == attackTime2)
                {
                    //Utils.ChaseResults results = Utils.GetChaseResults(Projectile.Center, effectiveShootSpeed, aggroTarget.Center, aggroTarget.velocity);
                    //Vector2 spearDirection = results.InterceptionHappens ? results.ChaserVelocity : (Vector2.Normalize(aggroTarget.Center - Projectile.Center * shootSpeed));
                    //spearDirection.Y -= results.InterceptionTime * BandanaDeeSpearThrown.Gravity; //adjust for gravity

                    int type = ModContent.ProjectileType<BandanaDeeSpearThrown>();
                    float shootSpeed = 30f;
                    float effectiveShootSpeed = shootSpeed * ContentSamples.ProjectilesByType[type].MaxUpdates;

                    Vector2 spearDirection = Helper.PredictiveAimWithGravity(Projectile.Center, aggroTarget.Center, aggroTarget.velocity, effectiveShootSpeed, BandanaDeeSpearThrown.Gravity);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, spearDirection, type,
                        Projectile.damage, Projectile.knockBack, player.whoAmI);
                    SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);

                    Projectile.frame = 13; //throw spear frame
                }

                if (attack >= 10) //reset
                {
                    attacking = false;
                    attack = 0;
                }
            }
        }

        private void Jump()
        {
            Projectile.velocity.Y = -15f;
            jumpTimer = 7;
            Projectile.frame = 11; //jump frame
        }

        void RunAnimation()
        {
            Projectile.frameCounter++;

            if (Projectile.frameCounter >= 5)
            {
                if (Projectile.frame < 7 && Projectile.frame > 2)
                {
                    if (reverseRun)
                        Projectile.frame--; //go down
                    else
                        Projectile.frame++; //go up

                    Projectile.frameCounter = 0;
                }
                else
                {
                    if (Projectile.frame == 7)
                    {
                        if (reverseRun)
                            Projectile.frame--; //go down
                        else
                            reverseRun = true;
                    }
                    else if (Projectile.frame == 2)
                    {
                        if (!reverseRun)
                            Projectile.frame++; //false
                        else
                            reverseRun = false;
                    }
                    else
                    {
                        Projectile.frame = 2;
                    }
                    Projectile.frameCounter = 0;
                }
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
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }

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