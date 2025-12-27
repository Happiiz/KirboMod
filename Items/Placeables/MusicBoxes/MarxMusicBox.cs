using KirboMod.NPCs.Marx;
using KirboMod.Tiles.MusicBoxes;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Placeables.MusicBoxes
{
	public class MarxMusicBox : ModItem
	{
		public override void SetStaticDefaults() {
			ItemID.Sets.CanGetPrefixes[Type] = false; // music boxes can't get prefixes in vanilla
			ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox; // recorded music boxes transform into the basic form in shimmer

			MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Music/" + MarxBoss.MusicFileName), ModContent.ItemType<MarxMusicBox>(), ModContent.TileType<MarxMusicBoxTile>());
		}

		public override void SetDefaults() {
			Item.DefaultToMusicBox(ModContent.TileType<MarxMusicBoxTile>(), 0);
		}
	}
}
