using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class PlasmaLaser : ModProjectile
	{
		public override void SetStaticDefaults()
		{ 
			
		}

		public override void SetDefaults()
		{
			Projectile.width = 22;
			Projectile.height = 22;
			DrawOffsetX = -43; //make hitbox line up with sprite middle
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.timeLeft = 60;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.localNPCHitCooldown = 40;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.extraUpdates = 1;
		}
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
			Vector2 normalizedVel = Vector2.Normalize(Projectile.velocity);
			float a = 0;
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - normalizedVel * 80, Projectile.Center + normalizedVel * 80, 60, ref a);
        }
		static void AABBLineVisualizer(Vector2 lineStart, Vector2 lineEnd, float lineWidth)
		{
			Texture2D blankTexture = Terraria.GameContent.TextureAssets.Extra[195].Value;
			Vector2 texScale = new Vector2((lineStart - lineEnd).Length(), lineWidth) * 0.00390625f;//1/256, texture is 256x256
			Main.EntitySpriteDraw(blankTexture, (lineStart) - Main.screenPosition, null, Color.Red, (lineEnd - lineStart).ToRotation(), new Vector2(0, 128), texScale, SpriteEffects.None);
		}
		Color PlasmaColor(float progress)
        {
			return Color.Lerp(Color.Green, Color.Yellow, (progress + Main.GlobalTimeWrappedHourly * 3) % 1);
        }
        public override bool PreDraw(ref Color lightColor)
        {

		Texture2D texture = TextureAssets.Extra[98].Value;
			Vector2 normalizedVel = Vector2.Normalize(Projectile.velocity);
            for (float i = 0; i < 8; i+= 1f/3f)
            {
				Color color = PlasmaColor(i / 8f);
				Vector2 drawOffset = normalizedVel * (i - 8) * 26;
				drawOffset -= Main.screenPosition;
				color.A = 0;
				Main.EntitySpriteDraw(texture, Projectile.Center + drawOffset, null, color * 0.8f, Projectile.rotation + MathF.PI / 2, texture.Size() / 2, Projectile.scale, SpriteEffects.None);
            }
			for (float i = 0; i < 8; i+= 1/3f)
			{
				Color color = new(100, 100, 100, 0);
				Vector2 drawOffset = normalizedVel * (i - 8) * 26;
				drawOffset -= Main.screenPosition;
				Main.EntitySpriteDraw(texture, Projectile.Center + drawOffset, null, color, Projectile.rotation + MathF.PI / 2, texture.Size() / 2, Projectile.scale * new Vector2(0.7f,0.5f), SpriteEffects.None);
			}
            for (int i = 0; i < 8; i += 2)
            {
				Color color = PlasmaColor(i / 8f);
				Vector2 scale = new Vector2(0.5f, 0.2f);
				Vector2 drawOffset = normalizedVel * (i - 7) * 26;
				drawOffset -= Main.screenPosition;
				color.A = 0;
				Main.EntitySpriteDraw(VFX.Ring, Projectile.Center + drawOffset, null, color, Projectile.rotation + MathF.PI / 2, VFX.Ring.Size() / 2, Projectile.scale * scale, SpriteEffects.None);
			}
			return false;
        }
        public override void AI()
		{
			//projectile.spriteDirection = projectile.direction;
			Projectile.rotation = Projectile.velocity.ToRotation();
			int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.TerraBlade, 0f, 0f, 200, default, 1f); //dust
			Main.dust[dustnumber].velocity *= 0.3f;
			Main.dust[dustnumber].noGravity = true;
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}

	}
}