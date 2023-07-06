using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BuzzCutterProj : ModProjectile
	{
		private int lives = 8; //eight chances to bounce

		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
			// DisplayName.SetDefault("Buzz Cutter");
		}

		public override void SetDefaults()
		{
			Projectile.width = 73;
			Projectile.height = 73;
			DrawOffsetX = -24;
			DrawOriginOffsetY = -24;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 3600;
			Projectile.tileCollide = false; //inital so doesn't collide with tiles upon spawn
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true; //uses own immunity frames
			Projectile.localNPCHitCooldown = 10; //time before hit again
		}
		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			Projectile.rotation += 0.3f; // rotates projectile
			Projectile.ai[0]++;

			if (Projectile.ai[0] >= 5)
            {
				Projectile.tileCollide = true; //now collide
			}

			if (Projectile.ai[0] >= 50) //not colliding with tiles
            {
                float speed = 20f; //top speed(original shoot speed)

				float inertia = 10f; //acceleration and decceleration speed

				Vector2 direction = player.Center - Projectile.Center; //start - end 																	
				direction.Normalize();
				direction *= speed;
				Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;

				Rectangle box = Projectile.Hitbox;
				if (box.Intersects(player.Hitbox)) //if touching player
				{
					Projectile.Kill(); //KILL
				}
			}

			if (Projectile.ai[1] >= 4)
			{
				Projectile.ai[0] = 50;
				Projectile.ai[1] = -30;
			}
			if (Projectile.ai[1] < 0) //go up until 0
			{
                Projectile.ai[1]++;
            }

			if (lives <= 0) //enough bouncing
            {
				Projectile.Kill(); //KILL

                for (int i = 0; i < 10; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                    Gore.NewGorePerfect(Projectile.GetSource_FromThis(), Projectile.Center, speed, Main.rand.Next(61, 63), Scale: 1f); //smoke
                }
            }
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			if (Projectile.ai[1] >= 0 && Projectile.ai[1] < 4) //can grind again
			{
                Projectile.ai[1]++;
				Projectile.ai[0] = 39;
				Projectile.velocity *= 0.01f;
            }

            //dust
            for (int i = 0; i < 3; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
            {
                Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, speed, Scale: 2f); //Makes dust in a messy circle
                d.noGravity = false;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) 
        {
			lives--;

			//dust
			for (int i = 0; i < 3; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, speed, Scale: 2f); //Makes dust in a messy circle
				d.noGravity = false;
			}
			//thump
			SoundEngine.PlaySound(SoundID.Item40, Projectile.Center); //sniper shot

			if (Projectile.velocity.X != oldVelocity.X) //bounce
			{
				Projectile.velocity.X = -oldVelocity.X;
			}
			if (Projectile.velocity.Y != oldVelocity.Y) //bounce
			{
				Projectile.velocity.Y = -oldVelocity.Y;
			}

			Projectile.ai[0] = 5; //reset timer
			return false;
		}
    }
}