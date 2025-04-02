using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class Bomb : ModItem
	{
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Jolly Bomb"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Detonate after a while or upon contact with an enemy");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99; //amount needed to research
        }

		public override void SetDefaults() 
		{
			Item.damage = 30;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 50;
			Item.height = 50;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5;
			Item.value = Item.buyPrice(0, 0, 0, 10);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.BombProj>();
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
        public override bool AltFunctionUse(Player player)
        {
			return player.ItemTimeIsZero;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if(player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer, player.altFunctionUse);
			}
            return false;
        }
        public override void AddRecipes()
        {
            Recipe bombrecipe = CreateRecipe(10);
			bombrecipe.AddIngredient(ItemID.Bomb);
			bombrecipe.AddIngredient(ModContent.ItemType<Starbit>(), 5);
			bombrecipe.AddTile(TileID.Anvils);
			bombrecipe.Register();
        }
    }
}