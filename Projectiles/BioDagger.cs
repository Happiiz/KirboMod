using KirboMod.NPCs;
using KirboMod.NPCs.PlasmaWisp;
using KirboMod.Systems;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class BioDagger : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Throwing Knife");
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            DrawOffsetX = -8;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 70;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
        }
        public override void AI()
        {
            if (Projectile.timeLeft % (20 * Projectile.MaxUpdates) == (3 + Projectile.whoAmI) % 20)
            {
                KirbyTransformationModCompatibilityHelper.SpawnSingleDropStar(Projectile.GetSource_FromAI(), Projectile.Center, BioSpark.DaggerDropStarDamage);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
}