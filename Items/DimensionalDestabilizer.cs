using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Terraria.GameContent.NetModules;
using Microsoft.Xna.Framework.Input;

namespace KirboMod.Items
{
	public class DimensionalDestabilizer : ModItem
	{
		public override void SetStaticDefaults() 
		{
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
			ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
			Item.staff[Type] = true; //hold like staff
        }

		public override void SetDefaults() 
		{
			Item.width = 52;
			Item.height = 52;
			Item.value = Item.buyPrice(0, 0, 0, 50);
			Item.rare = ItemRarityID.Yellow;
			Item.maxStack = 1;
			Item.useTime = 60;
			Item.useAnimation = 60;
			Item.UseSound = SoundID.Item1;
			Item.useStyle = ItemUseStyleID.Shoot;
		}

        public override bool CanUseItem(Player player)
        {
            Point mouselocation = Main.MouseWorld.ToTileCoordinates();

            if (NPC.AnyNPCs(ModContent.NPCType<NPCs.MidbossRift>()) || NPC.AnyNPCs(ModContent.NPCType<NPCs.MidBosses.Bonkers>()) ||
                NPC.AnyNPCs(ModContent.NPCType<NPCs.MidBosses.MrFrosty>())
                 || player.CountItem(ModContent.ItemType<Starbit>()) < 50
				 || WorldGen.SolidOrSlopedTile(Main.tile[mouselocation.X, mouselocation.Y]))
			{
				return false;
			}

            return true;
        }
        public override bool? UseItem(Player player)
        {
            for (int i = 0; i < 50; i++)
            {
                player.ConsumeItem(ModContent.ItemType<Starbit>());
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int rift = NPC.NewNPC(Item.GetSource_FromThis(), (int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, ModContent.NPCType<NPCs.MidbossRift>(), ai1: 1);

                if (Main.netMode == NetmodeID.Server) //sync to server
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: rift);
                }
            }
            return true;
        }

        public override void AddRecipes()
        {
			Recipe recipe = CreateRecipe();//the result is dimen destabilizer
			recipe.AddIngredient(ItemID.FallenStar, 5); //5 fallen stars
			recipe.AddIngredient(ItemID.MeteoriteBar, 20); //20 meteorite bars
            recipe.AddTile(TileID.MythrilAnvil); //crafted at hardmode anvil
			recipe.Register(); //adds this recipe to the game
		}
    }
}