using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items
{
	public class HeartMatter : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Heart Matter");
			// Tooltip.SetDefault("Matter made from the loving care of the nice things in the universe");
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 1002; //go to *this* spot in material group

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3; // Configure the amount of this item that's needed to research it in Journey mode.
        }

		public override void SetDefaults() 
		{
			Item.width = 40;
			Item.height = 40;
			Item.value = Item.buyPrice(0, 0, 10, 0);
			Item.rare = ItemRarityID.Lime; //post plantera
			Item.maxStack = 9999;
		}

		public override void AddRecipes()
		{
			Recipe heartmatterrecipe = CreateRecipe();
			heartmatterrecipe.AddIngredient(ModContent.ItemType<Starbit>(), 20);
			heartmatterrecipe.AddIngredient(ModContent.ItemType<DreamEssence>(), 5);
			heartmatterrecipe.AddIngredient(ItemID.LifeCrystal);
			heartmatterrecipe.AddTile(TileID.MythrilAnvil);
			heartmatterrecipe.Register();

			Recipe heartmatterrecipe2 = CreateRecipe();
			heartmatterrecipe2.AddIngredient(ModContent.ItemType<Starbit>(), 20);
			heartmatterrecipe2.AddIngredient(ItemID.ChlorophyteBar, 2);
			heartmatterrecipe2.AddIngredient(ItemID.LifeFruit);
			heartmatterrecipe2.AddTile(TileID.MythrilAnvil);
			heartmatterrecipe2.Register();
		}
	}
}