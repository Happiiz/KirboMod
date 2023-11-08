using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class GoodWhisp : ModProjectile
	{
		private bool collidedWithTile = false;
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 42;
			Projectile.height = 42;
			DrawOffsetX = 6;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.timeLeft = 180;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

			if (++Projectile.frameCounter >= 12) //changes frames every 12 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}
			if (Projectile.scale >= 1f)
            {
				Projectile.scale = 1f;
            }
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			for (int i = 0; i < 4; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				Vector2 speed = -Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(90));
				speed.Normalize();
				speed *= 5;
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, speed, Mod.Find<ModProjectile>("Puff").Type, Projectile.damage / 2, 2, Projectile.owner);
			}
			collidedWithTile = true;
			return true; //kill proj
		}

        public override void OnKill(int timeLeft)
        {
			SoundEngine.PlaySound(SoundID.NPCDeath15.WithVolumeScale(0.8f), Projectile.position); //snow

			if (collidedWithTile == false)
            {
				for (int i = 0; i < 4; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2CircularEdge(5, 5);
					speed.Normalize();
					speed *= 5;
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, speed, Mod.Find<ModProjectile>("Puff").Type, Projectile.damage / 2, 2, Projectile.owner); 
				}
			}

			//dust
			for (int i = 0; i < 8; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				Vector2 speed = Main.rand.NextVector2Circular(3f, 3f); //circle                     
				Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, speed * 2, Scale: 1f); //Makes scattered dust
			}
		}
    }
}