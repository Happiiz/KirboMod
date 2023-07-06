using KirboMod.Tiles;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace KirboMod.Items.Banners
{
	public class UFOBanner : ModItem
	{
        public override void SetStaticDefaults()
        {
			// DisplayName.SetDefault("UFO Banner"); //only need for display name because all letters in UFO are capitalized so it would space out
			// Tooltip.SetDefault("{$CommonItemTooltip.BannerBonus}UFO");
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
			Item.placeStyle = 14; //15th banner on sprite sheet
		}
	}
}