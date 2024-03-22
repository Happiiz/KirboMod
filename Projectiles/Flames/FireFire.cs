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
            debuffID = BuffID.OnFire3;
            debuffDuration = 600;
            duration = defaultDuration;
            fadeOutDuration = defaultFadeOutDuration;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.extraUpdates = 1;
            startScale = .5f;
            endScale = 1f;
            trailLengthMultiplier = .6f;
            TotalDuration /= 2;
            whiteInsideSizeMultiplier = 1;
        }
    }
}