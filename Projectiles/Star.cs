using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class Star : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 50; 
			Projectile.height = 50;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 180;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;
		}
		public override void AI()
		{
			Lighting.AddLight(Projectile.Center, 0.8f, 0.8f, 0f); //yellow light half of a torch

			Projectile.rotation += 0.3f * (float)Projectile.direction; // rotates projectile
			if (Main.rand.NextBool(5)) // happens 1/5 times
			{
				int dustnumber = Dust.NewDust(Projectile.position, 50, 50, ModContent.DustType<Dusts.LilStar>(), 0f, 0f, 200, default, 0.5f); //dust
				Main.dust[dustnumber].velocity *= 0.3f;
			}
        }
        public override void Kill(int timeLeft) //when the projectile dies
        {
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