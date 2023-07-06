using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class StormTornadoCloud : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 60;
			Projectile.height = 60;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 180;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;

			//only waits for projectiles of own type
			Projectile.usesIDStaticNPCImmunity = true;
			Projectile.idStaticNPCHitCooldown = 10;
		}
		public override void AI()
		{
			Projectile.velocity *= 0.9f; //slow
			Projectile.rotation += Projectile.direction * 0.04f; // rotates projectile depending on direction it's facing

			if (Projectile.timeLeft <= 60) //no more time
            {
				Projectile.alpha += 5;
            }

			/*if (++projectile.frameCounter >= 15) //changes frames every 15 ticks 
			{
				projectile.frameCounter = 0;
				if (++projectile.frame >= Main.projFrames[projectile.type])
				{
					projectile.frame = 0;
				}
			}*/
		}
    }
}