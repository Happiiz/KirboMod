using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace KirboMod.Items
{
	public class DededeBrooch : ModItem
	{
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("King's Brooch");
			/* Tooltip.SetDefault("Summons King Dedede" +
				"\nA phony of one of the king's life-saving pins"); */
			ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 0; //go to *this* spot in boss spawn group

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3; // Configure the amount of this item that's needed to research it in Journey mode.
        }

        public override void SetDefaults() 
		{
			Item.width = 20;
			Item.height = 20;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.value = Item.buyPrice(0, 0, 0, 5);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item1;
			Item.consumable = true;
			Item.maxStack = 9999;
		}

        public override bool CanUseItem(Player player)
        {
			if (!NPC.AnyNPCs(Mod.Find<ModNPC>("KingDedede").Type)) //can use item if no whispy
			{
				return true;
			}
			else
			{
				return false;
			}
		}

        public override bool? UseItem(Player player)
		{
            if (player.whoAmI == Main.myPlayer) //if the player using the item is the client
            {
                if (Main.netMode != NetmodeID.MultiplayerClient) // If the player is not in multiplayer, spawn directly
                {
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<NPCs.KingDedede>());
                }
                else // If the player is in multiplayer, request a spawn
                {
                    //this will only work if NPCID.Sets.MPAllowedEnemies[type] is set in boss
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: ModContent.NPCType<NPCs.KingDedede>());
                }

                SoundStyle HeartyLaugh = new("KirboMod/Sounds/NPC/KingDedede/summon");
                SoundEngine.PlaySound(HeartyLaugh, player.position);
            }
            return true;
		}

        public override void AddRecipes()
        {
			Recipe recipe = CreateRecipe();//the result is whispy seed
			recipe.AddIngredient(ModContent.ItemType<Items.Starbit>(), 40); //40 starbits
			recipe.AddIngredient(ItemID.GoldBar, 5); //gold bar 
			recipe.AddIngredient(ItemID.Bone, 20); //bone
			recipe.AddTile(TileID.DemonAltar); //crafted at demon or crimson altar
			recipe.Register(); //adds this recipe to the game

			Recipe recipe2 = CreateRecipe();//the result is whispy seed
			recipe2.AddIngredient(ModContent.ItemType<Items.Starbit>(), 40); //40 starbits
			recipe2.AddIngredient(ItemID.PlatinumBar, 5); //platinum bar
			recipe2.AddIngredient(ItemID.Bone, 20); //bone
			recipe2.AddTile(TileID.DemonAltar); //crafted at demon or crimson altar
			recipe2.Register(); //adds this recipe to the game
		}
    }
}