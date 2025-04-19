using Microsoft.Xna.Framework;

namespace KirboMod.Projectiles
{
    public class MetalUppercut : FighterUppercut
    {
        public override Color EndColor => Color.DarkGray;
        public override Color StartColor => Color.LightGray;
        public override int AnimationDuration => 8;
        public override int DecelerateDuration => 1;
        public override float HighSpeed => 40;
    }
}