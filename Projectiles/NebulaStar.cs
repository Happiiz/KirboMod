using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class NebulaStar : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dark Nebula Star");
			Main.projFrames[Projectile.type] = 8;
		}

		public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 180;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			
		}
		public override void AI()
		{
			Projectile.velocity.Y = Projectile.velocity.Y + 0.1f;
			if (Projectile.velocity.Y >= 6f)
            {
				Projectile.velocity.Y = 6f;
            }

			if (++Projectile.frameCounter >= 5) //changes frames every 5 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}
		}
        /* public override void Kill(int timeLeft) //when the projectile dies
         {
             for (int i = 0; i < 10; i++)
             {
                 Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
                 Dust d = Dust.NewDustPerfect(projectile.position, DustID.Enchanted_Gold, speed * 3, Scale: 1f); //Makes dust in a messy circle
             }
         }*/

        public override bool OnTileCollide(Vector2 oldVelocity) 
        {
			if (Projectile.velocity.X != oldVelocity.X) //bounce
			{
				Projectile.velocity.X = -oldVelocity.X;
			}
			if (Projectile.velocity.Y != oldVelocity.Y) //bounce
			{
				Projectile.velocity.Y = -oldVelocity.Y;
			}
			return false;
		}

		public override void Kill(int timeLeft) //when the projectile dies
		{
			for (int i = 0; i < 3; i++)
			{
				Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, Scale: 0.5f); //Makes dust in a messy circle
				d.noGravity = true;
			}
		}

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; //unaffected by light
        }
    }
}