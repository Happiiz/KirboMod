using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BirdonFeatherBad : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Birdon Feather");
		}
		public override void SetDefaults()
		{
			Projectile.width = 22;
			Projectile.height = 14;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

			/*if (Main.rand.Next(5) == 1) // happens 1/5 times
			{
				int dustnumber = Dust.NewDust(projectile.position, 24, 24, DustID.Fire, 0f, 0f, 200, default, 1.5f); //dust
				Main.dust[dustnumber].velocity *= 0.3f;
				Main.dust[dustnumber].noGravity = true;
			}*/
		}

        public override void OnKill(int timeLeft) //when the projectile dies
        {
            for (int i = 0; i < 6; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(3f, 3f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center, 78, speed, Scale: 1.25f); //Makes dust in a messy circle
            }
        }
    }
}