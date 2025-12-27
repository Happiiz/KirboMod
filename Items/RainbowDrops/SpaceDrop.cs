using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.RainbowDrops
{
    public class SpaceDrop : SnowDrop
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Yellow; //post golem
            Item.maxStack = 9999;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<Starbit>(), 20);
            recipe.AddIngredient(ItemID.FallenStar, 7);
            recipe.AddIngredient(ItemID.Meteorite, 20);
            recipe.AddIngredient(ItemID.SoulofFlight, 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}