using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class BeamStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Beam Staff"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Item.staff[Item.type] = true; //staff not gun
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.damage = 10;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 4; //lower than use animation to repeat projectiles
			Item.useAnimation = 40;
			Item.reuseDelay = 40;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = -3;
			Item.value = Item.buyPrice(0, 0, 0, 20);
			Item.rare = ItemRarityID.White;
			Item.UseSound = SoundID.Item93; //electro zap
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.BeamBall>();
			Item.shootSpeed = 8f;
			Item.mana = 8;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();//the result is staff
			recipe.AddIngredient(ModContent.ItemType<Starbit>(), 20); //20 starbits
			recipe.AddTile(TileID.Anvils); //crafted at anvil
			recipe.Register(); //adds this recipe to the game
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            Vector2 shootdir = Main.MouseWorld - player.Center;
            shootdir.Normalize();
            shootdir *= 8f;
            position = player.Center + shootdir * 5;
            velocity.X = shootdir.X;
            velocity.Y = shootdir.Y;
        }
    }
}