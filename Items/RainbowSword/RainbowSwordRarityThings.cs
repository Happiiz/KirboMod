using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Terraria.GameContent;

namespace KirboMod.Items.RainbowSword
{
	public partial class RainbowSword : ModItem
	{
		//private static readonly MethodInfo DynamicSpriteFontExtensionMethods_DrawString = typeof(DynamicSpriteFontExtensionMethods).GetMethod(nameof(DynamicSpriteFontExtensionMethods.DrawString), BindingFlags.Public | BindingFlags.Static, new Type[] { typeof(SpriteBatch), typeof(DynamicSpriteFont), typeof(string), typeof(Vector2), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) });

  //      private delegate void orig_DynamicSpriteFontExtensionMethods_DrawString(SpriteBatch spriteBatch, DynamicSpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth);
  //      private delegate void hook_DynamicSpriteFontExtensionMethods_DrawString(orig_DynamicSpriteFontExtensionMethods_DrawString orig, SpriteBatch spriteBatch, DynamicSpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth);
  //      private static Hook On_DynamicSpriteFontExtensionMethods_DrawString;
        public override void Unload()
        {
           // On_DynamicSpriteFontExtensionMethods_DrawString = null;
        }
       //private void Hook_DynamicSpriteFontExtensionMethods_DrawString(orig_DynamicSpriteFontExtensionMethods_DrawString orig, SpriteBatch spriteBatch, DynamicSpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
       // {
       //     if (color != ModContent.GetInstance<RainbowSwordRarity>().RarityColor)
       //     {
       //         //orig(spriteBatch, spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
       //         //return;//test comments
       //     }
       //     SpritebatchSetupForShaderTooltip(ref spriteBatch);
       //     orig(spriteBatch, spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
       //     SetupVanillaSpritebatch(ref spriteBatch);
       // }
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
            On_Main.DrawItemTextPopups += On_Main_DrawItemTextPopups;
            On_ChatManager.DrawColorCodedString_SpriteBatch_DynamicSpriteFont_TextSnippetArray_Vector2_Color_float_Vector2_Vector2_refInt32_float_bool += RainbowSwordEffectOnWorldHover;
		}

		private void On_Main_DrawItemTextPopups(On_Main.orig_DrawItemTextPopups orig, float scaleTarget)
		{
			orig.Invoke(scaleTarget);
			for (int i = 0; i < 20; i++)
			{
				PopupText popupText = Main.popupText[i];
				if (!popupText.active)
				{
					continue;
				}
				string text = popupText.name;
				if (!text.Contains("Rainbow Sword"))
					continue;
				if (popupText.stack > 1)
				{
					text = text + " (" + popupText.stack + ")";
				}
				Vector2 vector = FontAssets.MouseText.Value.MeasureString(text);
				Vector2 vector2 = new Vector2(vector.X * 0.5f, vector.Y * 0.5f);
				float num = popupText.scale / scaleTarget;
				int num2 = (int)(255f - 255f * num);
				float num6 = (int)popupText.color.A;
				num6 *= num * popupText.alpha;
				Color color2 = Color.Black;
				float num8 = (float)num2 / 255f;
				Color outlineColor = popupText.context == PopupTextContext.ItemPickupToVoidContainer ? (new Color(127, 20, 255) * 0.4f) : Color.Black;
				for (int j = 0; j < 5; j++)
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
						num6 = (float)(int)popupText.color.A * num * popupText.alpha;
					}
					if (color2 != Color.Black && j < 4)
					{
						num9 *= 1.3f + 1.3f * num8;
						num10 *= 1.3f + 1.3f * num8;
					}
					float num11 = popupText.position.Y - Main.screenPosition.Y + num10;
					if (Main.player[Main.myPlayer].gravDir == -1f)
					{
						num11 = (float)Main.screenHeight - num11;
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
						if(j == 4)
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
			bool doRainbowSwordEffect = snippets.Length > 0 && snippets[0].Text.Contains("Rainbow Sword") && baseColor.Equals(Color.White);
			if(!doRainbowSwordEffect)
				return orig.Invoke(spriteBatch, font, snippets, position, baseColor, rotation, origin, baseScale, out hoveredSnippet, maxWidth, ignoreColors);

			SpritebatchSetupForShaderTooltip(ref spriteBatch);		
			Vector2 result = orig.Invoke(spriteBatch, font, snippets, position, baseColor, rotation, origin, baseScale, out hoveredSnippet, maxWidth, ignoreColors);
			SetupVanillaSpritebatch(ref spriteBatch);		
			return result;
		}


        bool ShaderTooltip(DrawableTooltipLine line)
		{

			if (line.Index == 0)
			{
				LoadShaderIfNeeded();
				SetTooltipShaderParams();
				Vector2 textPos = new Vector2(line.X, line.Y);
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
		public override Color RarityColor => new(78, 34, 56, 255);
    }//more of an identifier than anything
}