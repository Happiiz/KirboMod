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
			Projectile.extraUpdates = 1;

			Projectile.timeLeft *= Projectile.MaxUpdates;
		}
		public override void AI()
		{
			Projectile.velocity.Y = Projectile.velocity.Y + 0.15f;
			if (Projectile.velocity.Y >= 20)
            {
				Projectile.velocity.Y = 20;
            }
			Projectile.rotation += 0.1f;
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