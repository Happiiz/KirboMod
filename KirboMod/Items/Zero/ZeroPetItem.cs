using KirboMod.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Zero
{
	public class ZeroPetItem : ModItem
	{
		public override void SetStaticDefaults() {
			// DisplayName and Tooltip are automatically set from the .lang files, but below is how it is done normally.
			// DisplayName.SetDefault("Hollow Shell");
			/* Tooltip.SetDefault("Summons a light wanderer" +
                "\n'Feels almost weightless'"); */

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.KingSlimePetItem);
			Item.width = 15;
			Item.height = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.master = true;
			Item.value = Item.buyPrice(0, 5, 0, 0); 
			Item.shoot = ModContent.ProjectileType<Projectiles.Pets.ZeroPet>();
			Item.buffType = ModContent.BuffType<Buffs.Pets.ZeroPetBuff>();
		}

		public override void UseStyle(Player player, Rectangle heldItemFrame) {
			if (player.whoAmI == Main.myPlayer && player.itemTime == 0) {
				player.AddBuff(Item.buffType, 3600, true);
			}
		}
	}
}