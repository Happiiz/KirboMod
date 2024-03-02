using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class BallsofMatter : ModItem
	{
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Ball O' Matter"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Springs out then moves towards mouse position");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() 
		{
			Item.damage = 145;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 28;
			Item.height = 26;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5;
			Item.value = Item.buyPrice(0, 0, 0, 50);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.GoodMatterOrb>();
			Item.shootSpeed = 18f;
			Item.consumable = true;
			Item.maxStack = 9999;
			Item.noMelee = true;
			Item.noUseGraphic = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(80);
			recipe.AddIngredient(ModContent.ItemType<KirbyBall>(), 40);
			recipe.AddIngredient(ModContent.ItemType<DarkMaterial>(), 1);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return base.Shoot(player, source, position, velocity, type, damage, knockback);	
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            velocity = new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, -5));

            position.Y = player.Center.Y - 10f;
        }
    }
}