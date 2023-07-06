using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class MinionFire : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			
		}
		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.friendly = true;
			Projectile.minion = true;
			Projectile.timeLeft = 20;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.alpha = 50;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 20; //hit each npc once
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.scale = Projectile.scale + 0.025f;

			if (Main.rand.NextBool(5)) // happens 1/5 times
			{
				int dustnumber = Dust.NewDust(Projectile.position, 24, 24, DustID.Torch, 0f, 0f, 200, default, 1.5f); //dust
				Main.dust[dustnumber].velocity *= 0.3f;
				Main.dust[dustnumber].noGravity = true;
			}
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			target.AddBuff(BuffID.OnFire, 600);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			Projectile.alpha = 50;
			return Color.White; // Makes it uneffected by light
		}
	}
}