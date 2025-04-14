using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class StormTornado : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Storm Tornado Ring"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Control a giant tornado by holding down the mouse" +
				"\nLeaves storm clouds and zaps nearby enemies"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.damage = 90;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.width = 25;
			Item.height = 40;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.knockBack = 2f;
			Item.value = Item.buyPrice(0, 5, 50, 0);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item34;
			Item.autoReuse = false;
			Item.shoot = ModContent.ProjectileType<Projectiles.StormTornadoNado>();
			Item.shootSpeed = 16f;
			Item.mana = 20;
            Item.channel = true;
        }

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            position.Y -= 70f; //spawn above player
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, Item.mana, 0);
            }
            return false;
        }
        public override bool CanUseItem(Player player)
        {
             return player.ownedProjectileCounts[Item.shoot] < 1;
        }

		public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe();//the result is Storm Tornado sword
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.FleurTornado>()); //Fleur Tornado
			recipe1.AddIngredient(ItemID.RazorbladeTyphoon); //Razorblade Typhoon
			recipe1.AddIngredient(ItemID.MagnetSphere); //Magnet Sphere
			recipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 100); //100 starbits
			recipe1.AddIngredient(ModContent.ItemType<Items.RareStone>(), 2); //2 rare stones
			recipe1.AddTile(TileID.MythrilAnvil); //crafted at mythril/orichalcum anvil
			recipe1.Register(); //adds this recipe to the game
		}
	}
}