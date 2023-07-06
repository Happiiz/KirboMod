using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items
{
	public class PillarOfLight : ModItem
	{
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Totem Of Light");
			// Tooltip.SetDefault("Summons a great darkness upon your world");
			ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 100; //go to *this* spot in boss spawn group
			ItemID.Sets.ItemNoGravity[Item.type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() 
		{
			Item.width = 20;
			Item.height = 20;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.value = Item.buyPrice(0, 0, 0, 5);
			Item.rare = ItemRarityID.Red;
			Item.UseSound = SoundID.Item1;
			Item.consumable = true;
			Item.maxStack = 9999;
			Item.noUseGraphic = true;
			Item.shoot = Mod.Find<ModProjectile>("FlyingPillarOfLight").Type;
		}

        public override bool CanUseItem(Player player)
        {
			//can use item if no Pure Dark Matter, Zero, Eye of Zero or Totem proj
			if (!NPC.AnyNPCs(Mod.Find<ModNPC>("PureDarkMatter").Type) && !NPC.AnyNPCs(Mod.Find<ModNPC>("Zero").Type) 
				&&!NPC.AnyNPCs(Mod.Find<ModNPC>("Zero").Type) && !NPC.AnyNPCs(Mod.Find<ModNPC>("ZeroEye").Type))
			{
				return player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.FlyingPillarOfLight>()] < 1; //can use if no pillars
			}
			else
			{
				return false;
			}
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            position.Y = player.Center.Y - 50;
            position.X = player.Center.X + player.direction * 50;

            velocity.X = 0;
            velocity.Y = 0; //moves up in code
        }

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}

		public override void AddRecipes()
        {
			Recipe recipe = CreateRecipe();//the result is totem
			recipe.AddIngredient(ModContent.ItemType<Items.PurifiedMaterial>()); //1 purified matter
			recipe.AddIngredient(ItemID.HallowedBar, 5); //5 hallowed bars
			recipe.AddTile(TileID.LunarCraftingStation); //crafted at ancient manipulator
			recipe.Register(); //adds this recipe to the game
		}
    }
}