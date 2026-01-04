using Microsoft.Xna.Framework;
using Terraria;

namespace KirboMod.Systems
{
    public static class Draw3D
    {
        public static Vector2 ScreenCenter => new(Main.screenPosition.X + Main.screenWidth / 2, Main.screenPosition.Y + Main.screenHeight / 2);
        public static float GetScaleFor3D(IProjWithZPos proj)
        {
            return GetScaleFor3D(proj.ZPos);
        }
        public static float GetScaleFor3D(float zPos)
        {
            //lower camera pos = smaller
            //higher z pos = smaller
            // div by 16 to make depth coords a bit easier to work with
            float safeDivisor = zPos / 16f + 1f;

            if (safeDivisor <= 0f || float.IsNaN(safeDivisor))
            {
                return 0f;
            }
            float scale = 1f / safeDivisor;
            return scale;
        }
        public static Vector2 Get3DDrawValues(IProjWithZPos proj, out float scale)
        {
            return Get3DDrawValues(proj.Projectile.Center, proj.ZPos, out scale);
        }
        public static Vector2 Get3DDrawValues(Vector2 worldPos, IProjWithZPos proj, out float scale)
        {
            return Get3DDrawValues(worldPos, proj.ZPos, out scale);
        }
        public static Vector2 Get3DDrawValues(Vector2 worldPos, float zPos, out float scale)
        {
            Vector2 screenCenter = ScreenCenter;
            scale = GetScaleFor3D(zPos);
            return Vector2.Lerp(screenCenter, worldPos, scale) - Main.screenPosition;
        }
    }
}