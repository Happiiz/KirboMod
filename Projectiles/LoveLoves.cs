using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class LoveLoves : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 6;
		}
		public override void SetDefaults()
		{
			Projectile.width = 58;
			Projectile.height = 50;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;
			Projectile.usesLocalNPCImmunity = true; //uses own immunity frames
			Projectile.localNPCHitCooldown = 20; //time before hit again
			Projectile.ignoreWater = true; //it looks ugly when in water
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.direction = Main.rand.Next ((Projectile.direction - 5), (Projectile.direction + 5));
			if (++Projectile.frameCounter >= 5) //changes frames every 5 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}
		}

        public override void Kill(int timeLeft)
        {
			SoundEngine.PlaySound(SoundID.Item67, Projectile.Center); //rainbow gun

			for (int i = 0; i <= 8; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				//Full circle
				if (i == 1) //down
				{
					Vector2 speed = new Vector2(0, 24);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, /*speed*/ Vector2.Zero, ModContent.ProjectileType<Projectiles.LoveDot>(), Projectile.damage, 0, Projectile.owner, Projectile.Center.X, Projectile.Center.Y, 0);
                }
				if (i == 2) //down right
				{
					Vector2 speed = new Vector2(16, 16);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, /*speed*/ Vector2.Zero, ModContent.ProjectileType<Projectiles.LoveDot>(), Projectile.damage, 0, Projectile.owner, Projectile.Center.X, Projectile.Center.Y, 45);
				}
				if (i == 3) //right
				{
					Vector2 speed = new Vector2(24, 0);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, /*speed*/ Vector2.Zero, ModContent.ProjectileType<Projectiles.LoveDot>(), Projectile.damage, 0, Projectile.owner, Projectile.Center.X, Projectile.Center.Y, 90);
				}
				if (i == 4)//up right
				{
					Vector2 speed = new Vector2(16, -16);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, /*speed*/ Vector2.Zero, ModContent.ProjectileType<Projectiles.LoveDot>(), Projectile.damage, 0, Projectile.owner, Projectile.Center.X, Projectile.Center.Y, 135);
				}
				if (i == 5)//up
				{
					Vector2 speed = new Vector2(0, -24);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, /*speed*/ Vector2.Zero, ModContent.ProjectileType<Projectiles.LoveDot>(), Projectile.damage, 0, Projectile.owner, Projectile.Center.X, Projectile.Center.Y, 180);
				}
				if (i == 6)//up left
				{
					Vector2 speed = new Vector2(-16, -16);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, /*speed*/ Vector2.Zero, ModContent.ProjectileType<Projectiles.LoveDot>(), Projectile.damage, 0, Projectile.owner, Projectile.Center.X, Projectile.Center.Y, 225);
				}
				if (i == 7)//left
				{
					Vector2 speed = new Vector2(-24, 0);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, /*speed*/ Vector2.Zero, ModContent.ProjectileType<Projectiles.LoveDot>(), Projectile.damage, 0, Projectile.owner, Projectile.Center.X, Projectile.Center.Y, 270);
				}
				if (i == 8)//down left
				{
					Vector2 speed = new Vector2(-16, 16);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, /*speed*/ Vector2.Zero, ModContent.ProjectileType<Projectiles.LoveDot>(), Projectile.damage, 0, Projectile.owner, Projectile.Center.X, Projectile.Center.Y, 315);
				}
			}
		}

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}