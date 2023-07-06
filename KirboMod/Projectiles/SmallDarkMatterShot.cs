using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class SmallDarkMatterShot : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dark Matter");
			Main.projFrames[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 46; 
			Projectile.height = 50;
			Projectile.friendly = true;
			Projectile.timeLeft = 500;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
		}

		public override void AI()
		{
			Projectile.spriteDirection = Projectile.direction;

			if (Main.rand.NextBool(5)) // happens 1/5 times
			{
				int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Mod.Find<ModDust>("DarkResidue").Type, 0f, 0f, 0, Color.White, 0.5f); //dust
				Main.dust[dustnumber].velocity *= 0.3f;
				Main.dust[dustnumber].noGravity = true;
			}

			//animation
			if (++Projectile.frameCounter >= 20) //changes frames every 20 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 2)
				{
					Projectile.frame = 0;
				}
			}

			Projectile.velocity.X *= 1.08f; //speed up

			Player player = Main.player[Projectile.owner];

			//go up or down if going fast enough
			if (Math.Abs(Projectile.velocity.X) > 10 && player.whoAmI == Main.myPlayer)
			{
				if (Main.MouseWorld.Y < Projectile.Center.Y)
				{
					Projectile.velocity.Y -= 0.6f;
				}
				else
				{
					Projectile.velocity.Y += 0.6f;
				}
			}

			//cap
			if (Projectile.velocity.X > 40f)
			{
				Projectile.velocity.X = 40f;
			}
			if (Projectile.velocity.X < -40f)
			{
				Projectile.velocity.X = -40f;
			}
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
    }
}