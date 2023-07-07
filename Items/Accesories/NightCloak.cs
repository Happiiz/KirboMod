using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace KirboMod.Items.Accesories
{
	[AutoloadEquip(EquipType.Back, EquipType.Front)]
	public class NightCloak : ModItem
	{
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Night Cloak");
			// Tooltip.SetDefault("Shoots stars in eight directions upon being hit");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() {
			Item.width = 16;
			Item.height = 16;
			Item.accessory = true;
			Item.value = Item.sellPrice(0, 3, 25, 0);
			Item.rare = ItemRarityID.Pink;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) 
		{
			player.GetModPlayer<KirbPlayer>().nightcloak = true;
			hideVisual = true;
		}

		public override int ChoosePrefix(UnifiedRandom rand) {
			// When the item is given a prefix, only roll the best modifiers for accessories
			return 0;
		}

		public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.StarCloak);
			recipe.AddIngredient(ModContent.ItemType<Items.NightCloth>(), 5);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}
