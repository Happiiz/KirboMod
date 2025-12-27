using KirboMod.NPCs.Marx.SpecialFX;
using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace KirboMod.Configs
{
    public class GFXConfig : ModConfig
    {
        static GFXConfig instance;
        public static GFXConfig Instance => instance;
        public override void OnLoaded()
        {
            instance = this;
        }
        // ConfigScope.ClientSide should be used for client side, usually visual or audio tweaks.
        // ConfigScope.ServerSide should be used for basically everything else, including disabling items or changing NPC behaviours

        public override ConfigScope Mode => ConfigScope.ClientSide;
        [Header("$Mods.KirboMod.Configs.GFXConfig.RainbowSwordGFXHeader")]
        [Range(0, 20)]
        [DefaultValue(10)]
        public int RainbowSwordHitSparklesAmount;
        [DefaultValue(1f)]
        [TooltipKey("$Mods.KirboMod.Configs.GFXConfig.RainbowSwordSwingSparklesAmountTooltip")]
        public float RainbowSwordSwingSparklesAmount;
        [DefaultValue(true)]
        [TooltipKey("$Mods.KirboMod.Configs.GFXConfig.ScaleSparkleAmountDependingOnQualitySettingTooltip")]
        public bool ScaleSparkleAmountDependingOnQualitySetting;
        [DefaultValue(true)]
        public bool Trail;
        [DefaultValue(1f)]
        public float TrailOpacity;
        [Header("$Mods.KirboMod.Configs.GFXConfig.MarxGFXHeader")]
        [DefaultValue(1f)]
        [Range(0f,2f)]
        public float MaxExposureIncrease;
        [DefaultValue(1f)]
        [Range(0,2f)]
        public float MaxSaturationIncrease;

        [DefaultValue(10f)]
        [Range(0f, 20f)]
        public float MaxLaserShootShakeStrength;
        [DefaultValue(4f)]
        [Range(0f, 20f)]
        public float MaxLaserChargeShake;

        //so players with autopause can immediately see changes if they pause while the laser is active
        public override void OnChanged()
        {
            float prevTime = LaserColorCorrection.time;
            LaserColorCorrection.Update();
            LaserColorCorrection.time = prevTime;
        }
    }
}