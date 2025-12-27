using KirboMod.Items.Weapons;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.RainbowDrops
{
	public class HallowDrop : SnowDrop
	{
		public override void SetDefaults() 
		{
			base.SetDefaults();
			Item.rare = ItemRarityID.Lime; //post plantera
		}

		public override void AddRecipes()
		{
			//8 drops in total
			//heart matter needs 3 souls of light
			//8 * 3 = 24
			//so the recipe in total already needs 24 souls of light due to the drops
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Starbit>(), 20);
			recipe.AddIngredient(ItemID.CrystalShard, 25);
			recipe.AddIngredient(ItemID.UnicornHorn, 5);
			recipe.AddIngredient(ItemID.ButterflyDust);
			recipe.AddIngredient(ItemID.QueenSlimeCrystal);//gelatin crystal, summons queen slime
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}