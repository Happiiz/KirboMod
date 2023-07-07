using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class GoodBioSlash : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 3;
			// DisplayName.SetDefault("Purple Slash");
		}

		public override void SetDefaults()
		{
			Projectile.width = 100;
			Projectile.height = 70;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.timeLeft = 15;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.usesLocalNPCImmunity = true; //uses own immunity frames
			Projectile.localNPCHitCooldown = 15; //time to hit the same npc again
			Projectile.minion = true; //deal summon damage
		}

		public override void AI()
		{
			if (Projectile.velocity.X >= 0f) //makes the projectile move a set amount depending on the velocity
			{
				Projectile.velocity.X = 20f;
			}
			else
			{
				Projectile.velocity.X = -20f;
			}
			Projectile.velocity.Y = 0f; //no Y movement

			Projectile.spriteDirection = Projectile.direction;

			if (++Projectile.frameCounter >= 5) //changes frames every 5 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}
		}
	}
}