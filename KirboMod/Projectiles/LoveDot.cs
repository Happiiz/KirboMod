using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using SoundType = Terraria.Audio.SoundType;

namespace KirboMod.Projectiles
{
	public class LoveDot : ModProjectile
	{
		Vector2 circleCenter = new Vector2(0, 0);
		float counter = 1;
		float gorate = 0.4f;
        public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;

            //for afterimages
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 30; 
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1;
			Projectile.usesIDStaticNPCImmunity = true; //shares immunity frames with proj of same type
			Projectile.idStaticNPCHitCooldown = 20; //time before hit again
		}

		public override void AI()
		{
			Projectile.scale *= 1.02f;
            if (++Projectile.frameCounter >= 4) //changes frames every 3 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}

			//set it to position here as setting it in love love would make it the love love position
            circleCenter = new Vector2(Projectile.ai[0] - (Projectile.width / 2), Projectile.ai[1] - (Projectile.height / 2));
			counter += gorate; //go up

			float rotationalOffset = MathHelper.ToRadians(Projectile.ai[2]); //convert degrees to radians

            //set a point(circleCenter) and then make the projectile spiral in a growing circle around that (starting at the center)
            Projectile.position.X = circleCenter.X + (float)Math.Cos(counter + rotationalOffset) * ((counter * 50) -  50);
            Projectile.position.Y = circleCenter.Y + (float)Math.Sin(counter + rotationalOffset) * ((counter * 50) - 50);

			gorate *= 0.90f;

			if (gorate < 0.04f) //slow enough
			{
                Projectile.alpha += 40;
            }
        }

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White * Projectile.Opacity; // Makes it uneffected by light
		}

        public static Asset<Texture2D> afterimagae;

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            afterimagae = ModContent.Request<Texture2D>(Texture);
            Texture2D texture = afterimagae.Value;

            // Redraw the projectile with the color not influenced by light
            for (int k = 1; k < Projectile.oldPos.Length; k++) //start at 1 so not ontop of actual dot
            {
                Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);

                Color color = (Projectile.GetAlpha(lightColor) * Projectile.Opacity) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, 1, SpriteEffects.None, 0);
            }

            return true; //draw og
        }
    }
}