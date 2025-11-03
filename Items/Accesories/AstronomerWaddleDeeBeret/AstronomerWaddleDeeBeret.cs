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
            Item.width = 30;
            Item.height = 14;
            Item.value = Item.buyPrice(0, 0, 0, 20);
            Item.rare = ItemRarityID.Green;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Silk, 20)
                .AddIngredient(ItemID.FallenStar, 3)
                .AddIngredient(ItemID.MeteoriteBar, 10)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
}
