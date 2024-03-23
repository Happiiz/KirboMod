using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class ZeroSparkExplosion : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
            // DisplayName.SetDefault("Spark Explosion");
        }
		static int Lifetime => 20;
		public override void SetDefaults()
		{
			Projectile.width = 100;
			Projectile.height = 100;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = Lifetime;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.alpha = 50;
		}
		public override void AI()
		{

			Projectile.scale = Utils.GetLerpValue(Lifetime, 0, Projectile.timeLeft);
			Projectile.scale = Easings.EaseOut(Projectile.scale, 2);
			Projectile.scale = MathHelper.Lerp(1, 1 + 0.05f * Lifetime, Projectile.scale);
			Projectile.Opacity = Utils.Remap(Projectile.timeLeft, Lifetime * .7f, 0, 0.8f, 0);
			Lighting.AddLight(Projectile.Center, 1f, 0.9f, 0);
		}

        public override Color? GetAlpha(Color lightColor)
        {
			return Color.White * Projectile.Opacity;
        }
    }
}