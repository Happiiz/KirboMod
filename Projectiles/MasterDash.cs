using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class MasterDash : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;

            //for afterimages
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

		public override void SetDefaults()
		{
			Projectile.width = 400;
			Projectile.height = 400;
			DrawOffsetX = -100;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			//projectile.aiStyle = 19; //pole arm
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10; //cooldown per enemy
            Projectile.ownerHitCheck = true; //check if owner has line of sight to hit
        }

		public override void AI()
		{
			//part of spear ai
			Vector2 rotato = Main.player[Projectile.owner].RotatedRelativePoint(Main.player[Projectile.owner].MountedCenter);
			Projectile.direction = Main.player[Projectile.owner].direction;

			Projectile.position.X = rotato.X - (Projectile.width / 2);
			Projectile.position.Y = rotato.Y - (Projectile.height / 2);

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

			//two groups of dust
			for (int i = 0; i < 6; ++i)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Scale: 2,
					SpeedX: -Projectile.velocity.X, SpeedY: -Projectile.velocity.Y);
				Main.dust[dust].noGravity = true;
			}

            for (int i = 0; i < 3; ++i)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SolarFlare, Scale: 2f,
                    SpeedX: -Projectile.velocity.X, SpeedY: -Projectile.velocity.Y);
                Main.dust[dust].noGravity = true;
            }

            if (player.itemAnimation < player.itemAnimationMax / 2) //done dashing
			{
				Projectile.Kill();
			}

			Projectile.Opacity = Utils.GetLerpValue(player.itemAnimationMax / 2, player.itemAnimationMax / 2 + 10, player.itemAnimation, true);
		}

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            for (int i = 0; i < 32; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(20f, 20f);

                Dust d = Dust.NewDustPerfect(target.Center, DustID.SolarFlare, speed, Scale: 2f);
                d.noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.Item14, Projectile.position); //bomb sound
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            for (int k = 1; k < Projectile.oldPos.Length; k++) //start at 1 so not ontop of actual projectile
            {
                Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
                Vector2 drawOrigin = frame.Size() / 2;
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(-100, Projectile.gfxOffY);

                Color color = Color.White * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * 0.5f * Projectile.Opacity;
                Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.rotation, drawOrigin, 1f, SpriteEffects.None, 0);
            }
            return true; //draw og
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			Main.instance.DrawCacheProjsOverPlayers.Add(index); //go in front of players
		}

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White * 0.5f * Projectile.Opacity; // Makes it unaffected by light
		}
	}
}