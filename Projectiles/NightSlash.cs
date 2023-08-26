using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class NightSlash : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;

            //for afterimages
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

		public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 100;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 240;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
			Projectile.scale = 1f;
		}
		public override void AI()
		{
			Projectile.spriteDirection = Projectile.direction; //face direction it's going

			Lighting.AddLight(Projectile.Center, 0.255f, 0f, 0.255f);
		}
        public override Color? GetAlpha(Color lightColor)
        {
			return Color.White; // Makes it uneffected by light
        }

        // This projectile uses an additional texture for drawing
        public static Asset<Texture2D> SlashTexture;

        public override bool PreDraw(ref Color lightColor) //blue "afterimage" thing
        {
			SlashTexture = ModContent.Request<Texture2D>("KirboMod/Projectiles/NightSlash");

            Main.instance.LoadProjectile(Projectile.type);
            SlashTexture = ModContent.Request<Texture2D>(Texture);
            Texture2D texture = SlashTexture.Value;

            // Redraw the projectile with the color not influenced by light
            for (int k = 1; k < Projectile.oldPos.Length; k++) //start at 1 so not ontop of actual ring
            {
                Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);

                SpriteEffects direction = SpriteEffects.None;

                if (Projectile.direction == -1) //facing left instead of right
                {
                    direction = SpriteEffects.FlipHorizontally;
                }

                Main.EntitySpriteDraw(texture, drawPos, null, new Color(0, 0, 100), 0, drawOrigin, 1 - (k * 0.1f), direction, 0);
            }

            return true; //draw og
        }
    }
}