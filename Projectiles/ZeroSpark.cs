using KirboMod.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
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
			Projectile.timeLeft = 90;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
		}
		public override void AI()
		{
			Projectile.rotation += Projectile.velocity.X * 0.02f;

			Projectile.velocity *= 0.96f;
        }
         public override void OnKill(int timeLeft) //when the projectile dies
         {
            SoundEngine.PlaySound(SoundID.Item11.WithVolumeScale(0.8f), Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity *= 0.01f, ModContent.ProjectileType<ZeroSparkExplosion>(), 100 / 2, 12f, Main.myPlayer);
		 }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            VFX.DrawGlowBallAdditive(Projectile.Center, 0.5f, Color.Blue, Color.White);
			return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}