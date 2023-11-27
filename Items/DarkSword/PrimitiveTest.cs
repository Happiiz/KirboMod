using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

internal class DarkSwordTrailSystem : ModSystem
{
    public BasicEffect effect;
    public GraphicsDevice _device = Main.instance.GraphicsDevice;
    public float progress = 0;
    public bool increasing = true;
    public static Vector2[] subtractivePosToRender;
    public override void Load()
    {
        Main.QueueMainThreadAction(() =>
        {
            effect = new BasicEffect(_device);
            effect.VertexColorEnabled = true;
        });
        On_Main.DrawDust += Main_DrawDust;
    }
    private void Main_DrawDust(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        effect.World = Matrix.CreateTranslation(-new Vector3(Main.screenPosition.X, Main.screenPosition.Y, 0));
        effect.View = Main.GameViewMatrix.TransformationMatrix;
        Viewport viewport = Main.instance.GraphicsDevice.Viewport;
        effect.Projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, -1, 1);
        _device.RasterizerState = RasterizerState.CullNone;
            BlendState subtractive;
            _ = BlendState.AlphaBlend;//idk why these are here
            _ = BlendState.Additive;//idk why these are here
            subtractive = new BlendState
            {
                ColorBlendFunction = BlendFunction.ReverseSubtract,
                ColorDestinationBlend = Blend.One,
                ColorSourceBlend = Blend.SourceAlpha,
                AlphaBlendFunction = BlendFunction.ReverseSubtract,
                AlphaDestinationBlend = Blend.One,
                AlphaSourceBlend = Blend.SourceAlpha
            };
        if (subtractivePosToRender == null || subtractivePosToRender.Length <= 1)
            return;
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, subtractive, null, null, RasterizerState.CullNone);
        List<VertexPositionColor> trailVertices = new List<VertexPositionColor>(subtractivePosToRender.Length * 2);
        float trailWidth = 40;
        Color endCol = Color.White;
        Color beginCol = Color.Green;
        for (int i = 1; i < subtractivePosToRender.Length; i++)
        {
            Vector2 normal = Vector2.Normalize(subtractivePosToRender[i - 1] - subtractivePosToRender[i]);
            normal = new Vector2(normal.Y, -normal.X);
            float progress = Utils.GetLerpValue(0, subtractivePosToRender.Length, i);
            trailVertices.Add(new VertexPositionColor(new Vector3(subtractivePosToRender[i] + (-normal * trailWidth), 0), Color.Lerp(beginCol, endCol, progress)));
            trailVertices.Add(new VertexPositionColor(new Vector3(subtractivePosToRender[i] + (normal * trailWidth), 0), Color.Lerp(beginCol, endCol, progress)));
        }
        int[] indices = new int[trailVertices.Count * 3];
        for (int i = 0; i < indices.Length - 2; i += 3)
        {
            indices[i] = i / 3;
            indices[i + 1] = i / 3 + 1;
            indices[i + 2] = i / 3 + 2;
        }
        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _device.DrawUserIndexedPrimitives(PrimitiveType.TriangleStrip, trailVertices.ToArray(), vertexOffset: 0, trailVertices.Count, indices, indexOffset: 0, primitiveCount: (int)MathF.Max(3, trailVertices.Count * 3 - 6));
        }
        Main.spriteBatch.End();
    }
}