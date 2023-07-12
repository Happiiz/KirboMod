using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class DarkMatterShot : ModProjectile
	{
		int initalDir = 1;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dark Matter");
			Main.projFrames[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 120; //not actual size just hitbox
			Projectile.height = 120;
			DrawOriginOffsetY = -4;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 500;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;

        }

		public override void AI()
		{
            Player player = Main.player[(int)Projectile.ai[1]];
            float playerXDir = player.Center.X - Projectile.Center.X;

            //set inital direction manually
            if (Projectile.ai[0] == 0) 
			{
				if (playerXDir > 0) 
				{
					initalDir = 1;

                }
				else
				{
                    initalDir = -1;
                }
			}

            Projectile.spriteDirection = initalDir; //face same direction

			if (Main.rand.NextBool(5)) // happens 1/5 times
			{
				int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.DarkResidue>(), 0f, 0f, 0, Color.White, 0.5f); //dust
				Main.dust[dustnumber].velocity *= 0.3f;
				Main.dust[dustnumber].noGravity = true;
			}

			//animation
			if (++Projectile.frameCounter >= 20) //changes frames every 20 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 2)
				{
					Projectile.frame = 0;
				}
			}

			//backing up and speeding up
			Projectile.ai[0]++; 

			if (Projectile.ai[0] < 30)
			{
				if (playerXDir < 250 || playerXDir > -250) //player is close
				{
                    Projectile.velocity.X -= 0.5f * initalDir; //back up faster
                }
				else //regular speed
				{
					Projectile.velocity.X -= 0.25f * initalDir; //back up
				}
            }
			else
			{
                Projectile.velocity.X += 0.5f * initalDir; //speed up
            }

			//move up or down
			if (!player.dead) //check this so it doesn't flatline when player dies
			{
				float speed = 16f;
				float inertia = 8f;
				Vector2 direction = player.Center - Projectile.Center; //start - end
				direction.Normalize();
				direction *= speed;
				Projectile.velocity.Y = (Projectile.velocity.Y * (inertia - 1) + direction.Y) / inertia; //use .Y so it only effects vertical movement    
			}

            //cap
            if (Projectile.velocity.X > 60f)
			{
				Projectile.velocity.X = 60f;
			}
			if (Projectile.velocity.X < -60f)
			{
				Projectile.velocity.X = -60f;
			}
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
    }
}