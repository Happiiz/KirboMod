using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class CrystalTrap : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 32;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 120;
			Projectile.tileCollide = true;
			Projectile.penetrate = 5;
            Projectile.usesLocalNPCImmunity = true; //wait for no one else's immunity timer
            Projectile.localNPCHitCooldown = 10;
        }
		public override void AI()
		{
			//Gravity
			Projectile.velocity.Y = Projectile.velocity.Y + 0.4f;
			if (Projectile.velocity.Y >= 12f)
            {
				Projectile.velocity.Y = 12f;
            }

			Projectile.velocity.X *= 0; //stop
		}

        public override void OnKill(int timeLeft) //when the projectile dies
         {
			//crystal
			for (int i = 0; i < 2; i++)
			{
				Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.RainbowSparkle>(), speed * 3, 0, default, Scale: 1f); //Makes dust in a messy circle
			}
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