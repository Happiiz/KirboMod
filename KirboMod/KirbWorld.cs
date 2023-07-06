using KirboMod.Items;
using KirboMod.NPCs;
using KirboMod.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace KirboMod
{
	public class KirboWorld : ModSystem
	{
        public int frameYoffset = 0;
        public int frameCounter = 0;
        public override void PostUpdateDusts() //for meta bats
        {
            frameCounter++;

            if (frameCounter > 3) //every 3 frames
            {
                frameYoffset += 16; //change frames
                frameCounter = 0;
            }

            if (frameYoffset >= 64) //bigger than sprite Y
            {
                frameYoffset = 0; //reset
            }
        }

        /*public override void ModifyHardmodeTasks(List<GenPass> list)
        {
            //Find out which step [insert step pass] is.
            int hardmodeAnnouncmentTask = list.FindIndex(genpass => genpass.Name.Equals("Hardmode Announcement"));

			if (hardmodeAnnouncmentTask != -1)
			{
				//Put new pass after [insert step pass].
				list.Insert(hardmodeAnnouncmentTask + 1, new RareStonePass("Placing Rare Shinies", 237.4298f));
			} 
        }

        public class RareStonePass : GenPass
        {
            public RareStonePass(string name, float loadWeight) : base(name, loadWeight)
            {
            }

            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                // don't make progress text for when making a Hardmode pass

                for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 10e-6); k++) //3e-6 is 10 one millionths
                {
                    bool placeSuccessful = false;
                    Tile tile;
                    int tileToPlace = ModContent.TileType<Tiles.RareStone>();
                    while (!placeSuccessful)
                    {
                        int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                        int y = WorldGen.genRand.Next((int)GenVars.rockLayerLow, Main.maxTilesY - 300); //generate within cave layer

                        List<ushort> dungeonbrick = new List<ushort> //for marking all dungeon bricks
                        {
                            TileID.BlueDungeonBrick,
                            TileID.PinkDungeonBrick,
                            TileID.GreenDungeonBrick,
                            TileID.CrackedBlueDungeonBrick,
                            TileID.CrackedPinkDungeonBrick,
                            TileID.CrackedGreenDungeonBrick
                        };

                        for (int i = 0; i < 3; i++)
                        {
                            if (!Main.tile[x, y].HasTile && Main.tile[i, y + 3].TileType != TileID.LihzahrdBrick
                                && !dungeonbrick.Contains(Main.tile[i, y + 3].TileType)) //just double check for pots, dungeon bricks, and temple bricks
                            {
                                WorldGen.PlaceTile(x, y, tileToPlace);
                                tile = Main.tile[x, y];
                                placeSuccessful = tile.HasTile && tile.TileType == tileToPlace;
                            }
                        }
                    }
                }
            }
        }*/
    }
}