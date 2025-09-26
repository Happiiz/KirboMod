using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Dusts
{
	public class MetaBat : ModDust
	{
		//See animation stuff in KirbWorld ModSystem
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 24, 16);
			dust.alpha = 255;
			//If our texture had 2 different dust on top of each other (a 30x60 pixel image), we might do this:
			//dust.frame = new Rectangle(0, Main.rand.Next(2) * 30, 30, 30);
		}

		public override bool Update(Dust dust)
		{
			dust.rotation = dust.velocity.ToRotation() + MathHelper.ToRadians(90); //rotates
			dust.scale += 0.05f;
			dust.position += dust.velocity;

            dust.frame = new Rectangle(0, ModContent.GetInstance<KirboWorld>().frameYoffset, 24, 16);

			if (dust.scale < 3)
			{
				dust.alpha -= 25;
			}
			else
			{
                dust.alpha += 25;

                if (dust.alpha >= 255)
				{
					dust.active = false;
				}
			}
			return false;
		}

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White * (float)((255f - dust.alpha) / 255f); //unaffected by light (but by opacity)
        }
    }
}