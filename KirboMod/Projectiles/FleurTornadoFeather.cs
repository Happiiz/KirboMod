using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class FleurTornadoFeather : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 60;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;

			//doesn't wait for npc immunity
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 60;
		}
		public override void AI()
		{
			Projectile.velocity *= 0.9f; //slow
			Projectile.rotation += Projectile.direction * 0.08f; // rotates projectile depending on direction it's facing

			if (Projectile.velocity.X < 1 && Projectile.velocity.X > -1 && Projectile.velocity.Y < 1 && Projectile.velocity.Y > -1) //slowed enough
            {
				Projectile.alpha += 10;
            }

			/*if (++projectile.frameCounter >= 15) //changes frames every 15 ticks 
			{
				projectile.frameCounter = 0;
				if (++projectile.frame >= Main.projFrames[projectile.type])
				{
					projectile.frame = 0;
				}
			}*/
		}
        /* public override void Kill(int timeLeft) //when the projectile dies
         {
             for (int i = 0; i < 10; i++)
             {
                 Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
                 Dust d = Dust.NewDustPerfect(projectile.position, DustID.Enchanted_Gold, speed * 3, Scale: 1f); //Makes dust in a messy circle
             }
         }*/
    }
}