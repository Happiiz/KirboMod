using KirboMod.Tiles.Relics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace KirboMod.Tiles
{
	public class DimensionMirror : WhispyWoodsRelic //reference relic code because it's easier that way
    {
        public override int FrameWidth => 18 * 6;
        public override int FrameHeight => 18 * 6;

        public override float YOffset => 0;

        public override float XOffset => 24;

        public override string RelicTextureName => "KirboMod/Tiles/DimensionMirror";

		public override string Texture => "KirboMod/Tiles/DimensionMirrorBase";

        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 400;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.InteractibleByNPCs[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;

            TileObjectData.newTile.Width = 6;
            TileObjectData.newTile.Height = 6;

            TileObjectData.newTile.Origin = new Point16(3, 5);

            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16 };

            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(233, 207, 94), name);

            DustType = DustID.Gold;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!fail)//when destroyed
            {
                SoundEngine.PlaySound(SoundID.Item27, new Vector2(i * 16, j * 16)); //crystal shatter

                for (int k = 0; k < 7; k++)
                {
                    Dust.NewDust(new Point(i - 2, j - 2).ToWorldCoordinates(), FrameWidth / 2, FrameHeight / 2, DustType);
                }
            }
        }
    }
}
