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

				for (int i = 0; i < 20; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
					Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, speed * 2, Scale: 2f); //Makes dust in a messy circle
					d.noGravity = true;
				}
			}
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 600);
        }
    }
}