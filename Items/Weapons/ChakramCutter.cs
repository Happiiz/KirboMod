using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class ChakramCutter : ModItem
	{
		public override void SetStaticDefaults()
		{
			ItemID.Sets.IsRangedSpecialistWeapon[Type] = true;
			// DisplayName.SetDefault("Chakram Cutter"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Comes back after a while" +
				"\nOnly four can be out at a time"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
        }
		public override void SetDefaults()
		{
			Item.damage = 40;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 8;
			Item.value = Item.buyPrice(0, 0, 45, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = false;
			Item.shoot = ModContent.ProjectileType<Projectiles.ChakramCutterProj>();
			Item.shootSpeed = 45;
			Item.autoReuse = true; 
			Item.noUseGraphic = true;
		}
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if (Main.myPlayer != player.whoAmI)
				return false;
            for (int i = -1; i < 2; i+=2)
            {
				int delayBeforeSecondChakramFires = 16 * (i == -1 ? 0 : 1);
				Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, -delayBeforeSecondChakramFires, i);
            }
			return false;
        }
        public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe();//the result is chakramcutter
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.Cutter>()); //Cutter
			recipe1.AddIngredient(ItemID.LightDisc); //Light Disc
			recipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 50); //50 starbits
			recipe1.AddIngredient(ModContent.ItemType<RareStone>(), 1); //1 rare stone
			recipe1.AddTile(TileID.Anvils); //crafted at anvil
			recipe1.Register(); //adds this recipe to the game
		}
	}
}