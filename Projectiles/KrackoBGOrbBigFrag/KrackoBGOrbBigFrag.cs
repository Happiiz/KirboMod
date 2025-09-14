using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.KrackoBGOrbBigFrag
{
    internal class KrackoBGOrbBigFrag : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_1";
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.scale = 3f;
        }
        public override void AI()
        {
            Projectile.rotation += 0.15f * MathF.Sign(Projectile.velocity.X) * MathF.Sign(Projectile.velocity.Y); // rotates projectile
            if (++Projectile.frameCounter >= 2) //changes frames every 2 ticks 
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 2)
                {
                    Projectile.frame = 0;
                }
            }
            if (Main.rand.NextBool(2)) // happens 1/2 times
            {
                Vector2 offset = Projectile.velocity;
                offset.Normalize();//I don't use offset after this for positioning, so I can normalize it without issues
                offset = offset.RotatedBy(MathF.PI / 2);
                offset *= 4 * Main.rand.NextFloat() + 2;
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height) / 2, DustID.Electric, offset);
                dust.noGravity = true;
                dust.alpha = 200;
            }

        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Projectile.DistanceSQ(targetHitbox.ClosestPointInRect(Projectile.Center)) < 600;//circle hitbox and only hit if fully visible
        }
        public override bool PreDraw(ref Color lightColor)
        {
            VFX.DrawElectricOrb(Projectile.Center, Vector2.One * 4f, Projectile.Opacity, Projectile.rotation);
            return false;
        }
    }
}
