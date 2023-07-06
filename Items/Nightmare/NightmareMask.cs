using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace KirboMod.Items.Nightmare
{
	[AutoloadEquip(EquipType.Head)]
	public class NightmareMask : ModItem
	{
        public override void SetStaticDefaults()
        {
			// DisplayName.SetDefault("Nightmare Mask");
            ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }
        public override void SetDefaults() 
		{
			Item.width = 30;
			Item.height = 40;
			Item.rare = ItemRarityID.Blue;
			Item.vanity = true;
		}
	}
}