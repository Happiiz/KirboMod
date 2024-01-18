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
			Projectile.tileCollide = true;
			Projectile.penetrate = 99;
			Projectile.scale = 1f;
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

			//scale with timeLeft
			Projectile.scale = 1 + 0.025f * (20 - Projectile.timeLeft) < 1.5f ? 1 + 0.025f * (20 - Projectile.timeLeft) : 1.5f;

            if (Main.rand.NextBool(5)) // happens 1/5 times
			{
				int dustnumber = Dust.NewDust(Projectile.position, 24, 24, DustID.Torch, 0f, 0f, 200, default, 1.5f); //dust
				Main.dust[dustnumber].velocity *= 0.3f;
				Main.dust[dustnumber].noGravity = true;
			}

            if (Projectile.timeLeft <= 5) //fade when close to death
            {
                Projectile.alpha += 51;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //stop projectile
            Projectile.velocity *= 0.1f;

            Projectile.timeLeft = 5;

            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
			target.AddBuff(BuffID.OnFire, 180);
	    }

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White * Projectile.Opacity; // Makes it uneffected by light (still can be transparent)
		}
	}
}