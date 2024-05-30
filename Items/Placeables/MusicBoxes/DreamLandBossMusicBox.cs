using KirboMod.Tiles.MusicBoxes;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Placeables.MusicBoxes
{
	public class DreamLandBossMusicBox : ModItem
	{
		public override void SetStaticDefaults() {
			ItemID.Sets.CanGetPrefixes[Type] = false; // music boxes can't get prefixes in vanilla
			ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox; // recorded music boxes transform into the basic form in shimmer

			MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Music/Evobyte_K1Boss"), ModContent.ItemType<DreamLandBossMusicBox>(), ModContent.TileType<DreamLandBossMusicBoxTile>());
		}

		public override void SetDefaults() {
			Item.DefaultToMusicBox(ModContent.TileType<DreamLandBossMusicBoxTile>(), 0);
		}
	}
}
