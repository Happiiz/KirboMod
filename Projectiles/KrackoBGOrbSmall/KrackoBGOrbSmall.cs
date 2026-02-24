using KirboMod.NPCs;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using KirboMod.Systems;

namespace KirboMod.Projectiles.KrackoBGOrbSmall
{
    internal class KrackoBGOrbSmall : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_1";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            //Projectile.hostile = true;
            Projectile.scale = 0;
        }

        int TargetPlayerIndex => (int)Projectile.ai[0];
        ref float Timer => ref Projectile.localAI[0];
        static float ScaleupDuration => 1f;
        static float StayStillDuration => 0f;
        static float TravelDuration => 70;
        Vector2 FinalPos
        {
            get => new Vector2(Projectile.ai[1], Projectile.ai[2]); set
            {
                {
                    Projectile.ai[1] = value.X;
                    Projectile.ai[2] = value.Y;
                }
            }
        }
        ref float ProjSpawnOffset => ref Projectile.localAI[1];
        int KrackoIndex => (int)Projectile.localAI[2];
        ref int AfterimageDrawCount => ref Projectile.soundDelay;
        public static void GetSpawnParams(out float ai0, out float ai1, out float ai2, out Vector2 velocity, NPC npc, int playerTarget)
        {
            ai0 = playerTarget;
            ai1 = 0;
            ai2 = 0;
            velocity = new Vector2(0f, npc.whoAmI);
        }
        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                Projectile.localAI[2] = Projectile.velocity.Y;//workaround for out of ai slots

            }
            if (Timer <= StayStillDuration + ScaleupDuration)
            {
                if (KrackoIndex >= 0 && KrackoIndex < Main.maxNPCs)
                {
                    NPC kracko = Main.npc[KrackoIndex];
                    if (kracko.type == ModContent.NPCType<Kracko>() && kracko.active)
                    {
                        Projectile.Center = kracko.Center;
                    }
                }
            }
            Projectile.scale = Utils.Remap(Timer, 0f, ScaleupDuration, 0f, 1.5f);
            if (Timer == StayStillDuration + ScaleupDuration && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Player target = Main.player[TargetPlayerIndex];
                Vector2 targetPos = target.Center + target.velocity * TravelDuration;// + new Vector2(target.velocity.X * TravelDuration, 0);
                int randRange = 300;
                targetPos += new Vector2(Main.rand.Next(-randRange, randRange + 1), Main.rand.Next(-randRange, randRange + 1));
                Projectile.velocity = (targetPos - Projectile.Center) / TravelDuration;
                FinalPos = targetPos;
                Projectile.netUpdate = true;
            }
            int killTreshold = (int)(StayStillDuration + ScaleupDuration + TravelDuration);
            if (Timer > killTreshold)
            {
                SoundEngine.PlaySound(SoundID.Item4, Projectile.Center);
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0, -5);
                }
                Projectile.hostile = true;
            }
            if(Timer > killTreshold + 1)
            {
                Projectile.Kill();

            }
        }
        float GetScaleFor3D(int timeOffset)
        {
            float zPos = Helper.Remap(Timer - ScaleupDuration - StayStillDuration + timeOffset, 0, TravelDuration, Kracko.TargetZPosForBGAttacks, 0f);
            return Draw3D.GetScaleFor3D(zPos);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects fx = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = tex.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Color col = new(255, 255, 255, 200);
            int maxAfterimages = ProjectileID.Sets.TrailCacheLength[Type];
            //It was delayed if I didn't subtract 1 and the afterimages started appearing earlier than they were supposed to
            AfterimageDrawCount = (int)(Timer - ScaleupDuration - StayStillDuration - 1);
            float globalScaleMult = 3f;
            float globalOpacityMult = 0.3f;
            if (AfterimageDrawCount > maxAfterimages - 1)
            {
                AfterimageDrawCount = maxAfterimages - 1;
            }
            //if (AfterimageDrawCount > 0)
            //{
            //    for (int i = AfterimageDrawCount; i >= 0; i--)
            //    {
            //        Vector2 oldCenter = Projectile.oldPos[i] + Projectile.Size / 2;
            //        float oldZPosScaleMult = GetScaleFor3D(-i - 1);
            //        oldCenter = Vector2.Lerp(Main.screenPosition + new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f), oldCenter, oldZPosScaleMult);
            //        float opacityMult = 1f - (float)i / (ProjectileID.Sets.TrailCacheLength[Type] - 1f);
            //        VFX.DrawElectricOrb(oldCenter, Vector2.One * oldZPosScaleMult * globalScaleMult, Projectile.Opacity * opacityMult * globalOpacityMult, 0f);
            //    }
            //}
            float projScale = Projectile.scale;
            float zPosScaleMult = GetScaleFor3D(0);
            Vector2 center = Vector2.Lerp(Main.screenPosition + new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f), Projectile.Center, zPosScaleMult);
            Projectile.scale *= zPosScaleMult;

            VFX.DrawElectricOrb(center, Vector2.One * zPosScaleMult* globalScaleMult, Projectile.Opacity * globalOpacityMult, 0f);

            Projectile.scale = projScale;

            Vector2 finalPos = FinalPos;
            //explosion spot indicator
            VFX.DrawPrettyStarSparkle(1f, finalPos - Main.screenPosition, Color.White with { A = 0 }, VFX.RndElectricCol, 2f, 0f, 1f, 3f, 4f, 0f, Vector2.One * 2f, Vector2.One);
            return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Kracko.AddElectrifiedDebuff(target);
        }
        public override bool ShouldUpdatePosition()
        {
            return Timer > ScaleupDuration + StayStillDuration;
        }
    }
}
