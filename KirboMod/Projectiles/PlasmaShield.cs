using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class PlasmaShield : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 140;
			Projectile.height = 140;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 2;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.alpha = 150;
            Projectile.hide = true;
        }

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			Projectile.Center = player.Center;
			Lighting.AddLight(Projectile.Center, 0, 1, 0);
		}

		public override bool? CanCutTiles()
		{
			return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity; //unaffected by light, but can be transparent
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }
    }
}