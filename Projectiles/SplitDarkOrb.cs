using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class SplitDarkOrb : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.timeLeft = 120;
			Projectile.tileCollide = false;
			Projectile.penetrate = 1;
		}

		public override void AI()
		{

		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}

        public override void Kill(int timeLeft)
        {
			for (int i = 0; i < 10; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
				Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 5, 0, default, 0.5f); //Makes dust in a messy circle
				d.noGravity = true;
			}
		}
    }
}