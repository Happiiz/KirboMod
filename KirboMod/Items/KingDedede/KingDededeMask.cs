using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace KirboMod.Items.KingDedede
{
	[AutoloadEquip(EquipType.Head)]
	public class KingDededeMask : ModItem
	{
        public override void SetStaticDefaults()
        {
			// DisplayName.SetDefault("King Dedede Mask");
            ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }
        public override void SetDefaults() 
		{
			Item.width = 24;
			Item.height = 24;
			Item.rare = ItemRarityID.Blue;
			Item.vanity = true;
		}
	}
}