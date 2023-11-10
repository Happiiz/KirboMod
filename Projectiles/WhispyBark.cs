using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class WhispyBark : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Whispy Bark");
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 36;
			Projectile.height = 36;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 240;
			Projectile.tileCollide = false;
			Projectile.penetrate = 1;
		}
		public override void AI()
		{
			Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
			if (Projectile.velocity.Y >= 6f)
            {
				Projectile.velocity.Y = 6f;
            }
			Projectile.rotation += 0.1f; // rotates projectile
		}
        public override void OnKill(int timeLeft) //when the projectile dies
         {
             for (int i = 0; i < 5; i++)
             {
                 Vector2 speed = Main.rand.NextVector2Circular(3f, 3f); //circle
                 Dust d = Dust.NewDustPerfect(Projectile.position, DustID.Dirt, speed, Scale: 1f); //Makes dust in a messy circle
             }
		}
    }
}