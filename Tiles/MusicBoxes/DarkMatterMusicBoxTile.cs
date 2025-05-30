using KirboMod.Items.Placeables.MusicBoxes;
using KirboMod.NPCs.DarkMatter;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace KirboMod.Tiles.MusicBoxes
{
	public class DarkMatterMusicBoxTile : ModTile
	{
		public override void SetStaticDefaults() {
			Main.tileFrameImportant[Type] = true;
			Main.tileObsidianKill[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.StyleLineSkip = 2;
			TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(131, 54, 210), name);
		}

		public override void MouseOver(int i, int j) {
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = ModContent.ItemType<DarkMatterMusicBox>();
		}
	}
}
