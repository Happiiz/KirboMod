using KirboMod.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod
{
    public static class Easings
    {
        public static float BackOut(float progress)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            return 1 + c3 * MathF.Pow(progress - 1, 3) + c1 * MathF.Pow(progress - 1, 2);
        }
        public static float EaseOut(float progress, float exponent)
        {
            return 1 - MathF.Pow(1 - progress, exponent);
        }
        public static float EaseIn(float progress, float exponent)
        {
            return MathF.Pow(progress, exponent);
        }
        public static float EaseInOutSine(float progress)
        {
            return -(MathF.Cos(MathF.PI * progress) - 1) / 2;
        }
        public static float EaseInOut(float progress, float exponent)
        {
            if( progress < .5f)
            {
                return MathF.Pow(2 * progress - 2, exponent) * .5f;
            }
            return 1 - MathF.Pow(2 * progress - 2, exponent) * .5f;
        }
        public static float InOutCirc(float progress)
        {
            return progress < 0.5f
             ? (1 - MathF.Sqrt(1 - MathF.Pow(2 * progress, 2))) / 2
             : (MathF.Sqrt(1 - MathF.Pow(-2 * progress + 2, 2)) + 1) / 2;
        }
        public static float BackInOut(float progress)
        {
            const float c1 = 1.70158f;
            float c2 = c1 * 1.525f;
            return progress < 0.5f
                ? (MathF.Pow(2 * progress, 2) * ((c2 + 1) * 2 * progress - c2)) / 2
                : (MathF.Pow(2 * progress - 2, 2) * ((c2 + 1) * (progress * 2 - 2) + c2) + 2) / 2;
        }
        public static float RemapProgress(float increaseStart, float increaseEnd, float decreaseStart, float decreaseEnd, float value)
        {
            return Utils.GetLerpValue(increaseStart, increaseEnd, value, true) * Utils.GetLerpValue(decreaseEnd, decreaseStart, value, true);
        }
    }
}
