using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Dusts
{
	public class Flake : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 18, 18);
			dust.alpha = 0;
			//If our texture had 2 different dust on top of each other (a 30x60 pixel image), we might do this:
			//dust.frame = new Rectangle(0, Main.rand.Next(2) * 30, 30, 30);
		}

		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity *= 0.95f; //slows down
			dust.alpha += 4;
			if(dust.alpha > 240)
			{
				dust.active = false;
			}
            return false;
		}
    }
}