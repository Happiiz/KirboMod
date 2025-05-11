using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.RainbowDrops
{
	public class DesertDrop : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Yellow Drop");
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 1010;
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 6)); //ticks per frame, frame count
            ItemID.Sets.AnimatesAsSoul[Item.type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation
            // Tooltip.SetDefault("The rainbow drop of the barren desert");
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
			recipe.AddIngredient(ModContent.ItemType<HeartMatter>(), 2);
			recipe.AddIngredient(ItemID.Waterleaf, 3);
			recipe.AddIngredient(ItemID.FossilOre, 5);
			recipe.AddIngredient(ItemID.AncientBattleArmorMaterial); //forbidden fragment
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}