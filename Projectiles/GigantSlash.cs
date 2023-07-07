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
			Projectile.width = 114;
			Projectile.height = 200;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.alpha = 50;
		}

		public override void AI()
		{
			Projectile.spriteDirection = Projectile.direction; //face direction

			Projectile.velocity.X *= 0.9f; //slow

			if (Math.Abs(Projectile.velocity.X) < 0.5f)
			{
				Projectile.alpha += 10; //become more transparent
			}

			if (Projectile.alpha >= 255) //transparent
			{
				Projectile.Kill();
			}

			Projectile.ai[0]++; //goes up for gore smoke

			// all for dust btw

			if (Math.Abs(Projectile.velocity.X) > 5) //moving fast enough
			{
				for (int i = 0; i < Projectile.width / 16; i++) //tile width
				{
					for (int j = 0; j < Projectile.height / 16; j++) //tile height
					{
						/*Point tileLocation = Projectile.position.ToTileCoordinates() + new Point(i, j);

						Point aboveTileLocation = Projectile.position.ToTileCoordinates() + new Point(i, j - 1);

						Tile tile = Main.tile[tileLocation];

						Tile aboveTile = Main.tile[aboveTileLocation];*/

						int projTileX = Projectile.position.ToTileCoordinates().X;
						int projTileY = Projectile.position.ToTileCoordinates().Y;

						Tile tile = Framing.GetTileSafely(projTileX + i, projTileY + j);

						Tile aboveTile = Framing.GetTileSafely(projTileX + i, projTileY + j - 1);

						if (WorldGen.SolidOrSlopedTile(tile) && !WorldGen.SolidOrSlopedTile(aboveTile)) //no tile above tile
						{
                            SoundEngine.PlaySound(SoundID.Dig.WithVolumeScale(0.25f), Projectile.Center); // Dig
                            Dust d = Main.dust[WorldGen.KillTile_MakeTileDust(projTileX + i, projTileY + j, tile)];

							if (Projectile.ai[0] % 5 == 0) //every multiple of 5 (spawn yellow smoke)
							{
								Gore.NewGoreDirect(Projectile.GetSource_FromThis(), new Point(projTileX + i, projTileY + j).ToWorldCoordinates(), new Vector2(0, -0.5f),
									61 + Main.rand.Next(3), 0.75f);
							}
						}
					}
				}
			}
        }
    }
}