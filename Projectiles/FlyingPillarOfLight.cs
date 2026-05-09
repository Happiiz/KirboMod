using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class FlyingPillarOfLight : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 114;
			Projectile.height = 94;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
									//why did I add + 389 here??????
			Projectile.timeLeft = BossSpawnTime + (KirboWorld.summonedDarkMatterRematchBefore ? 10 : 389);
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
		}
		public static int BossSpawnTime =>  KirboWorld.summonedDarkMatterRematchBefore? 20 : 360;
		public static float BlackBallBuildupDuration => KirboWorld.summonedDarkMatterRematchBefore ? 10f : 60f;
		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			float bossSpawnTime = BossSpawnTime;
            Projectile.ai[0]++;

			if (Projectile.ai[0] < bossSpawnTime / 2f)
			{
				Projectile.velocity.Y *= KirboWorld.summonedDarkMatterRematchBefore ? 0.89f : 0.96f;

				Vector2 speed = Main.rand.NextVector2Circular(10, 10);
				Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.RainbowSparkle>(), speed, 0); //Makes dust in a messy circle
				d.noGravity = true;

				if (Projectile.ai[0] % 10 == 0)
					SoundEngine.PlaySound(SoundID.Pixie, Projectile.Center); //pixie noises
			}
			else if (Projectile.ai[0] <= bossSpawnTime)
			{
				Projectile.velocity.Y = 0;

				Vector2 speed = Main.rand.NextVector2Circular(20, 20);
				Dust d = Dust.NewDustPerfect(Projectile.Center + speed * 20, ModContent.DustType<Dusts.DarkResidue>(), -speed, Scale: 1 + (Projectile.ai[0] - 180) / 180); //Makes dust in a messy circle
				d.noGravity = true;
			}

			float bossIntroDuration = 10;
			if (Projectile.ai[0] == bossSpawnTime)
			{

                if (Main.netMode != NetmodeID.MultiplayerClient) // If not a client
				{
					//-300 to compensate for zero spawning above for some reason
					int yOffset = KirboWorld.summonedDarkMatterRematchBefore ? 20 : 300;
					int xOffset = 20; 
					NPC.SpawnBoss((int)Projectile.Center.X + xOffset, (int)Projectile.Center.Y + yOffset, ModContent.NPCType<NPCs.PureDarkMatterRematch.PureDarkMatterRematch>(), player.whoAmI);
				}

				SoundEngine.PlaySound(SoundID.Item74, Projectile.Center); //inferno explosion

				for (int i = 0; i < 40; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
					Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 20, Scale: 5); //Makes dust in a messy circle
					d.noGravity = true;
				}
			}
			//float progress = Easings.RemapProgress(0, 30, bossSpawnTime + 389 - 20, bossSpawnTime + 389, Projectile.ai[0]);
			if (!KirboWorld.summonedDarkMatterRematchBefore)
			{
				float progress = Easings.RemapProgress(0, 30, bossSpawnTime + bossIntroDuration - 20, bossSpawnTime + bossIntroDuration, Projectile.ai[0]);
				progress = Easings.EaseInOutSine(progress);
				CameraScrollToZero.cameraCenter = Vector2.Lerp(Main.LocalPlayer.Center, Projectile.Center, progress);
			}
		}
		public static Asset<Texture2D> Flash;
        public override void OnKill(int timeLeft)
        {
            KirboWorld.summonedDarkMatterRematchBefore = true;
            CameraScrollToZero.cameraCenter = null;//just in case
        }
        public override void PostDraw(Color lightColor)
		{
			Flash = ModContent.Request<Texture2D>("KirboMod/Projectiles/FlyingPillarOfLightFlash");
           float bossSpawnTime = BossSpawnTime;

			float blackBallProgress = Utils.GetLerpValue(bossSpawnTime - BlackBallBuildupDuration, bossSpawnTime, Projectile.ai[0]);
			if (blackBallProgress >= 0 && blackBallProgress <= 1f)
			{
				Main.EntitySpriteDraw(Flash.Value, Projectile.Center - Main.screenPosition, null, Color.White, 0, Flash.Size() / 2, MathHelper.Lerp(0.01f, KirboWorld.summonedDarkMatterRematchBefore ? 0.3f : 1.2f, blackBallProgress), SpriteEffects.None);
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Projectile.ai[0] <= BossSpawnTime ? Color.White : default; // Makes it uneffected by light, and invisible after 360 frames
		}
		private class CameraScrollToZero : ModSystem
		{
			public static Vector2? cameraCenter = null;
			public override void ModifyScreenPosition()
			{
				if (cameraCenter.HasValue)
				{
					Main.screenPosition = cameraCenter.Value - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
					cameraCenter = null;
				}

			}
		}
	}
}