using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using SoundType = Terraria.Audio.SoundType;

namespace KirboMod.Projectiles
{
	public class Puff : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 120;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;
		}

		public override void AI()
		{
			Projectile.rotation += Projectile.velocity.X * 0.1f;
			if (++Projectile.frameCounter >= 3) //changes frames every 3 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}
			if (Projectile.timeLeft <= 30) //shrink
            {
				Projectile.scale *= 0.95f;
            }
		}

		public override void OnKill(int timeLeft)
		{
			if (Projectile.timeLeft <= 0)
			{
				for (int i = 0; i < 4; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle                     //(int) is required
					Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, speed * 2, Scale: 1f); //Makes scattered dust
				}
			}
		}
	}
}