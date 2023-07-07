using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class RangerGun : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Pow Shot Gun"); // display name
			/* Tooltip.SetDefault("Converts all bullets into star bubbles" + //first line
				"\nConverts star bullets into giant star bubbles"); */ //second line
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.damage = 24;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.width = 70;
			Item.height = 40;
			Item.useTime = 40;
			Item.useAnimation = 40;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 8;
			Item.value = Item.buyPrice(0, 0, 30, 5);
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item11; //basic gun shot
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.RangerStar>();
			Item.shootSpeed = 8f;
			Item.useAmmo = AmmoID.Bullet;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            if (type != ModContent.ProjectileType<Projectiles.StarBulletProj>())
            {
                type = ModContent.ProjectileType<Projectiles.RangerStar>();
            }

            Vector2 shootdir = Main.MouseWorld - player.Center; //distance 
            shootdir.Normalize();//reduce to 1
            shootdir *= 40f;//speed
            position = player.Center + shootdir;//move from player apon spawning
        }

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if (type != ModContent.ProjectileType<Projectiles.StarBulletProj>())
			{
				return true;
			}
			else
            {
				Projectile.NewProjectile(source, position.X, position.Y - 15, velocity.X, velocity.Y, ModContent.ProjectileType<Projectiles.BigRangerStar>(), (int)(damage * 1.5), 5, Item.playerIndexTheItemIsReservedFor, 0, 0);
				return false;
			}
		}

		public override void AddRecipes()
		{
			Recipe rangergunrecipe1 = CreateRecipe();//the result is rangergun
			rangergunrecipe1.AddIngredient(ItemID.Musket); //Musket
			rangergunrecipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 20); //20 starbits
			rangergunrecipe1.AddTile(TileID.Anvils); //crafted at anvil
			rangergunrecipe1.Register(); //adds this recipe to the game

			//alternate recipe for crimson worlds
			Recipe rangergunrecipe2 = CreateRecipe();//the result is rangergun
			rangergunrecipe2.AddIngredient(ItemID.TheUndertaker); //Undertaker
			rangergunrecipe2.AddIngredient(ModContent.ItemType<Items.Starbit>(), 20); //20 starbits
			rangergunrecipe2.AddTile(TileID.Anvils); //crafted at anvil
			rangergunrecipe2.Register(); //adds this recipe to the game
		}
	}
}