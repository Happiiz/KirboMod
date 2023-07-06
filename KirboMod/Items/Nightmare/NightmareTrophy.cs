using KirboMod.Tiles;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace KirboMod.Items.Nightmare
{
	public class NightmareTrophy : ModItem
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
			Item.value = 5000;
			Item.rare = ItemRarityID.Blue;
			Item.createTile = ModContent.TileType<BossTrophy>();
			Item.placeStyle = 2; //third in trophy spritesheet
		}
	}
}