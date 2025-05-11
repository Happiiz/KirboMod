using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class CyborgArcherBow : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Bot Shot"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Converts all arrows into a laser arrow" +
				"\nConverts star arrows into a laser beam & four extra laser arrows"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
        }

		public override void SetDefaults()
		{
			Item.damage = 75;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.width = 48;
			Item.height = 62;
			Item.useTime = Item.useAnimation = 12;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 7f;
			Item.crit = 10;//stake launcher has this
			Item.value = Item.buyPrice(0, 0, 30, 5);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item5; //bow shot
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.CyborgArcherArrow>();
			Item.shootSpeed = 8f;
			Item.useAmmo = AmmoID.Arrow;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            if (type != ModContent.ProjectileType<Projectiles.StarArrowProj>())
            {
                type = ModContent.ProjectileType<Projectiles.CyborgArcherArrow>();
            }
            else
            {
				
                type = ModContent.ProjectileType<Projectiles.CyborgArcherLaser>();
            }

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if (Main.myPlayer == player.whoAmI && type == ModContent.ProjectileType<Projectiles.CyborgArcherLaser>()) //instead of star arrow because this is called AFTER mod shoot stats
			{
				//Get distances of each probe from mouse
				float vel = velocity.Length();
				Vector2 projshoot1 = Main.MouseWorld - (player.Center + new Vector2(0, -60)); //get distance
				projshoot1.Normalize(); //to one
				projshoot1 *= vel; 

				Vector2 projshoot2 = Main.MouseWorld - (player.Center + new Vector2(0, 60)); //get distance
				projshoot2.Normalize(); //to one
				projshoot2 *= vel;  

				Vector2 projshoot3 = Main.MouseWorld - (player.Center + new Vector2(-60, 0)); //get distance
				projshoot3.Normalize(); //to one
				projshoot3 *= vel;

				Vector2 projshoot4 = Main.MouseWorld - (player.Center + new Vector2(60, 0)); //get distance
				projshoot4.Normalize(); //to one
				projshoot4 *= vel; 

				//Shoot probe laser arrows
				Projectile.NewProjectile(source, player.Center.X, player.Center.Y - 60, projshoot1.X, projshoot1.Y, ModContent.ProjectileType<Projectiles.CyborgArcherArrow>(), damage / 4, knockback, Main.myPlayer);
				Projectile.NewProjectile(source, player.Center.X, player.Center.Y + 60, projshoot2.X, projshoot2.Y, ModContent.ProjectileType<Projectiles.CyborgArcherArrow>(), damage / 4, knockback, Main.myPlayer);
				Projectile.NewProjectile(source, player.Center.X - 60, player.Center.Y, projshoot3.X, projshoot3.Y, ModContent.ProjectileType<Projectiles.CyborgArcherArrow>(), damage / 4, knockback, Main.myPlayer);
				Projectile.NewProjectile(source, player.Center.X + 60, player.Center.Y, projshoot4.X, projshoot4.Y, ModContent.ProjectileType<Projectiles.CyborgArcherArrow>(), damage / 4, knockback, Main.myPlayer);
				//moved code to laser for multiplayer reasons
				//SoundEngine.PlaySound(SoundID.Item158, player.Center); //zapinator
			}
               return true;
		}

        public override void HoldItem(Player player)
        {
			//Create probes
			Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.Center.X, player.Center.Y - 60, 0, 0, ModContent.ProjectileType<Projectiles.CyborgArcherProbe>(), -1, 0, Main.myPlayer);
			Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.Center.X, player.Center.Y + 60, 0, 0, ModContent.ProjectileType<Projectiles.CyborgArcherProbe>(), -1, 0, Main.myPlayer);
			Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.Center.X - 60, player.Center.Y, 0, 0, ModContent.ProjectileType<Projectiles.CyborgArcherProbe>(), -1, 0, Main.myPlayer);
			Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.Center.X + 60, player.Center.Y, 0, 0, ModContent.ProjectileType<Projectiles.CyborgArcherProbe>(), -1, 0, Main.myPlayer);
        }

        public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe();//the result is gigantsword
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.HunterArcherBow>()); //Hunter Archer Bow
			recipe1.AddIngredient(ItemID.StakeLauncher);
            recipe1.AddIngredient(ItemID.ChlorophyteShotbow);
            recipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 100); //100 starbits
			recipe1.AddIngredient(ModContent.ItemType<Items.RareStone>(), 2); //2 rare stones
			recipe1.AddTile(TileID.MythrilAnvil); //crafted at mythril/orichalcum anvil
			recipe1.Register(); //adds this recipe to the game
		}
	}
}