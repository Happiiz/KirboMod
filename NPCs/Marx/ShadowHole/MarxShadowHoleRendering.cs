using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.NPCs.Marx
{
    public partial class MarxBoss : ModNPC
    {
        static Asset<Texture2D> gradientSmallTexture;
        static Asset<Texture2D> sparksBigTexture;
        static Asset<Texture2D> sparksSmallTexture;
        static Asset<Texture2D> gradientBigTexture;
        public static int MaxShadowHoleDepthTiles => 30;
        void RenderShadowHole(SpriteBatch sb, Vector2 screenPos, float scaleMult, bool big = false)
        {
            if (gradientSmallTexture == null || gradientBigTexture == null || sparksSmallTexture == null || sparksBigTexture == null)
            {
                LoadShadowHoleTextures(AssetRequestMode.ImmediateLoad);
            }
            Vector2 pos = SearchForShadowHolePosition();
            if (pos == Vector2.Zero)
            {
                return;//invalid rendering position
            }
            int framesY = 30;                           //256 is gradient texture size
            int time = (int)(Main.timeForVisualEffects % int.MaxValue);
            Texture2D gradient = big ? gradientBigTexture.Value : gradientSmallTexture.Value;
            Texture2D sparks = big ? sparksBigTexture.Value : sparksSmallTexture.Value;
            float textureScrollSpeed = 1f;
            int frameY = (int)((time / textureScrollSpeed) % framesY);
            Rectangle shadowHoleFrame = gradient.Frame(1, 1, 0, 0);
            Rectangle shadowSparksFrame = sparks.Frame(1, framesY, 0, frameY);
            pos -= screenPos;
            Vector2 scale = new(scaleMult * 1f, scaleMult * 1.5f);
            pos.Y -= 44 * scale.Y;//the amount to line up with blocks
            sb.Draw(gradient, pos, shadowHoleFrame, Color.White, 0f, shadowHoleFrame.Size() / 2, scale, SpriteEffects.None, 0f);
            sb.Draw(sparks, pos, shadowSparksFrame, Color.White with { A = 128 }, 0f, shadowSparksFrame.Size() / 2, scale, SpriteEffects.None, 0f);
        }
        static void LoadShadowHoleTextures(AssetRequestMode mode)
        {
            const string path = "KirboMod/NPCs/Marx/ShadowHole/";
            gradientSmallTexture = ModContent.Request<Texture2D>(path + "GradientSmall");
            sparksSmallTexture = ModContent.Request<Texture2D>(path + "SparksSmall");
            sparksBigTexture = ModContent.Request<Texture2D>(path + "SparksBig");
            gradientBigTexture = ModContent.Request<Texture2D>(path + "GradientBig");
        }
        Vector2 SearchForShadowHolePosition()
        {
            if (!NPC.HasValidTarget)
            {
                return Vector2.Zero;
            }
            Player player = Main.player[NPC.target];

            int maxScanDepthTiles = MaxShadowHoleDepthTiles;
            int validTilesInScannedRow = 0;
            int scanWidth = 30;
            int validTilesInScanNeeded = (int)(scanWidth * .66f);
            Point scanPos = NPC.Center.ToTileCoordinates();
            int scanX = scanPos.X;
            int scanY = scanPos.Y;
            for (int j = 0; j < maxScanDepthTiles; j++)
            {
                int yToCheck = j + scanY;
                //-1 to compensate for checking tiles above also
                if (yToCheck < 0 || yToCheck >= Main.maxTilesY - 1)
                {
                    continue;
                }
                int yAboveToCheck = j + scanY - 1;
                for (int i = 0; i < scanWidth; i++)
                {
                    int xToCheck = scanX + i - scanWidth / 2;
                    if (xToCheck < 0 || xToCheck >= Main.maxTilesX)
                    {
                        continue;
                    }
                    if (ValidShadowHoleTile(xToCheck, yToCheck))
                    {
                        validTilesInScannedRow++;
                    }
                }
                if (validTilesInScannedRow >= validTilesInScanNeeded)
                {
                    return new Vector2(NPC.position.X + NPC.width / 2, (scanY + j) * 16 + 8);
                }
                validTilesInScannedRow = 0;
            }
            return new Vector2(NPC.position.X + NPC.width / 2, player.position.Y + player.height / 2  + maxScanDepthTiles * 10);
        }
        static int FindShadowHoleTileDown(int x, int y, int maxDepth)
        {
            for (int i = 0; i < maxDepth; i++)
            {
                int xToCheck = x;
                int yToCheck = y + i;
                if(ValidShadowHoleTile(xToCheck, yToCheck))
                {
                    return yToCheck;
                }
            }
            return -1;
        }
        private static bool ValidShadowHoleTile(int xToCheck, int yToCheck)
        {
            return SolidOrPlatform(xToCheck, yToCheck) && !SolidOrPlatform(xToCheck, yToCheck - 1);
        }

        private static bool SolidOrPlatform(int x, int y)
        {
            Tile t = Main.tile[x, y];
            if (!t.HasTile || !Main.tileSolid[t.TileType] && !TileID.Sets.Platforms[t.TileType])
            {
                return false;
            }
            t = Main.tile[x, y - 1];
            //has a tile or has a non-solid tile above
            return !t.HasTile || (t.HasTile && !Main.tileSolid[t.TileType]);
        }
    }
}
