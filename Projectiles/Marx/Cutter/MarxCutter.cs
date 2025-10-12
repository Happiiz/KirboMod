using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Marx.Cutter
{
    public class MarxCutter : ModProjectile
    {
        public static void ShootCutter(int delayBeforeReturning, Vector2 velocity, NPC marx, int damage)
        {
            if (Main.myPlayer == NetmodeID.MultiplayerClient)
            {
                return;
            }
            float ai0 = delayBeforeReturning;
            float ai1 = GetTimeToReturn(velocity, delayBeforeReturning, ReturnAcceleration) - 1;
            IEntitySource source = null;
            Vector2 spawnPos = Main.MouseWorld;
            if (marx != null)
            {
                marx.GetSource_FromAI();
                spawnPos = marx.Center;
            }
            Projectile.NewProjectile(source, spawnPos, velocity, ModContent.ProjectileType<MarxCutter>(), damage, 0f, -1, ai0, ai1);
        }

        static float GetTimeToReturn(Vector2 initialVel, float timeBeforeStartReturning, float returnAcceleration)
        {
            float v0 = initialVel.Length();
            float t0 = timeBeforeStartReturning;
            float a = returnAcceleration;

            // distance travelled straight first
            float d0 = v0 * t0;

            // extra distance covered while stopping
            float dStop = v0 * v0 / (2f * a);
            float tStop = v0 / a;

            // total distance to cover on return
            float d1 = d0 + dStop;

            // accelerate back to max speed
            float dAccel = v0 * v0 / (2f * a);
            float tAccel = v0 / a;

            float tCruise = 0f;
            if (d1 > dAccel)
                tCruise = (d1 - dAccel) / v0;

            return t0 + tStop + tAccel + tCruise;
        }


        ref float ReturnCountdown => ref Projectile.ai[0];
        ref float TimeLeft => ref Projectile.ai[1];
        public static float ReturnAcceleration => 0.3f;
        ref float RotationSpeed => ref Projectile.localAI[0];
        ref float InitialVelX => ref Projectile.localAI[2];
        ref float InitialVelY => ref Projectile.localAI[1];
        int AfterimageCancelAmount => (int)MathF.Max(-TimeLeft, 0);
        Vector2 InitialVel { get => new Vector2(InitialVelX, InitialVelY); set
            {
                InitialVelX = value.X;
                InitialVelY = value.Y;
            } }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CanHitPastShimmer[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.width = Projectile.height = 60;
            Projectile.ignoreWater = true;

        }
        public override void AI()
        {
            Projectile.frameCounter--;//used as a counter for afterimage
           
            if(RotationSpeed == 0)
            {
                RotationSpeed = Main.rand.NextFloat(.2f,.4f) * (Main.rand.Next(2) * 2 - 1);
            }
            TimeLeft--;
            if(TimeLeft < 0)
            {
                //don't deal damage anymore because it's invisible
                //but don't kill yet so afterimages can fade out nicely
                Projectile.damage = -1;
                if(AfterimageCancelAmount > ProjectileID.Sets.TrailCacheLength[Type])
                {
                    Projectile.Kill();
                }
            }
            if(InitialVelX == 0 && InitialVelY == 0)
            {
                InitialVel = Projectile.velocity;
            }
            Projectile.rotation += RotationSpeed * MathF.Max(.5f, Projectile.velocity.Length() * 0.05f);
            ReturnCountdown--;
            if(ReturnCountdown < 0)
            {
                Projectile.velocity = Projectile.velocity.MoveTowards(-InitialVel, ReturnAcceleration);
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D trailTex = ModContent.Request<Texture2D>("KirboMod/Projectiles/Marx/Cutter/MarxCutterTrail").Value;
            int skipCount = 6;
            if (Projectile.frameCounter < skipCount)
            {
                Projectile.frameCounter = skipCount * 100000;
            }
            for (int i = Projectile.oldPos.Length - 1; i >= 0; i -= skipCount)
            {
                int index = i - Projectile.frameCounter % skipCount;
                if (index < AfterimageCancelAmount || index >= Projectile.oldPos.Length)
                {
                    continue;
                }
                Vector2 oldPos = Projectile.oldPos[index] + Projectile.Size / 2;
                float oldRot = Projectile.oldRot[index];
                float scaleMult = Projectile.scale;
                float progress = (float)index / Projectile.oldPos.Length;
                Color col = Color.White;
                col.A = 128;
                col *= 1 - progress;
                Main.EntitySpriteDraw(trailTex, oldPos - Main.screenPosition, null, col, oldRot, trailTex.Size() / 2, scaleMult, SpriteEffects.None, 0f);
            }
            if (AfterimageCancelAmount <= 0)
            {
                Projectile.DrawSelf();
            }
            return false;
        }
    }
}
