using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items
{
	public class KrackoSpikeItem : ModItem
	{
		public override void SetStaticDefaults() 
		{
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3; // Configure the amount of this item that's needed to research it in Journey mode.
        }

		public override void SetDefaults() 
		{
			Item.width = 24;
			Item.height = 24;
			Item.value = Item.buyPrice(0, 0, 0, 30);
			Item.rare = ItemRarityID.Green;
			Item.maxStack = 9999;
		}
	}
}