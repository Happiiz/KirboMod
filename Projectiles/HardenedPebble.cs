using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class HardenedPebble : ModProjectile
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
			Projectile.DamageType = DamageClass.Melee;
			Projectile.timeLeft = 300; //5 seconds
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true; //doesn't wait for other projectiles to hit again
			Projectile.localNPCHitCooldown = 10; //time until able to hit npc even if npc has just been struck (default)
			
		}
		public override void AI()
		{
			//Gravity
			Projectile.velocity.Y = Projectile.velocity.Y + 0.4f;
			if (Projectile.velocity.Y >= 12f)
            {
				Projectile.velocity.Y = 12f;
            }

			Projectile.ai[0]++;

			if (Projectile.ai[0] % 5 == 0)
            {
				Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, Projectile.velocity * 0); //Makes dust
			}
		}

        public override void OnKill(int timeLeft)
        {
			for (int i = 0; i < 4; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				Vector2 circle = Main.rand.NextVector2Circular(1f, 1f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Dirt, circle, Scale: 1f); //Makes dust in a messy circle
			}
			SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
		}
    }
}