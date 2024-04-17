using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class ChainBombExplosion : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 150;
			Projectile.height = 150;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 60;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.alpha = 50;
		}
		public override void AI()
		{
			Projectile.ai[0]++;
			if (Projectile.ai[0] == 1)
			{
				SoundEngine.PlaySound(SoundID.Item14, Projectile.position); //bomb sound
			}

			Projectile.Opacity = Utils.GetLerpValue(0, 10, Projectile.timeLeft, true);

            Projectile.scale = 1 + Utils.GetLerpValue(10, 0, Projectile.timeLeft, true);

            Lighting.AddLight(Projectile.Center, 1f, 1f, 0);

			if (Projectile.ai[0] == 1)
			{
				for (int i = 0; i <= Main.maxProjectiles; i++)
				{
					Projectile proj = Main.projectile[i];

					if (Vector2.Distance(Projectile.Center, proj.Center) < 200 && proj.type == ModContent.ProjectileType<ChainBombProj>() && Projectile.whoAmI != proj.whoAmI)
					{
						proj.Kill();
					}
				}
			}
		}

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
    }
}