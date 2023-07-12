using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class GoodNightStar : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Night Star");
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 240;
			Projectile.tileCollide = false;
			Projectile.penetrate = 999;
			Projectile.scale = 1f;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
		}
		public override void AI()
		{
			Lighting.AddLight(Projectile.Center, 0.255f, 0f, 0.255f);
			
			if (Projectile.velocity.X >= 0)
            {
				Projectile.rotation += 0.3f;
            }
			else
			{
				Projectile.rotation -= 0.3f;
			}

			if (Main.rand.NextBool(5)) // happens 1/5 times
			{
				int dustnumber = Dust.NewDust(Projectile.position, 50, 50, DustID.Shadowflame, 0f, 0f, 200, default, 1f); //dust
				Main.dust[dustnumber].velocity *= 0.3f;
			}
		    /*if (++projectile.frameCounter >= 15) //changes frames every 15 ticks 
			{
				projectile.frameCounter = 0;
				if (++projectile.frame >= Main.projFrames[projectile.type])
				{
					projectile.frame = 0;
				}
			}*/
		}
        public override void Kill(int timeLeft) //when the projectile dies
        {
			/*for (int i = 0; i < 10; i++)
			{
				Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
				Dust d = Dust.NewDustPerfect(projectile.position, DustID.Enchanted_Gold, speed * 3, Scale: 1f); //Makes dust in a messy circle
			}*/
		}

        public override Color? GetAlpha(Color lightColor)
        {
			return Color.White; // Makes it uneffected by light
        }

        // This projectile uses additional textures for drawing
        public static Asset<Texture2D> StarTexture;

        public override bool PreDraw(ref Color lightColor) //blue "afterimage" thing
		{
            StarTexture = ModContent.Request<Texture2D>("KirboMod/Projectiles/BadStar");

            Texture2D star = StarTexture.Value;

            for (int i = 0; i <= 2; i++)
			{
                Main.EntitySpriteDraw(star,
					new Vector2
			   (
						(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f) - Projectile.velocity.X * (i * 1.5f),
						(Projectile.position.Y - Main.screenPosition.Y + Projectile.height - star.Height * 0.5f + 2f) - Projectile.velocity.Y * (i * 1.5f)
				),
					new Rectangle(0, 0, star.Width, star.Height),
					new Color(0, 0, 100, 0),
					Projectile.rotation,
                    star.Size() * 0.5f,
					1f,
					SpriteEffects.None,
					0);
			}
			return true;
		}
	}
}