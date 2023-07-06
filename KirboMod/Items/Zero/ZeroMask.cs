using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace KirboMod.Items.Zero
{
	[AutoloadEquip(EquipType.Head)]
	public class ZeroMask : ModItem
	{
        public override void SetStaticDefaults()
        {
			// DisplayName.SetDefault("Zero Mask");
            ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }
        public override void SetDefaults() 
		{
			Item.width = 30;
			Item.height = 28;
			Item.rare = ItemRarityID.Blue;
			Item.vanity = true;
		}
	}
}