using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace KirboMod.NPCs.Marx.SpecialFX
{
    public static class LaserColorCorrection
    {
        public const string FilterName = "LaserColorCorrect";
        public static float time;
        public static float IntensityMult => 1f;
        public static float MaxExposureIncrease => 1f;
        public static float FadeDuration => 20;
        public static float Intensity => 1f + Utils.GetLerpValue(0, FadeDuration, time, true) * IntensityMult;
        public static float ExposureIntensity => 1f + Utils.GetLerpValue(0, FadeDuration, time, true) * MaxExposureIncrease;
        static bool deactivated = true;
        public static void CallOnLoad()
        {
            time = 100;
            Asset<Effect> effect = ModContent.Request<Effect>("KirboMod/NPCs/Marx/SpecialFX/LaserColorCorrectShader", AssetRequestMode.ImmediateLoad);
            Filters.Scene[FilterName] = new Filter(new ScreenShaderData(effect, "SplitPass"), EffectPriority.Medium);
            Filters.Scene[FilterName].Load();
        }
        public static void CallOnWorldLoad()
        {
            deactivated = true;
            time = -1;
            Filters.Scene.Deactivate(FilterName);
        }
        public static void ActivateScreenSaturation(float duration, bool includeFadeDuration = true)
        {
            deactivated = false;
            time = duration;
            if (includeFadeDuration)
            {
                time += FadeDuration;
            }
            float intensity = Intensity;
            ScreenShaderData shader = Filters.Scene[FilterName].GetShader();
            shader.UseIntensity(intensity);
            shader.UseProgress(ExposureIntensity);
            Filters.Scene.Activate(FilterName);
        }
        public static void Update()
        {
            time--;
            float intensity = Intensity;
            //Main.NewText("intens: " + intensity + ", t: " + time + ", deactivated flag: " + deactivated + ", active: " 
            //    + Filters.Scene[FilterName].Active, Main.DiscoColor);
            if (time >= 0)
            {
                ScreenShaderData shader = Filters.Scene[FilterName].GetShader();
                shader.UseIntensity(intensity);
                shader.UseProgress(ExposureIntensity);
                Filters.Scene.Activate(FilterName);
            }
            else if (Filters.Scene[FilterName].Active && intensity == 1)
            {
                deactivated = true;
                Filters.Scene.Deactivate(FilterName);
            }
        }
    }
    public class LaserColorCorrectionUpdateSystem : ModSystem
    {
        public override void OnWorldLoad()
        {
            LaserColorCorrection.CallOnWorldLoad();
        }
        public override void Load()
        {
            LaserColorCorrection.CallOnLoad();
        }
        public override void PostUpdateNPCs()
        {
            LaserColorCorrection.Update();
        }
    }
}
