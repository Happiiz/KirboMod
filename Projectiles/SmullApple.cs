using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class SmullApple : ModProjectile //apple for MiniWhispy
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.MinionShot[Type] = true;
        }
		public static float Gravity => 0.2f;
		public static Vector2 GravityVec => new Vector2(0, Gravity);
		public static float MaxFallSpeed => 6f;
		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.timeLeft = 240;
			Projectile.tileCollide = true; 
			Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Summon;
        }
		public override void AI()
		{
			Projectile.ai[0]++;
			Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
			//if (Projectile.velocity.Y >= MaxFallSpeed)
   //         {
			//	Projectile.velocity.Y = MaxFallSpeed;
   //         }
			Projectile.rotation += Projectile.velocity.X * 0.02f; // rotates projectile
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