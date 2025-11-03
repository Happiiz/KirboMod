using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace KirboMod.Items.Accesories.DaroachCloak
{
	[AutoloadEquip(EquipType.Back, EquipType.Front)]
	public class DaroachCloak : ModItem
	{
		public override void SetStaticDefaults() {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() {
			Item.width = 16;
			Item.height = 16;
			Item.accessory = true;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.LightPurple;
			Item.vanity = true;
		}

		public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<DreamEssence>(), 40);
            recipe.AddIngredient(ItemID.Diamond, 2);
            recipe.AddIngredient(ItemID.Silk, 60);
            recipe.AddTile(TileID.Loom);
            recipe.Register();
		}
	}
}
