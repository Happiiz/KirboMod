using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class CrystalShardProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Crystal Shard"); 
		}
		public override void SetDefaults()
		{
			Projectile.width = 22;
			Projectile.height = 22;
			DrawOffsetX = -21;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;
			Projectile.alpha = 50;
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

			if (Main.rand.NextBool(5)) // happens 1/5 times
			{
				int dustnumber = Dust.NewDust(Projectile.position, 22, 22, ModContent.DustType<Dusts.RainbowSparkle>(), 0f, 0f, 200, default, 1f); //dust
				Main.dust[dustnumber].velocity *= 0.3f;
			}
		}

        public override void Kill(int timeLeft)
        {
			SoundEngine.PlaySound(SoundID.Item27, Projectile.Center); //crystal break

			for (int i = 0; i < 5; i++)
			{
				Vector2 speed = Main.rand.NextVector2Circular(8f, 8f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10)), ModContent.DustType<Dusts.RainbowSparkle>(), speed, 0, default, Scale: 1f); //Makes dust in a messy circle
			}
            for (int i = 0; i < 4; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(8f, 8f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10)), ModContent.DustType<Dusts.CrystalBit>(), speed, 0, default, Scale: 1f); //Makes dust in a messy circle
            }
        }

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}