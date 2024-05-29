using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class MetalSlam : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 6;
		}

		public override void SetDefaults()
		{
			Projectile.width = 140;
			Projectile.height = 122;
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

			player.mount.Dismount(player); //dismount mounts


			if (player.itemTime == 0 || player.active == false || player.dead == true) //done/can't attack
			{
				Projectile.Kill(); //kill projectile
			}

			Projectile.Center = player.Center + new Vector2(player.direction * 36, -38); //stay near player

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

			if (Projectile.frame >= 4 && Projectile.ai[0] == 0) //fist on bottom
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

                    SlamSpikes(player);

                    for (int i = 0; i < 16; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
					{
						Vector2 circle = Main.rand.NextVector2Circular(8f, 8f); //circle
						Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(player.direction * 45, 45), DustID.Smoke, circle, Scale: 2f); //Makes dust in a messy circle
						d.noGravity = true;
					}
					SoundEngine.PlaySound(SoundID.Item14.WithVolumeScale(0.8f), player.Center);
				}
			}
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			Player player = Main.player[Projectile.owner];

			if (Projectile.frame >= 4 && Projectile.ai[0] == 0) //fist on bottom
			{
                if (player.velocity.Y != 0f) //in air
				{
                    Projectile.ai[0]++; //disable stomping

                    player.velocity.X = player.direction * -5;
					player.velocity.Y = -8;

					player.immuneTime = 8;
					player.immune = true;
					player.immuneNoBlink = true;

					SlamSpikes(player);
                }
			}
		}
		/// <summary>
         /// Custom method used for the spikes that shoot out if a slam hits the ground or an NPC. (This was written just to test summaries)
         /// </summary>

        private void SlamSpikes(Player player) //custom method for 
		{
            //spikes
            for (int i = 0; i < 16; i++)
            {
                float rotationalOffset = MathHelper.ToRadians(i * 22.5f); //convert degrees to radians

                //use sine and cosine for circular formation
                float projX = Projectile.Center.X + MathF.Cos(rotationalOffset) * 5; //angle = rotation offset
                float projY = Projectile.Center.Y + MathF.Sin(rotationalOffset) * 5; //multiplier = spread

                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), projX + player.direction * 45, projY + 45, Vector2.Zero.X, Vector2.Zero.Y,
                    ModContent.ProjectileType<MetalFighterSpike>(), Projectile.damage / 2, 6, Projectile.owner, 0, 0);

                Vector2 direction = Main.projectile[proj].Center - (Projectile.Center + new Vector2(player.direction * 45, 45));
                direction.Normalize();
                direction *= 25;
                Main.projectile[proj].velocity = direction;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity; //independent from light level while still being affected by opacity
        }
    }
}