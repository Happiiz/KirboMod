using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BadPlasmaZap : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Plasma Zap");
		}

		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			DrawOffsetX = -4; //make hitbox line up with sprite middle
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 20;
			Projectile.tileCollide = false;
			Projectile.penetrate = 3;
		}
		public override void AI()
		{
			Projectile.spriteDirection = Projectile.direction;
			Projectile.rotation = Projectile.velocity.ToRotation();
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}