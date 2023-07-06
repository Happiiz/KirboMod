using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace KirboMod
{
    public class FighterComboSystem : ModSystem
    {
        public override void UpdateUI(GameTime gameTime)
        {
            ModContent.GetInstance<KirboMod>().fighterComboMeterInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (resourceBarIndex != -1)
            {
                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "KirboMod: Fighter Combo Meter",
                    delegate {
                        ModContent.GetInstance<KirboMod>().fighterComboMeterInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}