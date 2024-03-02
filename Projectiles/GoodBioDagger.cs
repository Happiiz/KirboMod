using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class GoodBioDagger : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            ProjectileID.Sets.MinionShot[Type] = true;
        }
		public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.timeLeft = 120;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
            Projectile.DamageType = DamageClass.Summon;
        }
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
		}
	}
}