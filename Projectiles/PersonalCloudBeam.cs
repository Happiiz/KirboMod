using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class PersonalCloudBeam : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.MinionShot[Type] = true;
        }

		public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 50;
			Projectile.friendly = true;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10; //default
		}

		public override void AI()
		{
			Projectile.rotation += 0.2f * (float)Projectile.direction; // rotates projectile
			if (++Projectile.frameCounter >= 2) //changes frames every 2 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 2)
				{
					Projectile.frame = 0;
				}
			}
		}
        public override bool PreDraw(ref Color lightColor)
        {
			VFX.DrawElectricOrb(Projectile.Center, Vector2.One * 1.3f, Projectile.Opacity, Projectile.rotation);
			return false;
        }
        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}