using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace KirboMod.Items.Kracko
{
	[AutoloadEquip(EquipType.Head)]
	public class KrackoMask : ModItem
	{
        public override void SetStaticDefaults()
        {
			// DisplayName.SetDefault("Kracko Mask");
            ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }
        public override void SetDefaults() 
		{
			Item.width = 36;
			Item.height = 38;
			Item.rare = ItemRarityID.Blue;
			Item.vanity = true;
		}
	}
}