using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class NewHammerShockwave : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 2;
			// DisplayName.SetDefault("Shockwave");
		}

		public override void SetDefaults()
		{
			Projectile.width = 84;
			Projectile.height = 186;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.timeLeft = 180;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.usesIDStaticNPCImmunity = true; //all projectiles of type share cooldown
			Projectile.idStaticNPCHitCooldown = 10; //time until all projectiles of type can hit npc
			Projectile.ignoreWater = true; //doesn't confine to water physics for it is on a higher realm of existance
		}

		public override bool? CanCutTiles()
		{
			return false; //can't break pots/grass/etc
		}
		public override void AI()
        {
            /*for (int i = -3; i < 5; i++) //count up when less than 25
			{
				Point projectileBottomFront = new Vector2(Projectile.Center.X + Projectile.direction * 25, 
					Projectile.position.Y + Projectile.height).ToTileCoordinates();

				Tile bottomFrontTile = Main.tile[new Point(projectileBottomFront.X, projectileBottomFront.Y + i)];

                Tile tileAboveBottomFrontTile = Main.tile[new Point(projectileBottomFront.X, projectileBottomFront.Y + i - 1)];

                if (WorldGen.SolidOrSlopedTile(bottomFrontTile) == false 
                    && bottomFrontTile.IsHalfBlock == false)
				{
					if (i >= 4) //checks if run out
					{
						Projectile.Kill();
					}
                }
				else if (tileAboveBottomFrontTile.HasTile == false)
				{
                    Projectile.position.Y += i * 16;
                    break; //stop loop
                }
            }

			if (Projectile.ai[0] % 3 == 0)
			{
				int dust = Dust.NewDust(Projectile.position + new Vector2(Projectile.direction == -1 ? 0 : 46, Projectile.height - 16), 1, 1, DustID.Electric, Projectile.direction * -0.4f, 0f, 0, default, 0.75f); //dust
				Main.dust[dust].noGravity = true;
			}*/

            // scanning if every tile on it exists, and if not kills the projectile

            for (int i = 0; i < Projectile.width / 16; i++) //tile width
            {
                int missedTiles = 0;

                if (Projectile.ai[0] > 120)
                {
                    break; //stop if already past death point
                }

                for (int j = 0; j < Projectile.height / 16; j++) //tile height
                {
                    Point tileLocation = Projectile.position.ToTileCoordinates() + new Point(i, j);

                    Tile tile = Main.tile[tileLocation];

                    if (!WorldGen.SolidOrSlopedTile(tile)) //no tile
                    {
                        missedTiles += 1;
                        
                        continue;
                    }

                }

                if (missedTiles > 10)
                {
                    Projectile.velocity *= 0.001f;
                    Projectile.ai[0] = 120; //jump to disappear
                    break;
                }
            }

            if (Projectile.ai[0] % 2 == 0)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 
                    Projectile.direction * -0.4f, 0f, 0, default, 0.75f); //dust
                Main.dust[dust].noGravity = true;
            }

            if (Projectile.ai[0] > 120)
            {
                Projectile.alpha += 5;
            }
            if (Projectile.alpha >= 255) //when invisble
            {
                Projectile.Kill();
            }

            Projectile.ai[0]++;

            Lighting.AddLight(Projectile.Center, 0f, 0.8f, 1f); //light blue light

            if (++Projectile.frameCounter >= 3) //changes frames every 3 ticks 
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        /*public override void Kill(int timeLeft) //when the projectile dies
         {
             for (int i = 0; i < 15; i++)
             {
                 Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
                 Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Electric, speed * 3, Scale: 0.75f); //Makes dust in a messy circle
             }
        }*/

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false; //dont die
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false; //don't fall through platforms
			return true;
        }

        public override Color? GetAlpha(Color lightColor)
		{
            return Color.White * Projectile.Opacity; //unaffedcted light color that can fade
        }
    }
}