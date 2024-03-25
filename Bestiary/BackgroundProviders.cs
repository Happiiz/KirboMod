using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.GameContent.Bestiary;
using ReLogic.Content;

namespace KirboMod.Bestiary
{
    public class HyperzoneBackgroundProvider : IBestiaryInfoElement, IBestiaryBackgroundImagePathAndColorProvider
    {
        public Asset<Texture2D> GetBackgroundImage()
        {
            return Main.Assets.Request<Texture2D>("Images/MapBG1"); //placeholder
        }

        public Color? GetBackgroundColor()
        {
            return Color.Blue;
        }

        public UIElement ProvideUIElement(BestiaryUICollectionInfo info)
        {
            return null;
        }
    }

    public class  SurfaceBackgroundProvider: IBestiaryInfoElement, IBestiaryBackgroundImagePathAndColorProvider
    {
        public Asset<Texture2D> GetBackgroundImage()
        {
            return Main.Assets.Request<Texture2D>("Images/MapBG1"); //surface background
        }

        public Color? GetBackgroundColor()
        {
            return default;
        }

        public UIElement ProvideUIElement(BestiaryUICollectionInfo info)
        {
            return null;
        }
    }
}