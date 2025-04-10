using KirboMod.NPCs;
using KirboMod.Projectiles.Lightnings;
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
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
			
        }

		public override void SetDefaults()
		{
			Item.damage = 138;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.width = 22;
			Item.height = 22;
			Item.useAnimation = Item.useTime = 12;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4;
			Item.value = Item.buyPrice(0, 5, 50, 50);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = PureDarkMatter.LaserSFX.WithVolumeScale(.25f);
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<GoodDarkMatterLaser>();
			Item.shootSpeed = 24f;
			Item.mana = 5;
		}

        public override bool AltFunctionUse(Player player)
        {
			return true;
        }

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(); //the result is darkness ray
			recipe.AddIngredient(ItemID.HeatRay);
			recipe.AddIngredient(ModContent.ItemType<DarkMaterial>(), 15); //15 dark material
			recipe.AddTile(TileID.MythrilAnvil); //crafted at mythril anvil
			recipe.Register(); //adds this recipe to the game
		}

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
			position += Vector2.Normalize(velocity) * 30; //move forward a smidge
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            LightningProj.GetSpawningStats(velocity, out float ai0, out float ai1);
            Projectile.NewProjectile(source, position, velocity,
                ModContent.ProjectileType<GoodDarkMatterLaser>(), damage, knockback, player.whoAmI, ai0, ai1);
            return false;
		}

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