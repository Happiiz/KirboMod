using KirboMod.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BonkersSmash : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Smash");
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 100; 
			Projectile.height = 120; 
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 5;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
		}
		public override void AI()
		{
            NPC bonkers = Main.npc[(int)Projectile.ai[1]];

            if (bonkers.velocity.Y != 0) //suspend time
            {
                Projectile.timeLeft = 5;
            }

            Projectile.Center = bonkers.Center + new Vector2(bonkers.direction * 130, -10);

            if (bonkers.type == ModContent.NPCType<KingDedede>()) //not bonkers
            {
                Projectile.Center = bonkers.Center + new Vector2(bonkers.direction * 130, 20);
            }
            if (Projectile.ai[2] == 0) //Only do once
            {
                for (int i = 0; i < Projectile.width; i++)
                {
                    Point tileLocation = (Projectile.BottomLeft + new Vector2(i, 0)).ToTileCoordinates();

                    Tile tile = Main.tile[tileLocation];

                    if ((WorldGen.SolidOrSlopedTile(tile) || TileID.Sets.Platforms[tile.TileType]) && bonkers.velocity.Y == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item14, Projectile.position); //bomb sound

                        Projectile.ai[2] = 1;
                    }
                }
            }
            else
            {
                while (Projectile.ai[0] < 20)
                {
                    DoDustEffect();
                    Projectile.ai[0]++;
                }
            }
        }

        private void DoDustEffect()
        {
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

                    Tile tile = Framing.GetTileSafely(i, j);
                    if (!tile.HasTile)
                    {
                        continue;
                    }
                    Tile tileAbove = Framing.GetTileSafely(i, j - 1);
                    if (WorldGen.SolidOrSlopedTile(tileAbove) && TileID.Sets.Platforms[tileAbove.TileType] == true)
                    {
                        continue;
                    }

                    int dustAmount = WorldGen.KillTile_GetTileDustAmount(fail: true, tile, i, j);
                    for (int k = 0; k < dustAmount; k++)
                    {
                        Dust dust = Main.dust[WorldGen.KillTile_MakeTileDust(i, j, tile)];
                        dust.velocity.Y -= 3f + (float)((int)Projectile.ai[0] / 3) * 1.5f;
                        dust.velocity.Y *= Main.rand.NextFloat();
                        dust.scale += (float)((int)Projectile.ai[0] / 3) * 0.03f;
                        dust.position.Y -= 32;
                    }
                    if ((int)Projectile.ai[0] / 3 >= 2)
                    {
                        for (int l = 0; l < dustAmount - 1; l++)
                        {
                            Dust dust = Main.dust[WorldGen.KillTile_MakeTileDust(i, j, tile)];
                            dust.velocity.Y -= 1f + (float)((int)Projectile.ai[0] / 3);
                            dust.velocity.Y *= Main.rand.NextFloat();
                            dust.position.Y -= 32;
                        }
                    }
                    if (dustAmount > 0 && !Main.rand.NextBool(3))
                    {
                        float dustVel = (float)Math.Abs((topleft.X / 2 + topright.X / 2) - i) / 20f;
                        Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), Projectile.position, Vector2.Zero, 61 + Main.rand.Next(3), 1f - (float)((int)Projectile.ai[0] / 3) * 0.15f + dustVel * 0.5f);
                        gore.velocity.Y -= 0.1f + (float)((int)Projectile.ai[0] / 3) * 0.5f + dustVel * (float)((int)Projectile.ai[0] / 3) * 1f;
                        gore.velocity.Y *= Main.rand.NextFloat();
                        gore.position = new Vector2(i * 16 - 20, j * 16 - 20);
                    }
                }
            }
        }
    }
}