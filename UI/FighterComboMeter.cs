using KirboMod.Items.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;

namespace KirboMod.UI
{
    internal class FighterComboMeter : UIState
    {
        // For this bar we'll be using a frame texture and then a gradient inside bar, as it's one of the more simpler approaches while still looking decent.
        // Once this is all set up make sure to go and do the required stuff for most UI's in the Mod class.
        private UIText text;
        private UIElement area;
        private UIImage barFrame;
        private Color gradientA;
        private Color gradientB;

        public static Asset<Texture2D> Frame;

        public override void OnInitialize()
        {
            // Create a UIElement for all the elements to sit on top of, this simplifies the numbers as nested elements can be positioned relative to the top left corner of this element. 
            // UIElement is invisible and has no padding. You can use a UIPanel if you wish for a background.
            area = new UIElement();
            area.HAlign = 0.5f;
            area.VAlign = 0.5f;
            Frame = ModContent.Request<Texture2D>("KirboMod/UI/FighterComboFrame");

            barFrame = new UIImage(Frame);
            barFrame.Left.Set(0, 0f);
            barFrame.Top.Set(0, 0f);
            barFrame.Width.Set(50, 0f);
            barFrame.Height.Set(18, 0f);

            text = new UIText("0/0", 0.8f); // text to show stat
            text.Width.Set(50, 0f);
            text.Height.Set(18, 0f);
            text.Top.Set(40, 0f);
            text.Left.Set(0, 0f);

            gradientA = new Color(255, 125, 0); // An orange
            gradientB = new Color(255, 0, 0); // A red

            barFrame.HAlign = 0.5f;
            barFrame.VAlign = 0.5f;
            text.HAlign = 0.5f;
            text.VAlign = 0.5f;
            barFrame.Top.Set(25f, 0f);
            barFrame.Left.Set(-25f, 0f);
            text.Top.Set(50f, 0f);

            area.Append(text);
            area.Append(barFrame);
            Append(area);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            // This prevents drawing unless we are using a fighter glove or metal fighter glove
            int heldItemType = Main.LocalPlayer.HeldItem.type;
            if (heldItemType == ModContent.ItemType<FighterGlove>() || heldItemType == ItemType<HardenedFighter>() || heldItemType == ItemType<MetalFighter>())
            {
                base.Draw(spriteBatch);
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            KirbPlayer modPlayer = Main.LocalPlayer.GetModPlayer<KirbPlayer>();
            // Calculate quotient

            // Creating a quotient that represents the difference of your currentResource vs your maximumResource, resulting in a float of 0-1f.
            float quotient = (float)modPlayer.fighterComboCounter / 100;

            quotient = Utils.Clamp(quotient, 0f, 1f); // Clamping it to 0-1f so it doesn't go over that.

            Rectangle hitbox = barFrame.GetInnerDimensions().ToRectangle(); //Adjust dimensions of gradient
            hitbox.X += 6;
            hitbox.Width = 39;
            hitbox.Y += 4;
            hitbox.Height = 10;

            // Now, using this hitbox, we draw a gradient by drawing vertical lines while slowly interpolating between the 2 colors.
            int left = hitbox.Left;
            int right = hitbox.Right;
            int steps = (int)((right - left) * quotient);
            for (int i = 0; i < steps; i += 1)
            {
                //float percent = (float)i / steps; // Alternate Gradient Approach
                float percent = (float)i / (right - left);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left + i, hitbox.Y, 1, hitbox.Height), Color.Lerp(gradientA, gradientB, percent));
            }
        }
        public override void Update(GameTime gameTime)
        {
            if (!(Main.LocalPlayer.HeldItem.ModItem is FighterGlove) && !(Main.LocalPlayer.HeldItem.ModItem is HardenedFighter)
                 && !(Main.LocalPlayer.HeldItem.ModItem is MetalFighter))
                return;

            KirbPlayer modPlayer = Main.LocalPlayer.GetModPlayer<KirbPlayer>();
            // Setting the text per tick to update and show our resource values.
            text.SetText($"{modPlayer.fighterComboCounter}");

            if (modPlayer.fighterComboCounter >= 100) //max combo
            {
                text.TextColor = Color.Yellow;
            }
            else
            {
                text.TextColor = Color.White;
            }
            base.Update(gameTime);
        }
    }
}
