using KirboMod.NPCs;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
namespace KirboMod.Biomes
{
    internal class HyperzoneFront : ModSystem
    {
        public override void Load()
        {
            On_Main.DrawItemTextPopups += On_Main_DrawItemTextPopups;
        }
        private void On_Main_DrawItemTextPopups(On_Main.orig_DrawItemTextPopups orig, float scaleTarget)
        {
            orig(scaleTarget);
            if (!SkyManager.Instance["KirboMod:HyperZone"].IsActive() || !ModContent.GetInstance<KirbConfig>().HyperzoneClouds)
                return;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer);
            ZeroSky.DrawFrontLayer(Main.spriteBatch);
            //DON'T END SPRITEBATCH BECAUSE VANILLA ENDS IT AFTER THIS DETOUR!!
        }
    }
}
