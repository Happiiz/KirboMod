using KirboMod.Systems;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class BadPlasmaZap : ModProjectile, ITrailedProjectile
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Type] = 40;
			ProjectileID.Sets.TrailingMode[Type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.friendly = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.hostile = true;
			Projectile.timeLeft = 240;
			Projectile.extraUpdates = 3;
			Projectile.tileCollide = false;
			Projectile.penetrate = 3;
            Projectile.localNPCHitCooldown = 10;
            Projectile.usesLocalNPCImmunity = true;
        }
		public override void AI()
		{
			
			Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.TerraBlade);
			dust.noGravity = true;
			dust.velocity *= .5f;
			dust.fadeIn = 1;
			Projectile.spriteDirection = Projectile.direction;
			Projectile.rotation = Projectile.velocity.ToRotation();
			Lighting.AddLight(Projectile.Center, Color.Green.ToVector3());
		}
        public override bool PreDraw(ref Color lightColor)
		{		
			return Projectile.DrawSelf();
        }

        public void AddTrail()
        {
			TrailSystem.Trail.AddAlphaBlend(Projectile, 6, Color.Cyan, Color.LimeGreen);
			TrailSystem.Trail.AddAlphaBlend(Projectile, 3, Color.White, Color.White);
		}
    }
}