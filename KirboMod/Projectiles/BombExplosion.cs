using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;

namespace KirboMod.Projectiles
{
	public class BombExplosion : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 150;
			Projectile.height = 150;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 20;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 20;
			Projectile.alpha = 50;
		}
		public override void AI()
		{
			Projectile.ai[0]++;
			if (Projectile.ai[0] == 1)
			{
				SoundEngine.PlaySound(SoundID.Item14, Projectile.position); //bomb sound

                for (int j = 0; j < 3; j++)
                {
                    Vector2 positionOffset = new Vector2(Main.rand.Next(-100, 100), Main.rand.Next(-100, 100));

                    int dustID = Main.rand.Next(130, 134);

                    for (int i = 0; i < 16; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                    {
                        float rotationalOffset = MathHelper.ToRadians(i * 22.5f); //convert degrees to radians

                        float dustX = Projectile.Center.X + positionOffset.X + (float)Math.Cos(rotationalOffset) * 5;
                        float dustY = Projectile.Center.Y + positionOffset.Y + (float)Math.Sin(rotationalOffset) * 5;

                        Dust d = Dust.NewDustPerfect(new Vector2(dustX, dustY), dustID, Vector2.Zero); //Makes dust in a messy circle
                        d.noGravity = true;
                        Vector2 direction = d.position - (Projectile.Center + positionOffset);
                        direction.Normalize();
                        direction *= 5;
                        d.velocity = direction;
                    }
                }
            }

		    if (Projectile.ai[0] >= 10)
            {
				Projectile.alpha += 10;
            }
			Projectile.scale += 0.02f;

			Lighting.AddLight(Projectile.Center, 1f, 0.5f, 0);
        }

        public override Color? GetAlpha(Color lightColor)
        {
			return Color.White * Projectile.Opacity;
        }
    }
}