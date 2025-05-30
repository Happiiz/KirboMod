using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.DataStructures;

namespace KirboMod.Items.Armor.AirWalker
{
	[AutoloadEquip(EquipType.Head)]
	public class AirWalkerHelmet : ModItem
	{
		public override void SetStaticDefaults() {
			// Tooltip.SetDefault("Boosts jump speed by 20%");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() {
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.buyPrice(0, 0, 0, 10);
			Item.rare = ItemRarityID.Green;
			Item.defense = 4;
		}

		public override void UpdateEquip(Player player)
		{
			player.jumpSpeedBoost += 0.10f;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) //checks if have entire set
		{
			return body.type == ModContent.ItemType<AirWalkerBreastplate>() && legs.type == ModContent.ItemType<AirWalkerLeggings>();
		}

		public override void UpdateArmorSet(Player player)  //set bonus perks
		{
			player.setBonus = "You have an extra double jump";
			player.GetModPlayer<KirbPlayer>().airWalkerSet = true;
		}
	}
}