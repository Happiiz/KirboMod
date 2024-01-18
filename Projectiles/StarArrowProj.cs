using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class StarArrowProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			
		}
		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			DrawOffsetX = -20;
			DrawOriginOffsetY = -5;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 1200; //20 seconds
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;
			Projectile.alpha = 50;
			Projectile.aiStyle = 0;
			Projectile.ignoreWater = true;
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

            if (Main.rand.NextBool(2)) // happens 1/2 times
            {
                int dustnumber = Dust.NewDust(Projectile.position, 10, 10, DustID.Enchanted_Gold, 0f, 0f, 200, default, 1.5f); //dust
                Main.dust[dustnumber].velocity *= 0.2f;
                Main.dust[dustnumber].noGravity = true;
            }
        }

        public override void OnKill(int timeLeft) //when the projectile dies
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 velocity = Projectile.velocity.RotatedByRandom(MathF.PI / 4) / Main.rand.Next(2, 4); //spread
                Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, DustID.Enchanted_Gold, velocity, Scale: 1.5f); //Makes dust in a messy circle
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