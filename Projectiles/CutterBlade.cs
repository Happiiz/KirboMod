using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class CutterBlade : ModProjectile
	{
		private int backtrack = 0;
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			Projectile.width = 80;
			Projectile.height = 38; //30 less than sprite
			DrawOriginOffsetY = -11;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true; //allows to have npc immunity frames on its own accord
			Projectile.localNPCHitCooldown = 20; //time until it can damage again regardless if a projectile just struck the target
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			if (Projectile.ai[0] == 0) //move before turn
			{
				Projectile.velocity.X = player.direction * 15;
			}

			Projectile.ai[0]++;
			Projectile.velocity.Y *= 0.99f;

			if (++Projectile.frameCounter >= 2) //changes frames every 3 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
					SoundEngine.PlaySound(SoundID.Run.WithVolumeScale(0.5f), Projectile.Center); 
				}
			}
			if (Projectile.ai[0] == 15) //point of turn 
			{
				
				Projectile.tileCollide = true; //collide with tiles

				if (Projectile.velocity.X == 15f)
				{
					backtrack = 1;
				}
				else if (Projectile.velocity.X == -15f)
				{
					backtrack = 2;
				}
			}
			//invert direction based on direction

			if (backtrack == 1) //negative 10
			{
				Projectile.velocity.X -= 0.75f;
			}
			else if (backtrack == 2) //positive 10
			{
				Projectile.velocity.X += 0.75f;
			}

			//caps
			if (Projectile.velocity.X > 15)
			{
				Projectile.velocity.X = 15;
			}
			if (Projectile.velocity.X < -15)
			{
				Projectile.velocity.X = -15;
			}

			//kill when far away
			float Xdistance = player.Center.X - Projectile.Center.X;
			
			if (Xdistance > 1000 || Xdistance < -1000)
            {
				Projectile.Kill();
            }
		}

        public override bool OnTileCollide(Vector2 oldVelocity) 
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position); //impact
            return true; //kill like normal
        }

        public override void OnKill(int timeLeft)
		{
			if (Projectile.ai[1] != 1) //checks if not killed by player contact
			{
                for (int i = 0; i < 4; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                    Gore.NewGorePerfect(Projectile.GetSource_FromAI(), Projectile.Center, speed, Main.rand.Next(16, 18));
                }

                for (int i = 0; i < 5; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                    Gore.NewGorePerfect(Projectile.GetSource_FromThis(), Projectile.Center, speed, Main.rand.Next(11, 13), Scale: 0.5f); //smoke
                }
            }
        }
    }
}