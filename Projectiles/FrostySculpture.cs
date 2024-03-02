using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class FrostySculpture : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
            DrawOffsetX = -29;
            DrawOriginOffsetY = -29;
            Projectile.friendly = true;
			Projectile.timeLeft = 180;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
			Projectile.usesIDStaticNPCImmunity = true;
			Projectile.idStaticNPCHitCooldown = 10;
		}

		public override void AI()
		{
			Projectile.ai[0]++;
			if (Projectile.ai[0] == 1)
			{
				SoundEngine.PlaySound(SoundID.Item46, Projectile.position); //ice hydra

				for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
					Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.SnowBlock, speed * 2, Scale: 2f); //Makes dust in a messy circle
					d.noGravity = true;
				}
			}
			
			Projectile.velocity *= 0.96f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) //make hitbox into a circle
        {
            return Utils.IntersectsConeFastInaccurate(targetHitbox, Projectile.Center, 38, 0, MathF.Tau);
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

        public override bool? CanHitNPC(NPC target) //can hit only if there's a line of sight
        {
            return Collision.CanHit(Projectile, target);
        }
        public override bool CanHitPvp(Player target) //can hit only if there's a line of sight
        {
            return Collision.CanHit(Projectile, target);
        }

        public override void OnKill(int timeLeft)
        {
			for (int i = 0; i < 15; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.SnowBlock, speed * 2, Scale: 2f); //Makes dust in a messy circle
				d.noGravity = true;
			}
		}
    }
}