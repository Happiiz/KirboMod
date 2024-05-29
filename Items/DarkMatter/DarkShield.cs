using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace KirboMod.Items.DarkMatter
{
	[AutoloadEquip(EquipType.Shield)]
	public class DarkShield : ModItem
	{
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Dark Shield");
			/* Tooltip.SetDefault("Allows the player to dash through enemies, dealing damage" +
                "\nDefense is increased while dashing" +
				"\nDouble tap a direction"); */

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() 
		{
            Player player = Main.player[Main.myPlayer];

            Item.width = 25;
			Item.height = 25;
			Item.damage = 70;
            Item.crit = (int)player.GetCritChance(DamageClass.Melee);
            Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.accessory = true;
			Item.value = Item.sellPrice(0, 0, 30, 15);
			Item.rare = ItemRarityID.Blue;
			Item.expert = true;
			Item.defense = 3;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) 
		{
			player.GetModPlayer<KirbPlayer>().darkShield = true;

			if (player.GetModPlayer<KirbPlayer>().darkDashDelay < 0)
			{
				player.endurance += 0.3f; ; //give 30% damage reduction for dash duration
            }
		}

		public override int ChoosePrefix(UnifiedRandom rand) {
			// When the item is given a prefix, only roll the best modifiers for accessories
			return -1;
		}
	}
}
