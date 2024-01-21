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

            // scanning if every tile on it exists, and if not kills the projectile

            for (int i = 0; i < Projectile.width / 16; i++) //tile width
            {
                int missedTiles = 0;

                for (int j = 0; j < Projectile.height / 16; j++) //tile height
                {
                    Point tileLocation = Projectile.position.ToTileCoordinates() + new Point(i, j);

                    Tile tile = Main.tile[tileLocation];

                    if (!tile.HasTile) //no tile
                    {
                        missedTiles += 1;
                        
                        continue;
                    }

                }

                if (missedTiles > 10)
                {
                    Projectile.velocity *= 0.001f;

                    if (Projectile.ai[0] <= 120) //not past dissipate point
                    {
                        Projectile.ai[0] = 120; //jump to disappear
                    }
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