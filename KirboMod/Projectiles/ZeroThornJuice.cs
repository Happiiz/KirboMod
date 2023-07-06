using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class ZeroThornJuice : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Thorn Juice");
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 180;
			Projectile.tileCollide = false;
			Projectile.penetrate = 1;
		}
		public override void AI()
		{
			Projectile.velocity.Y = Projectile.velocity.Y + 0.4f;
			if (Projectile.velocity.Y >= 12f)
            {
				Projectile.velocity.Y = 12f;
            }
		}

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
			target.AddBuff(BuffID.Poisoned, 600); //poisoned for 10 seconds
			Projectile.Kill();
        }

        public override void Kill(int timeLeft) //when the projectile dies
		{
			for (int i = 0; i < 4; i++)
			{
				Vector2 speed = Main.rand.NextVector2Unit((float)MathHelper.Pi / 4, (float)MathHelper.Pi / 2) * Main.rand.NextFloat(); //arc
				Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.t_Cactus, -speed, Scale: 1f, newColor: Color.LimeGreen); //Makes dust in an arc
				d.noGravity = false; //fall biiih
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}