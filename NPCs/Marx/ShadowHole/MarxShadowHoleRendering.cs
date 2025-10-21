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
        static Asset<Texture2D> shadowHoleTexture;
        static Asset<Texture2D> shadowHoleSparksCoverInnerTexture;
        static Asset<Texture2D> shadowHoleSparksCoverEverythingTexture;
        static Asset<Texture2D> shadowHoleBigTexture;
        static Asset<Texture2D> shadowHoleSparksBigTexture;
        public static int MaxShadowHoleDepthTiles => 30;
        void RenderShadowHole(SpriteBatch sb, Vector2 screenPos, float scaleMult)
        {
            if (shadowHoleTexture == null || shadowHoleBigTexture == null || shadowHoleSparksCoverEverythingTexture == null || shadowHoleSparksCoverInnerTexture == null)
            {
                LoadShadowHoleTextures(AssetRequestMode.ImmediateLoad);
            }
            Vector2 pos = SearchForShadowHolePosition();
            if (pos == Vector2.Zero)
            {
                return;//invalid rendering position
            }
            int framesXInImage = 13;
            int framesY = 30;                           //256 is gradient texture size
            int time = (int)(Main.timeForVisualEffects % int.MaxValue);
            Texture2D shadowHole = shadowHoleTexture.Value;
            Texture2D sparks = shadowHoleSparksCoverInnerTexture.Value;
            float textureScrollSpeed = .5f;
            int animationSpeed = 1;
            int frameY = (int)((time / textureScrollSpeed) % framesY);
            int frameX = (time / animationSpeed) % framesXInImage;
            Rectangle shadowHoleFrame = shadowHole.Frame(framesXInImage, 1, frameX, 0);
            Rectangle shadowSparksFrame = sparks.Frame(framesXInImage, framesY, frameX, frameY);
            pos -= screenPos;
            Vector2 scale = new(scaleMult * 2f, scaleMult * .5f );
            sb.Draw(shadowHole, pos, shadowHoleFrame, Color.White, 0f, shadowHoleFrame.Size() / 2, scale, SpriteEffects.None, 0f);
            sb.Draw(sparks, pos, shadowSparksFrame, Color.White, 0f, shadowSparksFrame.Size() / 2, scale, SpriteEffects.None, 0f);
        }
        static void LoadShadowHoleTextures(AssetRequestMode mode)
        {
            const string path = "KirboMod/NPCs/Marx/ShadowHole/";
            shadowHoleTexture = ModContent.Request<Texture2D>(path + "MarxShadowHole");
            shadowHoleSparksCoverEverythingTexture = ModContent.Request<Texture2D>(path + "Sparks_CoverEverything");
            shadowHoleSparksCoverInnerTexture = ModContent.Request<Texture2D>(path + "Sparks_CoverInner");
            shadowHoleSparksBigTexture = ModContent.Request<Texture2D>(path + "MarxShadowHole");//REPLACE WITH BIG VERSION LATER
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
                    return new Vector2((scanX) * 16 + 8, (scanY + j) * 16 + 8);
                }
                validTilesInScannedRow = 0;
            }
            return player.Center + new Vector2(0, maxScanDepthTiles * 10);
        }

        private bool ValidShadowHoleTile(int xToCheck, int yToCheck)
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
