using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class GoodBioDagger : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            ProjectileID.Sets.MinionShot[Type] = true;
        }
		public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.timeLeft = 200;
			Projectile.tileCollide = false;
			Projectile.penetrate = 10;
			Projectile.scale = 1f;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Summon;
        }
		ref float InitialVelLength => ref Projectile.localAI[0];
		ref float TargetIndex => ref Projectile.ai[0];
		public override void AI()
		{
			if(InitialVelLength == 0)
			{
				InitialVelLength = Projectile.velocity.Length();
			}
			if (Projectile.penetrate == 10)
			{
				Helper.Homing(Projectile, InitialVelLength, ref TargetIndex, ref Projectile.localAI[1]);
			}
			else
			{
				Projectile.velocity.Normalize();
				Projectile.velocity *= InitialVelLength;
			}
			Projectile.rotation = Projectile.velocity.ToRotation();
		}
	}
}