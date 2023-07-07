using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class LaserBeamLaser : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Laser beam");
		}
		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 200; //200 seconds
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.ignoreWater = true;

			//Doesn't wait for npc immunity frames
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10; //regular npc immunity

			Projectile.extraUpdates = 100; //additional updates per tick (make object move twice in one tick for example)
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

			for (int i = 0; i < 4; i++)
			{
				Vector2 position = Projectile.position;
				position -= Projectile.velocity * ((float)i * 0.25f);
				Projectile.alpha = 255;
				int deez = Dust.NewDust(position, 10, 10, ModContent.DustType<Dusts.CyborgArcherLaser>());
				//int deez = Dust.NewDust(position, 1, 1, DustID.RedTorch);
				Main.dust[deez].position = position;
				Main.dust[deez].position.X += Projectile.width / 2;
				Main.dust[deez].position.Y += Projectile.height / 2;
				Main.dust[deez].scale = 1.80f; //Twice as thick as cyborg archer laser
				Main.dust[deez].velocity *= 0.2f;
				Main.dust[deez].noGravity = true;
			}
		}

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; //make it unaffected by light
        }
	}
}