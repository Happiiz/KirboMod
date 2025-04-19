using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class MetalFistProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }
        public static int MaxPenetrate => 5;
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.timeLeft = 7;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
            Projectile.penetrate = MaxPenetrate;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true; //uses own immunity frames
            Projectile.localNPCHitCooldown = 7; //time before hit again
            Projectile.ownerHitCheck = true;
            Projectile.alpha = 30;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.penetrate == MaxPenetrate)
            {
                KirbPlayer.IncreaseComboCounter(Projectile.owner);
            }
            Projectile.damage = (int)(Projectile.damage * 0.7f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity; //independent from light level while still being affected by opacity
        }

        public override bool? CanCutTiles() //only cut if player can "see" projectile (Hasn't gone through a wall)
        {
            Player player = Main.player[Projectile.owner];

            if (Collision.CanHit(player, Projectile))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}