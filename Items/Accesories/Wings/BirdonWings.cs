using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Accesories.Wings
{
	[AutoloadEquip(EquipType.Wings)]
	public class BirdonWings : ModItem
	{

		public override void SetStaticDefaults() {
			ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(150, 7f, 2f);
		}

		public override void SetDefaults() {
			Item.width = 22;
			Item.height = 20;
			Item.value = Item.buyPrice(gold: 8);
			Item.rare = ItemRarityID.LightPurple;
			Item.accessory = true;
		}

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
			speed *= 1.5f;
        }

		public override void AddRecipes() {
			Recipe birdonWings = CreateRecipe();
			birdonWings.AddIngredient(ItemID.HarpyWings);
			birdonWings.AddIngredient(ModContent.ItemType<BirdonFeather>(), 15);
            birdonWings.AddIngredient(ModContent.ItemType<DreamEssence>(), 20);
            birdonWings.AddTile(TileID.MythrilAnvil);
			birdonWings.SortBefore(Main.recipe.First(recipe => recipe.createItem.wingSlot != -1)); //groups wings together in the crafting menu
			birdonWings.Register();
		}
	}
}
