using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class ExplosiveCoconut : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
		}
		public override void AI()
		{
			Projectile.velocity.Y = Projectile.velocity.Y + 0.3f;
			if (Projectile.velocity.Y >= 10f)
			{
				Projectile.velocity.Y = 10f;
			}

			if (Projectile.velocity.X >= 0)
			{
				Projectile.rotation += MathHelper.ToRadians(18);
			}
			else
			{
                Projectile.rotation -= MathHelper.ToRadians(18);
            }
		}
         public override void OnKill(int timeLeft) //when the projectile dies
         {
			SoundEngine.PlaySound(SoundID.Item14, Projectile.position); //bomb sound

			for (int i = 0; i < 18; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, speed, Scale: 2f); //Makes dust in a messy circle
				d.noGravity = true;
			}
		}

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
			Projectile.Kill(); 
        }
    }
}