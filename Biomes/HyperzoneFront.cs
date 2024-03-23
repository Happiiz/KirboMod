using KirboMod.NPCs;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
namespace KirboMod.Biomes
{
    internal class HyperzoneFront : ModSystem
    {
        public override void Load()
        {
            On_ScreenObstruction.Draw += On_ScreenObstruction_Draw;
        }

        private void On_ScreenObstruction_Draw(On_ScreenObstruction.orig_Draw orig, SpriteBatch spriteBatch)
        {
            orig(spriteBatch);
            if (!SkyManager.Instance["KirboMod:HyperZone"].IsActive() || !ModContent.GetInstance<KirbConfig>().HyperzoneClouds)
                return;
            ZeroSky.DrawFrontLayer(spriteBatch);
        }
    }
}
