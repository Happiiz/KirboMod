using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Placeables
{
	public class FountainOfDreams : ModItem
	{
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Fountain Of Dreams");
			/* Tooltip.SetDefault("Gives good dreams during the day" +
				"\n'Perfect place to attract nocturnal evil doers!'"); */

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() 
		{
			Item.width = 20;
			Item.height = 28;
			Item.value = Item.buyPrice(0, 0, 5, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.maxStack = 9999;
			Item.createTile = ModContent.TileType<Tiles.FountainOfDreams>();

			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
		}

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Starbit>(), 20);
			recipe.AddIngredient(ModContent.ItemType<DreamEssence>(), 40);
			recipe.AddRecipeGroup(RecipeGroupID.IronBar, 15); //Iron & Lead
            recipe.AddTile(TileID.Anvils);
			recipe.Register(); //Add this recipe to the game
		}
    }
}