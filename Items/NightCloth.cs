using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items
{
	public class NightCloth : ModItem
	{
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Night Cloth");
			// Tooltip.SetDefault("The stuff nightmares are made of");
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 1007; //go to *this* spot in material group

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5; // Configure the amount of this item that's needed to research it in Journey mode.
        }

		public override void SetDefaults() 
		{
			Item.width = 36;
			Item.height = 32;
			Item.value = Item.buyPrice(0, 0, 7, 0);
			Item.rare = ItemRarityID.Pink;
			Item.maxStack = 9999;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}