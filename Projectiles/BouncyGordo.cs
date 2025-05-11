using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BouncyGordo : ModProjectile //gordo projectile used by Whispy Woods
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Gordo");
			Main.projFrames[Projectile.type] = 1;
		}
		public static float GordoGravity => /*.12f*/ 1.2f;
		public override void SetDefaults()
		{
			Projectile.width = 78;
			Projectile.height = 78;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
		}
		public override void AI()
		{
			Projectile.velocity.Y += GordoGravity;
			Projectile.rotation += 0.1f; // rotates projectile
		}

        public override bool OnTileCollide(Vector2 oldVelocity) 
        {
			if (Projectile.velocity.X != oldVelocity.X) //bounce
			{
				Projectile.velocity.X = -oldVelocity.X;
			}
			if (Projectile.velocity.Y != oldVelocity.Y) //bounce
			{
				Projectile.velocity.Y = -oldVelocity.Y;
			}
			return false;
		}

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
			fallThrough = true;
			return true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			Projectile.damage -= 1;
			if (Projectile.damage <= 6)
            {
				Projectile.damage = 6;
            }
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