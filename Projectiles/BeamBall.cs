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
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 15;
			Projectile.tileCollide = false;
			Projectile.penetrate = 99;
			
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

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}