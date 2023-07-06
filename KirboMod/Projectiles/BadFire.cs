using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BadFire : ModProjectile
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
			Projectile.timeLeft = 20;
			Projectile.tileCollide = false;
			Projectile.penetrate = 99;
			Projectile.scale = 1f;
			Projectile.alpha = 128;
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.scale = Projectile.scale + 0.025f;

			if (Main.rand.Next(5) == 1) // happens 1/5 times
			{
				int dustnumber = Dust.NewDust(Projectile.position, 24, 24, DustID.Torch, 0f, 0f, 200, default, 1.5f); //dust
				Main.dust[dustnumber].velocity *= 0.3f;
				Main.dust[dustnumber].noGravity = true;
			}
		}

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {

			target.AddBuff(BuffID.OnFire, 180);
		
	    }

		public override Color? GetAlpha(Color lightColor)
		{
			Projectile.alpha = 50;
			return Color.White; // Makes it uneffected by light
		}
	}
}