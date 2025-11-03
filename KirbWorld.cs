using KirboMod.Items;
using KirboMod.NPCs;
using KirboMod.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

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
            RecipeGroup group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.GoldBar)}", ItemID.GoldBar, ItemID.PlatinumBar);
            RecipeGroup.RegisterGroup("Gold", group);
        }
    }
}