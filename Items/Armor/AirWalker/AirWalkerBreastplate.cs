using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Armor.AirWalker
{
	[AutoloadEquip(EquipType.Body)]
	public class AirWalkerBreastplate : ModItem
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
			// DisplayName.SetDefault("Air Walker Breastplate");
			/* Tooltip.SetDefault("20% more movement speed"
				+ "\n20% more jump speed"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() {
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.buyPrice(0, 0, 0, 40);
			Item.rare = ItemRarityID.Green;
			Item.defense = 8;
		}

		public override void UpdateEquip(Player player) {
			player.moveSpeed += 0.20f; // The acceleration multiplier of the player's movement speed
			player.maxRunSpeed += 0.20f; //20%
			player.jumpSpeedBoost += 0.20f;
		}
	}
}