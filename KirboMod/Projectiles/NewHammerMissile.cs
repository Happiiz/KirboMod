using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class NewHammerMissile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Hammer Missile");
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 42;
			Projectile.height = 26;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.timeLeft = 120;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;
			
		}
		public override void AI()
		{
			if (Main.rand.NextBool(5)) // happens 1/5 times
			{
				int dustnumber = Dust.NewDust(Projectile.position, 1, 26, DustID.Smoke, Projectile.velocity.X * -1, 0f, 0, default, 2f); //dust
				Main.dust[dustnumber].velocity *= 0.3f;
			}

			Projectile.spriteDirection = Projectile.direction; //face way its facing
		}
        public override void Kill(int timeLeft) //when the projectile dies
        {
			for (int i = 0; i < 10; i++)
			{
				Vector2 speed = Main.rand.NextVector2Circular(4f, 4f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.position, DustID.Smoke, speed, Scale: 2f); //Makes dust in a messy circle

				Vector2 speed2 = Main.rand.NextVector2Circular(4f, 4f); //circle
				Dust u = Dust.NewDustPerfect(Projectile.position, DustID.Torch, speed2, Scale: 2f); //Makes dust in a messy circle
				u.noGravity = true;
			}

			SoundEngine.PlaySound(SoundID.Item14, Projectile.Center); //explosion
		}
    }
}