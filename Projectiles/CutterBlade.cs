using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class CutterBlade : ModProjectile
	{
		private int backtrack = 0;
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			Projectile.width = 80;
			Projectile.height = 38; //30 less than sprite
			DrawOriginOffsetY = -11;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true; //allows to have npc immunity frames on its own accord
			Projectile.localNPCHitCooldown = 20; //time until it can damage again regardless if a projectile just struck the target
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			if (Projectile.ai[0] == 0) //move before turn
			{
				Projectile.velocity.X = player.direction * 15;
			}

			Projectile.ai[0]++;
			Projectile.velocity.Y = 0f;

			if (++Projectile.frameCounter >= 2) //changes frames every 3 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
					SoundEngine.PlaySound(SoundID.Run.WithVolumeScale(0.5f), Projectile.Center); 
				}
			}
			if (Projectile.ai[0] == 15) //point of turn 
			{
				
				Projectile.tileCollide = true; //collide with tiles

				if (Projectile.velocity.X == 15f)
				{
					backtrack = 1;
				}
				else if (Projectile.velocity.X == -15f)
				{
					backtrack = 2;
				}
			}
			//invert direction based on direction

			if (backtrack == 1) //negative 10
			{
				Projectile.velocity.X -= 0.75f;
			}
			else if (backtrack == 2) //positive 10
			{
				Projectile.velocity.X += 0.75f;
			}

			//caps
			if (Projectile.velocity.X > 15)
			{
				Projectile.velocity.X = 15;
			}
			if (Projectile.velocity.X < -15)
			{
				Projectile.velocity.X = -15;
			}

			//kill when far away
			float Xdistance = player.Center.X - Projectile.Center.X;
			
			if (Xdistance > 1000 || Xdistance < -1000)
            {
				Projectile.Kill();
            }


			//Kill when touching player after turn
			if (Projectile.ai[0] >= 15)
			{
				Rectangle box = Projectile.Hitbox;
				if (box.Intersects(player.Hitbox)) //if touching player
				{
					Projectile.ai[1] = 1;
					Projectile.Kill(); //KILL
				}
			}
		}

        public override bool OnTileCollide(Vector2 oldVelocity) 
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position); //impact
            return true; //kill like normal
        }

        public override void OnKill(int timeLeft)
		{
			if (Projectile.ai[1] != 1) //checks if not killed by player contact
			{
				for (int i = 0; i < 4; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
					Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.LilStar>(), speed * 2, 0, default, 1f);//Makes dust in a messy circle
					d.noGravity = false;
				}

                for (int i = 0; i < 3; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
                    Gore.NewGorePerfect(Projectile.GetSource_FromThis(), Projectile.Center, speed, Main.rand.Next(11, 13), Scale: 0.5f); //smoke
                }
            }
        }
    }
}