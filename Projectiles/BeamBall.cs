using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BeamBall : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 15;
			Projectile.height = 15;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 3000;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
			
		}
		public override void AI()
		{
			Projectile.rotation += 0.4f * (float)Projectile.direction; // rotates projectile
			if (++Projectile.frameCounter >= 2) //changes frames every 2 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 2)
				{
					Projectile.frame = 0;
				}
			}
		}
        public override bool PreDraw(ref Color lightColor)
        {
			VFX.DrawElectricOrb(Projectile.Center, new Vector2(1.2f), Projectile.Opacity, Projectile.rotation);
			return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
			return Utils.CenteredRectangle(Projectile.Center, Projectile.Size * 2).Intersects(targetHitbox);
        }

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}