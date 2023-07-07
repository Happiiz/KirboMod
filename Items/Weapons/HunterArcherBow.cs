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
	public class HunterArcherBow : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Tri-Killer"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Converts all arrows into three jester arrows" +
				"\nConverts star arrows into five charged variants"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
        }

		public override void SetDefaults()
		{
			Item.damage = 40;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.width = 48;
			Item.height = 62;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4;
			Item.value = Item.buyPrice(0, 0, 45, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item5; //bow shot
			Item.autoReuse = true;
			Item.shoot = ItemID.JestersArrow;
			Item.shootSpeed = 10f;
			Item.useAmmo = AmmoID.Arrow;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            Vector2 projshoot = Main.MouseWorld - player.Center;
            projshoot.Normalize();

            if (type != ModContent.ProjectileType<Projectiles.StarArrowProj>())
            {
                type = ProjectileID.JestersArrow;
                projshoot *= 10f;
            }
            else
            {
                type = ModContent.ProjectileType<Projectiles.ChargedArrowProj>();
                damage = (int)(damage * 1.5f);
                projshoot *= 30f;
            }

            velocity.X = projshoot.X;
            velocity.Y = projshoot.Y;
        }
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			Vector2 projshoot = Main.MouseWorld - player.Center;
			projshoot.Normalize();

            //shoot two extra arrows

            Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(10)), type, damage, knockback, Item.playerIndexTheItemIsReservedFor);
            Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(-10)), type, damage, knockback, Item.playerIndexTheItemIsReservedFor);

			//extra extra arrows
            if (type == ModContent.ProjectileType<Projectiles.ChargedArrowProj>())
			{
                Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(20)), type, damage, knockback, Item.playerIndexTheItemIsReservedFor);
                Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(-20)), type, damage, knockback, Item.playerIndexTheItemIsReservedFor);
            }
            velocity.X = projshoot.X;
			velocity.Y = projshoot.Y;
			return true;
		}

		public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe();//the result is tri-kill bow
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.ArcherBow>()); //Archer Bow
			recipe1.AddIngredient(ItemID.Marrow); //Marrow
			recipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 50); //50 starbits
			recipe1.AddIngredient(ModContent.ItemType<Items.RareStone>(), 2); //2 rare stones
			recipe1.AddTile(TileID.Anvils); //crafted at anvil
			recipe1.Register(); //adds this recipe to the game
		}
	}
}