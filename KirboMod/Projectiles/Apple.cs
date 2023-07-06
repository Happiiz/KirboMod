using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class Apple : ModProjectile
	{
		private bool hmmnah = false; // if its true then it will bounce in one direction

		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 46;
			Projectile.height = 46;
			//drawOriginOffsetX = -2;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = true;
			Projectile.penetrate = 999;
		}
		public override void AI()
		{
            Player player = Main.player[(int)Projectile.ai[1]]; //chooses player already being targeted by npc

            if (Main.netMode == NetmodeID.SinglePlayer) //checks if singleplayer(no friends?)
			{
				player = Main.player[Main.myPlayer]; //chooses me :)
			}

			Projectile.velocity.Y = Projectile.velocity.Y + 0.1f;
			if (Projectile.velocity.Y >= 6f)
            {
				Projectile.velocity.Y = 6f;
            }
			Projectile.rotation += 0.1f; // rotates projectile
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Player player = Main.player[(int)Projectile.ai[1]]; //chooses player already being targeted by npc

            if (Main.netMode == NetmodeID.SinglePlayer) //checks if singleplayer(no friends?)
            {
                player = Main.player[Main.myPlayer]; //chooses me :)
            }

            Vector2 hmm = player.Center - Projectile.position;

            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.Kill(); //run kill function
            }
            if (oldVelocity.Y > 0) //going down
            {
                Projectile.velocity.Y = -6;
            }

            if (hmmnah == false)
			{
				if (hmm.X >= 0)
				{
					Projectile.velocity.X = 2f; //go one way upon drop
					hmmnah = true;
				}
				else
				{
					Projectile.velocity.X = -2f; //go other way
					hmmnah = true;
				}
			}

			return false;
		}

		public override void Kill(int timeLeft) //when the projectile dies
		{
			for (int i = 0; i < 15; i++)
			{
				Vector2 speed = Main.rand.NextVector2Circular(4f, 4f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.CrimtaneWeapons, speed, Scale: 2f); //Makes dust in a messy circle
				d.noGravity = true;
			}
		}

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            Player player = Main.player[(int)Projectile.ai[1]]; //chooses player already being targeted by npc

            if (Main.netMode == NetmodeID.SinglePlayer) //checks if singleplayer(no friends?)
            {
                player = Main.player[Main.myPlayer]; //chooses me :)
            }

            if (player.Center.Y - 10 > Projectile.Center.Y && hmmnah == false) //player is lower & hasn't bounced yet
            {
				fallThrough = true;
            }
			else //player is higher
            {
				fallThrough = false;
			}
			return true;
        }
	}
}