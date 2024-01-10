using KirboMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BadPlasmaLaser : ModProjectile
	{
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            DrawOffsetX = -43; //make hitbox line up with sprite middle
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 1000;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 40;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 255;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 normalizedVel = Vector2.Normalize(Projectile.velocity);
            float a = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - normalizedVel * 80, Projectile.Center + normalizedVel * 80, 60, ref a);
        }
        Color PlasmaColor(float progress)
        {
            return Color.Lerp(Color.DarkGreen, Color.DarkKhaki, (progress + Main.GlobalTimeWrappedHourly * 3) % 1);
        }
        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D texture = TextureAssets.Extra[98].Value;
            Color color;
            for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
            {
                float progress = Utils.GetLerpValue(0, Projectile.oldPos.Length, i);
                color = PlasmaColor(progress);
                color *= Projectile.Opacity;
                color.A = 0;
                int steps = 2;
                for (float j = 0; j < .99f; j += 1f/steps)
                {
                    Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition + (Projectile.velocity * j), null, color * 0.8f, Projectile.rotation + MathF.PI / 2, texture.Size() / 2, Projectile.scale, SpriteEffects.None);
                }
            }
            color = PlasmaColor(0);
            color *= Projectile.Opacity;
            color.A = 0;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color * 0.8f, Projectile.rotation + MathF.PI / 2, texture.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }

        public override void AI()
		{
            if(Projectile.timeLeft % 4 == 0)
            {
                Ring ring = Ring.ShotRing(Projectile.Center, Color.LimeGreen, Projectile.velocity);
                ring.shineBrightness = .35f;
            }
            Projectile.Opacity += 0.1f;
			Projectile.rotation = Projectile.velocity.ToRotation();
			int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.TerraBlade, 0f, 0f, 200, default, 1f); //dust
			Main.dust[dustnumber].velocity *= 0.3f;
			Main.dust[dustnumber].noGravity = true;
		}
	}
}