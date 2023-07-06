using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class MasterDash : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 5;
		}

		public override void SetDefaults()
		{
			Player player = Main.player[Projectile.owner];

			Projectile.width = 120;
			Projectile.height = 120;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			//projectile.aiStyle = 19; //pole arm
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10; //cooldown per enemy
		}

		public override void AI()
		{
			//part of spear ai
			Vector2 rotato = Main.player[Projectile.owner].RotatedRelativePoint(Main.player[Projectile.owner].MountedCenter);
			Projectile.direction = Main.player[Projectile.owner].direction;

			Projectile.position.X = rotato.X - (float)(Projectile.width / 2);
			Projectile.position.Y = rotato.Y - (float)(Projectile.height / 2);

			Projectile.rotation = Projectile.velocity.ToRotation(); //point direction it's going

            //animation
            if (++Projectile.frameCounter >= 2) //changes frames every 2 ticks 
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            Player player = Main.player[Projectile.owner];

            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Scale: 2, 
				SpeedX: -Projectile.velocity.X, SpeedY: -Projectile.velocity.Y);

            //if max 60, then this 30
            if (player.itemAnimation < player.itemAnimationMax - (player.itemAnimationMax / 2)) //done dashing
			{
				Projectile.Kill();
			}
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