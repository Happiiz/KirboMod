using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class ZeroBloodPellet : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Blood Pellet");
		}
		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			DrawOffsetX = 4;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 60;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1;
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
		}

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}