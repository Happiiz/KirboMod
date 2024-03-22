using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
    public class ZeroSky : CustomSky
    {

        private readonly Random _random = new Random();
        private bool isActive;
        private Asset<Texture2D> cloudstexture;
        private bool playerleaving;

        private static int EffectOffset = 0;
        const int slideSpeed = 2;
        public override void Update(GameTime gameTime)
        {
            if (Main.gamePaused || !Main.hasFocus)
            {
                return;
            }

            //loop around
            EffectOffset += slideSpeed; //go faster than special sky in sonic mod
            if (EffectOffset >= 512)
            {
                EffectOffset %= 512;//wrap around
            }
        }

        private float GetIntensity()
        {
            return 1f - Utils.SmoothStep(3000f, 6000f, 200f);
        }

        public override Color OnTileColor(Color inColor)
        {
            float intensity = GetIntensity();
            return new Color(Vector4.Lerp(new Vector4(0.0f, 0.0f, 1f, 1f), inColor.ToVector4(), 1f - intensity));
        }

        public static Asset<Texture2D> Cloud;
        public static Asset<Texture2D> BlueCloud;
        public static Asset<Texture2D> hyperZoneBack;
        public static Asset<Texture2D> hyperZoneFront;
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= 0f && minDepth < 0f)
            {
                float intensity = GetIntensity();
                spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Blue);
            }
            //Main cloud
            if (ModContent.GetInstance<KirbConfig>().HyperzoneClouds) //enabled in the config
            {
                hyperZoneFront ??= ModContent.Request<Texture2D>("KirboMod/ExtraTextures/HyperZoneFront");
                hyperZoneBack ??= ModContent.Request<Texture2D>("KirboMod/ExtraTextures/HyperZoneBack");
                Texture2D back = hyperZoneBack.Value;
                Vector2 texSize = hyperZoneBack.Size();
                for (int i = -2; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        int offsetX = (int)(i * texSize.X);
                        int offsetY = (int)(j * texSize.Y);
                        spriteBatch.Draw(back, new Rectangle(offsetX + EffectOffset, offsetY + EffectOffset * -1, (int)texSize.X, (int)texSize.Y), Color.White);
                    }
                }
            }
        }
        public static void DrawFrontLayer(SpriteBatch sb)
        {
            hyperZoneFront ??= ModContent.Request<Texture2D>("KirboMod/ExtraTextures/HyperZoneFront");
            Texture2D back = hyperZoneFront.Value;
            Vector2 texSize = hyperZoneFront.Size();
            for (int i = 0; i < 7; i++)
            {
                for (int j = -2; j < 3; j++)
                {
                    int offsetX = (int)(i * texSize.X);
                    int offsetY = (int)(j * texSize.Y);
                    sb.Draw(back, new Rectangle(offsetX + EffectOffset * -1, offsetY + EffectOffset, (int)texSize.X, (int)texSize.Y), Color.White * .5f);
                }
            }

        }
        private void HyperZoneBG_Old(SpriteBatch spriteBatch)
        {
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
                            spriteBatch.Draw(BlueCloud.Value, new Rectangle(j - EffectOffset - 200, i + EffectOffset * 1, BlueCloud.Width(), BlueCloud.Height()), new Color(255, 255, 255));

                            Cloud = ModContent.Request<Texture2D>("KirboMod/NPCs/DarkCloud");
                            //this one goes up and right
                            spriteBatch.Draw(Cloud.Value, new Rectangle(j + EffectOffset, i + EffectOffset * -1, Cloud.Width(), Cloud.Height()), new Color(255, 255, 255));
                        }
                    }
                }
            }
        }

        public override float GetCloudAlpha()
        {
            return 0f;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
            playerleaving = false;
        }

        public override void Deactivate(params object[] args)
        {
            isActive = false;
            playerleaving = true;
        }

        public override void Reset()
        {
            isActive = false;
        }

        public override bool IsActive()
        {
            return isActive;
        }

    }
}
