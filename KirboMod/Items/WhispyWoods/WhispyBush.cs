using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace KirboMod.Items.WhispyWoods
{
	public class WhispyBush : ModItem
	{
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Whispy Shrub");
			// Tooltip.SetDefault("When you're hit you spew apples from your body");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() {
			Item.width = 15; //half of world width
			Item.height = 26; //half of world height
			Item.accessory = true;
			Item.value = Item.sellPrice(0, 0, 4, 27);
			Item.rare = ItemRarityID.Blue;
			Item.expert = true;
			Item.defense = 3;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) 
		{
			player.GetModPlayer<KirbPlayer>().whispbush = true;
			hideVisual = true;
		}

		public override int ChoosePrefix(UnifiedRandom rand) {
			// When the item is given a prefix, only roll the best modifiers for accessories
			return -1;
		}
	}
}
