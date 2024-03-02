using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class KirbyBallProj : ModProjectile
	{
		private int bounces = 0;
		private Vector2 olderVelocity; //the velocity it had before it hit the ground, but it doesn't update until the ball unflattens
        bool flattening = false; //determines if in flattened state
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Friend Ball");
			Main.projFrames[Projectile.type] = 3;
		}

		public override void SetDefaults()
		{
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.friendly = true;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1; //infinity
			Projectile.usesLocalNPCImmunity = true; //doesn't wait for other projectiles to hit again
			Projectile.localNPCHitCooldown = 5; //time until able to hit npc even if npc has just been struck
		}

		public override void AI()
		{
			Projectile.spriteDirection = Projectile.direction;

			//gravity
			if (flattening == false) //not bouncing
			{
				Projectile.velocity.Y += 0.5f;
				if (Projectile.velocity.Y > 60)
				{
					Projectile.velocity.Y = 60;
				}
			}

			if (Projectile.velocity.Y > 0)
			{
				Projectile.frame = 2; //fall
			}
			else if (Projectile.ai[0] > 0)
			{
                Projectile.frame = 1; //flat
            }
			else
			{
				Projectile.frame = 0; //rise
			}

            //flatten timer
            if (flattening == true) 
			{
				if (Projectile.ai[0] < 15) //wait
				{
					if (Projectile.ai[0] == 1)
					{
						Projectile.velocity.X *= 0.001f; //make very small so it faces the same direction if facing left
					}

					Projectile.ai[0]++;
				}
				else
				{
					Projectile.velocity.Y = olderVelocity.Y;
					Projectile.velocity.X = olderVelocity.X; 
					Projectile.ai[0] = 0; //reset
					flattening = false;
				}
			}
        }

		public override bool OnTileCollide(Vector2 oldVelocity) //bounce
		{
			if (Projectile.velocity.X != oldVelocity.X) //reverse
			{
                Projectile.velocity.X = -oldVelocity.X;
			}
			if (oldVelocity.Y > 2 && Projectile.ai[0] == 0) //bounce if going down and not flattening
			{
                olderVelocity.Y = -oldVelocity.Y;
                olderVelocity.X = oldVelocity.X;
                bounces++; //go up by 1
                Projectile.ai[0]++; //increase by 1

                SoundEngine.PlaySound(SoundID.Item10, Projectile.position); //impact
            }

            //flatten 
            if (Projectile.velocity.Y == 0 && olderVelocity.Y * -1 > 0f && Projectile.ai[0] > 0) //just hit the floor
            {
                flattening = true;
            }

            return false;
        }

        /*public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
			fallThrough = false; //go through platforms

			return true;
        }*/

        public override bool? CanCutTiles()
        {
			return false; //no destroy plants and pots
        }

        public override void OnKill(int timeLeft) //when the projectile dies
		{
			for (int i = 0; i < 10; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                Gore.NewGorePerfect(Projectile.GetSource_FromAI(), Projectile.Center, speed, Main.rand.Next(16, 18)); //stars

                Vector2 speed2 = Main.rand.NextVector2Circular(5f, 5f); //circle
                Gore.NewGorePerfect(Projectile.GetSource_FromAI(), Projectile.Center, speed2, Main.rand.Next(11, 13)); //double jump smoke
            }
		}
	}
}