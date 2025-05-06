using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
    public partial class NightmareWizard : ModNPC
    {
        static Asset<Effect> shader;
        static Asset<Texture2D> starryTexture;
        static Asset<Texture2D> randTexture;
        static Asset<Texture2D> perlinTexture;
        static Asset<Texture2D> palette1;
        static Asset<Texture2D> palette2;
        static Asset<Texture2D> bodyMap;
        
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            TpEffectDraw();
            shader ??= ModContent.Request<Effect>("KirboMod/NPCs/Nightmare/WizardShader", AssetRequestMode.ImmediateLoad);
           
            Texture2D texture = ModContent.Request<Texture2D>("KirboMod/NPCs/Nightmare/NightmareWizard_ForShader").Value;
            Rectangle frame = NPC.frame;
            if (!InitializedTextures)
            {
                starryTexture = ModContent.Request<Texture2D>("KirboMod/NPCs/Nightmare/WizardCape", AssetRequestMode.ImmediateLoad);
                randTexture ??= ModContent.Request<Texture2D>("KirboMod/NPCs/Nightmare/randomRGBA", AssetRequestMode.ImmediateLoad);
                perlinTexture ??= ModContent.Request<Texture2D>("KirboMod/NPCs/Nightmare/PerlinRGBA", AssetRequestMode.ImmediateLoad);
                palette1 ??= ModContent.Request<Texture2D>("KirboMod/NPCs/Nightmare/BodyPalette1", AssetRequestMode.ImmediateLoad);
                palette2 ??= ModContent.Request<Texture2D>("KirboMod/NPCs/Nightmare/BodyPalette2", AssetRequestMode.ImmediateLoad);
                bodyMap ??= ModContent.Request<Texture2D>("KirboMod/NPCs/Nightmare/BodyMap",AssetRequestMode.ImmediateLoad);
                shader.Value.Parameters["capeTexture"].SetValue(starryTexture.Value);
                shader.Value.Parameters["perlinTexture"].SetValue(perlinTexture.Value);
                shader.Value.Parameters["randTexture"].SetValue(randTexture.Value);
                shader.Value.Parameters["palette1Texture"].SetValue(palette1.Value);
                shader.Value.Parameters["palette2Texture"].SetValue(palette2.Value);
                shader.Value.Parameters["bodyMapTexture"].SetValue(bodyMap.Value);
                InitializedTextures = true;
            }
            float deathCounterNormalized = DeathCounter / DeathAnimDuration;
            shader.Value.Parameters["deathCounterNormalized"].SetValue(deathCounterNormalized * 1.1f);
            shader.Value.Parameters["time"].SetValue((float)(Main.timeForVisualEffects % 1000));
            shader.Value.Parameters["frameSize"].SetValue(frame.Size());

            RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
            Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            Matrix matrix = NPC.IsABestiaryIconDummy ? Main.UIScaleMatrix : Main.Transform;
            SamplerState sampler = NPC.IsABestiaryIconDummy ? SamplerState.AnisotropicClamp : Main.DefaultSamplerState;
            RasterizerState rasterizer = NPC.IsABestiaryIconDummy ? new RasterizerState
            {
                CullMode = CullMode.None,
                ScissorTestEnable = true
            } : rasterizerState;    
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, rasterizer, shader.Value, matrix);
            if (NPC.IsABestiaryIconDummy)
            {
                spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
                screenPos.Y += 50;//offset it up by a bit so the portrait is centered on the face
            }
            spriteBatch.Draw(texture, NPC.Center - screenPos, frame, drawColor, NPC.rotation, frame.Size() / 2, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, Main.Rasterizer, null, matrix);
            return false;
        }
    }
}
