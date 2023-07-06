using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class GoodMatterOrb : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 500;
			Projectile.tileCollide = false;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;
		}
		public override void AI()
		{
			Player player = Main.player[Projectile.owner]; 

			Projectile.ai[0]++;

			if (Projectile.ai[0] == 1)
            {
				SoundEngine.PlaySound(SoundID.SplashWeak, player.Center);
			}
			if (Projectile.ai[0] < 20)
			{
				Projectile.velocity *= 0.92f;
			}

			if (Projectile.ai[0] == 20 && Projectile.owner == Main.myPlayer)
            {
				Vector2 move = move = Main.MouseWorld - Projectile.Center;
				move.Normalize();
				move *= 18;
				Projectile.velocity = move; //movemove = player.Center - projectile.Center; //update player position
			}

			if (Projectile.ai[0] == 30) //sixth of a second
			{
                Projectile.tileCollide = true;
            }

            if (++Projectile.frameCounter >= 5) //changes frames every 5 ticks 
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			SoundEngine.PlaySound(SoundID.Item10, Projectile.position); //impact
			return true; //collision
		}
		public override void Kill(int timeLeft) //when the projectile dies
		{
			for (int i = 0; i < 10; i++)
			{
				Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, ModContent.DustType<Dusts.DarkResidue>(), speed, Scale: 1f); //Makes dust in a messy circle
			}
		}

		public override Color? GetAlpha(Color lightColor)
        {
			return new Color(255, 255, 255); //white
        }
    }
}