using KirboMod.Biomes;
using KirboMod.NPCs;
using KirboMod.UI;
using System.IO;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.UI;

namespace KirboMod
{
    public class KirboMod : Mod
    {
        public static KirboMod instance;
        internal FighterComboMeter fighterComboMeter;
        internal UserInterface fighterComboMeterInterface;
        //if overhaul is enabled, the music fade skip implementation will cause a really bad noise when loading the mod
        //and (allegedly) also when loading into the world also
        public static bool DEBUG_NoMusicFadeSkip => ModLoader.TryGetMod("TerrariaOverhaul", out _);
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
                Filters.Scene[Hyperzone.BiomeName] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.4f, 0.4f, 0.9f).UseOpacity(0), EffectPriority.High);
                SkyManager.Instance[Hyperzone.BiomeName] = new ZeroSky();
                // Custom timer
                fighterComboMeter = new FighterComboMeter();
                fighterComboMeter.Activate();
                fighterComboMeterInterface = new UserInterface();
                fighterComboMeterInterface.SetState(fighterComboMeter);
            }
        }
    }
}