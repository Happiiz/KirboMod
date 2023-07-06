using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Dusts
{
	public class RainbowSparkle : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 14, 14);
			dust.alpha = 0;
			//If our texture had 2 different dust on top of each other (a 30x60 pixel image), we might do this:
			//dust.frame = new Rectangle(0, Main.rand.Next(2) * 30, 30, 30);
		}

		public override bool Update(Dust dust)
		{
			dust.rotation += dust.velocity.X * 0.15f; //rotates
			dust.position += dust.velocity *= 0.95f; //slows down

            float light = 0.35f * dust.scale;

            Lighting.AddLight(dust.position, light, light, light);

			if (Math.Abs(dust.velocity.X) < 0.5f && Math.Abs(dust.velocity.Y) < 0.5f) //slowed
			{
                dust.alpha += 9;
            }

            if (dust.alpha >= 255)
			{
				dust.active = false; //kill?
			}
			return false;
		}

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            int r;
            int g;
            int b;
            r = 255 - dust.alpha;
            g = 255 - dust.alpha;
            b = 255 - dust.alpha;
            return new Color(r, g, b, 0); //white color that can fade
        }
    }
}