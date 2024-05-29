using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class FighterUppercut : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 6;
		}

		public override void SetDefaults()
		{
			Projectile.width = 70;
			Projectile.height = 130;
			Projectile.friendly = true;
			Projectile.usesLocalNPCImmunity = true; //uses own immunity frames
			Projectile.localNPCHitCooldown = 12; //time before hit again
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 12;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			Projectile.spriteDirection = Projectile.direction; //look in direction

            //animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter < 2)
            {
                Projectile.frame = 0;
            }
            else if (Projectile.frameCounter < 4)
            {
                Projectile.frame = 1;
            }
            else if (Projectile.frameCounter < 6)
            {
                Projectile.frame = 2;
            }
            else if (Projectile.frameCounter < 8)
            {
                Projectile.frame = 3;
            }
            else if (Projectile.frameCounter < 10)
            {
                Projectile.frame = 4;
            }
            else
            {
                Projectile.frame = 5;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity; //independent from light level while still being affected by opacity
        }
    }
}