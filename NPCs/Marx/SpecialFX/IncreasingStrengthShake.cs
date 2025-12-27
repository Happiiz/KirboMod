using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.CameraModifiers;

namespace KirboMod.NPCs.Marx.SpecialFX
{
    public class IncreasingStrengthShake : ICameraModifier
    {
        float time;
        float maxStrength;
        float duration;
        public string UniqueIdentity => "MarxLaserPre";

        public bool Finished => time > duration;

        public void Update(ref CameraInfo cameraPosition)
        {
            time++;
            float strength = maxStrength * (time / duration);
            float direction;// = time * spiralingRate;
            direction = Main.rand.NextFloat(MathF.PI * 2f);
            Vector2 offset = direction.ToRotationVector2() * strength;
            cameraPosition.CameraPosition += offset;
        }
        public static void Add(float duration = 10, float maxStrength = 30)
        {
            IncreasingStrengthShake shake = new();
            shake.duration = duration;
            shake.maxStrength = maxStrength;
            shake.time = 0;
            Main.instance.CameraModifiers.Add(shake);
        }
    }
}
