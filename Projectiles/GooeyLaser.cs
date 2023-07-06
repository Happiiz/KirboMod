using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class GooeyLaser : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 5;
		}
		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			DrawOffsetX = -60;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.timeLeft = 120;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.alpha = 128;
			Projectile.minion = true;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 120;
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

			if (Main.rand.Next(5) == 1) // happens 1/5 times
			{
				int dustnumber = Dust.NewDust(Projectile.position, 124, 24, ModContent.DustType<Dusts.DarkResidue>(), 0f, 0f, 200, default, 0.8f); //dust
				Main.dust[dustnumber].velocity *= 0.3f;
				Main.dust[dustnumber].noGravity = true;
			}

			if (++Projectile.frameCounter >= 3) //changes frames every 3 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			//target.AddBuff(BuffID.OnFire, 600);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}