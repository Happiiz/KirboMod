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
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
}