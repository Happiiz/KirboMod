using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class ParosolHitbox : ModProjectile //hitbox for parasol dee swing attack
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.MinionShot[Type] = true;
        }

		public override void SetDefaults()
		{
			Projectile.width = 90;
			Projectile.height = 60;
			Projectile.friendly = true;
			Projectile.timeLeft = 3;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 4;
            Projectile.DamageType = DamageClass.Summon;
        }
    }
}