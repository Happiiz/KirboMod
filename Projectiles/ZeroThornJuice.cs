using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class ZeroThornJuice : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Thorn Juice");
			Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 180;
			Projectile.tileCollide = false;
			Projectile.penetrate = 1;
			//Projectile.alpha = 255;
		}
		public override void AI()
		{
			//Projectile.alpha -= 35; //make opaque

            if (Main.rand.NextBool(2)) // happens 1/2 times
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.t_Cactus, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 0, Color.White, 0.8f); //dust
				Main.dust[d].noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
			target.AddBuff(BuffID.Poisoned, 600); //poisoned for 10 seconds
			Projectile.Kill();
        }

        public override void OnKill(int timeLeft) //when the projectile dies
		{
			for (int i = 0; i < 4; i++)
			{
				Vector2 speed = Main.rand.NextVector2Unit((float)MathHelper.Pi / 4, (float)MathHelper.Pi / 2) * Main.rand.NextFloat(); //arc
				Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.t_Cactus, -speed, Scale: 1f, newColor: Color.LimeGreen); //Makes dust in an arc
				d.noGravity = false; 
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White * Projectile.Opacity; // Makes it uneffected by light
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
                Vector2 drawOrigin = new Vector2(8, 8);
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);

                Color color = Color.White * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, 1, SpriteEffects.None, 0);
            }
            return true; //draw og
        }
    }
}