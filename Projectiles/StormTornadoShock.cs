using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class StormTornadoShock : ModProjectile
	{
		public override void SetStaticDefaults()
		{ 

		}

		public override void SetDefaults()
		{
			Projectile.width = 100;
			Projectile.height = 100;
			DrawOffsetX = -50;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 60;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;

			Projectile.usesLocalNPCImmunity = true; //doesn't use npc immunity frames
			Projectile.localNPCHitCooldown = 60; //wait 30 frames before hitting npc again(death)
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.ai[0]++;
			if (Projectile.ai[0] >= 30)
			{
				Projectile.alpha += 30; //make clearer
				Projectile.damage = 0; //initial shock over
			}
			Projectile.velocity *= 0.90f; //REAL slow

			Lighting.AddLight(Projectile.Center, 1.5f, 0.1f, 1.5f); //light purple light (1 is torch)
		}

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
    }
}