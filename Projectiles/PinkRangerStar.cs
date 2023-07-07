using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class PinkRangerStar : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 36;
			Projectile.height = 36;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 180;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;
			
		}
		public override void AI()
		{
			Lighting.AddLight(Projectile.Center, 0.255f, 0.255f, 0f);
			Projectile.rotation += 0.3f * (float)Projectile.direction; // rotates projectile
			if (Main.rand.NextBool(5)) // happens 1/5 times
			{
				int dustnumber = Dust.NewDust(Projectile.position, 36, 36, DustID.Confetti_Pink, 0f, 0f, 200, default, 1f); //dust
				Main.dust[dustnumber].velocity *= 0.3f;
			}

			Projectile.ai[0]++;
			if (Projectile.ai[0] == 1) //if ai equal 1
            {
				SoundEngine.PlaySound(SoundID.MaxMana, Projectile.position); //star sound
			}
		}

        public override void Kill(int timeLeft) //when the projectile dies
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * 2, DustID.Confetti_Pink, speed, Scale: 1f); //Makes dust in a messy circle
            }

            for (int i = 0; i < 5; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
                Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * 2, ModContent.DustType<Dusts.LilStar>(), speed, Scale: 1f); //Makes dust in a messy circle
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			SoundEngine.PlaySound(SoundID.Item10, Projectile.position); //impact

			return true; //collision
		}

        public override Color? GetAlpha(Color lightColor)
        {
			return Color.White; // Makes it uneffected by light
        }
    }
}