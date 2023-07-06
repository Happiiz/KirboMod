using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class HeroSlash : ModProjectile
	{
		public override void SetStaticDefaults()
		{ 

		}

		public override void SetDefaults()
		{
			Projectile.width = 52;
			Projectile.height = 26;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.timeLeft = 12;
			Projectile.tileCollide = false;
			Projectile.penetrate = 99;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 12; //wait 12 frames before dealing damage again, but it will be dead so it can only hit once per npc
		}
		public override void AI()
		{
			Projectile.velocity.Y = 0f;
			Projectile.spriteDirection = Projectile.direction;
			if (Projectile.velocity.X >= 0f)
            {
				Projectile.velocity.X = 10f;
            }
			else
            {
				Projectile.velocity.X = -10f;
            }
		}
	}
}