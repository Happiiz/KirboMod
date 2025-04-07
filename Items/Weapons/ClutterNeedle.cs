using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class ClutterNeedle : ModItem
	{
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Clutter Needle Ball"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("drops a bunch of clutter when expired"); 
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99; //amount needed to research 
        }

		public override void SetDefaults() 
		{
			Item.damage = 65;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 82;
			Item.height = 82;
			Item.useTime = 37;
			Item.useAnimation = 37;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5;
			Item.value = Item.buyPrice(0 , 0,  1, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.ClutterNeedleBall>();
			Item.shootSpeed = 15f;
			Item.consumable = true;
			Item.maxStack = 9999;
			Item.noMelee = true;
			Item.noUseGraphic = true;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            position.X = player.Center.X;
            position.Y = player.Center.Y - 25f;
        }

		public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe(600);//the result is clutter needle ball
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.Needle>(), 5); // 5 Needle balls
			recipe1.AddIngredient(ItemID.SpikyBall, 5); // 5 Spiky Balls
			recipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 10); //10 starbits
			recipe1.AddIngredient(ModContent.ItemType<Items.RareStone>(), 1); //1 rare stone
			recipe1.AddTile(TileID.Anvils); //crafted at anvil
			recipe1.Register(); //adds this recipe to the game
		}
	}
}