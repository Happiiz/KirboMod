using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class PlasmaShield : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 140;
			Projectile.height = 140;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = int.MaxValue;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 5;
			Projectile.alpha = 150;
            Projectile.hide = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
			return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
			return Helper.CheckCircleCollision(targetHitbox, Projectile.Center, Projectile.ai[0]);
        }
        public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			KirbPlayer mplr = player.GetModPlayer<KirbPlayer>();
			Projectile.ai[0] = mplr.PlasmaShieldRadius;
			Projectile.Center = player.Center;
			Lighting.AddLight(Projectile.Center, 0, 1, 0);
		}
    }
}