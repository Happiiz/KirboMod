using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class HardenedFistProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{ 

		}

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = true;
			Projectile.timeLeft = 7;
			Projectile.tileCollide = false;
			Projectile.penetrate = 99;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.usesLocalNPCImmunity = true; //uses own immunity frames
			Projectile.localNPCHitCooldown = 7; //time before hit again
			Projectile.alpha = 60;
		}
		public override void AI()
		{
            Projectile.rotation = Projectile.velocity.ToRotation();
			Player player = Main.player[Projectile.owner];
		}

		//No combo stuff here because this one doesn't combo
    }
}