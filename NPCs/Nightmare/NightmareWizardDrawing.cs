using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
    public partial class NightmareWizard : ModNPC
    {
        static Asset<Effect> shader;
        static Asset<Texture2D> starryTexture;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            TpEffectDraw();



            shader ??= ModContent.Request<Effect>("KirboMod/NPCs/Nightmare/WizardShader", AssetRequestMode.ImmediateLoad);
            starryTexture = ModContent.Request<Texture2D>("KirboMod/NPCs/Nightmare/WizardCape");
           
            Matrix matrix = NPC.IsABestiaryIconDummy ? Main.UIScaleMatrix : Main.Transform;
            SamplerState sampler = NPC.IsABestiaryIconDummy ? SamplerState.LinearClamp : Main.DefaultSamplerState;
            Texture2D texture = ModContent.Request<Texture2D>("KirboMod/NPCs/Nightmare/NightmareWizard_ForShader").Value;
            shader.Value.Parameters["capeTexture"].SetValue(starryTexture.Value);
            shader.Value.Parameters["time"].SetValue(1.5f * Main.GlobalTimeWrappedHourly);
            shader.Value.Parameters["frameSize"].SetValue(NPC.frame.Size());
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, shader.Value, matrix);
            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, Main.Rasterizer, null, matrix);
            return false;
        }
    }
}
