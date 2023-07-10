using Microsoft.Xna.Framework;
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
			Projectile.timeLeft = 360;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			Projectile.ai[0]++;

			if (Projectile.ai[0] < 180)
			{
				Projectile.velocity.Y = -2;

				Vector2 speed = Main.rand.NextVector2Circular(10, 10);
				Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.RainbowSparkle>(), speed, 0); //Makes dust in a messy circle
				d.noGravity = true;

				if (Projectile.ai[0] % 10 == 0)
				SoundEngine.PlaySound(SoundID.Pixie, Projectile.Center); //pixie noises
			}
			else
            {
				Projectile.velocity.Y = 0;

				Vector2 speed = Main.rand.NextVector2Circular(20, 20);
				Dust d = Dust.NewDustPerfect(Projectile.Center + speed * 20, ModContent.DustType<Dusts.DarkResidue>(), -speed, 0); //Makes dust in a messy circle
				d.noGravity = true;
			}

			if (Projectile.ai[0] == 300)
            {
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0, ModContent.ProjectileType<BallOfImpendingDoom>(), 0, 0f);
			}

			if (Projectile.ai[0] == 359)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient) // If the player is not in multiplayer, spawn directly
                {
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<NPCs.PureDarkMatter>());
                }
                else // If the player is in multiplayer, request a spawn
                {
                    //this will only work if NPCID.Sets.MPAllowedEnemies[type] is set in boss
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: ModContent.NPCType<NPCs.PureDarkMatter>());
                }

                SoundEngine.PlaySound(SoundID.Item74, Projectile.Center); //inferno explosion

				for (int i = 0; i < 40; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
					Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 20, Scale: 5); //Makes dust in a messy circle
					d.noGravity = true;
				}

				if (Main.netMode == NetmodeID.SinglePlayer)
				{
					Main.NewText("A great darkness has blanketed the world!", 175, 75);
				}
				else if (Main.netMode == NetmodeID.Server)
				{
					ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("A great darkness has blanketed the world!"), new Color(175, 75, 255));
				}
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
    }
}