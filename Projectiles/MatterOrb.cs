using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class MatterOrb : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;

            //for afterimages
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 240;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
		}
		public override void AI()
		{
			Player player = Main.player[(int)Projectile.ai[1]]; //chooses player that was already being targeted by npc

			if (Main.netMode == NetmodeID.SinglePlayer)
			{
				player = Main.player[Main.myPlayer];
			}

			Vector2 move = player.Center - Projectile.Center; 

			Projectile.ai[0]++;

			if (Projectile.ai[0] < 20)
			{
				Projectile.velocity *= 0.95f;
			}
			else if (Projectile.ai[0] == 20)
            {
				Projectile.hostile = true;

				if (move.X > 0) //going right
				{
					Projectile.velocity.X = 30 + MathF.Cos(Main.GlobalTimeWrappedHourly * 2) * 25;
				}
                else //going left
                {
                    Projectile.velocity.X = -30 + MathF.Cos(Main.GlobalTimeWrappedHourly * 2) * 25;
                }

                if (Projectile.velocity.Y < 0)
				{
					Projectile.ai[2] = 1;
					Projectile.velocity.Y = -50; //start up
				}
				else
				{
                    Projectile.velocity.Y = 50; //start down
                }
            }
			else if (Projectile.ai[0] > 20) 
            {
				if (Projectile.ai[2] == 1) //go down
				{
					Projectile.velocity.Y += 4f;

					Projectile.velocity.X *= 0.98f;
				}
                else //go up
                {
                    Projectile.velocity.Y -= 4f;

                    Projectile.velocity.X *= 0.98f;
                }
            }

			if (++Projectile.frameCounter >= 5) //changes frames every 5 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}
		}
        public override void OnKill(int timeLeft) //when the projectile dies
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, ModContent.DustType<Dusts.DarkResidue>(), speed, Scale: 1f); //Makes dust in a messy circle
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
			return Color.White; //white
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
            for (int k = 1; k < Projectile.oldPos.Length; k++) //start at 1 so not ontop of actual projectile
            {
                Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + new Vector2(16, 64 + Projectile.gfxOffY);

                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle(0, Projectile.frame * Projectile.height, Projectile.width, Projectile.height), 
					color, Projectile.rotation, drawOrigin, 1, SpriteEffects.None, 0);
            }

            return true; //draw og
        }
    }
}