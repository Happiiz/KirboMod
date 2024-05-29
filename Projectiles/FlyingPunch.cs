using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class FlyingPunch : ModProjectile
	{
		public override void SetStaticDefaults()
		{ 

		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 10;
			Projectile.friendly = true;
			Projectile.timeLeft = 7;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.usesLocalNPCImmunity = true; //uses own immunity frames
			Projectile.localNPCHitCooldown = 7; //time before hit again
			Projectile.ownerHitCheck = true;
			Projectile.alpha = 60;
		}

		public override void AI()
		{
            Projectile.rotation = Projectile.velocity.ToRotation(); 
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			Player player = Main.player[Projectile.owner];

			player.GetModPlayer<KirbPlayer>().fighterComboCounter += 1;
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

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity; //independent from light level while still being affected by opacity
        }
    }
}