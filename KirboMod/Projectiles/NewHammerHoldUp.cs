using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class NewHammerHoldUp : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 122;
			Projectile.height = 112;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			Projectile.Center = player.Center + new Vector2(player.direction * 20, -20); //stay near player

			Projectile.direction = player.direction;

			Projectile.spriteDirection = Projectile.direction; //look

			//death
			if (player.itemTime == 1)
			{
				Projectile.Kill();
			}
		}
	}
}