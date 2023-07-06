using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace KirboMod.Items.KingDedede
{
	[AutoloadEquip(EquipType.Shoes)]
	public class RoyalSlippers : ModItem
	{
		public override void SetStaticDefaults() {
			/* Tooltip.SetDefault("Hold DOWN in the air and fall for a bit to ground pound" +
				"\nHold UP or JUMP to stop your descent" +
				"\n'Wait he doesn't wear shoes... Who's are these?'"); */
			
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() {
			Item.width = 23;
			Item.height = 23;
			Item.value = Item.buyPrice(0, 0, 8, 25);
			Item.rare = ItemRarityID.Blue;
			Item.expert = true;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			if (player.mount.Active == false)
			{
				//ground pound
				player.GetModPlayer<KirbPlayer>().royalslippers = true;
			}
		}
	}
}