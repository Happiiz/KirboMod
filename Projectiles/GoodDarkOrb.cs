using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class GoodDarkOrb : ModProjectile 
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dark Orb");
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 62;
			Projectile.height = 62;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.timeLeft = 120;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			
		}

		public override void AI()
		{
            if (Main.rand.NextBool(3)) // happens 1/3 times
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.DarkResidue>(), Projectile.velocity.X * 0.25f, Projectile.velocity.Y * -0.5f, 200, default, 0.8f); //dust
            }
        }

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 20; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
				Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 10, 10); //Makes dust in a messy circle
				d.noGravity = true;
			}
		}
	}
}