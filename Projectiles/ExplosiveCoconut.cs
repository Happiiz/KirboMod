using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class ExplosiveCoconut : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}
		public const float yAcceleration = .13f;
		ref float YVel { get => ref Projectile.ai[2]; }
		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = false;
			Projectile.penetrate = 1;
		}
		public override void AI()
		{
            Projectile.position.X += Projectile.velocity.X;
			Projectile.position.Y += YVel / 2;
			YVel += yAcceleration;
            Projectile.position.Y += YVel / 2;
            if (Projectile.velocity.X >= 0)
			{
				Projectile.rotation += MathHelper.ToRadians(18);
			}
			else
			{
                Projectile.rotation -= MathHelper.ToRadians(18);
            }
		}
         public override void OnKill(int timeLeft) //when the projectile dies
         {
			SoundEngine.PlaySound(SoundID.Item14, Projectile.position); //bomb sound

			for (int i = 0; i < 30; i++) 
			{
				Vector2 speed = Main.rand.NextVector2Circular(6f, 6f); //circle spread
				Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, speed, Scale: 2f); //Makes dust in a messy circle
				d.noGravity = true;
			}

            for (int k = 0; k < 10; k++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(6f, 6f); //circle spread
                Gore.NewGorePerfect(Projectile.GetSource_FromThis(), Projectile.Center, speed, Main.rand.Next(61, 63), Scale: 1f); //smoke
            }
        }
        public override bool ShouldUpdatePosition()
        {
			return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
			Projectile.Kill(); 
        }
    }
}