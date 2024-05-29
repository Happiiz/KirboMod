using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class ZeroBloodPellet : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Blood Pellet");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }
		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			DrawOffsetX = 4;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 60;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1;
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
		}

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}

        public static Asset<Texture2D> afterimage;

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            afterimage = ModContent.Request<Texture2D>(Texture);
            Texture2D texture = afterimage.Value;

            // Redraw the projectile with the color not influenced by light
            for (int k = 1; k < Projectile.oldPos.Length; k++) //start at 1 so not ontop of actual projectile
            {
                Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);

                Color color = Color.White * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, 1, SpriteEffects.None, 0);
            }
            return true; //draw og
        }
    }
}