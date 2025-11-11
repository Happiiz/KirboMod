using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Placeables
{
	public class JambaHeartItem : ModItem
	{
		public override void SetStaticDefaults() 
		{
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

		public override void SetDefaults() 
		{
			Item.width = 30;
			Item.height = 30;
			Item.value = Item.buyPrice(0, 2, 0, 0);
			Item.rare = ItemRarityID.LightPurple;
			Item.maxStack = 9999;
			Item.createTile = ModContent.TileType<Tiles.JambaHeart>();

			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe()
				.AddIngredient(ModContent.ItemType<DreamEssence>(), 30)
				.AddIngredient(ItemID.SoulofNight, 20)
				.AddIngredient(ItemID.DemoniteBar, 30)
				.AddTile(TileID.MythrilAnvil)
				.Register();

            Recipe recipe2 = CreateRecipe()
                .AddIngredient(ModContent.ItemType<DreamEssence>(), 30)
                .AddIngredient(ItemID.SoulofNight, 15)
                .AddIngredient(ItemID.CrimtaneBar, 30)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}