using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class HammerThrow : ModProjectile
	{
	    public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			Projectile.width = 120;
			Projectile.height = 60;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 3600; //1 minute
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			Projectile.ai[0]++;
			if (Projectile.ai[0] >= 45)//back to player
            {
				Vector2 moveto = player.Center - Projectile.Center;
				moveto.Normalize();
				moveto *= 10f;
				Projectile.velocity = moveto;

				Rectangle box = Projectile.Hitbox;
				if (box.Intersects(player.Hitbox)) //if touching player
                {
					Projectile.Kill(); //YES KILL
                }
            }
			if (++Projectile.frameCounter >= 3) //changes frames every 3 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}
			if (Projectile.ai[0] == 10) //point of turn and damage decrease
			{
			}
			//halve damage
			if (Projectile.ai[0] == 60)
            {
				Projectile.damage = Projectile.damage / 2;
			}
		}
	}
}