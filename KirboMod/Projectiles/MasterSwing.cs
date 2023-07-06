using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class MasterSwing : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			Projectile.width = 180;
			Projectile.height = 180;
			Projectile.friendly = true;
			Projectile.usesLocalNPCImmunity = true; //uses own immunity frames
			Projectile.localNPCHitCooldown = 10; //time before hit again
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true; //check if owner has line of sight to hit
            //Projectile.aiStyle = 19; //pole arm
        }

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			Vector2 distance = new Vector2(0, 0);

			if (Main.myPlayer == player.whoAmI)
			{
				distance = Main.MouseWorld - player.Center;
				distance.Normalize();
				distance *= 50f;

				Projectile.Center = player.Center + distance;
				Projectile.velocity = distance * 0.001f; //very small
			}
			Projectile.rotation = Projectile.velocity.ToRotation();

			//animation
			Projectile.frameCounter++; //go up by 1 each tick (1/60 of a second)
			if (Projectile.frameCounter < 2)
			{
				Projectile.frame = 0;
			}
			else if (Projectile.frameCounter < 3)
			{
				Projectile.frame = 1;
			}
			else if (Projectile.frameCounter < 4)
			{
				Projectile.frame = 2;
			}
			else
			{
				Projectile.frame = 3;
			}

            //death
            if (player.itemTime == 1 || player.active == false || player.dead == true) //done/can't attack
            {
                Projectile.Kill(); //kill projectile
            }
        }

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}