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
			Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			Projectile.width = 120; //not actual size just hitbox
			Projectile.height = 120;
			DrawOriginOffsetY = -3;
			DrawOffsetX = -23;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 500;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.alpha = 255;

        }

        public override string Texture => "KirboMod/NPCs/PureDarkMatter";

        public static float TimeSpentDecelerating => 20f;
        //call this
        public static void AccountForSpeed(ref Vector2 offset, Player target)
        {
            offset += target.velocity * TimeSpentDecelerating;
        }
        public static void NewDarkMatterShot(NPC zero, Vector2 target, Vector2 from, int damage, float directionY, float acceleration = 1.3f)
        {
            Projectile.NewProjectile(zero.GetSource_FromAI(), from, target - from, ModContent.ProjectileType<DarkMatterShot>(), damage, 0, -1, 0, acceleration * directionY, directionY);
        }
        public override void AI()
        {
            if (Projectile.ai[0] < TimeSpentDecelerating)
            {
                Projectile.velocity *= .5f;

				Projectile.damage = 0;

				if (Projectile.ai[0] > 5)
				{
                    Projectile.Opacity += .1f;//make it have 255 alpha on SetDefaults
                }
            }
            else
            {
                Projectile.velocity.Y -= Projectile.ai[1];
                Projectile.damage = Projectile.originalDamage;
            }
            Projectile.spriteDirection = (int)Projectile.ai[2];
            Projectile.rotation = MathF.PI / 2 * Projectile.ai[2];
            if (Projectile.spriteDirection < 0)
            {
                Projectile.rotation += MathF.PI;
            }
            Projectile.ai[0]++;
        }

        /*public override void AI()
		{
            Player player = Main.player[(int)Projectile.ai[1]];
            Vector2 playerDir = player.Center - Projectile.Center;

            //set inital direction manually
            if (Projectile.ai[0] == 0) 
			{
				if (playerDir.X > 0) 
				{
					initalDir = 1;

                }
				else
				{
                    initalDir = -1;
                }
			}

            Projectile.spriteDirection = -initalDir; //face same direction (negative 'cause sprite is flipped)

			if (Main.rand.NextBool(5)) // happens 1/5 times
			{
				int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.DarkResidue>(), 0f, 0f, 0, Color.White, 0.5f); //dust
				Main.dust[dustnumber].velocity *= 0.3f;
				Main.dust[dustnumber].noGravity = true;
			}

			//animation
			if (++Projectile.frameCounter >= 10) //changes frames every 10 ticks 
            {
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 2)
				{
					Projectile.frame = 0;
				}
			}

			//slowing down, backing up and speeding up
			Projectile.ai[0]++;

			if (Projectile.ai[0] < 30)
			{
                Projectile.velocity *= 0.9f;
            }
			else
			{
                Projectile.velocity.X += 1.2f * initalDir; //speed up
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
		}*/
        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White * Projectile.Opacity; // Makes it uneffected by light
		}
    }
}