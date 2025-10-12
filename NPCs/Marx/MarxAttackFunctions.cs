using KirboMod.Projectiles.Marx.Cutter;
using KirboMod.Projectiles.Marx.GiantBlackHoleOfDoom;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.NPCs.Marx
{
    public partial class MarxBoss : ModNPC
    {
        public static SoundStyle CutterShootLeftSFX => new("KirboMod/Sounds/NPC/Marx/CutterShotLeft");
        public static SoundStyle CutterShootRightSFX => new("KirboMod/Sounds/NPC/Marx/CutterShotRight");
        public static int CutterDamage => 80;//ADJUST LATER
        public void ShootCutters()
        {
            Vector2 sfxOffset = new(200, 0);
            SoundEngine.PlaySound(CutterShootLeftSFX, NPC.Center + sfxOffset);
            SoundEngine.PlaySound(CutterShootRightSFX, NPC.Center - sfxOffset);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            float shootSpeed = 17;
            int delayBeforeStartReturn = 30;
            if (Main.getGoodWorld)
            {
                delayBeforeStartReturn += 10;
                shootSpeed *= 1.2f;
            }
            if (Main.expertMode)
            {
                delayBeforeStartReturn += 10;
                shootSpeed *= 1.2f;
            }

            int cutters = 8;
            if (Main.getGoodWorld)
            {
                cutters = 12;
            }
            for (int i = 0; i < cutters; i++)
            {
                if (Main.expertMode || (i == 0 || i == cutters / 2 || i == 1 || i == cutters / 2 - 1))
                {
                    Vector2 velocity = Utils.Remap(i, 0, cutters, 0, MathF.Tau).ToRotationVector2() * shootSpeed;
                    MarxCutter.ShootCutter(delayBeforeStartReturn, velocity, NPC, CutterDamage);
                }
            }
            if (Main.getGoodWorld)
            {
                for (int i = 0; i < cutters; i++)
                {
                    Vector2 velocity = ((MathF.PI / cutters) +  Utils.Remap(i, 0, cutters, 0, MathF.Tau)).ToRotationVector2() * shootSpeed;
                    MarxCutter.ShootCutter(delayBeforeStartReturn + 50, velocity * .6f, NPC, CutterDamage);
                }
            }
        }
        void SpawnBlackHole()
        {
            if(Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MarxBlackHole>(), BlackHoleDamage, 0f);
            }
        }
    }
}
