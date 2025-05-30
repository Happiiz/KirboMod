using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BladoProj : ModProjectile
	{
		private bool touchedground = false;
		private float rotationspeed = 0;
		static int AccelerationDelay => 5;
		static float MaxSpeed => 10f;
		static float Acceleration => 0.2f;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Blado");
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 78;
			Projectile.height = 78;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
			Projectile.localNPCHitCooldown = 30;
			Projectile.usesLocalNPCImmunity = true;
		}
		public override void AI()
		{
			if (Projectile.velocity.Y == 0)
            {
				touchedground = true; //touch ground
            }

			Projectile.velocity.Y += 0.4f;
			if (Projectile.velocity.Y >= 12f)
			{
				Projectile.velocity.Y = 12f;
			}

			if (touchedground == false)
			{
				rotationspeed = Projectile.velocity.X * 0.05f; // rotates projectile

				Projectile.velocity.X *= 0.98f;
			}
			else //touched ground
            {
				Projectile.ai[0]++;
				if (Projectile.ai[0] < AccelerationDelay)
                {
					rotationspeed *= 0.95f;
					Projectile.velocity.X *= 0.95f;
                }
				if (Projectile.ai[0] >= AccelerationDelay)
                {
					rotationspeed += MathHelper.ToRadians(Projectile.direction * 0.5f);

					if (rotationspeed >= MathHelper.ToRadians(20))
                    {
						rotationspeed = MathHelper.ToRadians(20);
                    }
					if (rotationspeed <= MathHelper.ToRadians(-20))
					{
						rotationspeed = MathHelper.ToRadians(-20);

					}

					if (Projectile.ai[0] == AccelerationDelay)
                    {
						SoundEngine.PlaySound(SoundID.Item22, Projectile.Center); //motor loop
                    }

					Point rightbelow = new Vector2(Projectile.Center.X, Projectile.position.Y + Projectile.height).ToTileCoordinates();

					if (Projectile.ai[0] % 2 == 0 && Main.tile[rightbelow.X, rightbelow.Y].HasTile)
					{
						int dust = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.position.Y + Projectile.height), 1, 1, DustID.Smoke, Projectile.direction * -0.4f, -2f, 0, default, 2f); //dust
						Main.dust[dust].noGravity = true;
					}
				}

				if (Projectile.ai[0] >= AccelerationDelay)
                {
					Projectile.velocity.X += Projectile.direction * Acceleration;
					if (Projectile.velocity.X < -MaxSpeed)
                    {
						Projectile.velocity.X = -MaxSpeed;
                    }
					if (Projectile.velocity.X > MaxSpeed)
					{
						Projectile.velocity.X = MaxSpeed;
					}
				}
            }

			Projectile.rotation += rotationspeed; //rotation

			Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
		}

        public override bool OnTileCollide(Vector2 oldVelocity) 
        {
			return false; //doesn't die
		}

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
			fallThrough = false;

			return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) //decrease damage for each hit
        {
			Projectile.damage -= 1;
			if (Projectile.damage <= 6)
            {
				Projectile.damage = 6;
            }
		}

		public override void OnKill(int timeLeft) //when the projectile dies
		{
			for (int i = 0; i < 15; i++)
			{
				Vector2 speed = Main.rand.NextVector2Circular(4f, 4f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Obsidian, speed, Scale: 2f, newColor: Color.Violet); //Makes dust in a messy circle
				d.noGravity = true;
			}
		}
	}
}