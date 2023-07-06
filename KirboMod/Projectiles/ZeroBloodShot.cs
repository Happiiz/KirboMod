using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class ZeroBloodShot : ModProjectile
    {
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Blood Shot");
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 30;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 500;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
		}

		public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction;

            if (Main.rand.NextBool(5)) // happens 1/5 times
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.Redsidue>(), 0f, 0f, 0, Color.White, 0.5f); //dust
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
                Main.dust[dustnumber].GetColor(Color.White);
            }
        }
		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
    }
}