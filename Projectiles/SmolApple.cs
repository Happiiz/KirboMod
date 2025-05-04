using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class SmolApple : ModProjectile //Apple for swishy tree
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.timeLeft = 200;
			Projectile.tileCollide = true; 
			Projectile.penetrate = 1;
		}
		public override void AI()
		{
			Projectile.velocity.Y = Projectile.velocity.Y + 0.3f;
			if (Projectile.velocity.Y >= 6f)
            {
				Projectile.velocity.Y = 6f;
            }
			Projectile.rotation += 0.1f; // rotates projectile
			/*if (++projectile.frameCounter >= 15) //changes frames every 15 ticks 
			{
				projectile.frameCounter = 0;
				if (++projectile.frame >= Main.projFrames[projectile.type])
				{
					projectile.frame = 0;
				}
			}*/
		}
        public override void OnKill(int timeLeft) //when the projectile dies
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(3f, 3f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.position, 115, speed * 2, Scale: 1f); //Makes dust in a messy circle
                d.noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			Player player = Main.player[0];

			if (Projectile.velocity.X != oldVelocity.X)
			{
				Projectile.velocity.X = -oldVelocity.X;
			}
			if (Projectile.velocity.Y != oldVelocity.Y)
			{
				Projectile.velocity.Y = -oldVelocity.Y;
			}

			return false;
		}
    }
}