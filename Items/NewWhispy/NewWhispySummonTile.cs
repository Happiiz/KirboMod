using KirboMod.NPCs.NewWhispy;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace KirboMod.Items.NewWhispy
{
    public class NewWhispySummonTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileSolidTop[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            DustType = DustID.Grass;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.CoordinateHeights = [16];
            TileObjectData.addTile(Type);

            //AddMapEntry(new Color(200, 200, 200), CreateMapEntryName(), MapChestName)
        }
        public static bool TileStatus(int i, int j, out Vector2[] tileBlockingTilePositions, out Vector2[] tilePlacingNeededPositions)
        {

            int tileCount = (NewWhispyBoss.FightAreaWidth / 16) * (NewWhispyBoss.FightAreaHeight / 16);

            tileBlockingTilePositions = new Vector2[tileCount * 2];//*2 so both sides
            int tileBlockingPositionsCount = 0;
            int tilePlacingNeededPositionsCount = 0;
            tilePlacingNeededPositions = new Vector2[NewWhispyBoss.FightAreaWidth / 16];
            for (int k = -NewWhispyBoss.FightAreaWidth / 16; k <= NewWhispyBoss.FightAreaWidth / 16; k++)
            {
                for (int l = -NewWhispyBoss.FightAreaHeight / 16; l <= 0; l++)
                {
                    Tile tileToCheck = Main.tile[k + i, l + j];
                    if (tileToCheck == null)
                    {
                        continue;
                    }
                    if (tileToCheck.HasTile && !tileToCheck.IsActuated && (Main.tileSolid[tileToCheck.TileType] || Main.tileSolidTop[tileToCheck.TileType]))
                    {
                        if (tileBlockingPositionsCount >= tileBlockingTilePositions.Length)
                        {
                            Array.Resize(ref tileBlockingTilePositions, tilePlacingNeededPositionsCount + 1);
                        }
                        tileBlockingTilePositions[tileBlockingPositionsCount] = new Vector2((k + i) * 16 + 8, (l + j) * 16 + 8);
                        tileBlockingPositionsCount++;
                    }
                }
            }
            j++;//move DOWN one tile
            for (int k = -NewWhispyBoss.FightAreaWidth / 16; k <= NewWhispyBoss.FightAreaWidth / 16; k++)
            {
                Tile tileToCheck = Main.tile[k + i, j];
                if (!tileToCheck.HasTile || (tileToCheck.HasTile && !Main.tileSolid[tileToCheck.TileType] && !Main.tileSolidTop[tileToCheck.TileType]))
                {
                    if (tilePlacingNeededPositionsCount >= tilePlacingNeededPositions.Length)
                    {
                        Array.Resize(ref tilePlacingNeededPositions, tilePlacingNeededPositionsCount + 1);
                    }
                    tilePlacingNeededPositions[tilePlacingNeededPositionsCount] = new Vector2((i + k) * 16 + 8, j * 16 + 8);
                    tilePlacingNeededPositionsCount++;
                }
            }
            if (tileBlockingTilePositions[0] == Vector2.Zero)
            {
                tileBlockingTilePositions = null;
            }
            else
            {
                Array.Resize(ref tileBlockingTilePositions, tileBlockingPositionsCount);
            }
            if (tilePlacingNeededPositions[0] == Vector2.Zero)
            {
                tilePlacingNeededPositions = null;
            }
            else
            {
               
                Array.Resize(ref tilePlacingNeededPositions, tilePlacingNeededPositionsCount);
                
               
            }
           
            return tileBlockingPositionsCount == 0 && tilePlacingNeededPositionsCount == 0;
        }
        public override bool RightClick(int i, int j)
        {
            Tile curTile = Main.tile[i, j];
            Player player = Main.LocalPlayer;
            //IF NOT ENOUGH SPACE
            bool canSpawn = TileStatus(i, j, out Vector2[] blockingTiles, out Vector2[] neededTiles);
            Vector2 spawnPos = new(i * 16 + 8f, j * 16 + 8f);
            if (!canSpawn)
            {
                bool disabledEffect = false;
                int dustGeneratorProjType = RemoveWhispyDustIndicatorProjs(ref disabledEffect);
                if (!disabledEffect)
                {
                    Projectile.NewProjectile(new EntitySource_TileInteraction(Main.LocalPlayer, i, j), spawnPos, Vector2.Zero, dustGeneratorProjType, 0, 0);
                    string printText = "Inadequate Terrain!";
                    if (blockingTiles != null)
                    {
                        printText += " Remove tiles marked red";
                    }
                    bool capitalizeNext = true;
                    if (blockingTiles != null && neededTiles != null)
                    {
                        printText += " and";
                        capitalizeNext = false;
                    }
                    if (neededTiles != null)
                    {
                        printText += capitalizeNext ? " A" : " a";
                        printText += "dd solid tiles to places marked green.";
                    }
                    Main.NewText(printText, Color.Cyan);
                    Main.NewText("Right click the tile again to remove the colored indication", Color.Magenta);
                }
                return true;
            }
            int spawnX = i * 16 + 8;
            int spawnY = j * 16 + 16;
            if (NPC.AnyNPCs(ModContent.NPCType<NewWhispyBoss>()))
            {
                return true;
            }
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                bool unused = false;
                RemoveWhispyDustIndicatorProjs(ref unused);
                NPC.NewNPC(new EntitySource_TileInteraction(Main.LocalPlayer, i, j), spawnX, spawnY, ModContent.NPCType<NewWhispyBoss>());
            }
            else
            {
                //TODO: ADD NETCODE FOR WHISPY SPAWN
            }
            SoundEngine.PlaySound(SoundID.Roar, player.position);

            return true;
        }

        private static int RemoveWhispyDustIndicatorProjs(ref bool disabledEffect)
        {
            int dustGeneratorProjType = ModContent.ProjectileType<NewWhispySummonTileDustGenerator>();
            for (int k = 0; k < Main.maxProjectiles; k++)
            {
                Projectile proj = Main.projectile[k];
                if (proj.active && proj.type == dustGeneratorProjType)
                {
                    disabledEffect = true;
                    NewWhispySummonTileDustGenerator.QueueKill(proj);
                }
            }

            return dustGeneratorProjType;
        }
    }
    //doing it like this so that it syncs in multiplayer
    public class NewWhispySummonTileDustGenerator : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_1";//placeholder/dummy texture
        public override void SetDefaults()
        {
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 2;
        }
        public static void QueueKill(Projectile proj)
        {
            proj.ai[1] = 1f;
            proj.netUpdate = true;
        }
        public override void AI()
        {
            if (Projectile.localAI[0] % 45 == 0)
            {
                int i = (int)Projectile.position.X / 16;
                int j = (int)Projectile.position.Y / 16;
                NewWhispySummonTile.TileStatus(i, j, out Vector2[] tileBlockingPositions, out Vector2[] tileNeededPositions);
                if (tileBlockingPositions != null && tileBlockingPositions.Length > 0)
                {
                    for (int k = 0; k < tileBlockingPositions.Length; k++)
                    {
                        Vector2 pos = tileBlockingPositions[k];
                        Dust d = Dust.NewDustPerfect(pos, DustID.RainbowMk2, Vector2.Zero, 0, Color.Red, 2f);
                        d.noGravity = true;
                    }
                }
                if (tileNeededPositions != null && tileNeededPositions.Length > 0)
                {
                    for (int k = 0; k < tileNeededPositions.Length; k++)
                    {
                        Vector2 pos = tileNeededPositions[k];
                        Dust d = Dust.NewDustPerfect(pos, DustID.RainbowMk2, Vector2.Zero, 0, Color.Green, 2f);
                        d.noGravity = true;
                    }
                }
            }
            Projectile.localAI[0]++;

            //multiplayer reasons
            if (Projectile.ai[1] == 1)
            {
                Projectile.Kill();
            }
        }
    }
    //public class NewWhispySummonTileItem : ModItem
    //{

    //}
}
