using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Dusts
{
	public class CrystalBit : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = false; //has gravity
			dust.frame = new Rectangle(0, 0, 12, 10);
			//If our texture had 2 different dust on top of each other (a 30x60 pixel image), we might do this:
			//dust.frame = new Rectangle(0, Main.rand.Next(2) * 30, 30, 30);
		}

		public override bool Update(Dust dust)
		{
			dust.rotation += dust.velocity.X * 0.15f; //rotates

			dust.position += dust.velocity; //actually move

			//gravity
			dust.velocity.Y += 0.2f; //fall
			if (dust.velocity.Y > 10) //cap fall speed
			{
				dust.velocity.Y = 10;
			}

			dust.scale *= 0.975f; //gets smaller

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