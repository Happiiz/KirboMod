using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.ID;
using Terraria.Localization;

namespace KirboMod.Tiles
{
	public class KingDededePortrait : ModTile
	{
		public override void SetStaticDefaults() {
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.FramesOnKillWall[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);

            TileObjectData.newTile.Width = 5;
            TileObjectData.newTile.Height = 5;

            TileObjectData.newTile.Origin = new Point16(2, 2);

            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16 };

            TileObjectData.addTile(Type);
			DustType = DustID.Marble;
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(255, 226, 156), name);
		}
	}
}