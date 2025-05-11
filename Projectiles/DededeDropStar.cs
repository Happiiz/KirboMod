using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class DededeDropStar : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;

            //for drawing afterimages and stuff alike
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

		float dropStarMode { get => Projectile.ai[0];}

        public override string Texture => "KirboMod/Projectiles/TripleStarStar";

        public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 50;
			DrawOriginOffsetY = -3;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
        }
		public override void AI()
		{
			if (dropStarMode == 1) //gravity mode
			{
				Projectile.velocity.Y = Projectile.velocity.Y + 0.3f;
				if (Projectile.velocity.Y >= 30)
				{
					Projectile.velocity.Y = 30;
				}
			}
			else
			{
				Projectile.velocity *= 1.013f;
			}
			Projectile.rotation += 0.1f; // rotates projectile

			Projectile.Opacity = Utils.GetLerpValue(0, 60, Projectile.timeLeft, true);
		}

        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
			fallThrough = false;
			return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("KirboMod/Projectiles/TripleStarStarAfterimage").Value;

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawOrigin = texture.Size() / 2;
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);

                Color color = Color.DodgerBlue * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color * 0.8f * Projectile.Opacity, 
					Projectile.rotation, drawOrigin, 1, SpriteEffects.None, 0);
            }

            return true; //draw og
        }
    }
}