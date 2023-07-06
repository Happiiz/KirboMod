using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class RainbowSlash : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = true;
			Projectile.usesLocalNPCImmunity = true; //uses own immunity frames
			Projectile.localNPCHitCooldown = 10; //time before hit again
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
            //Projectile.ownerHitCheck = true; //check if owner has line of sight to hit
        }

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) - Projectile.velocity; //stay near me

			Projectile.spriteDirection = Projectile.direction; //look

            //death
            if (player.itemTime == 1 || player.active == false || player.dead == true) //done/can't attack
            {
                Projectile.Kill(); //kill projectile
            }

            //animation
            Projectile.frameCounter++; //slash
			if (Projectile.frameCounter < 2)
			{
				Projectile.frame = 0; //start
			}
			else if (Projectile.frameCounter < 4)
			{
				Projectile.frame = 1; //inital slash
			}
			else if (Projectile.frameCounter < 6)
			{
				Projectile.frame = 2; //mid slash
			}
			else if (Projectile.frameCounter < 8)
			{
				Projectile.frame = 3; //falling slash
			}
			else
			{
				Projectile.frame = 4; //tail end slash
			}

			//Dusts

			/*if (Projectile.ai[0] == 0)
			{
				float DustY = player.Center.Y - 100;

				for (int i = 0; i < 5; i++)
				{
					//Emit dusts when the sword is swung
					Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.RainbowSparkle>(),
						player.direction * Main.rand.Next(2, 4));
				}
			}*/

			Projectile.ai[0]++;

            //REFLECTING
            for (int i = 0; i <= Main.maxProjectiles; i++)
            {
				Projectile proj = Main.projectile[i];

				//Very specific requirements (proj is reflected, projectile is slash)
				if (Projectile.Hitbox.Intersects(proj.Hitbox) && proj.hostile == true && proj.friendly == false && Projectile.frame == 2)
				{
					if (proj.type == ModContent.ProjectileType<DarkOrb>())
					{
						proj.Kill();
						if (Main.expertMode == false)
						{
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), proj.Center.X, proj.Center.Y, Projectile.direction * 10, 0, ModContent.ProjectileType<SplitDarkOrb>(), 500, 0f, Projectile.owner);
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), proj.Center.X, proj.Center.Y, Projectile.direction * 10, -5, ModContent.ProjectileType<SplitDarkOrb>(), 500, 0f, Projectile.owner);
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), proj.Center.X, proj.Center.Y, Projectile.direction * 10, 5, ModContent.ProjectileType<SplitDarkOrb>(), 500, 0f, Projectile.owner);
						}
						else
						{
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), proj.Center.X, proj.Center.Y, Projectile.direction * 10, 0, ModContent.ProjectileType<SplitDarkOrb>(), 1000, 0f, Projectile.owner);
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), proj.Center.X, proj.Center.Y, Projectile.direction * 10, -5, ModContent.ProjectileType<SplitDarkOrb>(), 1000, 0f, Projectile.owner);
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), proj.Center.X, proj.Center.Y, Projectile.direction * 10, 5, ModContent.ProjectileType<SplitDarkOrb>(), 1000, 0f, Projectile.owner);
						}
					}

					else if (proj.type == ModContent.ProjectileType<MatterOrb>())
					{
						proj.velocity *= -1;
						proj.friendly = true;
						proj.hostile = false;

						if (Main.expertMode == true)
						{
							proj.damage = 100; //deal more damage
						}
						else
                        {
							proj.damage = 50;
						}
					}
				}
            }
		}

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}