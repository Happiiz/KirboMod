using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class MetaSlash : ModProjectile
	{
		public override void SetStaticDefaults()
		{ 

		}

		public override void SetDefaults()
		{
			Projectile.width = 64;
			Projectile.height = 70;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.timeLeft = 36;
			Projectile.tileCollide = false;
			Projectile.penetrate = 99;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 36; //wait 12 frames before dealing damage again, but it will be dead so it can only hit once per npc
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.alpha += 6;
			Projectile.velocity *= 0.925f;
		}
	}
}