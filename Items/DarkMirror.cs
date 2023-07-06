using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

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
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.DarkMirrorProj>();
			Item.shootSpeed = 10f;
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

        public override void HoldItem(Player player)
        {
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];

				if (npc.boss == false) //no bosses around
				{
					player.AddBuff(Mod.Find<ModBuff>("DarkFeeling").Type, 2);
				}
			}
        }

        public override void AddRecipes()
        {
			Recipe recipe = CreateRecipe();//the result is dark mirror
			recipe.AddIngredient(ModContent.ItemType<Items.NightCloth>(), 10); //10 nightcloth
			recipe.AddIngredient(ItemID.SoulofNight, 20); //20 souls of night
			recipe.AddIngredient(ItemID.Glass, 10); //10 glass
			recipe.AddTile(TileID.MythrilAnvil); //crafted at hardmode anvil
			recipe.Register(); //adds this recipe to the game
		}
    }
}