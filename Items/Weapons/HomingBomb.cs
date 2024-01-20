using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class HomingBomb : ModItem
	{
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Homing Bomb"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Chases after enemies before detonation" +
				"\nSets off other nearby homing bombs when detonated"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99; //amount needed to research 
        }

		public override void SetDefaults() 
		{
			Item.damage = 108;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 50;
			Item.height = 50;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5;
			Item.value = Item.buyPrice(0, 0, 10, 0);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.HomingBombProj>();
			Item.shootSpeed = 15f;
			Item.consumable = true;
			Item.maxStack = 9999;
			Item.noMelee = true;
			Item.noUseGraphic = true;
		}

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position.X = player.Center.X;
            position.Y = player.Center.Y - 25f;
        }

        public override void AddRecipes()
        {
			Recipe homingbombrecipe = CreateRecipe(300);//the result is homing bombs
			homingbombrecipe.AddIngredient(ModContent.ItemType<Items.Weapons.ChainBomb>());
			homingbombrecipe.AddIngredient(ItemID.ClusterRocketI, 5); // 5 Cluster Rockets
			homingbombrecipe.AddIngredient(ItemID.ChlorophyteBullet, 5); //5 Chlorophyte Bullets
			homingbombrecipe.AddIngredient(ModContent.ItemType<Items.Starbit>(), 30);
			homingbombrecipe.AddIngredient(ModContent.ItemType<Items.RareStone>(), 2); //2 rare stones
			homingbombrecipe.AddTile(TileID.Anvils); //crafted at anvil
			homingbombrecipe.Register(); //adds this recipe to the game
		}
    }
}