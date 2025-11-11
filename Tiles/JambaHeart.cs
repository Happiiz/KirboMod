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
	public class JambaHeart : ModTile
	{
		public override void SetStaticDefaults()
        {
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3); //basic size

            Main.tileSolid[Type] = false;
			Main.tileSolidTop[Type] = false;
			Main.tileTable[Type] = false;
			Main.tileMergeDirt[Type] = false;
			Main.tileLavaDeath[Type] = false;
			Main.tileWaterDeath[Type] = false;
			Main.tileCut[Type] = false;
			Main.tileSpelunker[Type] = false;
            //Main.tileShine[Type] = 1200;
            //.tileShine2[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

			Main.tileLighted[Type] = true; //emits light

			Main.tileNoAttach[Type] = true;
			Main.tileFrameImportant[Type] = true;

			TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.Height = 4;

			TileObjectData.newTile.Origin = new Point16(2, 2);

			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };

			LocalizedText name = CreateMapEntryName();
			AddMapEntry(Color.DarkViolet, name);

			MinPick = 30; //can be mined by all picks
			MineResist = 5f; //resists a lot to mining
			DustType = DustID.Shadowflame;
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
			//purple light
			r = 0.63f;
			g = 0f;
			b = 1f;
		}
	}
}
