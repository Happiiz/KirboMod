using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items
{
	public class PurifiedMaterial : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Purified Matter");
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2)); //ticks per frame, frame count
            ItemID.Sets.AnimatesAsSoul[Item.type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation
            // Tooltip.SetDefault("Broken away fron darkness, this matter beams with happy thoughts and emotions");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; // Configure the amount of this item that's needed to research it in Journey mode.
        }

		public override void SetDefaults() 
		{
			Item.width = 28;
			Item.height = 16;
			Item.value = Item.buyPrice(0, 0, 1, 0);
			Item.rare = ItemRarityID.Yellow; //post golem
			Item.maxStack = 9999;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<DarkMaterial>());
			recipe.AddIngredient(ModContent.ItemType<HeartMatter>(), 3);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}