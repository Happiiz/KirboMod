using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class MetalFistProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{ 

		}

		public override void SetDefaults()
		{
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.friendly = true;
			Projectile.timeLeft = 7;
			Projectile.tileCollide = false;
			Projectile.penetrate = 99;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.usesLocalNPCImmunity = true; //uses own immunity frames
			Projectile.localNPCHitCooldown = 7; //time before hit again
			Projectile.alpha = 30;
		}
		public override void AI()
		{
            Projectile.rotation = Projectile.velocity.ToRotation();

            Player player = Main.player[Projectile.owner];
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.player[Projectile.owner];

			player.GetModPlayer<KirbPlayer>().fighterComboResetDelay = 60;

			player.GetModPlayer<KirbPlayer>().fighterComboCounter += 1;
		}
	}
}