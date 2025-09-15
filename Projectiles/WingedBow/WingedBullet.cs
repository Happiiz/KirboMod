using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.WingedBow
{
	public class WingedBullet : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			
		}
		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			DrawOffsetX = -4;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 1200; //20 seconds
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;
			Projectile.ignoreWater = true;
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft) //when the projectile dies
        {
            for (int i = 0; i < 6; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(3f, 3f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center, 78, speed, Scale: 1.25f); //Makes dust in a messy circle
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position); //impact
            return true; //collision
        }
	}
}