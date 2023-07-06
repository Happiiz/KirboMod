using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class MaskedFireTornado : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 3;
			// DisplayName.SetDefault("Fire Spin");
		}

		public override void SetDefaults()
		{
			Projectile.width = 140;
			Projectile.height = 140;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.timeLeft = 24;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 4;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			Projectile.Center = player.Center;

			//Animation
			if (++Projectile.frameCounter >= 2) //changes frames every 2 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}

			int dustnumber = Dust.NewDust(Projectile.position, 124, 24, DustID.SolarFlare, 0f, 0f, 200, default, 0.8f); //dust
			Main.dust[dustnumber].velocity *= 0.3f;
			Main.dust[dustnumber].noGravity = true;

			Lighting.AddLight(Projectile.Center, 0.9f, 0.5f, 0f); //orange light
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.Daybreak, 1200);
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			Main.instance.DrawCacheProjsOverWiresUI.Add(index); //go in front of players
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}