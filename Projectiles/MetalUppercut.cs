using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class MetalUppercut : FighterUppercut
    {
        public override Color EndColor => Color.DarkGray;
        public override Color StartColor => Color.LightGray;
        public override int AnimationDuration => 8;
        public override int DecelerateDuration => 1;
        public override float HighSpeed => 40;
        new public static MetalUppercut SampleInstance => ContentSamples.ProjectilesByType[ModContent.ProjectileType<MetalUppercut>()].ModProjectile as MetalUppercut;

    }
}