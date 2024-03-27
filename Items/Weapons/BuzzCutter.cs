using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class BuzzCutter : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Buzz Cutter"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Shoots a flying sawblade that bounces off nearby walls" +
				"\nReturns to the player if not hitting any walls" +
				"\nFive can be out at a time"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
        }

		public override void SetDefaults()
		{
			Item.ArmorPenetration = 1000;
			Item.damage = 40;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 50;
			Item.height = 38;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 3;
			Item.value = Item.buyPrice(0, 5, 50, 0);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item84 with { MaxInstances = 10 };
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.BuzzCutterProj>();
			Item.shootSpeed = 20; //doesn't matter
		}
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			position.Y -= 30; //go up a smidge
		}

        // Help, my gun isn't being held at the handle! Adjust these 2 numbers until it looks right.
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, -10);
        }

        public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe();//the result is buzzcutter
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.ChakramCutter>()); //Chakram Cutter
			recipe1.AddIngredient(ItemID.Stynger); //Stynger
			recipe1.AddIngredient(ItemID.ChainGun); //Chain Gun
			recipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 100); //100 starbits
			recipe1.AddIngredient(ModContent.ItemType<Items.RareStone>(), 5); //5 rare stones
			recipe1.AddTile(TileID.MythrilAnvil); //crafted at mythril/orichalcum anvil
			recipe1.Register(); //adds this recipe to the game
        }
	}
}