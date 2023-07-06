using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BioSlash : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 3;
		}

		public override void SetDefaults()
		{
			Projectile.width = 100;
			Projectile.height = 70;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 27;
			Projectile.tileCollide = false;
			Projectile.penetrate = 300;
			Projectile.scale = 1f;
		}

		public override void AI()
		{
			if (Projectile.velocity.X >= 0f) //makes the projectile move a set amount depending on the velocity
			{
				Projectile.velocity.X = 7f;
			}
			else
			{
				Projectile.velocity.X = -7f;
			}
			Projectile.velocity.Y = 0f; //no Y movement

			Projectile.spriteDirection = Projectile.direction;

			if (++Projectile.frameCounter >= 9) //changes frames every 9 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}
		}
	}
}