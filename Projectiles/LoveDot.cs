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
		float counter = 20;
		float girate = 0.4f;
        public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;

            //for afterimages
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

		public override void SetDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30; // The length of old position to be recorded

			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1;
			Projectile.extraUpdates = 2;//more afterimages for a more cohesive trail
			Projectile.usesLocalNPCImmunity = true; //shares immunity frames with proj of same type
			Projectile.localNPCHitCooldown = 20; //time before hit again
		}
		Vector2 SpawnPos { get => new Vector2(Projectile.ai[0], Projectile.ai[1]); }
		ref float RotationalOffset { get => ref Projectile.ai[2]; }
		int afterimgCancelDrawCount = 0;
		public override void AI()
		{
			
            if (++Projectile.frameCounter >= 8) //changes frames every 3 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}
			if(afterimgCancelDrawCount == 0)
				Projectile.scale *= 1.005f;
			if (afterimgCancelDrawCount > Projectile.oldPos.Length || Projectile.Opacity == 0)
            {
				Projectile.Kill();
            }
			if(Projectile.localAI[0] > 100)//how many updates you want it to last
            {
				//afterimgCancelDrawCount++;
				Projectile.Opacity -= 1f / Projectile.oldPos.Length;
            }
			Projectile.localAI[0]++;
			float progress = Projectile.localAI[0] / 70;
			counter = (1 - progress) * 10  + 10;
			float rotationalOffset = Projectile.ai[2];
			progress = - progress * 0.5f * progress + 2 * progress;
			//basically I want a distance function + a constant changing angle
			Projectile.Center = (rotationalOffset + Projectile.localAI[0] * 0.04f).ToRotationVector2() * progress * 400 + SpawnPos;
		
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
            for (int k = Projectile.oldPos.Length - 1; k >= afterimgCancelDrawCount; k--)
            {
				Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
				Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Vector2 lerpedPos = k == 0 ? Vector2.Lerp(Projectile.position, Projectile.oldPos[0], 0.5f) : Vector2.Lerp(Projectile.oldPos[k - 1], Projectile.oldPos[k], 0.5f);
				lerpedPos -= -drawOrigin + new Vector2(0f, Projectile.gfxOffY) + Main.screenPosition;
				Color color = (Projectile.GetAlpha(lightColor) * Projectile.Opacity) * (1 - (float)k / Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, null, color * 0.75f, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
				Main.EntitySpriteDraw(texture, lerpedPos, null, color * 0.75f, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}
			return afterimgCancelDrawCount == 0 && Projectile.Opacity == 1; //draw og
        }
    }
}