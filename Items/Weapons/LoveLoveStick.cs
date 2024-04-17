using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class LoveLoveStick : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Love Love Stick");
			// Tooltip.SetDefault("A weapon crafted from nothing but love and the determination to defeat evil");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.damage = 260;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 12;
			Item.width = 20; //half of sprite size(only effects world dimensions)
			Item.height = 20; //half of sprite size(only effects world dimensions)
			Item.useTime = 16;
			Item.useAnimation = 16;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 8;
            Item.value = Item.buyPrice(0, 25, 0, 0);
            Item.rare = ItemRarityID.Purple;
			Item.UseSound = SoundID.Item8; //magic cast
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.LoveLoves>();
			Item.shootSpeed = 24f;
			Item.noMelee = true;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            position.Y -= 20; //love loves up by 20
        }

        public override void AddRecipes()
		{
			Recipe lovelovestickrecipe = CreateRecipe();
			lovelovestickrecipe.AddIngredient(ModContent.ItemType<HeartStar>(), 30);
			lovelovestickrecipe.AddIngredient(ModContent.ItemType<MiracleMatter>()); //Zero material drop
			lovelovestickrecipe.AddTile(TileID.LunarCraftingStation); //Ancient Manipulator
			lovelovestickrecipe.Register();
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}
