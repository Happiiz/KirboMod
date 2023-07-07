using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class LightBeamLaser : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Light Beam");
		}
		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 60; //seconds = timeLeft - extraUpdates / 60
			Projectile.tileCollide = false; //initally
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.ignoreWater = true;

			//Doesn't wait for npc global immunity frames
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10; //regular npc immunity

			Projectile.extraUpdates = 20; //cycle through code multiple times in one tick
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

			for (int i = 0; i < 4; i++)
			{
				Vector2 position = Projectile.position;
				position -= Projectile.velocity * ((float)i * 0.25f);
				Projectile.alpha = 255;
				int deez = Dust.NewDust(position, 10, 10, ModContent.DustType<Dusts.LightBeamLaser>(), 0, 0, 0, Color.White);
				//int deez = Dust.NewDust(position, 1, 1, DustID.RedTorch);
				Main.dust[deez].position = position;
				Main.dust[deez].position.X += Projectile.width / 2;
				Main.dust[deez].position.Y += Projectile.height / 2;
				Main.dust[deez].scale = 1.80f; //Twice as thick as cyborg archer laser
				Main.dust[deez].velocity *= 0.2f;
				Main.dust[deez].noGravity = true;
			}

			Projectile.ai[0]++;

			if (Projectile.ai[0] == 1)
            {
				Vector2 speed = Main.MouseWorld - Projectile.Center;
				speed.Normalize();
				speed *= 30;
				Projectile.velocity.X = speed.X;
            }


			Player player = Main.player[Projectile.owner];
			//below player
			if (Projectile.Center.Y >= player.Center.Y)
            {
				Projectile.tileCollide = true;
            }
		}

        public override void Kill(int timeLeft)
        {
			for (int i = 0; i < 15; i++)
			{
				Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.RainbowSparkle>(), speed, Scale: 1f); //Makes dust in a messy circle
				d.noGravity = true;
			}
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; //make it unaffected by light
        }
	}
}