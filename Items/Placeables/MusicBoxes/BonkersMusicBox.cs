using KirboMod.Tiles.MusicBoxes;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Placeables.MusicBoxes
{
	public class BonkersMusicBox : ModItem
	{
		public override void SetStaticDefaults() {
			ItemID.Sets.CanGetPrefixes[Type] = false; // music boxes can't get prefixes in vanilla
			ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox; // recorded music boxes transform into the basic form in shimmer

			//MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Music/Photonic0_BatafireBattle_WithLoopMetadata"), ModContent.ItemType<BatafireMusicBox>(), ModContent.TileType<BatafireMusicBoxTile>());
		}

		public override void SetDefaults() {
			Item.DefaultToMusicBox(ModContent.TileType<BonkersMusicBoxTile>(), 0);
		}
	}
}
