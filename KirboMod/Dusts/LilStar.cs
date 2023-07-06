using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Dusts
{
	public class LilStar : ModDust //This one rotates based on direction
    {
		public override void OnSpawn(Dust dust)
		{
            //dust.noLight = true; // Makes the dust emit no light.
			dust.frame = new Rectangle(0, 0, 22, 22);
            //If our texture had 2 different dust on top of each other (a 30x60 pixel image), we might do this:
            //dust.frame = new Rectangle(0, Main.rand.Next(2) * 30, 30, 30);
        }

		public override bool Update(Dust dust)
		{
            if (dust.velocity.X > 0)
            {
                dust.rotation += Math.Abs(dust.velocity.X) * 0.15f; //rotates
            }
            else
            {
                dust.rotation -= Math.Abs(dust.velocity.X) * 0.15f; //rotates
            }

            dust.position += dust.velocity *= 0.9f; //slows down

            if (Math.Abs(dust.velocity.X) < 0.1f && Math.Abs(dust.velocity.Y) < 0.1f) //slowed
            {
                dust.scale *= 0.9f;
            }

            if (dust.scale <= 0.01)
            {
                dust.active = false;
            }
            return false;


        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
			return Color.White; // Makes it uneffected by light
		}
    }
}