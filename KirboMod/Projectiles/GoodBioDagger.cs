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
            // DisplayName.SetDefault("Throwing Knife");
        }
		public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.timeLeft = 40;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.minion = true; //deal summon damage
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
		}
	}
}