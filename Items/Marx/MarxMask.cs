using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace KirboMod.Items.Marx
{
	[AutoloadEquip(EquipType.Head)]
	public class MarxMask : ModItem
	{
        public override void SetStaticDefaults()
        {
			ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
			
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }
        public override void SetDefaults() 
		{
			Item.width = 26;
			Item.height = 28;
			Item.rare = ItemRarityID.Blue;
			Item.vanity = true;
		}
	}
}