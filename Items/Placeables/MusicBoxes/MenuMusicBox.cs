using KirboMod.Tiles.MusicBoxes;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Placeables.MusicBoxes
{
	public class MenuMusicBox : ModItem
	{
		public override void SetStaticDefaults() {
			ItemID.Sets.CanGetPrefixes[Type] = false; // music boxes can't get prefixes in vanilla
			ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox; // recorded music boxes transform into the basic form in shimmer

			MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Music/Happiz_MilkyWayWishesRemix"), ModContent.ItemType<MenuMusicBox>(), ModContent.TileType<MenuMusicBoxTile>());
		}

		public override void SetDefaults() {
			Item.DefaultToMusicBox(ModContent.TileType<MenuMusicBoxTile>(), 0);
		}

        public override void AddRecipes()
        {
			Recipe musicBox = CreateRecipe()
				.AddIngredient(ModContent.ItemType<DreamEssence>(), 3)
				.AddIngredient(ModContent.ItemType<HeartMatter>(), 3)
				.AddIngredient(ModContent.ItemType<DarkMaterial>(), 3)
				.AddIngredient(ModContent.ItemType<SoulMatter>(), 3)
				.AddIngredient(ItemID.MusicBox)
				.AddTile(TileID.TinkerersWorkbench)
				.Register();
        }
	}
}
