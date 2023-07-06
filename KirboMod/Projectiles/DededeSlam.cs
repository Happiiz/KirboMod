using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class DededeSlam : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Slam");
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 15;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 120;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
		}
		public override void AI()
		{
			//alot of this was taken from ogre smash ai

			Projectile.ai[0] += 1f;
			if (Projectile.ai[0] > 9f)
			{
				Projectile.Kill();
				return;
			}

			Projectile.velocity = Vector2.Zero;
			Projectile.position = Projectile.Center;
			Projectile.Size = new Vector2(16f, 8) * MathHelper.Lerp(5f, 40f, Utils.GetLerpValue(0f, 9f, Projectile.ai[0]));
			Projectile.Center = Projectile.position;

			Point topleft = Projectile.TopLeft.ToTileCoordinates();
			Point topright = Projectile.BottomRight.ToTileCoordinates();

			if ((int)Projectile.ai[0] % 3 != 0)
			{
				return;
			}

			int num3 = (int)Projectile.ai[0] / 3;

			for (int i = topleft.X; i <= topright.X; i++)
			{
				for (int j = topleft.Y; j <= topright.Y; j++)
				{
					if (Vector2.Distance(Projectile.Center, new Vector2(i * 16, j * 16)) > (float)Projectile.width / 2)
					{
						continue;
					}

					Tile tileSafely = Framing.GetTileSafely(i, j);
					if (!tileSafely.HasTile)
					{
						continue;
					}
					Tile tileSafely2 = Framing.GetTileSafely(i, j - 1);
					if (tileSafely2.HasTile)
					{
						continue;
					}

					int num4 = WorldGen.KillTile_GetTileDustAmount(fail: true, tileSafely, i, j);
					for (int k = 0; k < num4; k++)
					{
						Dust obj = Main.dust[WorldGen.KillTile_MakeTileDust(i, j, tileSafely)];
						obj.velocity.Y -= 3f + (float)((int)Projectile.ai[0] / 3) * 1.5f;
						obj.velocity.Y *= Main.rand.NextFloat();
						obj.scale += (float)((int)Projectile.ai[0] / 3) * 0.03f;
					}
					if ((int)Projectile.ai[0] / 3 >= 2)
					{
						for (int l = 0; l < num4 - 1; l++)
						{
							Dust obj2 = Main.dust[WorldGen.KillTile_MakeTileDust(i, j, tileSafely)];
							obj2.velocity.Y -= 1f + (float)((int)Projectile.ai[0] / 3);
							obj2.velocity.Y *= Main.rand.NextFloat();
						}
					}
					if (num4 > 0 && !Main.rand.NextBool(3))
					{
						float num5 = (float)Math.Abs((topleft.X / 2 + topright.X / 2) - i) / 20f;
						Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), Projectile.position, Vector2.Zero, 61 + Main.rand.Next(3), 1f - (float)((int)Projectile.ai[0] / 3) * 0.15f + num5 * 0.5f);
						gore.velocity.Y -= 0.1f + (float)((int)Projectile.ai[0] / 3) * 0.5f + num5 * (float)((int)Projectile.ai[0] / 3) * 1f;
						gore.velocity.Y *= Main.rand.NextFloat();
						gore.position = new Vector2(i * 16 + 20, j * 16 + 20);
					}
				}
			}
		}
	}
}