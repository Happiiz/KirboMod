using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class VolcanoFireExplode : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Volcanic Explosion");
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 70;
			Projectile.height = 70;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 5;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 5;
		}
		public override void AI()
		{
			Projectile.ai[0]++;
			if (Projectile.ai[0] == 1)
			{
				SoundEngine.PlaySound(SoundID.Item14, Projectile.position); //bomb sound

				for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 velocity = Main.rand.NextVector2Circular(3f, 3f); //circle
					Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, velocity, Scale: 2f); //Makes dust in a messy circle

                    Vector2 velocity2 = Main.rand.NextVector2Circular(3f, 3f); //circle
                    Gore.NewGorePerfect(Projectile.GetSource_FromThis(), Projectile.Center, velocity2, Main.rand.Next(61, 63), Scale: 1f); //smoke
                }
			}
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 600);
        }
    }
}