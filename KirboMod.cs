using KirboMod.NPCs;
using KirboMod.UI;
using System.IO;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace KirboMod
{
	public class KirboMod : Mod
	{
        public static KirboMod instance;
        internal FighterComboMeter fighterComboMeter;
        internal UserInterface fighterComboMeterInterface;
       
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            NetMethods.HandlePacket(reader);
        }

       

        public override void Unload()
        {
            instance = null;
        }

        public override void Load()
        {
            instance = this;
            // All code below runs only if we're not loading on a server
            if (!Main.dedServ)
            {
                // Create new filters
                Filters.Scene["KirboMod:HyperZone"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.4f, 0.4f, 0.9f).UseOpacity(0), EffectPriority.High);
                SkyManager.Instance["KirboMod:HyperZone"] = new ZeroSky();

                // Custom timer
                fighterComboMeter = new FighterComboMeter();
                fighterComboMeter.Activate();
                fighterComboMeterInterface = new UserInterface();
                fighterComboMeterInterface.SetState(fighterComboMeter);
            }
        }
    }
}