using Microsoft.Xna.Framework;
using System.Diagnostics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class MiniWhispy : ModProjectile
    {
        private int animation = 0;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mini Whispy");
            // Sets the amount of frames this minion has on its spritesheet
            Main.projFrames[Projectile.type] = 4;
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            // Denotes that this projectile is a pet or minion
            Main.projPet[Projectile.type] = true;

            // These below are needed for a minion
            // Denotes that this projectile is a pet or minion
            //Main.projPet[projectile.type] = true;
            // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            //ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            // Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
            //ProjectileID.Sets.Homing[projectile.type] = true;

        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 76;
            Projectile.height = 142;
            DrawOriginOffsetY = 2; //touch floor
                                   // Makes the sentry unable to go through tiles
            Projectile.tileCollide = true;

            // These below are needed for a minion weapon
            // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.friendly = true;
            // Only determines the damage type
            Projectile.sentry = true;
            // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.penetrate = -1;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.netImportant = true;
            Projectile.ignoreWater = true;
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
            Player player = Main.player[Projectile.owner];

            Projectile.velocity.Y += 0.2f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }

            Vector2 targetCenter = Projectile.position;
            float distanceFromTarget = 200f;
            bool foundTarget = false;

            NPC targetNPC = null;
            //Right click to target an npc
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                float distance = Vector2.Distance(npc.Center, Projectile.Center);
                // Reasonable distance away so it doesn't target across multiple screens
                if (distance < 100f)
                {
                    distanceFromTarget = distance;
                    targetCenter = npc.Center;
                    foundTarget = true;
                    targetNPC = npc;

                }
            }
            if (!foundTarget) //search target
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy())
                    {
                        float distance = Vector2.Distance(npc.Center, Projectile.Center);
                        bool closest = Vector2.Distance(Projectile.Center, npc.Center) > distance;
                        bool inRange = distance < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = distance < 100f;
                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = distance;
                            targetCenter = npc.Center;
                            foundTarget = true;
                            targetNPC = npc;
                        }
                    }
                }
            }

            if (foundTarget) //spew apples
            {
                animation = 1; //shake

                Projectile.ai[0]++;
                if (Projectile.ai[0] % 85 == 0 && Projectile.owner == Main.myPlayer) //every multiple of 90
                {
                    Vector2 firingPos = Projectile.Center + new Vector2(0, -50f);
                    float shootSpeed = 17f;
                    Vector2 velocity;
                    Vector2 targetPos = targetNPC.Center;

                    velocity = CalculateInterceptVelocityIterative(firingPos, targetPos, targetNPC.velocity, shootSpeed, SmullApple.Gravity, 60);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), firingPos, velocity, ModContent.ProjectileType<SmullApple>(), Projectile.damage, Projectile.knockBack, player.whoAmI, 0, 0);
                    for (int i = 0; i < 5; i++)
                    {
                        Dust.NewDustPerfect(firingPos, DustID.GrassBlades, velocity, 0, default, 1);
                    }
                }
            }
            else //not found target
            {
                Projectile.ai[0] = 0;
                animation = 0; //idle
            }

            //Animation
            Projectile.frameCounter++;
            if (animation == 0)
            {
                if (Projectile.frameCounter >= 180 & Projectile.frameCounter <= 190)
                {
                    Projectile.frame = 1; //blink
                }
                else
                {
                    Projectile.frame = 0; //eyes open
                }
                if (Projectile.frameCounter > 190)
                {
                    Projectile.frameCounter = 0;
                }
            }
            if (animation == 1)
            {
                if (Projectile.frameCounter <= 10)
                {
                    Projectile.frame = 2; //shake left
                }
                else //higher than 10, lower than 20
                {
                    Projectile.frame = 3; //shake right
                }
                if (Projectile.frameCounter >= 20)
                {
                    Projectile.frameCounter = 2;
                }
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false; //don't die when touching tiles
        }

        //thank you chatgpt
        static Vector2 CalculateInterceptVelocityIterative(Vector2 start, Vector2 target, Vector2 targetVelocity, float launchSpeed, float gravity, int iterations = 3)
        {
            Vector2 predictedTarget = target + targetVelocity;
            for (int i = 0; i < iterations; i++)
            {
                // Recalculate the velocity needed to hit the current prediction
                Vector2 launchVelocity = CalculateLaunchVelocity(start, predictedTarget, launchSpeed, gravity);

                // If launchVelocity is zero (no solution), break early
                if (launchVelocity == Vector2.Zero)
                    break;

                // Estimate travel time using actual velocity vector
                float time = (predictedTarget - start).Length() / launchVelocity.Length();

                // Predict target's future position using that time
                predictedTarget = target + targetVelocity * time;
            }
            // After iterations, calculate final velocity to predicted position
            Vector2 result = CalculateLaunchVelocity(start, predictedTarget, launchSpeed, gravity);

            if (result == Vector2.Zero)
            {//throw in random velocity as a failsafe
                result = Main.rand.NextVector2CircularEdge(launchSpeed, launchSpeed);
            }
            return result;
        }

        static Vector2 CalculateLaunchVelocity(Vector2 start, Vector2 target, float launchSpeed, float gravity)
        {
            Vector2 delta = target - start;
            float dx = delta.X;
            float dy = delta.Y;

            // Quadratic formula coefficients
            float g = gravity;
            float v = launchSpeed;
            float v2 = v * v;

            float A = (g * dx * dx) / (2f * v2);
            float B = dx;
            float C = A - dy;

            float discriminant = B * B - 4f * A * C;

            if (discriminant < 0f)
            {
                // No real solution: target is out of range
                return Vector2.Zero;
            }

            float sqrtDiscriminant = (float)System.Math.Sqrt(discriminant);

            // Two possible solutions for k (slope of velocity vector)
            float k1 = (-B + sqrtDiscriminant) / (2f * A);
            float k2 = (-B - sqrtDiscriminant) / (2f * A);

            // Pick the k that gives lower time (i.e., flatter arc = smaller |k|)
            float k = System.Math.Abs(k1) < System.Math.Abs(k2) ? k1 : k2;

            // Now recover vx and vy
            float vx = v / (float)System.Math.Sqrt(1f + k * k);
            vx *= dx < 0f ? -1f : 1f; // Fix direction based on dx
            float vy = k * vx;

            return new Vector2(vx, vy);
        }
    }
}