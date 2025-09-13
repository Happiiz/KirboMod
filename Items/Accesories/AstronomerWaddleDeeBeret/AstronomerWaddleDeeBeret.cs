using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Accesories.AstronomerWaddleDeeBeret
{
    [AutoloadEquip(EquipType.Head)]
    public class AstronomerWaddleDeeBeret : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
        }
        public override void SetDefaults()
        {
            Item.vanity = true;
            Item.accessory = true;
            Item.width = 30;
            Item.height = 14;
        }

    }
}
