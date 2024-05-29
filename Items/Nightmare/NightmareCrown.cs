using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace KirboMod.Items.Nightmare
{
	[AutoloadEquip(EquipType.Head)]
	public class NightmareCrown : ModItem
	{
		public override void SetStaticDefaults() 
		{
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() 
		{
			Item.width = 22;
			Item.height = 22;
			Item.value = Item.sellPrice(0, 0, 15, 0);
			Item.rare = ItemRarityID.Blue;
			Item.expert = true; //gives the accesory its permenant rainbow color
			Item.defense = 15;
		}

        public override void UpdateEquip(Player player)
        {

            player.GetModPlayer<KirbPlayer>().nightcrown = true;
        }

        public override int ChoosePrefix(UnifiedRandom rand) 
		{
			return -1;
		}
	}
}
