using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items
{
	public class RareStone : ModItem
	{
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Rare Stone");
			// Tooltip.SetDefault("Used to evolve certain weapons");
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 1006; //go to *this* spot in material group

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5; // Configure the amount of this item that's needed to research it in Journey mode.
        }

		public override void SetDefaults() 
		{
			Item.width = 30;
			Item.height = 30;
			Item.value = Item.buyPrice(0, 0, 5, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.maxStack = 9999;
			Item.createTile = ModContent.TileType<Tiles.RareStone>();

			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}