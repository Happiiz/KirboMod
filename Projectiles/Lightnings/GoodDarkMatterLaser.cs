using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Lightnings
{
    public class GoodDarkMatterLaser : LightningProj
    {
        public override void SetStaticDefaults()
        {
            SetAmountOfLightingSegments(7, Projectile.type);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            outerColor = Color.MediumSlateBlue;
            innerColor = Color.Black;
            width = 15;
            maxDeviation = 70;
            Projectile.scale = 1.5f;
            opacityFunction = OpacityFunction;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Magic;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Projectile.damage = (int)(Projectile.damage * 0.8);
        }
        float OpacityFunction(float progress)
        {
            return 1;
        }
    }
}
