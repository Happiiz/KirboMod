using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace KirboMod.Projectiles
{
	public class ClutterNeedleBall : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 82;
			Projectile.height = 82;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = true;
			Projectile.penetrate = 5;
            Projectile.usesLocalNPCImmunity = true; //wait for very own timer
            Projectile.localNPCHitCooldown = 25;
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

            Projectile.ai[0]++;

            if (Projectile.ai[0] % 30 == 0 && Main.myPlayer == Projectile.owner) //every 30 ticks
            {
                Vector2 speed = Main.rand.BetterNextVector2Circular(8f); //circle
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, speed, ModContent.ProjectileType<Projectiles.Clutter>(), Projectile.damage, 4, Projectile.owner, 0, 0);
            }

            //for stepping up tiles
            Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
		}

        public override void OnKill(int timeLeft) //when the projectile dies
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position); //bomb sound

            //smoke
            for (int i = 0; i < 30; i++)
             {
                 Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
                 Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, speed * 3, Scale: 1.5f); //Makes dust in a messy circle
             }
			 //metal
			for (int i = 0; i < 20; i++)
			{
				Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Iron, speed * 3, Scale: 1.5f); //Makes dust in a messy circle
			}

            if (Main.myPlayer == Projectile.owner)
            {
                //nail projectiles
                for (int i = 0; i < 20; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(16f, 16f); //circle
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, speed, ModContent.ProjectileType<Projectiles.Clutter>(), Projectile.damage, 4, Projectile.owner, 0, 0);
                }
            }
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X) //bounce
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            return false; //dont die
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false; //don't fall through platforms
            return true;
        }
    }
}