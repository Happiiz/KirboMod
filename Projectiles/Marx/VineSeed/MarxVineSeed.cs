using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Marx.VineSeed
{
    public class MarxVineSeed : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CanHitPastShimmer[Type] = true;
        }
    }
}
