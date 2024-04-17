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
			Item.damage = 80; 
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
			Item.scale = 1f;
			Item.shoot = ModContent.ProjectileType<Projectiles.Star>();
			Item.shootSpeed = 32f;
			Item.crit += 24;
            Item.ArmorPenetration = 60;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position.Y -= 10; //move up slightly
			position += velocity.SafeNormalize(Vector2.Zero) * 10;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (Main.myPlayer != player.whoAmI)
				return false;
			Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<Projectiles.Star>(), damage, knockback, player.whoAmI);
			proj.rotation += Main.rand.NextFloat() * MathF.Tau;
			proj.netUpdate = true;
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
            recipe.AddIngredient(ItemID.HallowedBar, 5); //5 hallowed bars
            recipe.AddTile(TileID.Anvils); //Craft with any anvil
			recipe.Register(); //Adds this recipe to the game
		}
    }
}