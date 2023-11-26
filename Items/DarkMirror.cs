using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.Audio;

namespace KirboMod.Items
{
	public class DarkMirror : ModItem
	{
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Dark Mirror");
			/* Tooltip.SetDefault("Left click to throw it out, walk up to it to collect it" +
				"\n'A bright light reflecting off of this could attract a dark presence...'"); */ 
			ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 100; //go to *this* spot in boss spawn group
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() 
		{
			Item.width = 52;
			Item.height = 52;
			Item.value = Item.buyPrice(0, 0, 0, 50);
			Item.rare = ItemRarityID.Yellow;
			Item.maxStack = 1;
			Item.useTime = 16;
			Item.useAnimation = 16;
			Item.UseSound = SoundID.Item1;
			Item.useStyle = ItemUseStyleID.HoldUp;
		}

        public override bool CanUseItem(Player player)
        {
			if (NPC.AnyNPCs(Mod.Find<ModNPC>("DarkMatter").Type))
			{
				return false;
			}
			else
            {
				return player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.DarkMirrorProj>()] < 1;
            }
        }
        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer) //if the player using the item is the client
            {
                if (Main.netMode != NetmodeID.MultiplayerClient) // If the player is not in multiplayer, spawn directly
                {
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<NPCs.DarkMatter>());
                }
                else // If the player is in multiplayer, request a spawn
                {
                    //this will only work if NPCID.Sets.MPAllowedEnemies[type] is set in boss
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: ModContent.NPCType<NPCs.DarkMatter>());
                }
                SoundEngine.PlaySound(SoundID.Roar, player.position);
            }
            return true;
        }

        public override void AddRecipes()
        {
			Recipe recipe = CreateRecipe();//the result is dark mirror
			recipe.AddIngredient(ModContent.ItemType<Items.NightCloth>(), 5); //5 nightcloth
			recipe.AddIngredient(ItemID.SoulofNight, 20); //20 souls of night
			recipe.AddIngredient(ItemID.Ectoplasm, 10); //10 ectoplasm
            recipe.AddTile(TileID.MythrilAnvil); //crafted at hardmode anvil
			recipe.Register(); //adds this recipe to the game
		}
    }
}