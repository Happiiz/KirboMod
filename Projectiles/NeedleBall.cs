using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class NeedleBall : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 62;
			Projectile.height = 62;
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

			//for stepping up tiles
			float stepspeed = Projectile.velocity.X * 0.005f;
			float localgfxOffY = 0f;
			Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref stepspeed, ref localgfxOffY);
		}

        public override void OnKill(int timeLeft) //when the projectile dies
        {
             for (int i = 0; i < 8; i++)
             {
                Vector2 speed = Main.rand.NextVector2Circular(3f, 3f); //circle
                Gore.NewGorePerfect(Projectile.GetSource_FromThis(), Projectile.Center, speed, Main.rand.Next(61, 63), Scale: 1f); //smoke
             }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) 
        {
			return false; //dont die
		}
    }
}