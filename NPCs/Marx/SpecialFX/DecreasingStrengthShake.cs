using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.CameraModifiers;

namespace KirboMod.NPCs.Marx.SpecialFX
{
    public class DecreasingStrengthShake : ICameraModifier
    {
        float time;
        float maxStrength;
        float duration;
        float spiralingRate;
        public string UniqueIdentity => "MarxLaser";
        public bool Finished => time > duration;

        public void Update(ref CameraInfo cameraPosition)
        {
            time++;
            float strength = maxStrength * (1f - (time / duration));
            float direction;// = time * spiralingRate;
            direction = Main.rand.NextFloat(MathF.PI * 2f);
            Vector2 offset = direction.ToRotationVector2() * strength;
            cameraPosition.CameraPosition += offset;
        }
        public static void Add(float duration = 10, float maxStrength = 30, float spiralingRate = 2f)
        {
            DecreasingStrengthShake shake = new();
            shake.duration = duration;
            shake.maxStrength = maxStrength;
            shake.spiralingRate = spiralingRate;
            shake.time = 0;
            Main.instance.CameraModifiers.Add(shake);
        }
    }
}
