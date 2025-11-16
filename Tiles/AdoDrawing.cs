using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.ID;

namespace KirboMod.Tiles
{
	public class AdoDrawing : ModTile
	{
		public override void SetStaticDefaults()
        {
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3); //basic size

            Main.tileSolid[Type] = false;
			Main.tileSolidTop[Type] = false;
			Main.tileTable[Type] = false;
			Main.tileMergeDirt[Type] = false;
			Main.tileLavaDeath[Type] = true;
			Main.tileWaterDeath[Type] = false;
			Main.tileSpelunker[Type] = false;
            //Main.tileShine[Type] = 1200;
            //.tileShine2[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

			Main.tileNoAttach[Type] = true;
			Main.tileFrameImportant[Type] = true;

			TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.Height = 4;

			TileObjectData.newTile.Origin = new Point16(2, 2);

			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };

			LocalizedText name = CreateMapEntryName();
			AddMapEntry(Color.Moccasin, name);

			DustType = DustID.PalmWood;

			TileObjectData.addTile(Type);
		}
	}
}
