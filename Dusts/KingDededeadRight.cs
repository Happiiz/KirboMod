using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Dusts
{
	public class KingDededeadRight : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.frame = new Rectangle(18, 30, 256, 250);
			//If our texture had 2 different dust on top of each other (a 30x60 pixel image), we might do this:
			//dust.frame = new Rectangle(0, Main.rand.Next(2) * 30, 30, 30);
		}

		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity; //move
			dust.scale = 1;
			return false;
		}
    }
}