using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class DarkMirrorProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dark Mirror");
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			DrawOriginOffsetY = -24;
			Projectile.friendly = true;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

			Player player = Main.player[Projectile.owner];

			Projectile.velocity *= 0.96f;

			Projectile.ai[0]++;

			if (Projectile.Hitbox.Intersects(player.Hitbox) && Projectile.ai[0] > 30) //collect it after being out for a while
            {
				Projectile.Kill(); 
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}