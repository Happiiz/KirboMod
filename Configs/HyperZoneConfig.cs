using KirboMod.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace KirboMod.Configs
{
    public class HyperZoneConfig : ModConfig
    {
        static HyperZoneConfig instance;
        public static HyperZoneConfig Instance => instance;
        public override void OnLoaded()
        {
            instance = this;
        }
        // ConfigScope.ClientSide should be used for client side, usually visual or audio tweaks.
        // ConfigScope.ServerSide should be used for basically everything else, including disabling items or changing NPC behaviours
        public override ConfigScope Mode => ConfigScope.ClientSide;
        public static Color BossButchFrontCloudsColor => new(255, 0, 0, 255);
        public static Color BossButchBGTintColor => new(255, 255, 115, 255);
        public static Color BossButchBackCloudsColor => new(74, 99, 255, 255);
        public const float DefaultBGTintOpacity = 0.4f;
        public const float DefaultFrontCloudsOpacity = 0.35f;
        public const byte DefaultSkyBrightness = 200;
        public const byte BossButchSkyBrightness = 255;
        public const float BossButchBGTingOpacity = 0.55f;
        public bool RegularPalette
        {
            get => HyperZoneCloudsBackColor == Color.Black && HyperZoneCloudsFrontColor == Color.Black && HyperZoneBackgroundTintColor == Color.Blue
                && HyperZoneCloudsFrontOpacity == DefaultFrontCloudsOpacity && HyperZoneCloudsBackOpacity == 1f && HyperZoneSkyBrightness == DefaultSkyBrightness
                && HyperZoneBackgroundTintOpacity == DefaultBGTintOpacity;
            set
            {
                if (value)
                {
                    HyperZoneCloudsBackColor = Color.Black;
                    HyperZoneCloudsFrontColor = Color.Black;
                    HyperZoneBackgroundTintColor = Color.Blue;
                    HyperZoneCloudsFrontOpacity = DefaultFrontCloudsOpacity;
                    HyperZoneCloudsBackOpacity = 1f;
                    HyperZoneSkyBrightness = DefaultSkyBrightness;
                    HyperZoneBackgroundTintOpacity = DefaultBGTintOpacity;
                }
            }
        }

        public bool BossButchPalette
        {
            get => HyperZoneCloudsBackColor == BossButchBackCloudsColor && HyperZoneCloudsFrontColor == BossButchFrontCloudsColor && HyperZoneBackgroundTintColor == BossButchBGTintColor
                && HyperZoneCloudsFrontOpacity == DefaultFrontCloudsOpacity && HyperZoneCloudsBackOpacity == 1f && HyperZoneSkyBrightness == BossButchSkyBrightness && HyperZoneBackgroundTintOpacity == BossButchBGTingOpacity;
            set
            {
                if (value)
                {
                    HyperZoneCloudsBackColor = BossButchBackCloudsColor;
                    HyperZoneCloudsFrontColor = BossButchFrontCloudsColor;
                    HyperZoneBackgroundTintColor = BossButchBGTintColor;
                    HyperZoneCloudsFrontOpacity = DefaultFrontCloudsOpacity;
                    HyperZoneCloudsBackOpacity = 1f;
                    HyperZoneSkyBrightness = BossButchSkyBrightness;
                    HyperZoneBackgroundTintOpacity = BossButchBGTingOpacity;

                }
            }
        }
        public bool StationaryClouds
        {
            get => HyperZoneBackScrollSpeed == 0 && HyperZoneFrontScrollSpeed == 0;
            set
            {
                if (value)
                {
                    HyperZoneBackScrollSpeed = 0;
                    HyperZoneFrontScrollSpeed = 0;
                }
            }
        }
        [DefaultValue(true)] // This sets the configs default value.
        //[ReloadRequired] // Marking it with [ReloadRequired] makes tModLoader force a mod reload if the option is changed. It should be used for things like item toggles, which only take effect during mod loading
        public bool DisableClouds; // master toggle (still will include tint)

        [DefaultValue(typeof(Color), "0, 0, 0, 255"), ColorHSLSlider(true)]
        public Color HyperZoneCloudsFrontColor;//black default
        [DefaultValue(DefaultFrontCloudsOpacity)]
        public float HyperZoneCloudsFrontOpacity;
        [DefaultValue(DefaultBGTintOpacity)]
        public float HyperZoneBackgroundTintOpacity;
        [DefaultValue(typeof(Color), "0, 0, 255, 255"), ColorHSLSlider(true)]
        public Color HyperZoneBackgroundTintColor;//blue default
        [DefaultValue(typeof(Color), "0, 0, 0, 255"), ColorHSLSlider(true)]
        public Color HyperZoneCloudsBackColor; //black default
        [DefaultValue(1f)]
        public float HyperZoneCloudsBackOpacity;
        [DefaultValue(200)]
        public byte HyperZoneSkyBrightness;
        [DefaultValue(ZeroSky.slideSpeed)]
        [Range(0, 20)]
        public float HyperZoneBackScrollSpeed;

        //is this right??
        //text should say
        //Default value is the Default Back scroll speed * 1.618
        [DefaultValue(ZeroSky.slideSpeed * Helper.Phi)]
        public float HyperZoneFrontScrollSpeed;





    }
}