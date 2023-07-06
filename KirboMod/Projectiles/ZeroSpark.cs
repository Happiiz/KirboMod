using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class ZeroSpark : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Spark");
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 60;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
		}
		public override void AI()
		{
			Projectile.rotation += Projectile.velocity.X * 0.02f;

			Projectile.velocity *= 0.96f;
		}
         public override void Kill(int timeLeft) //when the projectile dies
         {
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity *= 0, ModContent.ProjectileType<Projectiles.ZeroSparkExplosion>(), 100 / 2, 12f, Main.myPlayer);
         }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}