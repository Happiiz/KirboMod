using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class FinalCutterBlade : ModProjectile
	{
		private int backtrack;
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			Projectile.width = 80;
			Projectile.height = 38; //30 less than sprite
			DrawOriginOffsetY = -11;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true; //allows to have npc immunity frames on its own accord
			Projectile.localNPCHitCooldown = 10; //time until it can damage again regardless if a projectile just struck the target
		}

		public override void AI()
		{
			Projectile.ai[0]++;
			Projectile.velocity.Y = 0f;
			Player player = Main.player[Projectile.owner];

			if (Projectile.ai[0] == 1) //move before turn
			{
				Projectile.velocity.X = player.direction * 15;
			}

			if (++Projectile.frameCounter >= 2) //changes frames every 3 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
					SoundEngine.PlaySound(SoundID.Run.WithVolumeScale(0.5f), Projectile.Center);
				}
			}
			if (Projectile.ai[0] == 10) //point of turn and damage decrease
			{
				Projectile.tileCollide = true; //collide with tiles

				if (Projectile.velocity.X == 15f)
				{
					backtrack = 0;
				}
				else if (Projectile.velocity.X == -15f)
				{
					backtrack = 1;
				}
			}
			//invert direction
			if (backtrack == 0)
			{
				Projectile.velocity.X -= 0.25f;
			}
			else
			{
				Projectile.velocity.X += 0.25f;
			}
			if (Projectile.velocity.X > 15)
			{
				Projectile.velocity.X = 15;
			}
			if (Projectile.velocity.X < -15)
			{
				Projectile.velocity.X = -15;
			}
			//double damage
			if (Projectile.ai[0] == 80)
			{
				Projectile.damage = Projectile.damage * 2;
			}
		}
	}
}