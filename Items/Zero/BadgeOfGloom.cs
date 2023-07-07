using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace KirboMod.Items.Zero
{
	public class BadgeOfGloom : ModItem
	{
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Badge Of Gloom");
			/* Tooltip.SetDefault("While attacking, dark matter will shoot out from your body that follow your mouse" +
				"\n'No, this won't let you into the dark matter family'"); */

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() {
			Item.width = 31; //half of world width
			Item.height = 31; //half of world height
			Item.accessory = true;
			Item.value = Item.sellPrice( 0, 1, 9, 97);
			Item.rare = ItemRarityID.Blue;
			Item.expert = true;
			Item.defense = 6;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) 
		{
			player.GetModPlayer<KirbPlayer>().badgeofgloom = true;
			hideVisual = true;
		}

		public override int ChoosePrefix(UnifiedRandom rand) {
			// When the item is given a prefix, only roll the best modifiers for accessories
			return -1;
		}
	}
}
