using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace KirboMod.Items.Accesories
{
	[AutoloadEquip(EquipType.Balloon)]
	public class KirbyBalloon : ModItem
	{
		public override void SetStaticDefaults() {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() {
			Item.width = 30;
			Item.height = 50;
			Item.accessory = true;
			Item.value = Item.sellPrice(0, 2, 50, 0);
			Item.rare = ItemRarityID.Yellow;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) 
		{
			player.GetModPlayer<KirbPlayer>().kirbyballoon = true;
        }

        public override int ChoosePrefix(UnifiedRandom rand) {
			// When the item is given a prefix, only roll the best modifiers for accessories
			return 0;
		}

		public override void AddRecipes() {
			Recipe kirbyballoonrecipe = CreateRecipe();
			kirbyballoonrecipe.AddIngredient(ModContent.ItemType<HeartMatter>(), 3);
			kirbyballoonrecipe.AddIngredient(ItemID.ShinyRedBalloon);
			kirbyballoonrecipe.AddTile(TileID.MythrilAnvil);
			kirbyballoonrecipe.Register();
		}
	}
}
