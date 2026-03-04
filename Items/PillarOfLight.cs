using KirboMod.NPCs;
using KirboMod.NPCs.PureDarkMatterRematch;
using KirboMod.Projectiles;
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
			ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 5; //go to *this* spot in boss spawn group
			ItemID.Sets.ItemNoGravity[Item.type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() 
		{
			Item.width = 20;
			Item.height = 20;
			Item.useTime = 18;
			Item.useAnimation = 18;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.value = Item.buyPrice(0, 0, 0, 5);
			Item.rare = ItemRarityID.Red;
			Item.UseSound = SoundID.Item1;
			Item.consumable = false;
			Item.maxStack = 9999;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<FlyingPillarOfLight>();
		}

        public override bool AltFunctionUse(Player player)
        {
			return player.ItemTimeIsZero;
        }
        public override bool? UseItem(Player player)
        {
			if(player.altFunctionUse == 2)
			{
				KirboWorld.summonedDarkMatterRematchBefore = false;
			}
			return true;
        }
        public override bool CanUseItem(Player player)
        {
			//can use item if no Pure Dark Matter, Zero, Eye of Zero or Totem proj
			if (!NPC.AnyNPCs(ModContent.NPCType<NPCs.Zero>()) && !NPC.AnyNPCs(ModContent.NPCType<ZeroEye>()) && !NPC.AnyNPCs(ModContent.NPCType<PureDarkMatterRematch>()))
			{
				return player.ownedProjectileCounts[ModContent.ProjectileType<FlyingPillarOfLight>()] < 1; //can use if no pillars
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
            velocity.Y = -10;
        }

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}

		public override void AddRecipes()
        {
			Recipe recipe = CreateRecipe();//the result is totem
			recipe.AddIngredient(ModContent.ItemType<DarkMaterial>(), 3);
            recipe.AddIngredient(ModContent.ItemType<SoulMatter>(), 3);
            recipe.AddIngredient(ModContent.ItemType<HeartMatter>(), 10);
            recipe.AddIngredient(ItemID.LunarBar, 5); //5 luminite bars
			recipe.AddTile(TileID.LunarCraftingStation); //crafted at ancient manipulator
			recipe.Register(); //adds this recipe to the game
		}
    }
}