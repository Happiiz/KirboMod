using KirboMod.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Marx
{
	public class MarxPetItem : ModItem
	{
		public override void SetStaticDefaults() {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.KingSlimePetItem);
			Item.width = 15;
			Item.height = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.master = true;
			Item.value = Item.buyPrice(0, 5, 0, 0); 
			Item.shoot = ModContent.ProjectileType<Projectiles.Pets.MarxPet>();
			Item.buffType = ModContent.BuffType<Buffs.Pets.MarxPetBuff>();
		}

		public override void UseStyle(Player player, Rectangle heldItemFrame) {
			if (player.whoAmI == Main.myPlayer && player.itemTime == 0) {
				player.AddBuff(Item.buffType, 3600, true);
			}
		}
	}
}