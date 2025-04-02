using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.RainbowDrops
{
	public class EvilDrop : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Red & Purple Drop");
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 1011;
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 6)); //ticks per frame, frame count
            ItemID.Sets.AnimatesAsSoul[Item.type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation
            // Tooltip.SetDefault("The rainbow drop of the evil infections");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() 
		{
			Item.width = 40;
			Item.height = 40;
			Item.value = Item.buyPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Yellow; //post golem
			Item.maxStack = 9999;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Starbit>(), 20);
			recipe.AddIngredient(ModContent.ItemType<NightCloth>(), 5);
			recipe.AddIngredient(ItemID.Deathweed, 3);
			recipe.AddIngredient(ItemID.SoulofNight, 10);
			recipe.AddIngredient(ItemID.CursedFlame, 5);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();

			Recipe recipe2 = CreateRecipe();
			recipe2.AddIngredient(ModContent.ItemType<Starbit>(), 20);
			recipe2.AddIngredient(ModContent.ItemType<NightCloth>(), 5);
			recipe2.AddIngredient(ItemID.Deathweed, 3);
			recipe2.AddIngredient(ItemID.SoulofNight, 10);
			recipe2.AddIngredient(ItemID.Ichor, 5);
			recipe2.AddTile(TileID.MythrilAnvil);
			recipe2.Register();
		}
	}
}