using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Flames
{
    public class FireFire : BadFire
    {
        public override void SetDefaults()
        {
            base.FlamethrowerStats();//copy flamethrower stats of bad fire(including color)
            debuffID = BuffID.OnFire;
            debuffDuration = 600;
            duration = defaultDuration;
            fadeOutDuration = defaultFadeOutDuration;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            startScale = .7f;
            Projectile.extraUpdates = 2;
            startScale = .4f;
            endScale = 1.1f;
            TotalDuration /= 2;
            whiteInsideSizeMultiplier = 1;
        }
    }
}