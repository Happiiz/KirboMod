using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items
{
	public class Starbit : ModItem
	{
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Starbit");
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 1000; //go to *this* spot in material group

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25; // Configure the amount of this item that's needed to research it in Journey mode.
        }

		public override void SetDefaults() 
		{
			Item.width = 16;
			Item.height = 16;
			Item.value = 3;
			Item.rare = ItemRarityID.White;
			Item.maxStack = 9999;
		}

        public override void AddRecipes()
        {
            Recipe starbit = CreateRecipe(20);//the result is 20 starbits
            starbit.AddIngredient(ModContent.ItemType<DreamEssence>(), 10); //10 dream essence
            starbit.AddIngredient(ItemID.FallenStar); //1 fallen star
            starbit.AddTile(TileID.Anvils); //crafted at any anvil
            starbit.Register(); //adds this recipe to the game
        }

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}