using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Placeables
{
	public class StarBlockItem : ModItem
	{
		public override void SetStaticDefaults() 
		{
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10; //amount needed to research
        }

		public override void SetDefaults() 
		{
			Item.width = 10;
			Item.height = 10;
			Item.value = Item.buyPrice(0, 0, 0, 10);
			Item.rare = ItemRarityID.White;
			Item.maxStack = 9999;
			Item.createTile = ModContent.TileType<Tiles.StarBlock>();

			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
		}

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Starbit>(), 5);
			recipe.AddIngredient(ItemID.StoneBlock, 10);
            recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
    }
}