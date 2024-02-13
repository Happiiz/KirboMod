using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class ZeroScreenBlood : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Blood Shot");
		}
		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 60;
			Projectile.tileCollide = false;
			Projectile.penetrate = 1;
			Projectile.scale = 0.1f;
            Projectile.hide = true;
        }
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.scale = Projectile.scale + 0.015f;

			if (Projectile.scale < 0.95f)
            {
				Projectile.hostile = false;
			}
			else
            {
				Projectile.hostile = true;
			}
		}

        public override void OnKill(int timeLeft)
        {
			for (int i = 0; i < 20; i++)
			{
				Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, Mod.Find<ModDust>("Redsidue").Type, -speed); //Makes dust in a circle
				d.noGravity = true; 
			}

			//summon projectiles in 8 directions

			if (Projectile.ai[1] == 0) //normal circle
			{
                for (int i = 0; i < 8; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                {
                    float rotationalOffset = MathHelper.ToRadians(i * 45f); //convert degrees to radians

                    float projX = Projectile.Center.X + (float)Math.Cos(rotationalOffset) * 2;
                    float projY = Projectile.Center.Y + (float)Math.Sin(rotationalOffset) * 2;

                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), projX, projY, Vector2.Zero.X, Vector2.Zero.Y, ModContent.ProjectileType<ZeroBloodPellet>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Vector2 direction = Main.projectile[proj].Center - Projectile.Center;
                    direction.Normalize(); //unit of 1
                    direction *= 35; //speed of 35
					Main.projectile[proj].velocity = direction;
                }
            }
			else //offset circle
			{
                for (int i = 0; i < 8; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                {
                    float rotationalOffset = MathHelper.ToRadians((i * 45f) + 22.5f); //convert degrees to radians

                    float projX = Projectile.Center.X + (float)Math.Cos(rotationalOffset) * 2;
                    float projY = Projectile.Center.Y + (float)Math.Sin(rotationalOffset) * 2;

                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), projX, projY, Vector2.Zero.X, Vector2.Zero.Y, ModContent.ProjectileType<ZeroBloodPellet>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Vector2 direction = Main.projectile[proj].Center - Projectile.Center;
                    direction.Normalize(); //unit of 1
                    direction *= 35; //speed of 35
                    Main.projectile[proj].velocity = direction;
                }
            }
		}

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White * Projectile.Opacity; // Makes it uneffected by light
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			behindNPCsAndTiles.Add(index);
        }
	}
}