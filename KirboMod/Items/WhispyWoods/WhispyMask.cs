using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace KirboMod.Items.WhispyWoods
{
	[AutoloadEquip(EquipType.Head)]
	public class WhispyMask : ModItem
	{
        public override void SetStaticDefaults()
        {
			// DisplayName.SetDefault("Whispy Woods Mask");
            ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }
        public override void SetDefaults() 
		{
			Item.width = 18;
			Item.height = 18;
			Item.rare = ItemRarityID.Blue;
			Item.vanity = true;
		}
	}
}