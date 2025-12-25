using KirboMod.Biomes;
using KirboMod.Configs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
    public class ZeroSky : CustomSky
    {

        private bool isActive;
        static float fadeCounter;
        private static float bgOffset = 0;
        static float fgOffset = 0;
        public const float slideSpeed = 5;
        public static float FadeInSpeed => 1f / 40f;
        public static float FadeOutSpeed => 1f / 300f;
        public override void Update(GameTime gameTime)
        {
            if (Main.gamePaused || !Main.hasFocus)
            {
                return;
            }
            Fading();

            bgOffset += SafeFloat(HyperZoneConfig.Instance.HyperZoneBackScrollSpeed); //go faster than special sky in sonic mod
            fgOffset += SafeFloat(HyperZoneConfig.Instance.HyperZoneFrontScrollSpeed);
            fgOffset %= 1024;
            if (fgOffset >= 1024)
            {
                fgOffset %= 1024;
            }
            if (bgOffset >= 1024)//1024 is texture size
            {
                bgOffset %= 1024;//wrap around
            }

        }
        static float SafeFloat(float input)
        {
            //not NaN, or infinity I think?
            if (float.IsNormal(input))
            {
                return input;
            }
            return 0;
        }
        private void Fading()
        {
            if (isActive)
            {
                fadeCounter += FadeInSpeed;
            }
            else
            {
                fadeCounter -= FadeOutSpeed;
            }
            fadeCounter = MathHelper.Clamp(fadeCounter, 0, 1);
        }

        private float GetIntensity()
        {
            return 1f - Utils.SmoothStep(3000f, 6000f, 200f);
        }

        public override Color OnTileColor(Color inColor)
        {
            float intensity = GetIntensity() * GetOpacity();
            return new Color(Vector4.Lerp(new Vector4(0.75f, 0.75f, 1f, 1f), inColor.ToVector4(), 1f - intensity));
        }

        public static Asset<Texture2D> Cloud;
        public static Asset<Texture2D> BlueCloud;
        public static Asset<Texture2D> hyperZoneBack;
        public static Asset<Texture2D> hyperZoneFront;
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            float opacity = GetOpacity();
            if (maxDepth >= 0f && minDepth < 0f)
            {
                Color tintColor = HyperZoneConfig.Instance.HyperZoneBackgroundTintColor * HyperZoneConfig.Instance.HyperZoneBackgroundTintOpacity * opacity;
                spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), tintColor);
            }
            //background clouds
            if (ModContent.GetInstance<HyperZoneConfig>().DisableClouds) //enabled in the config
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                hyperZoneFront ??= ModContent.Request<Texture2D>("KirboMod/ExtraTextures/HyperZoneFront");
                hyperZoneBack ??= ModContent.Request<Texture2D>("KirboMod/ExtraTextures/HyperZoneBack");
                Texture2D back = hyperZoneBack.Value;
                Vector2 plrPos = Main.screenPosition;
                Vector2 texSize = hyperZoneBack.Size();
                Color drawCol = HyperZoneConfig.Instance.HyperZoneCloudsBackColor;
                drawCol *= opacity * HyperZoneConfig.Instance.HyperZoneCloudsBackOpacity;
                for (int i = -2; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        int offsetX = (int)(i * texSize.X);
                        int offsetY = (int)(j * texSize.Y);
                        offsetX -= (int)(plrPos.X % texSize.X);
                        offsetY -= (int)(plrPos.Y % texSize.Y);
                        offsetX += (int)bgOffset;
                        offsetY -= (int)bgOffset;
                        Rectangle destination = new(offsetX, offsetY, (int)texSize.X, (int)texSize.Y);

                        spriteBatch.Draw(back, destination, drawCol);
                    }
                }
                //what spritebatch was before
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.BackgroundViewMatrix.TransformationMatrix);
            }
        }
        public static void DrawFrontLayer(SpriteBatch sb)
        {
            hyperZoneFront ??= ModContent.Request<Texture2D>("KirboMod/ExtraTextures/HyperZoneFront");
            Texture2D front = hyperZoneFront.Value;
            Vector2 texSize = hyperZoneFront.Size();
            Vector2 referencePos = Main.screenPosition;
            float opacity = GetOpacity();
            Color drawCol = HyperZoneConfig.Instance.HyperZoneCloudsFrontColor;

            drawCol *= HyperZoneConfig.Instance.HyperZoneCloudsFrontOpacity * opacity;
            for (int i = 0; i < 4; i++)
            {
                for (int j = -1; j < 3; j++)
                {
                    float offsetX = i * (texSize.X);

                    float offsetY = j * (texSize.Y + 1);

                    offsetX -= referencePos.X % texSize.X;
                    offsetY -= referencePos.Y % texSize.Y;
                    offsetX -= fgOffset;
                    offsetY += fgOffset;
                    sb.Draw(front, new Rectangle((int)(offsetX), (int)(offsetY), (int)(texSize.X), (int)(texSize.Y)), drawCol);
                }
            }

        }
        private void HyperZoneBG_Old(SpriteBatch spriteBatch)
        {
            int offset = (int)bgOffset;
            for (int i = -1000; i < 4000; i++) //y
            {
                if (i % 200 == 0) //get remainder 
                {
                    for (int j = -2000; j < 8000; j++) //x
                    {
                        if (j % 400 == 0)
                        {
                            BlueCloud = ModContent.Request<Texture2D>("KirboMod/NPCs/DarkCloud2");
                            //this one goes left and down (also shifted to the left a bit)
                            spriteBatch.Draw(BlueCloud.Value, new Rectangle(j - offset - 200, i + offset * 1, BlueCloud.Width(), BlueCloud.Height()), new Color(255, 255, 255));

                            Cloud = ModContent.Request<Texture2D>("KirboMod/NPCs/DarkCloud");
                            //this one goes up and right
                            spriteBatch.Draw(Cloud.Value, new Rectangle(j + offset, i + offset * -1, Cloud.Width(), Cloud.Height()), new Color(255, 255, 255));
                        }
                    }
                }
            }
        }
        static float GetOpacity()
        {
            return fadeCounter;
        }
        public override float GetCloudAlpha()
        {
            //so regular clouds fade out
            return Easings.EaseOutSquare(1f - GetOpacity());
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            isActive = false;
        }

        public override void Reset()
        {
            fadeCounter = 0;
            isActive = false;
        }

        public override bool IsActive()
        {
            return fadeCounter > 0 || isActive;
        }
        public override void OnLoad()
        {
            On_Main.SetBackColor += On_Main_SetBackColor;
        }

        private void On_Main_SetBackColor(On_Main.orig_SetBackColor orig, Main.InfoToSetBackColor info, out Color sunColor, out Color moonColor)
        {
            orig(info, out sunColor, out moonColor);
            //make it so the night doesn't make hyper zone effect look like an ugly dark blue from the transparency + dark night background
            int skyColor = HyperZoneConfig.Instance.HyperZoneSkyBrightness;
            if (SkyManager.Instance[Hyperzone.BiomeName].IsActive())
            {
                Main.ColorOfTheSkies = Color.Lerp(Main.ColorOfTheSkies, new Color(skyColor, skyColor, skyColor), GetOpacity());
            }
        }
    }
}
