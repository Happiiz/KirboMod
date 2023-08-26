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
	public class DarknessRay : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Darkness Ray");
			/* Tooltip.SetDefault("Fires a dark laser that darts through the sky" +
				"\nRight click to fire a orb a darkness for more mana"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
        }

		public override void SetDefaults()
		{
			Item.damage = 90;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.width = 22;
			Item.height = 22;
			Item.useTime = 12;
			Item.useAnimation = 12;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4;
			Item.value = Item.buyPrice(0, 5, 50, 50);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item12; //space gun
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.GoodDarkLaser>();
			Item.shootSpeed = 24f;
			Item.mana = 4;
		}

        public override bool AltFunctionUse(Player player)
        {
			return true;
        }

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();//the result is staff
			recipe.AddIngredient(ItemID.HeatRay);
			recipe.AddIngredient(ModContent.ItemType<DarkMaterial>(), 15); //15 dark material
			recipe.AddTile(TileID.MythrilAnvil); //crafted at mythril anvil
			recipe.Register(); //adds this recipe to the game
		}

        public override bool CanUseItem(Player player)
        {
			if (player.altFunctionUse == 2) //right click
			{
                //damage increase in Shoot()
				Item.useTime = 60;
				Item.useAnimation = 60;
				Item.knockBack = 12;
				Item.shoot = ModContent.ProjectileType<Projectiles.GoodDarkOrb>();
				Item.shootSpeed = 16f;
				Item.mana = 20;
				Item.UseSound = SoundID.Item117; //conjure arcanum
            }
			else //left click
            {
				Item.useTime = 8;
				Item.useAnimation = 8;
				Item.knockBack = 4;
				Item.shoot = ModContent.ProjectileType<Projectiles.GoodDarkLaser>();
				Item.shootSpeed = 32f;
				Item.mana = 4;
				Item.UseSound = SoundID.Item12; //space gun
            }
			return true;
		}

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
			if (player.altFunctionUse == 2) //right clicking
            {
                damage *= 10;
				position.Y -= 15; //move up a little
            }
			position += velocity; //move forward a smidge
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			return true;
		}

		// Help, my gun isn't being held at the handle! Adjust these 2 numbers until it looks right.
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}

        public override void HoldItemFrame(Player player)
        {
			Item.scale = 0.8f; //make small while holding
        }
    }
}