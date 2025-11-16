using KirboMod.Tiles;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace KirboMod.Items.Placeables
{
	public class BeamAttackPictureItem : ModItem
	{
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults() 
		{
			Item.width = 30;
			Item.height = 30;
			Item.maxStack = 9999;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
            Item.value = Item.buyPrice(gold: 10);
            Item.rare = ItemRarityID.White;
			Item.createTile = ModContent.TileType<BeamAttackPicture>();
		}
	}
}