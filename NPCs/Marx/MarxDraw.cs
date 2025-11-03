using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace KirboMod.NPCs.Marx
{
    public partial class MarxBoss : ModNPC
    {
        public static int VerticalSlideStart => 10;
        public static int VerticalSlideDuration => 10;
        public static int HorizontalSplitStart => 30;
        public static int HorizontalSplitDuration => 12;
        public static float VerticalSlideDist => 20;
        public static float HorizontalSplitDist => 270;
        public static int SplitTeleportAwayTime => 160;

        public static int IdleFrameStart => 0;
        public static int IdleFrameEnd => 7;
        public static int IdleFrameDuration => 5;
        public static int RiseFrameStart => 8;
        public static int RiseFrameEnd => 9;
        public static int RiseFrameDuration => 5;
        public static int PuffUpFrameStart => 10;
        public static int PuffUpFrameEnd => 11;
        public static int PuffUpFrameDuration => 5;
        public static int SpitFrameStart => 12;
        public static int SpitFrameEnd => 14;
        public static int SpitFrameDuration => 5;
        public static int BigLaserShootLeftFrameStart => 15;
        public static int BigLaserShootLeftFrameEnd => 16;
        public static int BigLaserShootRightFrameStart => 18;
        public static int BigLaserShootRightFrameEnd => 18;
        public static int BigLaserShootFrameDuration => 3;
        public static int CutterChargeFrame => 19;
        public static int CutterThrowFrameStart => 19;
        public static int CutterThrowFrameEnd => 21;
        public static int CutterThrowFrameDuration => 5;
        public static int TeleportFrameStart => 22;
        public static int TeleportFrameEnd => 24;
        //2 from playing the animation twice (1 for out, 1 in reverse for in)
        public static int TotalTeleportInOutDuration => TeleportFrameDuration * (TeleportFrameEnd - TeleportFrameStart + 1) * 2;

        MarxWingRenderer wingRenderer = new();
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (animation == Animation.ShadowHole)
            {
                //-2 because framecounter is effectively 2(?) frames delayed from attack timer
                bool big = NPC.frameCounter > DashFromBelowChaseDuration - 2;
                RenderShadowHole(spriteBatch, screenPos, 1f, big);
                return false;
            }
            RenderCutterChargeRings();
            MarxWingRenderer.Initialize();
            wingRenderer ??= new MarxWingRenderer();
            wingRenderer.Update();
            Main.instance.LoadNPC(Type);
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Rectangle frame = NPC.frame;
            int frameHeight = texture.Height / Main.npcFrameCount[Type];
            int frameIndex = NPC.frame.Y / frameHeight;
            if (animation == Animation.Split)
            {
                Vector2 offsetL = default;
                Vector2 offsetR = default;
                DrawData drawDataL = default;
                DrawData drawDataR = default;
                for (int i = -1; i <= 1; i += 2)
                {
                    float yOff = Easings.EaseOutSquare(Utils.GetLerpValue(VerticalSlideStart, VerticalSlideStart + VerticalSlideDuration, AttackTimer, true));
                    yOff *= Easings.EaseInSquare(Utils.GetLerpValue(HorizontalSplitStart + HorizontalSplitDuration, HorizontalSplitStart, AttackTimer, true));
                    yOff *= VerticalSlideDist;
                    yOff *= i;
                    float xOff = Utils.GetLerpValue(HorizontalSplitStart, HorizontalSplitStart + HorizontalSplitDuration, AttackTimer, true);
                    xOff = Easings.EaseOutSine(xOff);
                    xOff *= i;

                    Rectangle splitFrame = frame;
                    splitFrame.Width /= 2;
                    if (i == 1)
                    {
                        splitFrame.X += splitFrame.Width;
                    }
                    xOff *= HorizontalSplitDist;
                    Vector2 offset = new(xOff, yOff);

                    if (i < 0)
                    {
                        offsetL = offset;
                    }
                    else
                    {
                        offsetR = offset;
                    }
                    if (AttackTimer <= SplitTeleportAwayTime)
                    {
                        xOff += splitFrame.Width / 2 * i;
                        offset.X += splitFrame.Width / 2 * i;
                    }
                    offset *= NPC.scale;
                    offset -= screenPos;
                    if (AttackTimer <= SplitTeleportAwayTime)
                    {
                        //doing like this for layering the wings behind
                        if (i == -1)
                        {
                            drawDataL = new(texture, NPC.Center + offset, splitFrame, drawColor, NPC.rotation, splitFrame.Size() / 2, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                        }
                        else
                        {
                            drawDataR = new(texture, NPC.Center + offset, splitFrame, drawColor, NPC.rotation, splitFrame.Size() / 2, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                        }
                    }
                    else
                    {
                        DrawTpFx(NPC.Center + offset, (int)AttackTimer - SplitTeleportAwayTime - 1, spriteBatch, NPC.rotation, NPC.scale);
                    }
                }
                if (AttackTimer <= SplitTeleportAwayTime)
                {
                    wingRenderer.RenderFrame(frameIndex, spriteBatch, NPC.Center, screenPos, NPC.rotation, NPC.scale, frame.Width, -frame.Size() / 2, offsetL, offsetR);
                    drawDataL.Draw(spriteBatch);
                    drawDataR.Draw(spriteBatch);
                }
                return false;
            }

            frame = NPC.frame;
            Vector2 drawPos = NPC.Center - screenPos;
            if (frameIndex >= TeleportFrameStart && frameIndex <= TeleportFrameEnd)
            {
                drawColor = Color.White;
            }
            wingRenderer.RenderFrame(frameIndex, spriteBatch, NPC.Center, screenPos, NPC.rotation, NPC.scale, frame.Width, -frame.Size() / 2);
            spriteBatch.Draw(texture, drawPos, frame, drawColor, NPC.rotation, frame.Size() / 2, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            if (attacktype == AttackType.MassiveLaser && AttackTimer < TotalTeleportInOutDuration + MassiveLaserChargeupTime)
            {
                float progress = Utils.GetLerpValue(TotalTeleportInOutDuration, TotalTeleportInOutDuration + MassiveLaserChargeupTime, AttackTimer, true);
                float scale = Helper.RemapEased(progress, 0f, 1f, 0.2f, 3f, Easings.EaseOutSquare);
                Vector2 scaleVec = new(scale);
                Vector2 fatness = new(1);
                Color color = new(255, 0, 255, 128);
                VFX.DrawPrettyStarSparkle(1f, NPC.Center - screenPos + new Vector2(0, 26), Color.White with { A = 0 }, color, 2f, 0, 1, 3, 4, 0f, scaleVec, fatness);
            }
            return false;
        }
        private void RenderCutterChargeRings()
        {

            if (attacktype != AttackType.Cutter)
            {
                return;
            }
            if(AttackTimer > CutterRoundDuration * CutterRounds)
            {
                return;
            }
            int relativeTimer = (int)(AttackTimer % CutterRoundDuration);
            relativeTimer -= TotalTeleportInOutDuration;
            if(relativeTimer <= 0)
            {
                return;
            }
            
            Vector2 drawPos = NPC.Center - Main.screenPosition;
        //  int shootTime = CutterMoveDuration + CutterChargeDuration + 1;
            int timeOffsetPerLoop = 10;
            int ringDuration = 25;
            int ringCount = 4;
            for (int i = 0; i < ringCount; i++)
            {
                int timeOffset = i * timeOffsetPerLoop;
                float scale = Utils.Remap(relativeTimer - timeOffset, 0, ringDuration, 10, 0);
                float opacityMult = Utils.Remap(relativeTimer - timeOffset, 0, 10, 0f, .9f);
                if (scale > 0 && opacityMult > 0)
                {
                    Main.EntitySpriteDraw(VFX.RingShine, drawPos, null, new Color(255, 255, 255) * opacityMult, Main.rand.NextFloat(MathF.Tau), VFX.ring.Size() / 2, scale, SpriteEffects.None);
                }
            }
        }
        public static void DrawTpFx(Vector2 drawPos, int timer, SpriteBatch spriteBatch, float rotation = 0, float scale = 1f)
        {
            int type = ModContent.NPCType<MarxBoss>();
            Main.instance.LoadNPC(type);
            Texture2D texture = TextureAssets.Npc[type].Value;
            int tpFrameDuration = TeleportFrameDuration;
            int tpFrameStart = TeleportFrameStart;
            int frameIndex = tpFrameStart + timer / tpFrameDuration;
            int frameCount = Main.npcFrameCount[type];
            if (frameIndex >= frameCount)
            {
                return;//don't draw because out of bounds of sheet so don't waste processing power
            }
            Rectangle frame = texture.Frame(1, frameCount, 0, frameIndex);
            spriteBatch.Draw(texture, drawPos, frame, Color.White, rotation, frame.Size() / 2, scale, SpriteEffects.None, 0);
        }
    }
}
