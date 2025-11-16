using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.BossDeaths
{
	public class KingDededead : ModProjectile
	{
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            DrawOffsetX = -78;
            DrawOriginOffsetY = -75;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 500;
            Projectile.hide = true;
        }
        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction;
            Projectile.Opacity = Utils.Remap(Projectile.timeLeft, 60, 0, 1, 0);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }
    }
}