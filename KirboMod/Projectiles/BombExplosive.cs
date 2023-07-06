using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BombExplosive : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 100;
			Projectile.height = 100;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 5;
			Projectile.tileCollide = false;
			Projectile.penetrate = 999;
			Projectile.scale = 1f;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 5;
		}
		public override void AI()
		{
			Projectile.ai[0]++;
			if (Projectile.ai[0] == 1)
			{
				SoundEngine.PlaySound(SoundID.Item14, Projectile.position); //bomb sound

                for (int i = 0; i < 10; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
					Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.LilStar>(), speed, Scale: 2f); //Makes dust in a messy circle
					d.noGravity = true;

                    Vector2 speed2 = Main.rand.NextVector2Circular(5f, 5f); //circle
                    Gore.NewGorePerfect(Projectile.GetSource_FromThis(), Projectile.Center, speed2, Main.rand.Next(11, 13), Scale: 1f); //double jump smoke
                }

            }

            /*if (Projectile.ai[0] % 2 == 0)
            {
                Vector2 positionOffset = new Vector2(Main.rand.Next(-100, 100), Main.rand.Next(-100, 100));

                int dustID = Main.rand.Next(130, 134);

                for (int i = 0; i < 16; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                {
                    float rotationalOffset = MathHelper.ToRadians(i * 22.5f); //convert degrees to radians

                    float dustX = Projectile.Center.X + positionOffset.X + (float)Math.Cos(rotationalOffset) * 5;
                    float dustY = Projectile.Center.Y + positionOffset.Y + (float)Math.Sin(rotationalOffset) * 5;

                    Dust d = Dust.NewDustPerfect(new Vector2(dustX, dustY), dustID, Vector2.Zero, Scale: 2f); //Makes dust in a messy circle
                    d.noGravity = true;
                    Vector2 direction = d.position - (Projectile.Center + positionOffset);
                    direction.Normalize();
                    direction *= 5;
                    d.velocity = direction;
                }
            }*/
        }
    }
}