using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BadCutter : ModProjectile
	{
		private int backtrack;
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			Projectile.width = 80;
			Projectile.height = 38; //30 less than sprite
		    DrawOriginOffsetY = -11;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 90;
			Projectile.tileCollide = false;
			Projectile.penetrate = 1;
		}

		public override void AI()
		{
			Projectile.ai[0]++;
			Projectile.velocity.Y = 0f;
			if (Projectile.ai[0] < 10)
			{
				if (Projectile.velocity.X >= 0f) //makes the projectile move a set amount depending on the velocity
				{
					Projectile.velocity.X = 10f;
				}
				else
				{
					Projectile.velocity.X = -10f;
				}
			}
			if (++Projectile.frameCounter >= 2) //changes frames every 3 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
					SoundEngine.PlaySound(SoundID.Run.WithVolumeScale(0.5f), Projectile.Center); //(int) converts it to int
				}
			}
			if (Projectile.ai[0] == 10)
            {
				if (Projectile.velocity.X == 10f)
                {
					backtrack = 0;
                }
				else if (Projectile.velocity.X == -10f)
                {
					backtrack = 1;
				}
            }
			if (backtrack == 0)
            {
				Projectile.velocity.X -= 0.25f;
            }
			else
            {
				Projectile.velocity.X += 0.25f;
			}
			if (Projectile.velocity.X > 10)
            {
				Projectile.velocity.X = 10;
            }
			if (Projectile.velocity.X < -10)
			{
				Projectile.velocity.X = -10;
			}
		}
    }
}