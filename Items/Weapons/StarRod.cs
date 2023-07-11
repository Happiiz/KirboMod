using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class StarRod : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Star Rod"); //display name
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.damage = 62;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 8;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = Item.buyPrice(0, 0, 15, 50);
			Item.rare = ItemRarityID.Pink;
			Item.autoReuse = true;
			//Item.UseSound = SoundID.Item9; //astral magic noise//put sound on the projectile
			Item.scale = 1f;
			Item.shoot = ModContent.ProjectileType<Projectiles.Star>();
			Item.shootSpeed = 28f;
			Item.crit += 24;

		}

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position.Y -= 10; //move up slightly
			position += (velocity).SafeNormalize(Vector2.Zero) * 20;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<Projectiles.Star>(), Item.damage, 6f, player.whoAmI);
			return false;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();//Sets result to this item
			recipe.AddIngredient(ModContent.ItemType<Starbit>(), 40);
			recipe.AddIngredient(ModContent.ItemType<DreamEssence>(), 20);
			recipe.AddTile(TileID.Anvils); //Craft with any anvil
			recipe.Register(); //Adds this recipe to the game
		}
    }
}