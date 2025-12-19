using KirboMod.Projectiles.Marx.Cutter;
using KirboMod.Projectiles.Marx.GiantBlackHoleOfDoom;
using KirboMod.Projectiles.Marx.MassiveLaser;
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
        public static SoundStyle CutterChargeSFX => new("KirboMod/Sounds/NPC/Marx/CutterCharge");
        public static SoundStyle CutterShootRightSFX => new("KirboMod/Sounds/NPC/Marx/CutterShotRight");
        public static SoundStyle IceBombPuffUpSFX => new("KirboMod/Sounds/NPC/Marx/IceBombPuffUpCheeks");
        public static SoundStyle IceBombChaseSFX => new("KirboMod/Sounds/NPC/Marx/IceBombChase");
        public static SoundStyle IceBombSpitSFX => new("KirboMod/Sounds/NPC/Marx/IceBombSpit");
        public static SoundStyle MassiveLaserCharge => new("KirboMod/Sounds/NPC/Marx/BigLaserCharge");
        public static SoundStyle MassiveLaserShoot1 => new("KirboMod/Sounds/NPC/Marx/BigLaserShoot1");
        public static SoundStyle MassiveLaserShoot2 => new("KirboMod/Sounds/NPC/Marx/BigLaserShoot2");
        public static SoundStyle ShadowHoleStop => new("KirboMod/Sounds/NPC/Marx/ShadowHoleStop");
        public static SoundStyle ShadowHoleAppear => new("KirboMod/Sounds/NPC/Marx/ShadowHoleAppear");
        public static SoundStyle ShadowHoleDash => new("KirboMod/Sounds/NPC/Marx/ShadowHoleDash");
        public static SoundStyle VineSeedSpawn => new("KirboMod/Sounds/NPC/Marx/VineSeedSpawn");
        public static SoundStyle VineGrow => new("KirboMod/Sounds/NPC/Marx/VineGrow");
        public static SoundStyle MarxAmbientLaugh => new("KirboMod/Sounds/NPC/Marx/MarxAmbientLaugh");
        public static SoundStyle MarxDefeat => new("KirboMod/Sounds/NPC/Marx/MarxDeath");
        public static int CutterDamage => 80 / 2;//ADJUST LATER
        public static int MassiveLaserDamage => Main.getGoodWorld ? 9999 : (150 / 2);
        public static int ThornSeedDamage => 50 / 2;
        public static int VineDamage => 70 / 2;
        public static int IceBombDamage => 50 / 2;
        public static int BlackHoleDamage => 80 / 2;
        static int VineSeedDamage => -1;//seeds deal no damage, but thorns deal damage

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
                    Vector2 velocity = ((MathF.PI / cutters) + Utils.Remap(i, 0, cutters, 0, MathF.Tau)).ToRotationVector2() * shootSpeed;
                    MarxCutter.ShootCutter(delayBeforeStartReturn + 50, velocity * .6f, NPC, CutterDamage);
                }
            }
        }
        void SpawnBlackHole()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MarxBlackHole>(), BlackHoleDamage, 0f);
            }
        }
        void ShootMassiveLaser()
        {
            SoundEngine.PlaySound(MassiveLaserShoot2, NPC.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - NPC.velocity.Normalized(80), NPC.velocity, ModContent.ProjectileType<MarxMassiveLaser>(), MassiveLaserDamage, 0f, -1, MassiveLaserDuration);
            }
        }
    }
}
