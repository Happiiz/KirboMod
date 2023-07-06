using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class CrystalClutter : ModProjectile
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
			Projectile.timeLeft = 120;
			Projectile.tileCollide = true;
			Projectile.penetrate = 3;
            Projectile.usesIDStaticNPCImmunity = true; //wait for npc to recover from other projectiles of same type
            Projectile.idStaticNPCHitCooldown = 10;
        }
		public override void AI()
		{
			Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
			if (Projectile.velocity.Y >= 3f)
            {
				Projectile.velocity.Y = 3f;
            }
			Projectile.rotation += 0.12f * (float)Projectile.direction; // rotates projectile
		}
         public override void Kill(int timeLeft) //when the projectile dies
         {
             for (int i = 0; i < 20; i++)
             {
                 Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
                 Dust d = Dust.NewDustPerfect(Projectile.position, 91, speed * 3, 0, new Color(Main.rand.Next(0, 255), Main.rand.Next(0, 255), Main.rand.Next(0, 255)), Scale: 1f); //Makes dust in a messy circle
             }
			
			SoundEngine.PlaySound(SoundID.Item27 with {Volume = 0.5f}, Projectile.Center); //quiet crystal break
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			if (Projectile.velocity.X != oldVelocity.X)
			{
				Projectile.velocity.X = -oldVelocity.X;
			}
			if (Projectile.velocity.Y != oldVelocity.Y)
			{
				Projectile.velocity.Y = -oldVelocity.Y;
			}

			return false;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}