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
	public class RareStone : ModTile
	{
		public override void SetStaticDefaults() {
			Main.tileSolid[Type] = false; //able to pass through
			Main.tileSolidTop[Type] = false; //can't stand on top of it
			Main.tileTable[Type] = false; //can't place things on it
			Main.tileMergeDirt[Type] = false; //doesn't merge with dirt
			Main.tileLavaDeath[Type] = true; //dosen't die by lava
			Main.tileWaterDeath[Type] = true; //dosen't die by water
			Main.tileCut[Type] = false; //can't be destroyed by weapons
			Main.tileSpelunker[Type] = true; // The tile will be affected by spelunker highlighting
			Main.tileOreFinderPriority[Type] = 500; //same detection as chests
            Main.tileShine[Type] = 1200;
            Main.tileShine2[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

			Main.tileLighted[Type] = true; //emits light

			Main.tileNoAttach[Type] = true;
			Main.tileFrameImportant[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3); //basic size

			//TileObjectData.newTile.Width = 3;
			//TileObjectData.newTile.Height = 3;

			TileObjectData.newTile.Origin = new Point16(1, 1); // one tile down and right

			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 }; // Don't extend into grass.

			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Rare Stone");
			AddMapEntry(Color.Orange, name);

			MinPick = 30; //can be mined by all picks
			MineResist = 1f; //resists a little to mining
			DustType = ModContent.DustType<Dusts.RareStoneBit>();
			HitSound = SoundID.Tink; //ore tink

			TileObjectData.addTile(Type);
		}

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
			if (!fail)//when destroyed
			{
				SoundEngine.PlaySound(SoundID.Item27, new Vector2(i * 16, j * 16)); //crystal shatter
			}
        }

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			//orange-ish light
			r = 1f;
			g = 0.6f;
			b = 0.6f;
		}
	}
}
