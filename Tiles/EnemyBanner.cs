using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.ID;
using Terraria.Localization;

namespace KirboMod.Tiles
{
	public class EnemyBanner : ModTile
	{
		public override void SetStaticDefaults() {
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.StyleWrapLimit = 111;
            TileObjectData.addTile(Type);
			DustType = -1;
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Banner");
			AddMapEntry(new Color(13, 88, 130), name);
        }

		public override void KillMultiTile(int i, int j, int frameX, int frameY) {
			int style = frameX / 18;
			string item;
			switch (style) {
				case 0:
					item = "BioSparkBanner";
					break;
				case 1:
					item = "BirdonBanner";
					break;
				case 2:
					item = "BladeKnightBanner";
					break;
				case 3:
					item = "BurningLeoBanner";
					break;
				case 4:
					item = "CappyBanner";
					break;
				case 5:
					item = "ChillyBanner";
					break;
				case 6:
					item = "KabuBanner";
					break;
				case 7:
					item = "KnuckleJoeBanner";
					break;
				case 8:
					item = "ParosolWaddleDeeBanner";
					break;
				case 9:
					item = "PlasmaWispBanner";
					break;
				case 10:
					item = "PoppyBrosJrBanner";
					break;
				case 11:
					item = "ScarfyBanner";
					break;
				case 12:
					item = "SirKibbleBanner";
					break;
				case 13:
					item = "TwisterBanner";
					break;
				case 14:
					item = "UFOBanner";
					break;
				case 15:
					item = "WaddleDeeBanner";
					break;
				case 16:
					item = "WaddleDooBanner";
					break;
				case 17:
					item = "BrontoBurtBanner";
					break;
                case 18:
                    item = "BroomHatterBanner";
                    break;
                case 19:
                    item = "BonkersBanner";
                    break;
                case 20:
                    item = "MrFrostyBanner";
                    break;
                default:
					return;
			}
		}

		public override void NearbyEffects(int i, int j, bool closer) 
		{
			if (closer == true) 
			{
				Player player = Main.LocalPlayer;
				int style = Main.tile[i, j].TileFrameX / 18;
				int type;
				switch (style) 
				{
					case 0:
						type = ModContent.NPCType<NPCs.BioSpark>();
						break;
					case 1:
						type = ModContent.NPCType<NPCs.Birdon>();
						break;
					case 2:
						type = ModContent.NPCType<NPCs.BladeKnight>();
						break;
					case 3:
						type = ModContent.NPCType<NPCs.BurningLeo>();
						break;
					case 4:
						type = ModContent.NPCType<NPCs.Cappy>();
						break;
					case 5:
						type = ModContent.NPCType<NPCs.Chilly>();
						break;
					case 6:
						type = ModContent.NPCType<NPCs.Kabu>();
						break;
					case 7:
						type = ModContent.NPCType<NPCs.KnuckleJoe>();
						break;
					case 8:
						type = ModContent.NPCType<NPCs.ParosolDee>();
						break;
					case 9:
						type = ModContent.NPCType<NPCs.PlasmaWisp>();
						break;
					case 10:
						type = ModContent.NPCType<NPCs.PoppyBrosJr>();
						break;
					case 11:
						type = ModContent.NPCType<NPCs.Scarfy>();
						break;
					case 12:
						type = ModContent.NPCType<NPCs.SirKibble>();
						break;
					case 13:
						type = ModContent.NPCType<NPCs.Twister>();
						break;
					case 14:
						type = ModContent.NPCType<NPCs.UFO>();
						break;
					case 15:
						type = ModContent.NPCType<NPCs.WaddleDee>();
						break;
					case 16:
						type = ModContent.NPCType<NPCs.WaddleDoo>();
						break;
					case 17:
						type = ModContent.NPCType<NPCs.BrontoBurt>();
						break;
                    case 18:
                        type = ModContent.NPCType<NPCs.BroomHatter>();
                        break;
                    default:
						return;
				}
				Main.SceneMetrics.hasBanner = true; //activates banner effect
				Main.SceneMetrics.NPCBannerBuff[type] = true; //specifically adds the npcs to the banner effect
            }
		}

		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) {
			if (i % 2 == 1) {
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
		}
	}
}
