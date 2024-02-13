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
			Projectile.width = 150;
			Projectile.height = 150;
			DrawOffsetX = 10;
			DrawOriginOffsetY = -20;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.alpha = 50;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
		}

		public override void AI()
		{
            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.velocity *= 0.9f; //slow

			if (Math.Abs(Projectile.velocity.X) < 0.5f || Math.Abs(Projectile.velocity.Y) < 0.5f)
			{
				Projectile.alpha += 10; //become more transparent
			}
			else //make dust
			{
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 0, Color.White, 1f); //dust
                Main.dust[dustnumber].noGravity = true;
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.75f); //reduce
        }
    }
}