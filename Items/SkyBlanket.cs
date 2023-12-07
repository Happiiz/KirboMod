using KirboMod.NPCs;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items
{
	public class SkyBlanket : ModItem
	{
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Sky Blanket");
			/* Tooltip.SetDefault("Calls on a Kracko Jr." +
				"\n'I need my blankey!"); */
			ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 3; //go to *this* spot in boss spawn group(multiple can occupy the same slot)

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3; // Configure the amount of this item that's needed to research it in Journey mode.
        }

		public override void SetDefaults() 
		{
			Item.width = 25;
			Item.height = 14;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.value = Item.buyPrice(0, 0, 0, 5);
			Item.rare = ItemRarityID.White;
			Item.UseSound = SoundID.Item1;
			Item.consumable = true;
			Item.maxStack = 9999;
		}

        public override bool CanUseItem(Player player)
        {
			if (!NPC.AnyNPCs(ModContent.NPCType<KrackoJr>()) && !NPC.AnyNPCs(ModContent.NPCType<NPCs.Kracko>())) //can use item if no krackos
			{
				return true;
			}			
			return false;
		}

        public override bool? UseItem(Player player)
		{
            if (player.whoAmI == Main.myPlayer) //if the player using the item is the client
            {
                if (Main.netMode != NetmodeID.MultiplayerClient) // If the player is not in multiplayer, spawn directly
                {
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<NPCs.KrackoJr>());
                }
                else // If the player is in multiplayer, request a spawn
                {
                    //this will only work if NPCID.Sets.MPAllowedEnemies[type] is set in boss
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: ModContent.NPCType<NPCs.KrackoJr>());
                }
                SoundEngine.PlaySound(SoundID.Roar, player.position);
            }
            return true;
		}

        public override void AddRecipes()
        {
			Recipe recipe = CreateRecipe();//the result is sky blanket
			recipe.AddIngredient(ModContent.ItemType<Items.Starbit>(), 40); //40 starbits
			recipe.AddIngredient(ItemID.Cloud, 10); //10 clouds
			recipe.AddTile(TileID.DemonAltar); //crafted at demon or crimson altar
			recipe.Register(); //adds this recipe to the game
		}
    }
}