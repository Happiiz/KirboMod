using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class SpaceRangerOrbField : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
			// DisplayName.SetDefault("Space Void");

			Main.projFrames[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 130;
			Projectile.height = 130;
			Projectile.friendly = true;
			Projectile.timeLeft = 120;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.alpha = 100;
			Projectile.ignoreWater = true;

			//wait for no one
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
		}

		public override void AI()
		{
			Lighting.AddLight(Projectile.Center, 0f, 0.8f, 0.8f); //yellow light 8/10 of a torch

			Projectile.ai[0]++;
			if (Projectile.ai[0] == 1)
			{
				SoundEngine.PlaySound(SoundID.Item94, Projectile.position); // electrosphere stop
			}

			if (Projectile.ai[0] % 10 == 0) //when multiple of 10
			{
				for (int i = 0; i < 2; i++) //first semicolon makes inital statement //second declares the conditional that if true will do loop// third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
					Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Electric, speed * 2, Scale: 1f); //Makes dust in a messy circle
					d.noGravity = true;
				}
			}
			
			Projectile.velocity *= 0f;

			//Animation
			if (++Projectile.frameCounter >= 5) //changes frames every 5 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}
		}

        public override void OnKill(int timeLeft)
        {
			for (int i = 0; i < 15; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Electric, speed * 2, default, Scale: 1f); //Makes dust in a messy circle
				d.noGravity = true;
			}
		}
    }
}