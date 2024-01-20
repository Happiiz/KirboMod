using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class Clutter : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			
		}
		public override void AI()
		{
			Projectile.velocity.Y = Projectile.velocity.Y + 0.4f;
			if (Projectile.velocity.Y >= 12f)
            {
				Projectile.velocity.Y = 12f;
            }
			Projectile.rotation += Projectile.direction * 0.06f; // rotates projectile depending on direction it's facing
		}
        public override void OnKill(int timeLeft) //when the projectile dies
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, speed, Scale: 1.5f); //Makes dust in a messy circle
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) 
        {
			Player player = Main.player[0];
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

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false; //don't fall through platforms
            return true;
        }
    }
}