using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class SpaceRangerOrb : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 2;
			// DisplayName.SetDefault("Space Orb");
        }

		public override void SetDefaults()
		{
			Projectile.width = 62;
			Projectile.height = 62;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.timeLeft = 60;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
		}

		public override void AI()
		{
			Projectile.ai[0]++;

			if (Projectile.ai[0] % 10 == 0) //when multiple of 10
			{
				Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Electric, null, default, Scale: 1f); //Makes dust 
				d.noGravity = true;
			}

			//Animation
			if (++Projectile.frameCounter >= 4) //changes frames every 4 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}

            //Dust
            Projectile.rotation = Projectile.velocity.ToRotation();
            int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 200, default, 1f); //dust
            Main.dust[dustnumber].velocity *= 0.3f;
            Main.dust[dustnumber].noGravity = true;
        }

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 30; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
				Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Electric, speed * 10, 10); //Makes dust in a messy circle
				d.noGravity = true;
			}

			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0, ModContent.ProjectileType<Projectiles.SpaceRangerOrbField>(), Projectile.damage / 4, 0, Projectile.owner, 0, 0);
		}
    }
}