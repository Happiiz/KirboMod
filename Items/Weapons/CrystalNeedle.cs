using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class CrystalNeedle : ModItem
	{
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Crystal Needle Ball"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("leaves crystals along the ground" +
				"\nShoots crystal spikes out when expired"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99; //amount needed to research 
        }

		public override void SetDefaults() 
		{
			Item.damage = 120;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 82;
			Item.height = 82;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5;
			Item.value = Item.buyPrice(0 , 0,  10, 0);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.CrystalNeedleBall>();
			Item.shootSpeed = 16f;
			Item.consumable = true;
			Item.maxStack = 9999;
			Item.noMelee = true;
			Item.noUseGraphic = true;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            position.X = player.Center.X;
            position.Y = player.Center.Y - 35f;
        }

		public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe(600);//the result is crystal needle ball
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.ClutterNeedle>(), 100); //Clutter Needle
			recipe1.AddIngredient(ItemID.CrystalShard, 5); //5 crystals
			recipe1.AddIngredient(ItemID.BeetleHusk); //1 Beetle Husk
			recipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 15); //15 starbits
			recipe1.AddIngredient(ModContent.ItemType<Items.RareStone>(), 1); //1 rare stone
			recipe1.AddTile(TileID.MythrilAnvil); //crafted at mythril/orichalcum anvil
			recipe1.Register(); //adds this recipe to the game
		}
	}
}