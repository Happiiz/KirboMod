using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Dusts
{
	public class RareStoneBit : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = false; //has gravity
			dust.frame = new Rectangle(0, Main.rand.Next(0,5) * 10, 12, 10);
			//If our texture had 2 different dust on top of each other (a 30x60 pixel image), we might do this:
			//dust.frame = new Rectangle(0, Main.rand.Next(2) * 30, 30, 30);
		}

		public override bool Update(Dust dust)
		{
			dust.rotation += dust.velocity.X * 0.15f; //rotates

			dust.position += dust.velocity; //actually move

			//gravity
			dust.velocity.Y += 0.1f; //fall
			if (dust.velocity.Y > 5) //cap fall speed
			{
				dust.velocity.Y = 5;
			}

            float light = 0.35f * dust.scale;

            Lighting.AddLight(dust.position, light, light * 0.6f, light * 0.6f); //orange-ish light

            dust.scale *= 0.950f; //gets smaller

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