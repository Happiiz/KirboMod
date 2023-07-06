using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Dusts
{
	public class CyborgArcherLaser : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 10, 10);
			//If our texture had 2 different dust on top of each other (a 30x60 pixel image), we might do this:
			//dust.frame = new Rectangle(0, Main.rand.Next(2) * 30, 30, 30);
		}

		public override bool Update(Dust dust)
		{
			dust.velocity *= 0;
			dust.scale *= 0.8f;
			if (dust.scale <= 0.05f)
			{
				dust.active = false; //kill dust
			}

			Lighting.AddLight(dust.position, 0.1f, 0f, 0f); //red
			return false;
		}

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
			return Color.White; // Makes it uneffected by light
		}
    }
}