using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace KirboMod.Projectiles
{
	public class GigantSlash : ModProjectile
	{
		private ref float tileSliding => ref Projectile.ai[1]; //tileSliding determines if the projectile is sliding on tile or not
		public override void SetStaticDefaults()
		{

		}

		public override void SetDefaults()
		{
			Projectile.width = 11;
			Projectile.height = 11;
			DrawOffsetX = -52;
			DrawOriginOffsetY = -95;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.alpha = 50;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
		}

		public override void AI()
		{
            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.velocity *= 0.93f; //slow

			if (Math.Abs(Projectile.velocity.X) < 1f && Math.Abs(Projectile.velocity.Y) < 1f)
			{
				Projectile.alpha += 10; //become more transparent
			}

			if (Projectile.alpha >= 255) //transparent
			{
				Projectile.Kill();
			}

			Projectile.ai[0]++; //goes up for gore smoke

            // all for dust btw
            for (int i = 0; i < 208 / 16; i++) //tile width
            {
                for (int j = 0; j < 208 / 16; j++) //tile height
                {
                    Vector2 positionOffset = new Vector2(Projectile.position.X - 104, Projectile.position.Y - 104);

                    int projTileX = positionOffset.ToTileCoordinates().X;
                    int projTileY = positionOffset.ToTileCoordinates().Y;

                    Tile tile = Framing.GetTileSafely(projTileX + i, projTileY + j);

                    Tile aboveTile = Framing.GetTileSafely(projTileX + i, projTileY + j - 1);

                    Point dustArea = new Point(projTileX + i, projTileY + j);

                    //no tile above tile and moving fast enough
                    if (WorldGen.SolidOrSlopedTile(tile) && !WorldGen.SolidOrSlopedTile(aboveTile) && Math.Abs(Projectile.velocity.X) > 5) 
                    {
                        SoundEngine.PlaySound(SoundID.Dig.WithVolumeScale(0.25f), Projectile.Center); // Dig
                        Dust d = Main.dust[WorldGen.KillTile_MakeTileDust(projTileX + i, projTileY + j, tile)];

                        if (Projectile.ai[0] % 5 == 0) //every multiple of 5 (spawn yellow smoke)
                        {
                            Gore.NewGoreDirect(Projectile.GetSource_FromThis(), dustArea.ToWorldCoordinates(), new Vector2(0, -0.5f),
                                61 + Main.rand.Next(3), 0.75f);
                        }
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.75f); //reduce
        }

        public override bool OnTileCollide(Vector2 oldVelocity) //if touching a tile (will kill it)
        {
            for (int i = 0; i < 20; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
                Gore.NewGorePerfect(Projectile.GetSource_FromAI(), Projectile.Center, speed,
                                61 + Main.rand.Next(3));
            }

            return true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return targetHitbox.IntersectsConeSlowMoreAccurate(Projectile.Center, 100, Projectile.rotation, MathF.PI / 2f);
        }
    }
}