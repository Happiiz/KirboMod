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
				Projectile.velocity.Y *= 0.98f;

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
				Dust d = Dust.NewDustPerfect(Projectile.Center + speed * 20, ModContent.DustType<Dusts.DarkResidue>(), -speed, Scale: 1 + (Projectile.ai[0] - 180) / 180); //Makes dust in a messy circle
				d.noGravity = true;
			}

			if (Projectile.ai[0] == 360)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient) // If not a client
                {
					NPC.SpawnBoss((int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<NPCs.Zero>(), player.whoAmI);
                }

                SoundEngine.PlaySound(SoundID.Item74, Projectile.Center); //inferno explosion

				for (int i = 0; i < 40; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
					Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 20, Scale: 5); //Makes dust in a messy circle
					d.noGravity = true;
				}
			}
		}
        public static Asset<Texture2D> Flash;

        public override void PostDraw(Color lightColor)
        {
            Flash = ModContent.Request<Texture2D>("KirboMod/Projectiles/FlyingPillarOfLightFlash");

            if (Projectile.ai[0] >= 300)
            {
				Main.EntitySpriteDraw(Flash.Value, Projectile.Center - Main.screenPosition, null, Color.White, 0, Flash.Size() / 2, 0.1f + (Projectile.ai[0] - 300) / 50, SpriteEffects.None);
            }
        }

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
    }
}