using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items
{
	public class DreamEssence : ModItem
	{
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Dream Matter");
			// Tooltip.SetDefault("Matter of the mind, the wishes and the dreams of all");
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 1001; //go to *this* spot in material group

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25; // Configure the amount of this item that's needed to research it in Journey mode.
        }

		public override void SetDefaults() 
		{
			Item.width = 26;
			Item.height = 22;
			Item.value = Item.buyPrice(0, 0, 0, 50);
			Item.rare = ItemRarityID.Pink;
			Item.maxStack = 9999;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}