using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class CrystalNeedleBall : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 102;
			Projectile.height = 102;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true; //wait for npc to recover from other projectiles of same type
            Projectile.idStaticNPCHitCooldown = 10;
        }
		public override void AI()
		{
			//Gravity
			Projectile.velocity.Y = Projectile.velocity.Y + 0.4f;
			if (Projectile.velocity.Y >= 12f)
            {
				Projectile.velocity.Y = 12f;
            }

			//Rotation
			Projectile.rotation += Projectile.velocity.X * 0.02f;

			if (Projectile.velocity.Y == 0)
			{
				Projectile.velocity.X *= 0.992f; //slow down
			}

			if (Main.rand.NextBool(3)) // happens 1/3 times
			{
				int dustnumber = Dust.NewDust(Projectile.position, 102, 102, ModContent.DustType<Dusts.RainbowSparkle>(), 0f, 0f, 200, default, 1f); //dust
				Main.dust[dustnumber].velocity *= 0.3f;
			}

			Projectile.ai[0]++;
			if (Projectile.ai[0] % 10 == 0) //multiple of 10
            {
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0, ModContent.ProjectileType<Projectiles.CrystalTrap>(), Projectile.damage / 2, 1, Projectile.owner, 0, 0);
			}

			//for stepping up tiles
			Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
		}

        public override void Kill(int timeLeft) //when the projectile dies
         {
			//crystal
			for (int i = 0; i < 30; i++)
			{
				Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, 91, speed * 3, 0, new Color( Main.rand.Next(0, 255), Main.rand.Next(0, 255), Main.rand.Next(0, 255)), Scale: 1.5f); //Makes dust in a messy circle
			}
			//crystal clutter projectiles
			for (int i = 0; i < 3; i++)
			{
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Projectile.direction * 10f, Main.rand.Next(-10, 0), ModContent.ProjectileType<Projectiles.CrystalClutter>(), Projectile.damage, 10f, Projectile.owner, 0, 0); 
			}

            SoundEngine.PlaySound(SoundID.Item27, Projectile.Center); //crystal break
        }

        public override bool OnTileCollide(Vector2 oldVelocity) 
        {
			Player player = Main.player[0];
			if (Projectile.velocity.X != oldVelocity.X) //KILL
			{
				Projectile.Kill();
			}/*
			if (projectile.velocity.Y != oldVelocity.Y) //bounce
			{
				projectile.velocity.Y = -oldVelocity.Y;
			}*/
			return false; //dont die
		}

        public override Color? GetAlpha(Color lightColor)
        {
			return Color.White; // Makes it uneffected by light
		}
	}
}