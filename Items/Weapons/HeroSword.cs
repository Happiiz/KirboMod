using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class HeroSword : ModItem
	{
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Hero Sword"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() 
		{
			Item.damage = 19;
			Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 32;
			Item.useAnimation = 32;
			Item.shootsEveryUse = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = Item.buyPrice(0, 0, 5, 0);
			Item.rare = ItemRarityID.White;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.HeroSlash>();
			Item.shootSpeed = 6.5f;
		}

        public override void AddRecipes()
		{
            Recipe herosword = CreateRecipe();//the result is herosword
            herosword.AddIngredient(ModContent.ItemType<Items.Starbit>(), 20); //20 starbits
            herosword.AddIngredient(ItemID.IronBroadsword);
            herosword.AddTile(TileID.Anvils); //crafted at anvil
            herosword.Register(); //adds this recipe to the game

            Recipe herosword2 = CreateRecipe();//the result is herosword
            herosword2.AddIngredient(ModContent.ItemType<Items.Starbit>(), 20); //20 starbits
            herosword2.AddIngredient(ItemID.LeadBroadsword);
            herosword2.AddTile(TileID.Anvils); //crafted at anvil
            herosword2.Register(); //adds this recipe to the game
        }
	}
}