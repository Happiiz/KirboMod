using KirboMod.NPCs;
using KirboMod.NPCs.Marx.Townie;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Marx
{
	public class FlyingSparklingSoul : ModProjectile
	{
        public override string Texture => "KirboMod/Items/SparklingSoul";

		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 50;
			DrawOriginOffsetY = -14;
            DrawOffsetX = -16;
            Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			//Projectile.timeLeft = 360 + 389;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
            Projectile.timeLeft = 2;

            Projectile.ai[0]++;

			if (Projectile.ai[0] == 1 && NPC.AnyNPCs(ModContent.NPCType<MarxTownie>()))
			{
                NPC marx = Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MarxTownie>())];

                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 0; i < 10; i++) //cloud particles to signify "leave of absence"
                    {
                        Gore.NewGorePerfect(marx.GetSource_FromThis(), marx.Center, Main.rand.NextVector2Circular(5, 5), Main.rand.Next(11, 14), Main.rand.NextFloat() * 0.5f + 0.5f);
                    }
                }

                marx.active = false; //delete first Marx
            }

			if (Projectile.ai[0] < 300)
            {
                Vector2 speed = Main.rand.NextVector2Circular(20, 20);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.MagicMirror, speed, Scale: 2);
                d.noGravity = true;

                Projectile.velocity.Y *= 0.9f;

                if (Projectile.ai[0] % 10 == 0)
                    SoundEngine.PlaySound(SoundID.Pixie, Projectile.Center); //pixie noises
            }
			else if (Projectile.ai[0] == 300)
			{
                Projectile.velocity.Y = 0;

                int index = -1;

                if (Main.netMode != NetmodeID.MultiplayerClient) // If not a client
				{
                    index = NPC.NewNPC(Projectile.GetSource_FromAI(), (int)Projectile.Center.X, (int)Projectile.Center.Y - 300, ModContent.NPCType<MarxPrelude>());
                }

                if (index != -1)
                {
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, index);
                }

                SoundEngine.PlaySound(new SoundStyle("KirboMod/Sounds/NPC/Marx/MarxSummonLaugh"), Projectile.Center);
			}
            else if (Projectile.ai[0] >= 360 && Projectile.ai[0] < 390)
            {
                NPC marc = Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MarxPrelude>())];

                Projectile.velocity = (marc.Center - Projectile.Center) / 5;
            }
            else if (Projectile.ai[0] >= 390)
            {
                Projectile.Kill();
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter < 5)
            {
                Projectile.frame = 0;
            }
            else if (Projectile.frameCounter < 10)
            {
                Projectile.frame = 1;
            }
            else if (Projectile.frameCounter < 15)
            {
                Projectile.frame = 2;
            }
            else if (Projectile.frameCounter < 20)
            {
                Projectile.frame = 3;
            }
            else
            {
                Projectile.frameCounter = 0;
            }
        }

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 60; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(50, 50);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.MagicMirror, speed, Scale: 2f);
                d.noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.Item28, Projectile.Center);
        }
	}
}