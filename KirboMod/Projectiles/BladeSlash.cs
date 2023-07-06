using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BladeSlash : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 3;
		}

		public override void SetDefaults()
		{
			Projectile.width = 40;
			Projectile.height = 50;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 15;
			Projectile.tileCollide = false;
			Projectile.penetrate = 300;
			Projectile.scale = 1f;
		}

		public override void AI()
		{
			Projectile.spriteDirection = Projectile.direction;
			if (++Projectile.frameCounter > 5) //changes frames every 5 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}
			if (Projectile.velocity.X >= 0f) //makes the projectile move a set amount depending on the velocity
			{
				Projectile.velocity.X = 2f;
			}
			else
			{
				Projectile.velocity.X = -2f;
			}
			Projectile.velocity.Y = 0f; //no Y movement
		}
	}
}