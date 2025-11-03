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
	public class StarBlock : ModTile
	{
		public override void SetStaticDefaults()
        {
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);

            Main.tileSolid[Type] = true; //unable to pass through
			Main.tileSolidTop[Type] = false; //can't stand on top of it
			Main.tileTable[Type] = false; //can't place things on it
			Main.tileMergeDirt[Type] = false; //doesn't merge with dirt
			Main.tileLavaDeath[Type] = true; //dies by lava
			Main.tileWaterDeath[Type] = false; //dosen't die by water
			Main.tileCut[Type] = true; //can be destroyed by weapons

			Main.tileNoAttach[Type] = false;
			Main.tileFrameImportant[Type] = true;

			//TileObjectData.newTile.Width = 2;
			//TileObjectData.newTile.Height = 2;

			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };

			LocalizedText name = CreateMapEntryName();
			AddMapEntry(Color.Gold, name);

			MinPick = 0; //can be mined by all picks
			MineResist = 0.5f; //barely resists to mining
			DustType = DustID.Gold;
			HitSound = SoundID.Dig;

			TileObjectData.addTile(Type);
		}

        public override bool CanDrop(int i, int j)
        {
            return false;
        }
	}
}
