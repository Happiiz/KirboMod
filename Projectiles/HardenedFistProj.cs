using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class HardenedFistProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{ 

		}

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = true;
			Projectile.timeLeft = 7;
			Projectile.tileCollide = false;
			Projectile.penetrate = 3;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.usesLocalNPCImmunity = true; //uses own immunity frames
			Projectile.localNPCHitCooldown = 7; //time before hit again
            Projectile.ownerHitCheck = true;
            Projectile.alpha = 60;
		}
        ref float InitialVelLength => ref Projectile.localAI[0];
		public override void AI()
		{
            if(InitialVelLength == 0)
            {
                InitialVelLength = Projectile.velocity.Length();
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
		}

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity; //independent from light level while still being affected by opacity
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.5f);
            if (Projectile.penetrate == ContentSamples.ProjectilesByType[Type].penetrate)
            {
                KirbPlayer.IncreaseComboCounter(Projectile.owner);
            }
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