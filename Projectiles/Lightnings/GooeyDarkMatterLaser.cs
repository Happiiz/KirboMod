using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Lightnings
{
    public class GooeyDarkMatterLaser : LightningProj
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            outerColor = Color.RoyalBlue;
            innerColor = Color.Black;
            width = 15;
            Projectile.scale = 1.5f;
            opacityFunction = OpacityFunction;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.ArmorPenetration = 30;
            SetAmountOfLightingSegments(5, Projectile.type);
            Projectile.usesLocalNPCImmunity = true;
            maxDeviation = 80;
            Projectile.localNPCHitCooldown = 30 * Projectile.MaxUpdates;
        }
        float OpacityFunction(float progress)
        {
            return Utils.GetLerpValue(0, .6f, progress);
        }
    }
}
