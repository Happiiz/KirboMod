using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Artist
{
	public class PaintBlob : ModProjectile //Apple for swishy tree
	{
        List<Color> availableColors = [Color.Red, Color.Orange, Color.Yellow, Color.DeepPink, Color.Lime, Color.Blue, Color.BlueViolet];
        Color chosenColor = Color.Red;
        public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 3;
		}

		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = true; 
			Projectile.penetrate = 1;

			Projectile.frame = Main.rand.Next(Main.projFrames[Projectile.type]);

            chosenColor = availableColors[Main.rand.Next(availableColors.Count)];
        }
		public override void AI()
		{
			Projectile.velocity.Y += 0.10f;
			if (Projectile.velocity.Y >= 10)
            {
				Projectile.velocity.Y = 10;
            }

			Projectile.rotation += 0.1f;
		}
        public override void OnKill(int timeLeft) //when the projectile dies
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(6f, 6f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Marble, speed, Scale: 1f, newColor: chosenColor); //Makes dust in a messy circle
            }

			SoundEngine.PlaySound(SoundID.Dig.WithVolumeScale(0.5f), Projectile.Center);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return chosenColor.MultiplyRGB(lightColor);
        }
    }
}