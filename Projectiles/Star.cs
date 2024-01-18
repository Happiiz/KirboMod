using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class Star : ModProjectile
	{
        public override string Texture => "KirboMod/Projectiles/TripleStarStar";
        public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 50; 
			Projectile.height = 50;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 180;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
			Projectile.scale = 1f;
			Projectile.alpha = 255;
		}
		public override void AI()
		{
			Lighting.AddLight(Projectile.Center, 0.8f, 0.8f, 0f); //yellow light half of a torch
			Projectile.Opacity += 0.1f;

			if (Main.rand.NextBool(3)) // happens 1/3 times
			{
				//Sparkle.NewSparkle(Projectile.Center + Main.rand.NextVector2Circular(20,20) - Projectile.velocity, Main.rand.NextBool(3, 5) ? Color.Yellow : Color.Blue, new Vector2(1, 1.5f), Projectile.velocity * 0.1f + Main.rand.NextVector2Circular(30, 30) / 10, 40, new Vector2(2, 2), null, 1, 0, 0.98f);
			}

			if (Projectile.soundDelay == 0)
			{
				Projectile.soundDelay = 20 + Main.rand.Next(40);
				SoundEngine.PlaySound(SoundID.Item9 with { MaxInstances = 0 }, Projectile.Center);
			}
			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.005f * Projectile.direction;
			//Vector2 screenSize = new Vector2(Main.screenWidth, Main.screenHeight);
			//if (Projectile.Hitbox.Intersects(Utils.CenteredRectangle(Main.screenPosition + screenSize / 2f, screenSize + new Vector2(400f))) && Main.rand.NextBool(3))
			//{
			//	if (Main.rand.NextBool(3))
			//	{
			//		int goreIndex = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0.2f, Utils.SelectRandom(Main.rand, 16, 17, 17, 17));
			//		Main.gore[goreIndex].scale = 1.5f;
			//	}
			//}
			for (int i = 0; i < 2; i++)
			{
				if (Main.rand.NextBool(3))
				{
					Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.BlueFairy, 0f, 0f, 127);
					dust.velocity *= 0.25f;
					dust.scale = 1.3f;
					dust.noGravity = true;
					dust.velocity += Projectile.velocity.RotatedBy(MathF.PI / 8f * (1f - (float)(2 * i))) * 0.2f;
				}
			}
        }
        public override void OnKill(int timeLeft) //when the projectile dies
        {
			for (int i = 0; i < 10; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(10, 10); //burst of sparkles
                //Sparkle.NewSparkle(Projectile.Center + Projectile.velocity, Main.rand.NextBool(3, 5) ? Color.Yellow : Color.Blue, 
					//new Vector2(1, 1f), velocity, 40, new Vector2(2, 2), null, 1, 0, 0.98f);
            }
		}
        public override bool PreDraw(ref Color lightColor)
        {
			VFX.DrawProjWithStarryTrail(Projectile, Color.Blue, Color.White * 0.4f, Color.White, 0.4f, 0, 0, 128);
			return false;// base.PreDraw(ref lightColor);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			SoundEngine.PlaySound(SoundID.Item10, Projectile.position); //impact
			return true; //collision
		}
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
			return AIUtils.CheckCircleCollision(targetHitbox, Projectile.Center, 50) || AIUtils.CheckCircleCollision(targetHitbox, Projectile.Center - Projectile.velocity * 4, 50); ;
        }
        public override Color? GetAlpha(Color lightColor)
        {
			return Color.White; // Makes it uneffected by light
        }
    }
}