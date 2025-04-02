using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BeamBad : ModProjectile
	{
		const int Duration = 9;
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 15;
			Projectile.height = 15;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = Duration;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
		}
        public override bool PreDraw(ref Color lightColor)
        {
			VFX.DrawWaddleDooBeam(Projectile.Center, Utils.Remap(Projectile.localAI[0], 0, Duration - 1, 0.2f, 1.5f), Projectile.Opacity);
			//VFX.DrawElectricOrb(Projectile.Center, new Vector2(1), Projectile.Opacity,Projectile.rotation);
			return false;
        }
        public override void AI()
		{
			Projectile.localAI[0]++;
			if (Projectile.damage > 24)
            {
				Projectile.damage = 24;
            }
			Projectile.rotation += 0.2f * (float)Projectile.direction; // rotates projectile
			if (++Projectile.frameCounter >= 1) //changes frames every 1 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 2)
				{
					Projectile.frame = 0;
				}
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}