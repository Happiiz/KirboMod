using KirboMod.NPCs;
using KirboMod.Systems;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class VulcanPunch : ModProjectile
	{
		public override void SetStaticDefaults()
		{ 
			
		}

		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			DrawOffsetX = -4; //make hitbox line up with sprite middle
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = (int)(KnuckleJoe.VulcanJabRange / KnuckleJoe.VulcanJabVelocity);
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
		}
		public override void AI()
		{
			if(Projectile.timeLeft == 2)
			{
				KirbyTransformationModCompatibilityHelper.SpawnSingleDropStar(Projectile.GetSource_Death(), Projectile.Center, KnuckleJoe.VolcanJabDropStarDamage);
			}
			//projectile.spriteDirection = projectile.direction;
			Projectile.rotation = Projectile.velocity.ToRotation();
		}
	}
}