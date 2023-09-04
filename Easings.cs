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
        public static float EaseInOutSine(float progress)
        {
            return -(MathF.Cos(MathF.PI * progress) - 1) / 2;
        }
        public static float RemapProgress(float increaseStart, float increaseEnd, float decreaseStart, float decreaseEnd, float value)
        {
            return Utils.GetLerpValue(increaseStart, increaseEnd, value, true) * Utils.GetLerpValue(decreaseEnd, decreaseStart, value, true);
        }
    }
}
