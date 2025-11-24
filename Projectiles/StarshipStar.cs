using KirboMod.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class StarshipStar : Star //same as regular but doesn't receive class buffs, has less sparkles and fades in faster
	{
        public override string Texture => "KirboMod/Projectiles/TripleStarStar";
        public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
            Projectile.CloneDefaults(ModContent.ProjectileType<Star>());
			Projectile.DamageType = DamageClass.Generic;
		}
		public override void AI()
		{
			if (Projectile.localAI[0] == 0)
			{
				Projectile.soundDelay = 20 + Main.rand.Next(40);
				Projectile.rotation = Main.rand.NextFloat(MathF.Tau);
				Projectile.localAI[0] = 1;
			}
			Lighting.AddLight(Projectile.Center, 0.8f, 0.8f, 0f); //yellow light half of a torch
			Projectile.Opacity += 0.2f; //twice as mucha as regular star

			if (Main.rand.NextBool(6)) // happens 1/6 times
			{
				Sparkle.NewSparkle(Projectile.Center + Main.rand.NextVector2Circular(20,20) - Projectile.velocity, Main.rand.NextBool(3, 5) ? Color.Yellow : Color.Blue, new Vector2(1, 1.5f), Projectile.velocity * 0.1f + Main.rand.NextVector2Circular(30, 30) / 10, 40, new Vector2(2, 2));
			}

			if (Projectile.soundDelay == 0)
			{
				Projectile.soundDelay = 20 + Main.rand.Next(40);
				SoundEngine.PlaySound(SoundID.Item9 with { MaxInstances = 0 }, Projectile.Center);
			}
			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.005f * Projectile.direction;

			for (int i = 0; i < 2; i++)
			{
				if (Main.rand.NextBool(3))
				{
					Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.BlueFairy, 0f, 0f, 127);
					dust.noGravity = true;
				}
			}
        }
    }
}