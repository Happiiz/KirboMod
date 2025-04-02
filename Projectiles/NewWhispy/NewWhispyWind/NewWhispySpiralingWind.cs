using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.NewWhispy.NewWhispyWind
{
    public class NewWhispySpiralingWind : ModProjectile
    {
        public override string Texture => "KirboMod/Projectiles/NewWhispy/NewWhispyWind/NewWhispyWindSmall";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.CanHitPastShimmer[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }
        ref float Timer => ref Projectile.localAI[0];
        Vector2 SpiralPivot { get => new(Projectile.localAI[2], Projectile.localAI[1]); set { Projectile.localAI[2] = value.X; Projectile.localAI[1] = value.Y; } }
        ref float RotationPerFrame => ref Projectile.ai[0];
        ref float NormalizedRotationOffset => ref Projectile.ai[1];
        ref float TimeLeftUntilDeath => ref Projectile.ai[2];
        float SpiralRadius => Projectile.velocity.X;
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public static void GetAIValues(int index, int projCount, float radius, int duration, out float ai0, out float ai1, out float ai2, out Vector2 velocity)
        {
            ai0 = -MathF.Tau / duration;
            ai1 = (float)index / projCount;
            ai2 = duration;
            velocity = Vector2.Zero;
            velocity.X = radius;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return Projectile.DrawSelf(lightColor);
        }
        public override void AI()
        {
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud);
            }
            if (Timer == 0)
            {
                SpiralPivot = Projectile.Center;
            }
            Timer++;
            if (Projectile.frameCounter++ >= 2)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Type])
                {
                    Projectile.frame = 0;
                }
            }
            float rotationAmount = RotationPerFrame * Timer + NormalizedRotationOffset * MathF.Tau;
            float progress = Timer / TimeLeftUntilDeath;
            Vector2 offset = new Vector2(MathF.Cos(rotationAmount), MathF.Sin(rotationAmount)) * progress * SpiralRadius;
            Projectile.Center = SpiralPivot + offset;
            if(Timer > TimeLeftUntilDeath)
            {
                Projectile.Kill();
            }
        }
    }
}
