using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BadPlasmaBlast : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Plasma Blast");

            //for space jump trail
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

		public override void SetDefaults()
		{
			Projectile.width = 140;
			Projectile.height = 140;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 120;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1; //infinite
		}

		public override void AI()
		{

			Projectile.ai[0]++;
			//Animation
			if (++Projectile.frameCounter >= 4) //changes frames every 4 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}

			//dust effects
			if (Main.rand.NextBool(2)) // happens 1/2 times
			{
				int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.TerraBlade, 0f, 0f, 200, default, 1f); //dust
				Main.dust[dustnumber].velocity *= 0.3f;
				Main.dust[dustnumber].noGravity = true;
			}
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 40; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				Vector2 speed = Main.rand.NextVector2Circular(20, 20); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.TerraBlade, speed, Scale: 1f); //Makes dust in a messy circle
				d.noGravity = true;
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }
}