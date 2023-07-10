using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace KirboMod
{
    public class ParticleSystem : ModSystem
    {
        public const int maxSparkles = 1300;
        public static Sparkle[] sparkle = new Sparkle[maxSparkles + 1];
        public override void Load()
        {
            for (int i = 0; i < sparkle.Length; i++)
                sparkle[i] = new Sparkle(i);
        }
        public override void Unload()
        {
            sparkle = null;
        }
        public override void PostDrawTiles()
        {
            Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            for (int i = 0; i < maxSparkles; i++)
            {
                if (i < maxSparkles && sparkle[i].Active)
                    sparkle[i].DrawWhitePart();
            }
            for (int i = 0; i < maxSparkles; i++)
            {
                if (i < maxSparkles && sparkle[i].Active)
                    sparkle[i].Draw();
            }
            Main.spriteBatch.End();
        }
        //I just chose PostUpdateDusts arbitrarily, it can be any update method that runs globally
        public override void PostUpdateDusts()
        {
            for (int i = 0; i < maxSparkles; i++)
            {
                if (i < maxSparkles && sparkle[i].Active)
                    sparkle[i].Update();
            }
            sparkle[maxSparkles].Active = false;
        }
    }

    public class Particle
    {
        public int WhoAmI { get; protected set; }
        public Color Color { get; set; }
        public int TimeLeft { get; set; }
        public Vector2 Scale { get; set; }
        public float Opacity { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Position { get; set; }
        public float Friction { get; set; }
        public Vector2 Acceleration { get; set; }
        public float Rotation { get; set; }
        public bool Active { get => TimeLeft > 0; set => TimeLeft = value ? TimeLeft : 0; }
    }
}
