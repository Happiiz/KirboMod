using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using SoundType = Terraria.Audio.SoundType;

namespace KirboMod.Projectiles
{
	public class StarBulletProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 120;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			Projectile.aiStyle = 0;
		}

		public override void AI()
		{
			Projectile.rotation += 0.15f * Projectile.direction; // rotates projectile

            if (Main.rand.NextBool(10)) // happens 1/10 times
            {
                int dustnumber = Dust.NewDust(Projectile.position, 14, 14, ModContent.DustType<Dusts.LilStar>(), Projectile.velocity.X, Projectile.velocity.Y, 0, default, 0.5f); //dust
                Main.dust[dustnumber].velocity *= 0.2f;
                Main.dust[dustnumber].noGravity = true;
            }
        }

        public override void OnKill(int timeLeft) //when the projectile dies
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, ModContent.DustType<Dusts.LilStar>(), speed, Scale: 1f); //Makes dust in a messy circle
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