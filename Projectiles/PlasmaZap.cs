using Terraria;

namespace KirboMod.Projectiles
{
    public class PlasmaZap : BadPlasmaZap
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true; //make collide
            Projectile.width = Projectile.height = 16;
        }
    }
}