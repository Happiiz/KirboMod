using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Marx
{
	public class MarxBall : ModProjectile
    {
		public override void SetStaticDefaults()
		{

			Main.projFrames[Projectile.type] = 1;
		}
		public static Vector2 Gravity => new Vector2(0f, 0.2f);
		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.timeLeft = 180;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
		}
		public override void AI()
		{
			Projectile.velocity += Gravity;
			Projectile.rotation += Projectile.velocity.X * 0.5f; // rotates projectile
		}

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

		public override void OnKill(int timeLeft) //when the projectile dies
		{
			for (int i = 0; i < 40; i++)
			{
				Vector2 speed = Main.rand.NextVector2Circular(8f, 8f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, Main.rand.Next(119, 143), speed);
			}

            SoundEngine.PlaySound(SoundID.Item41, Projectile.position); //chain gun
        }
	}
}