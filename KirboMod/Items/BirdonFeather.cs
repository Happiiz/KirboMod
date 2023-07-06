using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items
{
	public class BirdonFeather : ModItem
	{
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Birdon Feather");
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 1;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5; // Configure the amount of this item that's needed to research it in Journey mode.
        }

		public override void SetDefaults() 
		{
			Item.width = 24;
			Item.height = 24;
			Item.value = Item.buyPrice(0, 0, 0, 50);
			Item.rare = ItemRarityID.LightRed;
			Item.maxStack = 9999;
		}
	}
}