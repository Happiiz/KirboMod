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

                    for (int i = 0; i < 22; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                    {
						float rotationalOffset = (i * MathF.Tau) / 22f;
                        Dust d = Dust.NewDustPerfect(positionOffset + rotationalOffset.ToRotationVector2() * 5 + Projectile.Center, dustID, Vector2.Zero); //Makes dust in a circle
                        d.noGravity = true;
						d.velocity = rotationalOffset.ToRotationVector2() * 5;
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