using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
    public partial class NightmareOrb
    {
        static Asset<Effect> orbShader;
        static Asset<Texture2D> orbMask;
        static Asset<Texture2D> starryStrip;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            VFX.LoadTextures();
            NightmareOrbAtkType[] atkOrder = GetAttackOrder();
            NightmareOrbAtkType nextAtkType = atkOrder[(AttacksPerformedSinceSpawn) % atkOrder.Length];
            if (AttackType == NightmareOrbAtkType.Dash || (AttackType == NightmareOrbAtkType.DecideNext && nextAtkType == NightmareOrbAtkType.Dash))
            {
                float dashTime = (int)GetDashTime();
                dashTime -= 12;//make the animation end when the sound effect does
                Color blue = Color.Blue;
                float maxScale = 3;
                float minScale = 0.9f;
                Color color;
                float scale;
                float time;
                for (int i = 0; i < 11; i++)
                {
                    time = NPC.ai[0] + i * 5;
                    color = Helper.Remap(time, dashTime - 20, dashTime - 12, Color.Transparent, blue);
                    scale = Helper.RemapEased(time, dashTime - 20, dashTime, maxScale, minScale, Easings.EaseInSquare, false);
                    if (scale < minScale || scale > maxScale)
                    {
                        continue;
                    }
                    Main.EntitySpriteDraw(VFX.RingShine, NPC.Center - Main.screenPosition, null, color, Main.rand.NextFloat(MathF.Tau), VFX.ringShine.Size() / 2, scale, default);
                }
                time = NPC.ai[0] - 1;

                scale = Helper.RemapEased(time, dashTime, dashTime + 15, minScale, 4, Easings.EaseOutSquare);
                color = Helper.Remap(time, dashTime + 6, dashTime + 15, Color.White, Color.Transparent);
                Main.EntitySpriteDraw(VFX.Ring, NPC.Center - Main.screenPosition, null, color, Main.rand.NextFloat(MathF.Tau), VFX.ring.Size() / 2, scale, default);
            }
            if (orbShader == null || orbMask == null)
            {
                orbShader = ModContent.Request<Effect>("KirboMod/NPCs/Nightmare/OrbShader", AssetRequestMode.ImmediateLoad);
                orbMask = ModContent.Request<Texture2D>("KirboMod/NPCs/Nightmare/NightmareOrbMask", AssetRequestMode.ImmediateLoad);
                starryStrip = ModContent.Request<Texture2D>("KirboMod/NPCs/Nightmare/StarryStrip", AssetRequestMode.ImmediateLoad);

            }
            Matrix matrix = NPC.IsABestiaryIconDummy ? Main.UIScaleMatrix : Main.Transform;
            SamplerState sampler = NPC.IsABestiaryIconDummy ? SamplerState.LinearClamp : Main.DefaultSamplerState;
            orbShader.Value.Parameters["bgTexture"].SetValue(starryStrip.Value);
            orbShader.Value.Parameters["rotationAmount"].SetValue(1.5f * Main.GlobalTimeWrappedHourly);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, orbShader.Value, matrix);
            spriteBatch.Draw(orbMask.Value, NPC.Center - screenPos, null, Color.White, NPC.rotation, orbMask.Size() / 2, 1, SpriteEffects.FlipHorizontally, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, Main.Rasterizer, null, matrix);


            return false;
        }
        public override void Unload()
        {
            orbShader = null;
            orbMask = null;
        }
    }
}
