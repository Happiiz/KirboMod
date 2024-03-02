using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class MasterSwordProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{ 

		}

		public override void SetDefaults()
		{ 
			Projectile.width = 26;
			Projectile.height = 26;
			DrawOriginOffsetX = -30;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.aiStyle = 19; //pole arm
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 18; //use time
		}

		public override void AI()
		{
			//part of spear ai
			Vector2 rotato = Main.player[Projectile.owner].RotatedRelativePoint(Main.player[Projectile.owner].MountedCenter);
			Projectile.direction = Main.player[Projectile.owner].direction;

			Projectile.position.X = rotato.X - (Projectile.width / 2);
			Projectile.position.Y = rotato.Y - (Projectile.height / 2);

			// Apply proper rotation, with an offset of 135 degrees due to the sprite's rotation, notice the usage of MathHelper, use this class!
			// MathHelper.ToRadians(xx degrees here)
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);
			// Offset by 90 degrees here
			if (Projectile.spriteDirection == -1)
			{
				Projectile.rotation -= MathHelper.ToRadians(90f);
			}

			Player player = Main.player[Projectile.owner];
			                                                      
			if (player.itemAnimation < player.itemAnimationMax / 2) //done dashing
            {
				Projectile.Kill();
            }
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}