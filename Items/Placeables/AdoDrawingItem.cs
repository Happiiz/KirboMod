using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Placeables
{
	public class AdoDrawingItem : ModItem
	{
		public override void SetStaticDefaults() 
		{
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

		public override void SetDefaults() 
		{
			Item.width = 30;
			Item.height = 30;
			Item.value = Item.buyPrice(0, 3, 0, 0);
			Item.rare = ItemRarityID.Blue;
			Item.maxStack = 9999;
			Item.createTile = ModContent.TileType<Tiles.AdoDrawing>();

			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
        }
    }
}