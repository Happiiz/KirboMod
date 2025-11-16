using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.BossDeaths
{
	public class ZeroEyeless : ModProjectile
	{
        public override void SetDefaults()
        {
            Projectile.width = 398;
            Projectile.height = 398;
            DrawOffsetX = -198;
            DrawOriginOffsetY = -200;
            Projectile.tileCollide = false;
			Projectile.timeLeft = 500;
            Projectile.hide = true;
        }

        public override void AI()
        {
			Projectile.Opacity = Utils.Remap(Projectile.timeLeft, 60, 0, 1, 0);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            Main.instance.DrawCacheProjsBehindNPCs.Add(index);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
    }
}