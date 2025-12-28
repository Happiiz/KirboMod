using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items
{
	public class SoulMatter : ModItem
	{
		public override void SetStaticDefaults() 
		{
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 8));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

		public override void SetDefaults() 
		{
			Item.width = 10;
			Item.height = 10;
			Item.value = Item.buyPrice(0, 0, 0, 40);
			Item.rare = ItemRarityID.Yellow;
			Item.maxStack = 9999;
		}

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
	}
}