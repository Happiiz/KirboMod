using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace KirboMod.Items.Armor.AirWalker
{
	[AutoloadEquip(EquipType.Legs)]
	public class AirWalkerLeggings : ModItem
	{
		public override void SetStaticDefaults() {
			// Tooltip.SetDefault("10% increased movement speed");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() {
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.buyPrice(0, 0, 0, 20);
			Item.rare = ItemRarityID.Green;
			Item.defense = 6;
		}

		public override void UpdateEquip(Player player) {
			player.moveSpeed += 0.10f; //10%
			player.maxRunSpeed += 0.10f; //10%
		}
	}
}