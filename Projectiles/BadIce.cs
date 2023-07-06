using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{               
	public class BadIce : ModProjectile //bad ice cream lol
	{
		public override void SetStaticDefaults()
		{
			
		}
		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 10;
			Projectile.tileCollide = false;
			Projectile.penetrate = 99;
			Projectile.scale = 1f;
		}
		public override void AI()
		{
			Projectile.scale = Projectile.scale + 0.05f;
			if (Main.rand.Next(5) == 1) // happens 1/5 times
			{
				int dustnumber = Dust.NewDust(Projectile.position, 24, 24, DustID.Snow, 0f, 0f, 200, default, 1f); //dust
				Main.dust[dustnumber].velocity *= 0.3f;
				Main.dust[dustnumber].noGravity = true;
				Main.dust[dustnumber].alpha = 0;
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			if (Main.expertMode) //guranteed
			{
				target.AddBuff(BuffID.Frozen, 30);
			}
			else
			{
				if (Main.rand.NextBool(3)) // 1/3
				{
					target.AddBuff(BuffID.Frozen, 30);
				}
			}
		}
	}
}