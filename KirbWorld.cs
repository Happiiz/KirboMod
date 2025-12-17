using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace KirboMod
{
    public class KirboWorld : ModSystem
    {
        public int frameYoffset = 0;
        public int frameCounter = 0;
        public override void PostUpdateDusts() //for meta bats
        {
            frameCounter++;

            if (frameCounter > 3) //every 3 frames
            {
                frameYoffset += 16; //change frames
                frameCounter = 0;
            }

            if (frameYoffset >= 64) //bigger than sprite Y
            {
                frameYoffset = 0; //reset
            }
        }

        public override void AddRecipeGroups()
        {
            //"Any Gold" - makes a recipe groupd containing gold and platinum bars
            RecipeGroup group = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.GoldBar)}", ItemID.GoldBar, ItemID.PlatinumBar);
            RecipeGroup.RegisterGroup("Gold", group);
        }
    }
}