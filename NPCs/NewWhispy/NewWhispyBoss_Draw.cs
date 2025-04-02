using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.NPCs.NewWhispy
{
    public partial class NewWhispyBoss : ModNPC
    {
        static Asset<Texture2D> eyesRegular;
        static Asset<Texture2D> eyesAngry;
        static Asset<Texture2D> mouthRegular;
        static Asset<Texture2D> body;
        static Asset<Texture2D> nose;
        static Asset<Texture2D>[] leaves;
        static Asset<Texture2D> mouthClosed;
        static Asset<Texture2D> cheeks;
        static Asset<Texture2D> roots;
        const int LeavesIDGreenLarge = 0;
        const int LeavesIDLimeLarge = 1;
        const int LeavesIDTealLarge = 2;
        const int LeavesIDGreenMedium = 3;
        const int LeavesIDLimeMedium = 4;
        const int LeavesIDTealMedium = 5;
        const int LeavesIDGreenSmall = 6;
        const int LeavesIDLimeSmall = 7;
        const int LeavesIDTealSmall = 8;
        static readonly (int id, float xpos, float ypos)[] leavesDrawData = new[]
{
    (LeavesIDLimeSmall, 1650f, 116f),
    (LeavesIDTealMedium, 1329f, 119f),
    (LeavesIDGreenLarge, 1802f, 124f),
    //(LeavesIDGreenLarge, 1082f, 126f),
    (LeavesIDGreenSmall, 1527f, 142f),
    (LeavesIDLimeSmall, 1433f, 144f),
    (LeavesIDTealMedium, 1998f, 149f),
    (LeavesIDGreenMedium, 1177f, 151f),
    (LeavesIDGreenSmall, 1029f, 176f),
    (LeavesIDGreenMedium, 1613f, 169f),
    (LeavesIDLimeSmall, 1291f, 185f),
    //(LeavesIDLimeMedium, 145f, 185f),
    (LeavesIDLimeMedium, 1415f, 184f),
    (LeavesIDTealLarge, 1696f, 190f),
    (LeavesIDLimeSmall, 1822f, 198f),
    (LeavesIDTealLarge, 1962f, 200f),
    (LeavesIDTealMedium, 2132f, 203f),
    (LeavesIDTealSmall, 1537f, 217f),
    (LeavesIDLimeLarge, 1115f, 238f),
    (LeavesIDLimeMedium, 1577f, 245f),
    (LeavesIDTealMedium, 1225f, 249f),
    (LeavesIDGreenLarge, 1388f, 261f),
    (LeavesIDTealLarge, 1726f, 267f),
    (LeavesIDLimeLarge, 2019f, 279f),
    (LeavesIDGreenMedium, 1857f, 286f),
    (LeavesIDTealSmall, 1091f, 295f),
};
        //static readonly (int id, float xpos, float ypos)[] leavesPositionOld = new[]
        //{
        //    (LeavesIDTealLarge, 1726f, 267f),
        //    (LeavesIDLimeSmall, 1822f, 198f),
        //    (LeavesIDGreenMedium, 1857f, 286f),
        //    (LeavesIDGreenSmall, 1029f,176f),
        //    (LeavesIDLimeLarge,1115f, 238f),
        //    (LeavesIDTealSmall, 1091f,295f),
        //    (LeavesIDTealMedium, 1225f,249f),
        //    (LeavesIDGreenMedium, 1177f, 151f),
        //    (LeavesIDTealMedium, 1329f, 119f),
        //    (LeavesIDLimeSmall, 1433f, 144f),
        //    (LeavesIDGreenSmall, 1527f, 142f),
        //    (LeavesIDLimeSmall, 1291f, 185f),
        //    (LeavesIDGreenLarge, 1802f, 124f),
        //    (LeavesIDGreenLarge, 1388f, 261f),
        //    (LeavesIDLimeMedium, 145f, 185f),
        //    (LeavesIDLimeLarge,2019f, 279f),
        //    (LeavesIDTealLarge, 1962f, 200f),
        //    (LeavesIDTealSmall, 1538f, 218f),
        //    (LeavesIDLimeMedium, 1577f, 245f),
        //    (LeavesIDTealLarge, 1696f, 190f),
        //    (LeavesIDGreenMedium,1613f, 169f),
        //    (LeavesIDGreenLarge, 1082f, 126f),
        //    (LeavesIDLimeSmall, 1650f, 116f),
        //    (LeavesIDTealMedium, 2132f, 203f),
        //    (LeavesIDLimeMedium, 1998f, 149f),
        //    (LeavesIDLimeMedium, 1415f, 184f)
        //};



        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D eyes;
            Texture2D mouth;
            Vector2 eyeAndMouthOffset = default;
            Vector2 noseOffset = default;
            bool drawCheeks = false;
            switch (AnimState)
            {
                case AnimationState.Regular:
                    eyes = eyesRegular.Value;
                    mouth = mouthRegular.Value;
                    break;
                case AnimationState.Angry:
                    eyes = eyesAngry.Value;
                    mouth = mouthRegular.Value;
                    float offX = MathF.Sin(Timer * .7f) * 8f;
                    offX *= Utils.GetLerpValue(0, 15, Timer, true) * Utils.GetLerpValue(55, 40, Timer, true);
                    eyeAndMouthOffset.X += offX;
                    noseOffset.X += offX;
                    break;
                case AnimationState.PuffedUpCheeksAndClosedMouth:
                    mouth = mouthClosed.Value;
                    eyes = eyesRegular.Value;
                    drawCheeks = true;
                    break;
                default:
                    eyes = eyesRegular.Value;
                    mouth = mouthRegular.Value;
                    break;
            }
            Vector2 bodyTextureHalfSize = body.Size() / 2;
            SpriteEffects spriteDir = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 drawPos = NPC.Center - screenPos;
            spriteBatch.Draw(body.Value, drawPos, null, drawColor, 0, bodyTextureHalfSize, NPC.scale, spriteDir, 0);
            spriteBatch.Draw(eyes, drawPos + eyeAndMouthOffset, null, drawColor, 0, bodyTextureHalfSize, NPC.scale, spriteDir, 0);
            spriteBatch.Draw(mouth, drawPos + eyeAndMouthOffset, null, drawColor, 0, bodyTextureHalfSize, NPC.scale, spriteDir, 0);
            Vector2 rootPosOffset = new Vector2(66f * 2f + 1f, 177f * 2f - 1f) - bodyTextureHalfSize;
            Vector2 cheeksPosOffset = new Vector2(36f * 2f + 2f, 83f * 2f + 1f) - bodyTextureHalfSize;
            rootPosOffset.X *= -NPC.spriteDirection;
            cheeksPosOffset.X *= -NPC.spriteDirection;
            if (drawCheeks)
            {
                spriteBatch.Draw(cheeks.Value, drawPos + cheeksPosOffset, null, drawColor, 0, cheeks.Size() / 2, NPC.scale, spriteDir, 0f);
            }
            spriteBatch.Draw(nose.Value, drawPos + noseOffset, null, drawColor, 0, nose.Size() / 2, NPC.scale, spriteDir, 0);
            spriteBatch.Draw(roots.Value, drawPos + rootPosOffset, null, drawColor, 0, roots.Size() / 2, NPC.scale, spriteDir, 0f);
            float leavesMirrorCenterX = 1872f;
            float leavesCenterY = 477f;
            for (int i = 0; i < leavesDrawData.Length; i++)
            {
                (int id, float xpos, float ypos) = leavesDrawData[i];
                Texture2D tex = leaves[id].Value;
                xpos -= leavesMirrorCenterX;
                xpos *= -NPC.spriteDirection;
                ypos -= leavesCenterY;
                //ypos +=
                Vector2 offset = new(xpos, ypos);
                spriteBatch.Draw(tex, drawPos + offset, null, drawColor, NPC.rotation, tex.Size() / 2, NPC.scale, spriteDir, 0);
            }
            return false;
        }
        void LoadTextures()
        {
            const string path = "KirboMod/NPCs/NewWhispy/";
            string useEXVer = string.Empty;
            
            body = ModContent.Request<Texture2D>(path + "NewWhispyBody" + useEXVer);
            eyesRegular = ModContent.Request<Texture2D>(path + "NewWhispyEyesRegular" + useEXVer);
            eyesAngry = ModContent.Request<Texture2D>(path + "NewWhispyEyesAngry" + useEXVer);
            mouthRegular = ModContent.Request<Texture2D>(path + "NewWhispyMouthRegular" + useEXVer);
            mouthClosed = ModContent.Request<Texture2D>(path + "NewWhispyMouthClosed" + useEXVer);
            nose = ModContent.Request<Texture2D>(path + "NewWhispyNose" + useEXVer);
            cheeks = ModContent.Request<Texture2D>(path + "NewWhispyCheeks" + useEXVer);
            roots = ModContent.Request<Texture2D>(path + "NewWhispyRoots" + useEXVer);
            leaves = new Asset<Texture2D>[9];
            if (Main.expertMode)
            {
                useEXVer = "EX";
            }
            for (int i = 0; i < leaves.Length; i++)
            {
                string path2 = "KirboMod/NPCs/NewWhispy/WhispyLeaf";
                if (i % 3 == 0)
                {
                    path2 += "Green";
                }
                else if (i % 3 == 1)
                {
                    path2 += "Lime";
                }
                else
                {
                    path2 += "Teal";
                }
                if (i / 3 == 0)
                {
                    path2 += "Large";
                }
                else if (i / 3 == 1)
                {
                    path2 += "Medium";
                }
                else
                {
                    path2 += "Small";
                }
                leaves[i] = ModContent.Request<Texture2D>(path2 + useEXVer);
            }
        }
        
        public override void Load()
        {
            LoadTextures();
        }
        public override void Unload()
        {
            body = null;
            eyesRegular = null;
            eyesAngry = null;
            mouthRegular = null;
            nose = null;
            leaves = null;
        }
    }
}
