using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items
{
	public class CrystalShard : ModItem
	{
		public override void SetStaticDefaults() 
		{
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 1008; //go to *this* spot in material group

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3; // Configure the amount of this item that's needed to research it in Journey mode.
        }

		public override void SetDefaults() 
		{
			Item.width = 22;
			Item.height = 22;
			Item.value = Item.buyPrice(0, 0, 2, 0);
			Item.rare = ItemRarityID.Yellow; //post golem
			Item.maxStack = 9999;
            Item.ammo = Item.type; //be in this ammo group
			Item.consumable = true; //can be deleted by usage
        }

        public override void AddRecipes()
		{
			Recipe crystalshardrecipe = CreateRecipe(2);
			crystalshardrecipe.AddIngredient(ModContent.ItemType<Starbit>(), 2);
			crystalshardrecipe.AddIngredient(ModContent.ItemType<DreamEssence>(), 1);
			crystalshardrecipe.AddIngredient(ItemID.CrystalShard);
			crystalshardrecipe.AddTile(TileID.MythrilAnvil);
			crystalshardrecipe.Register();
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; // Makes it uneffected by light
        }
    }
}