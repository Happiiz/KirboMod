using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class HardenedSlam : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 6;
		}

		public override void SetDefaults()
		{
			Projectile.width = 110;
			Projectile.height = 98;
			Projectile.friendly = false;
			Projectile.usesLocalNPCImmunity = true; //uses own immunity frames
			Projectile.localNPCHitCooldown = 10; //time before hit again
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			if (player.itemTime == 0 || player.active == false || player.dead == true) //done/can't attack
			{
				Projectile.Kill(); //kill projectile
			}

			Projectile.Center = player.Center + new Vector2(player.direction * 24, -26); //stay near player

			Projectile.direction = player.direction;
			Projectile.spriteDirection = Projectile.direction; //look in direction

            //animation
            if (player.itemTime > player.itemTimeMax - player.itemTimeMax / 6)
            {
                Projectile.frame = 0;
            }
            else if (player.itemTime > player.itemTimeMax - player.itemTimeMax / 5)
            {
                Projectile.frame = 1;
            }
            else if (player.itemTime > player.itemTimeMax - player.itemTimeMax / 4)
            {
                Projectile.frame = 2;
            }
            else if (player.itemTime > player.itemTimeMax - player.itemTimeMax / 3)
            {
                Projectile.frame = 3;
            }
            else if (player.itemTime > player.itemTimeMax - player.itemTimeMax / 2)
            {
                Projectile.frame = 4;
            }
            else
            {
                Projectile.frame = 5;
            }

            //enable hitting
            if (Projectile.frame >= 4)
			{
				Projectile.friendly = true;
			}

			if (Projectile.frame == 4 && Projectile.ai[0] == 0) //fist on bottom
			{
				if (player.velocity.Y == 0f) //not in air
				{
                    Projectile.ai[0]++; //disable stomping

                    //MAKE TRAJECTORY OF ROCK
                    Vector2 speed = Main.MouseWorld - player.Center;
					speed.Normalize();
					speed *= 15;
					Vector2 peturbedspeed = speed.RotatedByRandom(MathHelper.ToRadians(20f)); //spread em

					if (peturbedspeed.Y > -4) //go upper
                    {
						peturbedspeed.Y = Main.rand.Next( -10, -4);
                    }

					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + (player.direction * 30), Projectile.Center.Y + 30, peturbedspeed.X, peturbedspeed.Y, ModContent.ProjectileType<HardenedPebble>(), Projectile.damage / 2, 6, Projectile.owner, 0, 0);

					for (int i = 0; i < 8; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
					{
						Vector2 circle = Main.rand.NextVector2Circular(8f, 8f); //circle
						Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(player.direction * 30, 30), DustID.Smoke, circle, Scale: 2f); //Makes dust in a messy circle
						d.noGravity = true;

						SoundEngine.PlaySound(SoundID.Item14.WithVolumeScale(0.8f), player.Center);
					}
				}
			}
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			Player player = Main.player[Projectile.owner];

			if (Projectile.frame >= 4) //fist on bottom
			{
				if (player.velocity.Y != 0f) //in air
				{
					player.velocity.X = player.direction * -5;
					player.velocity.Y = -8;

					player.immuneTime = 8;
					player.immune = true;
					player.immuneNoBlink = true;
				}
			}
		}
    }
}