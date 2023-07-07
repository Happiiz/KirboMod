using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Dusts
{
	public class BoldStar : ModDust //This one can't fall
	{
		public override void OnSpawn(Dust dust)
		{
			dust.frame = new Rectangle(0, 0, 22, 22);
			//If our texture had 2 different dust on top of each other (a 30x60 pixel image), we might do this:
			//dust.frame = new Rectangle(0, Main.rand.Next(2) * 30, 30, 30);
		}

		public override bool Update(Dust dust)
        {
            //dust.rotation += MathHelper.ToRadians(-12); //rotates
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