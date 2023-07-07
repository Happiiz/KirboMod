using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace KirboMod.Items.Kracko
{
	public class PersonalCloud : ModItem
	{
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Personal Cloud");
			// Tooltip.SetDefault("Summons a personal cloud to shock your enemies");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() 
		{
            Item.width = 10;
			Item.height = 10;
            Item.accessory = true;
			Item.value = Item.buyPrice(0, 0, 5, 50);
			Item.rare = ItemRarityID.Blue;
			Item.expert = true; //gives the accesory its permenant rainbow color
		}

		public override void UpdateAccessory(Player player, bool hideVisual) 
		{
			player.GetModPlayer<KirbPlayer>().personalcloud = true;
        }

        public override int ChoosePrefix(UnifiedRandom rand) {
			// When the item is given a prefix, only roll the best modifiers for accessories
			return 0;
		}
	}
}
