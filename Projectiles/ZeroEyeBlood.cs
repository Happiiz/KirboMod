using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class ZeroEyeBlood : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Blood Trail");
		}
		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 180;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
		}
		public override void AI()
		{
			Projectile.ai[0] += MathF.PI / 60;

			Projectile.scale = 1 + MathF.Sin(Projectile.ai[0]) * 0.1f;
		}

        public override void OnKill(int timeLeft)
        {
			for (int i = 0; i < 5; i++)
			{
				Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, Mod.Find<ModDust>("Redsidue").Type, -speed); //Makes dust in a circle
				d.noGravity = true; 
			}
		}

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}