using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class HeroSlash : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            //for afterimages
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

		public override void SetDefaults()
		{
			Projectile.width = 52;
			Projectile.height = 26;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.timeLeft = 6000;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
            Projectile.extraUpdates = 3;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1; 
		}
		public override void AI()
		{
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Main.rand.NextBool(Projectile.MaxUpdates))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10, 10) / 3, DustID.MagicMirror);
                dust.scale *= 2;
                dust.velocity *= 0.5f;
                dust.velocity += Projectile.velocity;
            }
		}

        public override Color? GetAlpha(Color lightColor)
        {
            //unaffected by light, but can change opacity aswell
            return Color.White * Projectile.Opacity;
        }

        public static Asset<Texture2D> afterimage;

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] < 0)
                return false;
            Main.instance.LoadProjectile(Projectile.type);
            afterimage = ModContent.Request<Texture2D>(Texture);
            Texture2D texture = afterimage.Value;

            // Redraw the projectile with the color not influenced by light
            for (int k = 1; k < Projectile.oldPos.Length; k++) //start at 1 so not ontop of actual ring
            {
                Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);

                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, 1, SpriteEffects.None, 0);
            }

            return true; //draw og
        }
    }
}