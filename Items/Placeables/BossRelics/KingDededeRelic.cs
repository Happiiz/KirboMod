using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Placeables.BossRelics
{
	public class KingDededeRelic : ModItem
	{
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            // Vanilla has many useful methods like these, use them! This substitutes setting Item.createTile and Item.placeStyle aswell as setting a few values that are common across all placeable items
            // The place style (here by default 0) is important if you decide to have more than one relic share the same tile type (more on that in the tiles' code)
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Relics.KingDededeRelic>(), 0);

            Item.width = 30;
            Item.height = 48;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Master;
            Item.master = true; // This makes sure that "Master" displays in the tooltip, as the rarity only changes the item name color
            Item.value = Item.buyPrice(0, 5);
        }
    }
}