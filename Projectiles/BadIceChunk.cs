using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BadIceChunk : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Ice Cube");
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 50;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
		}

		public override void AI()
		{
            Projectile.ai[0]++;

            //spawning dust on bottom
            if (Projectile.velocity.Y == 0) //make dust on floor
            {
                if (Projectile.ai[0] % 5 == 0)
                {
                    Dust.NewDustPerfect(new Vector2(Projectile.Center.X + Projectile.direction * -20, Projectile.position.Y + 50), DustID.GemDiamond,
                        new Vector2(Projectile.velocity.X * -1, Main.rand.Next(-3, -1)), Scale: 0.5f); //Makes dust
                }
            }

            //Gravity
            Projectile.velocity.Y += 0.3f;

			if (Projectile.velocity.Y >= 12f)
            {
				Projectile.velocity.Y = 12f;
            }
        }

		public override bool OnTileCollide(Vector2 oldVelocity) 
		{
			if (oldVelocity.X != Projectile.velocity.X)
			{
				return true; //kill
			}
			else
			{
				return false; //don't die 
			}
		}

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position); //crystal smash
            for (int i = 0; i < 15; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				Vector2 speed = Main.rand.NextVector2Circular(2f, 2f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Flake>(), speed * 2, Scale: 1f); //Makes dust in a messy circle
				d.noGravity = true;
			}
		}
    }
}