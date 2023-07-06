using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class IceChunk : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 50;
			Projectile.friendly = true;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = true;
			Projectile.penetrate = 20;
		}

		public override void AI()
		{
            Projectile.direction = 1; //always face this direction

            Projectile.ai[0]++;

			if (Projectile.ai[0] == 1) //inital after being increased from 0 before
			{
                SoundEngine.PlaySound(SoundID.Item46, Projectile.position); //ice hydra
                for (int i = 0; i < 8; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(4f, 4f); //circle
                    Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Flake>(), speed * 2, Scale: 1f); //Makes dust in a messy circle
                    d.noGravity = false;
                }
            }

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

            //for stepping up tiles
            Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
        }

        /*public override bool OnTileCollide(Vector2 oldVelocity) //bounce
		{
			if (Projectile.velocity.X != oldVelocity.X)
			{
				Projectile.velocity.X = -oldVelocity.X;
			}
			if (Projectile.velocity.Y != oldVelocity.Y)
			{
				Projectile.velocity.Y = -oldVelocity.Y;
			}
			return false;
		}*/

		//bad ice chunk code
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

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false; //don't fall through tiles
            return true;
        }

        public override bool? CanCutTiles() 
        {
            return false; //can't shave things like grass or bee larva
        }

        public override void Kill(int timeLeft)
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