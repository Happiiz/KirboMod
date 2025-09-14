using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

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
            Item.value = Item.buyPrice(0, 1, 0, 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Silk, 5)
                .AddIngredient<Starbit>()
                .AddTile(TileID.Loom)
                .Register();
        }
    }
}
