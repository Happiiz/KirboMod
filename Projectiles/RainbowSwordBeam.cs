using KirboMod.NPCs.DarkMatter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class RainbowSwordBeam : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Casting light");
		}
		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.timeLeft = 120; //seconds = timeLeft - extraUpdates / 60
			Projectile.tileCollide = false; 
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.ignoreWater = true;

			Projectile.extraUpdates = 20; //cycle through code multiple times in one tick
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
			
			Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0.5f); //white light

			for (int i = 0; i < 1; i++)
			{
				Vector2 position = Projectile.position;
				position -= Projectile.velocity * ((float)i * 0.25f);
				Projectile.alpha = 255;
				int deez = Dust.NewDust(position, 10, 10, ModContent.DustType<Dusts.RainbowSparkle>(), 0, 0, 0, Color.White);
				//int deez = Dust.NewDust(position, 1, 1, DustID.RedTorch);
				Main.dust[deez].position = position;
				Main.dust[deez].position.X += Projectile.width / 2;
				Main.dust[deez].position.Y += Projectile.height / 2;
				Main.dust[deez].scale = 1f; 
				Main.dust[deez].velocity *= 0.2f;
				Main.dust[deez].noGravity = true;
			}

			Player player = Main.player[Projectile.owner];

			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile proj = Main.projectile[i];

				if (Projectile.Hitbox.Intersects(proj.Hitbox) && proj.type == Mod.Find<ModProjectile>("DarkMirrorProj").Type && proj.active
					&& !NPC.AnyNPCs(ModContent.NPCType<DarkMatter>())) //summon NPC after contact with the mirror and no other NPCs of the same type are around
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        // If the player is not in multiplayer, spawn directly
                        NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<DarkMatter>());
                    }
                    else
                    {
                        // If the player is in multiplayer, request a spawn
                        // This will only work if NPCID.Sets.MPAllowedEnemies[type] is true in the boss
                        NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: ModContent.NPCType<DarkMatter>());
                    }

                    SoundEngine.PlaySound(SoundID.Roar, player.position);

                    //shoot another beam that's rising just to show reflection 
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0, -10), ModContent.ProjectileType<RisingRainbowBeam>(), 0, 0, player.whoAmI);

                    Projectile.Kill();
					break;
				}
			}
		}

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; //make it unaffected by light
        }
	}
}