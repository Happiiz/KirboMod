using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items
{
	public class MiracleMatter : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Miracle Matter");
			//Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(10, 2)); //ticks per frame, frame count
			// Tooltip.SetDefault("Matter straight from a fallen angel");
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 1005; //go to *this* spot in material group
			ItemID.Sets.ItemNoGravity[Item.type] = true; //no gravity

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; // Configure the amount of this item that's needed to research it in Journey mode.
        }

		public override void SetDefaults() 
		{
			Item.width = 66;
			Item.height = 70;
			Item.value = Item.buyPrice(0, 0, 25, 0);
			Item.rare = ItemRarityID.Purple; //post moon lord tier
			Item.maxStack = 9999;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}