using KirboMod.NPCs;
using KirboMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.KrackoBgOrbBig
{
    public class KrackoBGOrbBig : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.scale = 0;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        int TargetPlayerIndex => (int)Projectile.ai[0];
        ref float Timer => ref Projectile.localAI[0];
        static float ScaleupDuration => 12f;
        static float StayStillDuration => 3f;
        static float TravelDuration => 60f;
        Vector2 FinalPos
        {
            get => new(Projectile.ai[1], Projectile.ai[2]); set
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
        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                ProjSpawnOffset = Projectile.velocity.X;//workaround for out of ai slots
                Projectile.localAI[2] = Projectile.velocity.Y;//workaround for out of ai slots

            }
            if (Projectile.frameCounter++ >= 2)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                Projectile.frame %= Main.projFrames[Type];
            }
            if (Timer <= StayStillDuration + ScaleupDuration)
            {
                NPC kracko = Main.npc[KrackoIndex];
                if (kracko.type == ModContent.NPCType<Kracko>() && kracko.active)
                {
                    Projectile.Center = kracko.Center;
                }
            }
            Projectile.scale = Utils.Remap(Timer, 0f, ScaleupDuration, 0f, 1.5f);
            float initialYVel = -20f;
            if (Timer == StayStillDuration + ScaleupDuration)
            {
                SoundEngine.PlaySound(Kracko.BGBigOrbShot with { MaxInstances = 0 }, Projectile.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Player target = Main.player[TargetPlayerIndex];
                    Vector2 targetPos = target.Center + new Vector2(target.velocity.X * TravelDuration, 0);
                    Projectile.velocity = (targetPos - Projectile.Center) / TravelDuration;
                    Projectile.velocity.Y += initialYVel;
                    FinalPos = targetPos;
                    Projectile.netUpdate = true;
                }
            }
            if (Timer >= StayStillDuration + ScaleupDuration + TravelDuration)
            {
                SoundEngine.PlaySound(Kracko.BGBigOrbExplodeSFX, Projectile.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float projCount = 8f;
                    for (int i = 0; i < projCount; i++)
                    {
                        float offset = ProjSpawnOffset;
                        float angle = Helper.Remap(i, 0, projCount, 0, MathF.Tau) + offset;
                        Vector2 vel = angle.ToRotationVector2() * 20f;
                        Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, vel, ModContent.ProjectileType<KrackoBGOrbBigFrag.KrackoBGOrbBigFrag>(), 26 / 2, 0);
                    }
                }
                Projectile.Kill();
            }
            if (Timer > StayStillDuration + ScaleupDuration)
            {
                float yAccel = 2 * initialYVel / TravelDuration;
                Projectile.velocity.Y += -yAccel;
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
            if (AfterimageDrawCount > maxAfterimages - 1)
            {
                AfterimageDrawCount = maxAfterimages - 1;
            }
            if (AfterimageDrawCount > 0)
            {
                for (int i = AfterimageDrawCount; i >= 0; i--)
                {
                    Vector2 oldCenter = Projectile.oldPos[i] + Projectile.Size / 2;
                    float oldZPosScaleMult = GetScaleFor3D(-i - 1);
                    oldCenter = Vector2.Lerp(Main.screenPosition + new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f), oldCenter, oldZPosScaleMult);
                    float opacityMult = 1f - i / (ProjectileID.Sets.TrailCacheLength[Type] - 1f);
                    Main.EntitySpriteDraw(tex, oldCenter - Main.screenPosition, frame, col * Projectile.Opacity * opacityMult, Projectile.rotation, frame.Size() / 2, oldZPosScaleMult * Projectile.scale, fx);
                }
            }
            float projScale = Projectile.scale;
            float zPosScaleMult = GetScaleFor3D(0);
            Vector2 center = Vector2.Lerp(Main.screenPosition + new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f), Projectile.Center, zPosScaleMult);
            Projectile.scale *= zPosScaleMult;

            Main.EntitySpriteDraw(tex, center - Main.screenPosition, frame, col * Projectile.Opacity, Projectile.rotation, frame.Size() / 2, Projectile.scale, fx);
            Projectile.scale = projScale;

            Vector2 finalPos = FinalPos;
            Color elecCol = VFX.RndElectricCol with { A = 128 };
            float sparkleTipLength = 4f;
            VFX.DrawPrettyStarSparkle(1f, finalPos - Main.screenPosition, Color.White with { A = 0 }, elecCol, 2f, 0f, 1f, 3f, 4f, ProjSpawnOffset, Vector2.One * sparkleTipLength, Vector2.One);
            VFX.DrawPrettyStarSparkle(1f, finalPos - Main.screenPosition, Color.White with { A = 0 }, elecCol, 2f, 0f, 1f, 3f, 4f, ProjSpawnOffset + MathF.PI * .25f, Vector2.One * sparkleTipLength, Vector2.One);
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
