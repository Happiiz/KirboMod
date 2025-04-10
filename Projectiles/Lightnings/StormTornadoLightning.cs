using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Lightnings
{
    public class StormTornadoLightning : LightningProj
    {
        public override void SetStaticDefaults()
        {
            SetAmountOfLightingSegments(5, Type);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            outerColor = new Color(225, 73, 255);
            innerColor = Color.White;
            width = 15;
            Projectile.scale = 1f;
            opacityFunction = OpacityFunction;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Magic;
        }
        float OpacityFunction(float progress)
        {
            return 1;
        }
    }
}
