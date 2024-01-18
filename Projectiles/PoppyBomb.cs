using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class PoppyBomb : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
            Projectile.width = 38;
            Projectile.height = 38;
            DrawOriginOffsetY = -12;
            Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 120;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
		}
		public override void AI()
		{
			Projectile.rotation += Projectile.velocity.X * 0.02f;
			Projectile.velocity.Y = Projectile.velocity.Y + 0.4f;
			if (Projectile.velocity.Y >= 10f)
			{
				Projectile.velocity.Y = 10f;
			}

			for (int i = 0;i < Main.maxPlayers; i++) //loop statement that cycles completely every tick
			{
				Player player = Main.player[i]; //any player

				if (player.Hitbox.Intersects(Projectile.Hitbox)) //hitboxes touching
				{
					Projectile.Kill();
				}
			}

            //Step up half tiles
            Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
        }
         public override void OnKill(int timeLeft) //when the projectile dies
         {
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity *= 0.01f, ModContent.ProjectileType<PoppyBombExplode>(), Projectile.damage / 2, 12, Main.myPlayer);
         }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			return false;
        }
    }
}