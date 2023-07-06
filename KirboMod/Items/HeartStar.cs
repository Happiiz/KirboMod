using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items
{
	public class HeartStar : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Heart Star");
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 20)); //ticks per frame, frame count
            ItemID.Sets.AnimatesAsSoul[Item.type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation
            // Tooltip.SetDefault("Love and compassion imbued with the power of the stars");
			ItemID.Sets.ItemNoGravity[Item.type] = true; //float
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 1008; //go to *this* spot in material group

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; // Configure the amount of this item that's needed to research it in Journey mode.
        }

		public override void SetDefaults() 
		{
			Item.width = 20;
			Item.height = 20;
			Item.value = Item.buyPrice(0, 0, 15, 0);
			Item.rare = ItemRarityID.Yellow; //post golem
			Item.maxStack = 9999;
		}

		public override void AddRecipes()
		{
			Recipe heartstarrecipe = CreateRecipe();
			heartstarrecipe.AddIngredient(ModContent.ItemType<HeartMatter>());
			heartstarrecipe.AddIngredient(ModContent.ItemType<Starbit>(), 50);
			heartstarrecipe.AddTile(TileID.MythrilAnvil);
			heartstarrecipe.Register();
		}
	}
}