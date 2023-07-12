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
	public class ArcherBow : ModItem
	{
		public override void SetStaticDefaults()
		{
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.damage = 11;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.width = 30;
			Item.height = 50;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4;
			Item.value = Item.buyPrice(0, 0, 30, 5);
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item5; //bow shot
			Item.autoReuse = true;
			Item.shoot = ItemID.JestersArrow;
			Item.shootSpeed = 13;
			Item.useAmmo = AmmoID.Arrow;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            if (type == ModContent.ProjectileType<Projectiles.StarArrowProj>())
            {
                type = ModContent.ProjectileType<Projectiles.ChargedArrowProj>();
                damage *= 2;
				velocity *= 2;
            }
			else
			{
                type = ProjectileID.JestersArrow;
            }
        }

		public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe();//the result is archerbow
			recipe1.AddIngredient(ItemID.GoldBow); //Gold Bow
			recipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 20); //20 starbits
			recipe1.AddTile(TileID.Anvils); //crafted at anvil
			recipe1.Register(); //adds this recipe to the game

			Recipe recipe2 = CreateRecipe();//the result is archerbow
			recipe2.AddIngredient(ItemID.PlatinumBow); //Platinum Bow
			recipe2.AddIngredient(ModContent.ItemType<Items.Starbit>(), 20); //20 starbits
			recipe2.AddTile(TileID.Anvils); //crafted at anvil
			recipe2.Register(); //adds this recipe to the game
		}
	}
}