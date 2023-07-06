using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class MinionIce : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			
		}
		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.friendly = true;
			Projectile.minion = true;
			Projectile.timeLeft = 10;
			Projectile.tileCollide = false;
			Projectile.penetrate = 99;
			Projectile.scale = 1f;

			// local immunity makes it wait for it's own cooldown
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
		}
		public override void AI()
		{
			Projectile.scale = Projectile.scale + 0.05f;
			if (Main.rand.Next(5) == 1) // happens 1/5 times
			{
				int dustnumber = Dust.NewDust(Projectile.position, 24, 24, ModContent.DustType<Dusts.Flake>(), Projectile.velocity.X * 0.02f, Projectile.velocity.Y * 0.02f, 200, default, 1f); //dust
				Main.dust[dustnumber].velocity *= 0.3f;
				Main.dust[dustnumber].noGravity = true;
				Main.dust[dustnumber].alpha = 0;
			}
		}
	}
}