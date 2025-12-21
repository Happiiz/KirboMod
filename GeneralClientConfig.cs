using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Terraria.ModLoader.Config;
using RangeAttribute = Terraria.ModLoader.Config.RangeAttribute;

namespace KirboMod
{
    public class GeneralClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("$Mods.KirboMod.Configs.GeneralConfig.FighterMeter.Header")]

        [LabelKey("$Mods.KirboMod.Configs.GeneralConfig.FighterMeter.X")]
        [TooltipKey("$")]
        [DefaultValue(0f)]
        [Range(-0.5f, 0.5f)]
        public float FighterMeterXOffset;

        [LabelKey("$Mods.KirboMod.Configs.GeneralConfig.FighterMeter.Y")]
        [TooltipKey("$")]
        [DefaultValue(0f)]
        [Range(-0.5f, 0.5f)]
        public float FighterMeterYOffset;
    }
}