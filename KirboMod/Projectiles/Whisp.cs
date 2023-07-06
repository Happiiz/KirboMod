using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class Whisp : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 8;
		}

		public override void SetDefaults()
		{
			Projectile.width = 46;
			Projectile.height = 40;
			//drawOffsetX = -13;
			//drawOriginOffsetY = -13;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 500;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
		}

		public override void AI()
		{
			Projectile.spriteDirection = Projectile.direction;
			if (++Projectile.frameCounter >= 3) //changes frames every 3 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}
			if (Projectile.scale >= 1f)
            {
				Projectile.scale = 1f;
            }
		}
	}
}