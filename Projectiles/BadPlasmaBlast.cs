using KirboMod.Particles;
using KirboMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BadPlasmaBlast : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // The recording mode
        }

		public override void SetDefaults()
		{
			Projectile.width = 140;
			Projectile.height = 140;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 120;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1; //infinite
		}
		
		//the way it works is 
		//time to reach target is ai0
		//make it explode when timer is > ai0
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

			if (Projectile.ai[0] < Projectile.ai[1])
            {
				Projectile.Kill();
            }
			Projectile.ai[1]++;
			//dust effects
			if (Main.rand.NextBool(2)) // happens 1/2 times
			{
				int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.TerraBlade, 0f, 0f, 200, default, 1f); //dust
				Main.dust[dustnumber].velocity *= 0.3f;
				Main.dust[dustnumber].noGravity = true;
			}
		}

		public override void OnKill(int timeLeft)
		{
			//Projectile.Hitbox = Utils.CenteredRectangle(Projectile.Center, Projectile.Size * 1.75f);
			for (int i = 0; i < 80; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				Vector2 speed = Main.rand.NextVector2Circular(20, 20); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center + Vector2.Normalize(speed) * 10, DustID.TerraBlade, speed, Scale: 2f); //Makes dust in a messy circle
				d.noGravity = true;
			}
			Vector2 scale = new Vector2(3);
			Sparkle.EyeShine(Projectile.Center, Color.LimeGreen, scale, scale, 10);
			Sparkle.NewSparkle(Projectile.Center, Color.LimeGreen, scale * 2, Vector2.Zero, 10, scale);
			//Projectile.Damage();
		}

        public override bool PreDraw(ref Color lightColor)
		{

			TrailSystem.Trail.AddAdditive(Projectile, 100, Color.Cyan * Projectile.Opacity, Color.LimeGreen * Projectile.Opacity);
			TrailSystem.Trail.AddAdditive(Projectile, 50, Color.White * Projectile.Opacity, Color.White * Projectile.Opacity);
			float rotation = Projectile.rotation;
			Projectile.rotation = 0;
			VFX.DrawElectricOrb(Projectile, new Vector2(6));
			Projectile.rotation = rotation;
            return false;
        }
    }
}