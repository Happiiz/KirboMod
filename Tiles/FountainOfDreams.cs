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
using System;
using System.Reflection;
using KirboMod.NPCs;

namespace KirboMod.Tiles
{
	public class FountainOfDreams : ModTile
	{
		public override void SetStaticDefaults() {
			Main.tileSolid[Type] = false; //able to pass through
			Main.tileSolidTop[Type] = false; //can't stand on top of it
			Main.tileTable[Type] = false; //can't place things on it
			Main.tileMergeDirt[Type] = false; //doesn't merge with dirt
			Main.tileLavaDeath[Type] = true; //dies by lava
			Main.tileWaterDeath[Type] = true; //dies by water
			Main.tileCut[Type] = false; //can't be destroyed by weapons
            TileID.Sets.DisableSmartCursor[Type] = true;

            Main.tileLighted[Type] = true; //emits light

			Main.tileNoAttach[Type] = true;
			Main.tileFrameImportant[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3); //copy for it to work

			TileObjectData.newTile.Width = 4;
		    TileObjectData.newTile.Height = 5;

			TileObjectData.newTile.Origin = new Point16(2, 2); // two tiles down and right
			                                                     
			TileObjectData.newTile.CoordinateHeights = new int[] {16, 16, 16, 16, 18}; // how tall each row of tiles within the tile should be //if 18 is bottom it will stick to the ground

			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Fountain Of Dreams");
			AddMapEntry(Color.LightPink, name);

			MinPick = 100; //requires atleast a cobalt pickaxe
			MineResist = 6f; //resists a bit to mining
			DustType = DustID.Silver;
			HitSound = SoundID.Dig; //basic tile dig
			AnimationFrameHeight = 92; //for animated frames

			TileObjectData.addTile(Type);
		}

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
			if (!fail)//when destroyed
			{
				SoundEngine.PlaySound(SoundID.Dig, new Vector2(i * 16, j * 16)); //basic tile dig
			}
        }

		public override bool RightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;

			bool holdingStarRod = player.inventory[player.selectedItem].type == ModContent.ItemType<Items.Weapons.StarRod>()
				|| player.inventory[player.selectedItem].type == ModContent.ItemType<Items.Weapons.TripleStar>();

            //checks if nightmare isn't alive ,holding star rod & if night
            if (holdingStarRod && !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<NPCs.NightmareOrb>()) 
				&& !NPC.AnyNPCs(ModContent.NPCType<NPCs.NightmareWizard>()))
			{
                if (Main.netMode != NetmodeID.MultiplayerClient) // If the player is not in multiplayer, spawn directly
                {
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<NPCs.NightmareOrb>());
                }
                else // If the player is in multiplayer, request a spawn
                {
                    //this will only work if NPCID.Sets.MPAllowedEnemies[type] is set in boss
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: ModContent.NPCType<NPCs.NightmareOrb>());
                }

                SoundEngine.PlaySound(SoundID.Roar, new Vector2(i * 16, j * 16));
            }
			return true;
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;

			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = ModContent.ItemType<Items.Weapons.StarRod>();
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			//Blueish Light
			r = 0.6f;
			g = 0.6f;
			b = 1f;
		}

        public override void NearbyEffects(int i, int j, bool closer)
        {
			Player player = Main.LocalPlayer;

			//within range ,not dead & no nightmare
			if (closer == true & !player.dead)
			{
                if (Main.dayTime)
                {
                    player.AddBuff(ModContent.BuffType<Buffs.Dreamy>(), 15); //add dreamy buff
                }
                else if (!NPC.AnyDanger()) //no boss alive
                {
                    player.AddBuff(ModContent.BuffType<Buffs.Nightmare>(), 15); //add nightmare
                }
            }
		}

        public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			frameCounter++; 
			if (frameCounter < 4)
            {
				frame = 0;
            }
			else if (frameCounter < 8)
			{
				frame = 1;
			}
			else if (frameCounter < 12)
			{
				frame = 2;
			}
			else if (frameCounter < 16)
			{
				frame = 3;
			}
			else if (frameCounter < 20)
			{
				frame = 4;
			}
			else if (frameCounter < 24)
			{
				frame = 5;
			}
			else
			{
				frameCounter = 0;
			}
		}
	}
}
