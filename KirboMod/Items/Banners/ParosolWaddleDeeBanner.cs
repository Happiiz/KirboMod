using KirboMod.Tiles;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace KirboMod.Items.Banners
{
	public class ParosolWaddleDeeBanner : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Parasol Waddle Dee Banner"); //did this because I spelt parasol wrong in file name
			// Tooltip.SetDefault("{$CommonItemTooltip.BannerBonus}Parasol Waddle Dee");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }
		public override void SetDefaults() {
			Item.width = 10;
			Item.height = 24;
			Item.maxStack = 9999;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(0, 0, 10, 0);
			Item.createTile = ModContent.TileType<EnemyBanner>();
			Item.placeStyle = 8; //9th banner on sprite sheet
		}
	}
}