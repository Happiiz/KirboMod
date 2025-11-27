using KirboMod.Items.Placeables.MusicBoxes;
using KirboMod.NPCs.DarkMatter;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace KirboMod.Tiles.MusicBoxes
{
	public class MarxMusicBoxTile : DreamLandBossMusicBoxTile
    {
        public override int CursorItemID => ModContent.ItemType<MarxMusicBox>();
    }
}
