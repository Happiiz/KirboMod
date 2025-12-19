using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items
{
	public class TreasureStone : ModItem
	{
		public override void SetStaticDefaults() 
		{
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 1008; //go to *this* spot in material group

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 2; // Configure the amount of this item that's needed to research it in Journey mode.
        }

		public override void SetDefaults() 
		{
			Item.width = 30;
			Item.height = 30;
			Item.value = Item.buyPrice(0, 0, 5, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.maxStack = 9999;
			Item.createTile = ModContent.TileType<Tiles.TreasureStone>();

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

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<RareStone>());
            recipe.AddIngredient(ModContent.ItemType<MysticalStone>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register(); //adds this recipe to the game
            
			//here for people that collected many rare stones before 1.3 for a non-melee class
            Recipe rareStoneConvert = CreateRecipe();
            rareStoneConvert.AddIngredient(ModContent.ItemType<RareStone>());
            rareStoneConvert.AddIngredient(ItemID.TitaniumBar, 10);
            rareStoneConvert.AddTile(TileID.MythrilAnvil);
            rareStoneConvert.Register(); //adds this recipe to the game
        }
    }
}