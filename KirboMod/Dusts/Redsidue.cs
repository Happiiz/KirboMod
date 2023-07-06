using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Dusts
{
	public class Redsidue : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 14, 14);
			//If our texture had 2 different dust on top of each other (a 30x60 pixel image), we might do this:
			//dust.frame = new Rectangle(0, Main.rand.Next(2) * 30, 30, 30);
		}

		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity *= 0.95f; //slows down
			dust.scale *= 0.98f;

            if (dust.scale <= 0.001)
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