using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace KirboMod.Items.RainbowSword
{
    public partial class RainbowSword : ModItem
    {
        static void SetupVanillaSpritebatchPopupText()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
        }

        private static void SetupVanillaSpritebatch(ref SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
        }
        private void SpritebatchSetupForShaderTooltip(ref SpriteBatch spriteBatch)
        {
            LoadShaderIfNeeded();
            SetTooltipShaderParams();
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, tooltipShader, Main.UIScaleMatrix);
        }
        private void SpritebatchSetupForShaderTooltipPopupText(ref SpriteBatch spriteBatch)
        {
            LoadShaderIfNeeded();
            SetTooltipShaderParams();
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, tooltipShader, Main.GameViewMatrix.ZoomMatrix);
        }
        public override void Load()
        {
            On_Main.CraftItem += SpawnRainbowSwordCraftAnimation;
            On_SoundEngine.PlaySound_int_int_int_int_float_float += SkipPlayingCraftingSoundForRainbowSword;
            On_Main.DrawItemTextPopups += On_Main_DrawItemTextPopups;
            On_PopupText.NewText_PopupTextContext_Item_int_bool_bool += BlockRainbowSwordPopupIfItemCraft;
            On_ChatManager.DrawColorCodedString_SpriteBatch_DynamicSpriteFont_TextSnippetArray_Vector2_Color_float_Vector2_Vector2_refInt32_float_bool += RainbowSwordEffectOnWorldHover;
        }

        private SoundEffectInstance SkipPlayingCraftingSoundForRainbowSword(On_SoundEngine.orig_PlaySound_int_int_int_int_float_float orig, int type, int x, int y, int Style, float volumeScale, float pitchOffset)
        {
            if (!skipPlaySoundAndPopupText || type != 7 || x != -1 || y != -1 || Style != 1)
            {
                return orig(type, x, y, Style, volumeScale, pitchOffset);
            }
            return default(SoundEffectInstance);
        }

        private int BlockRainbowSwordPopupIfItemCraft(On_PopupText.orig_NewText_PopupTextContext_Item_int_bool_bool orig, PopupTextContext context, Item newItem, int stack, bool noStack, bool longText)
        {
            if (skipPlaySoundAndPopupText)
            {
                return -1;
            }
            return orig(context, newItem, stack, noStack, longText);
        }

        private void On_Main_DrawItemTextPopups(On_Main.orig_DrawItemTextPopups orig, float scaleTarget)
        {
            orig(scaleTarget);
            for (int i = 0; i < 20; i++)
            {
                PopupText popupText = Main.popupText[i];
                if (!popupText.active)
                {
                    continue;
                }
                string text = popupText.name;
                if (!text.Contains(Item.Name))
                    continue;
                if (popupText.stack > 1)
                {
                    text = text + " (" + popupText.stack + ")";
                }
                Vector2 vector = FontAssets.MouseText.Value.MeasureString(text);
                Vector2 vector2 = new(vector.X * 0.5f, vector.Y * 0.5f);
                float num = popupText.scale / scaleTarget;
                int num2 = (int)(255f - 255f * num);
                float num6 = popupText.color.A;
                num6 *= num * popupText.alpha;
                Color color2 = Color.Black;
                float num8 = num2 / 255f;
                Color outlineColor = popupText.context == PopupTextContext.ItemPickupToVoidContainer ? (new Color(127, 20, 255) * 0.4f) : Color.Black;
                for (int j = 0; j < 5; j++)//4 copies as the border, one as the actual text.
                {
                    float num9 = 0f;
                    float num10 = 0f;
                    switch (j)
                    {
                        case 0:
                            num9 -= scaleTarget * 2f;
                            break;
                        case 1:
                            num9 += scaleTarget * 2f;
                            break;
                        case 2:
                            num10 -= scaleTarget * 2f;
                            break;
                        case 3:
                            num10 += scaleTarget * 2f;
                            break;
                    }
                    if (j < 4)
                    {
                        num6 = popupText.color.A * num * popupText.alpha;
                    }
                    if (color2 != Color.Black && j < 4)
                    {
                        num9 *= 1.3f + 1.3f * num8;
                        num10 *= 1.3f + 1.3f * num8;
                    }
                    float num11 = popupText.position.Y - Main.screenPosition.Y + num10;
                    if (Main.player[Main.myPlayer].gravDir == -1f)
                    {
                        num11 = Main.screenHeight - num11;
                    }
                    if (color2 != Color.Black && j < 4)
                    {
                        Color color3 = color2;
                        color3.A = (byte)MathHelper.Lerp(60f, 127f, Utils.GetLerpValue(0f, 255f, num6, clamped: true));
                        DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.MouseText.Value, text, new Vector2(popupText.position.X - Main.screenPosition.X + num9 + vector2.X, num11 + vector2.Y), Color.Black, popupText.rotation, vector2, popupText.scale, SpriteEffects.None, 0f);
                        DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.MouseText.Value, text, new Vector2(popupText.position.X - Main.screenPosition.X + num9 + vector2.X, num11 + vector2.Y), Color.Black, popupText.rotation, vector2, popupText.scale, SpriteEffects.None, 0f);
                    }
                    else
                    {
                        if (j == 4)//j == 4 means the actual inside. the other is
                            SpritebatchSetupForShaderTooltipPopupText(ref Main.spriteBatch);
                        DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.MouseText.Value, text, new Vector2(popupText.position.X - Main.screenPosition.X + num9 + vector2.X, num11 + vector2.Y), outlineColor, popupText.rotation, vector2, popupText.scale, SpriteEffects.None, 0f);
                        if (j == 4)
                            SetupVanillaSpritebatchPopupText();

                    }
                }
            }
        }

        private Vector2 RainbowSwordEffectOnWorldHover(On_ChatManager.orig_DrawColorCodedString_SpriteBatch_DynamicSpriteFont_TextSnippetArray_Vector2_Color_float_Vector2_Vector2_refInt32_float_bool orig, SpriteBatch spriteBatch, DynamicSpriteFont font, TextSnippet[] snippets, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, out int hoveredSnippet, float maxWidth, bool ignoreColors)
        {
            bool doRainbowSwordEffect = snippets.Length > 0 && snippets[0].Text.Contains(Item.Name) && baseColor.Equals(Color.White) && ReferenceEquals(font, FontAssets.MouseText.Value);
            if (!doRainbowSwordEffect)
                return orig(spriteBatch, font, snippets, position, baseColor, rotation, origin, baseScale, out hoveredSnippet, maxWidth, ignoreColors);

            SpritebatchSetupForShaderTooltip(ref spriteBatch);
            Vector2 result = orig(spriteBatch, font, snippets, position, baseColor, rotation, origin, baseScale, out hoveredSnippet, maxWidth, ignoreColors);
            SetupVanillaSpritebatch(ref spriteBatch);
            return result;
        }


        bool ShaderTooltip(DrawableTooltipLine line)
        {

            if (line.Index == 0)
            {
                LoadShaderIfNeeded();
                SetTooltipShaderParams();
                Vector2 textPos = new(line.X, line.Y);
                for (float i = 0; i < 1; i += 0.25f)
                {
                    Vector2 borderOffset = (i * MathF.Tau).ToRotationVector2() * 2;
                    ChatManager.DrawColorCodedString(Main.spriteBatch, line.Font, line.Text, textPos + borderOffset, Color.Black, line.Rotation, line.Origin, line.BaseScale);
                }
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, tooltipShader, Main.UIScaleMatrix);
                ChatManager.DrawColorCodedString(Main.spriteBatch, line.Font, line.Text, textPos, Color.White, line.Rotation, line.Origin, line.BaseScale);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
                return false;
            }
            return true;
        }
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }
        static Effect tooltipShader;
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            return ShaderTooltip(line);
        }
        static void SetTooltipShaderParams()
        {
            tooltipShader.Parameters["s"].SetValue(1);
            tooltipShader.Parameters["l"].SetValue(0.5f);
            tooltipShader.Parameters["uOpacity"].SetValue(1);
            tooltipShader.Parameters["gradientScale"].SetValue(1);
            tooltipShader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 3f);
        }
    }
    public class RainbowSwordRarity : ModRarity
    {
        public static Color Color => new(78, 34, 56, 254);//more of an identifier than anything
        public override Color RarityColor => Color;
    }
}