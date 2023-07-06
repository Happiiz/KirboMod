using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class RisingRainbowBeam : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Casting light");
		}
		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.timeLeft = 120; //seconds = timeLeft - extraUpdates / 60
			Projectile.tileCollide = false; 
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.ignoreWater = true;

			Projectile.extraUpdates = 20; //cycle through code multiple times in one tick
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.damage = 0;

			Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0.5f); //white light

			for (int i = 0; i < 1; i++)
			{
				Vector2 position = Projectile.position;
				position -= Projectile.velocity * ((float)i * 0.25f);
				Projectile.alpha = 255;
				int deez = Dust.NewDust(position, 10, 10, ModContent.DustType<Dusts.RainbowSparkle>(), 0, 0, 0, Color.White);
				//int deez = Dust.NewDust(position, 1, 1, DustID.RedTorch);
				Main.dust[deez].position = position;
				Main.dust[deez].position.X += Projectile.width / 2;
				Main.dust[deez].position.Y += Projectile.height / 2;
				Main.dust[deez].scale = 1f; 
				Main.dust[deez].velocity *= 0.2f;
				Main.dust[deez].noGravity = true;
			}
		}

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; //make it unaffected by light
        }
	}
}