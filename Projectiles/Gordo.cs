using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class Gordo : ModProjectile //gordo projectile used by Whispy Woods
	{
		private bool stickToDirection = false; // if its true then it will bounce in one direction

        public override string Texture => "KirboMod/Projectiles/BouncyGordo";

        public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 78;
			Projectile.height = 78;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
		}
		public override void AI()
		{
            Player player = Main.player[(int)Projectile.ai[1]]; //chooses player already being targeted by npc

            if (Main.netMode == NetmodeID.SinglePlayer) //checks if singleplayer(no friends?)
			{
				player = Main.player[Main.myPlayer]; //chooses me :)
			}

			Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
			if (Projectile.velocity.Y >= 3f)
            {
				Projectile.velocity.Y = 3f;
            }
			Projectile.rotation += 0.12f * (float)Projectile.direction; // rotates projectile
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
				Projectile.velocity.Y = -3;
			}

			if (stickToDirection == false)
			{
				if (hmm.X >= 0)
				{
					Projectile.velocity.X = 4f; //bounce towards player upon drop
                    stickToDirection = true;
				}
				else
				{
					Projectile.velocity.X = -4f; //go other way
                    stickToDirection = true;
				}
			}

			return false;
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
            Player player = Main.player[(int)Projectile.ai[1]]; //chooses player already being targeted by npc

            if (Main.netMode == NetmodeID.SinglePlayer) //checks if singleplayer(no friends?)
            {
                player = Main.player[Main.myPlayer]; //chooses me :)
            }

            if (player.Top.Y - 10 > Projectile.Bottom.Y && stickToDirection == false) //player is lower & hasn't bounced yet
			{
				fallThrough = true;
			}
			else //player is higher
			{
				fallThrough = false;
			}
			return true;
		}

        public override void OnKill(int timeLeft) //when the projectile dies
		{
			for (int i = 0; i < 15; i++)
			{
				Vector2 speed = Main.rand.NextVector2Circular(4f, 4f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Obsidian, speed, Scale: 2f); //Makes dust in a messy circle
				d.noGravity = true;
			}
		}
	}
}