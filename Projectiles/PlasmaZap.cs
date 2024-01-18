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
        }
    }
}