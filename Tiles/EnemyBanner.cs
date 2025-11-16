using System;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace KirboMod.Tiles
{
	public class EnemyBanner : ModBannerTile //for automatic banner handling (and swaying animations!)
	{
		//each enum entry corresponds to the place style of the banners (ex: biospark banner place style is 0, and bio spark entry is also 0)
        //also has to match the name of the npc
		public enum StyleID
		{
			BioSpark,
			Birdon,
            BladeKnight,
            BurningLeo,
            Cappy,
            Chilly,
            Kabu,
            KnuckleJoe,
            ParosolDee,
            PlasmaWisp,
            PoppyBrosJr,
            Scarfy,
            SirKibble,
            Twister,
            UFO,
            WaddleDee,
            WaddleDoo,
            BrontoBurt,
            BroomHatter,
            Bonkers,
            MrFrosty,
            Wheelie,
            Bomber,
            Batafire
        }
	}
}
