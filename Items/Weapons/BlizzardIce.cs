using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class BlizzardIce : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Blizzard Ice Pot"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Fires a volley of icicles" +
				"\nEnemies killed by one condense into an ice cube" +
				"\nDoes not work on bosses"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.damage = 156;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.width = 48;
			Item.height = 52;
			Item.useTime = 13;
			Item.useAnimation = Item.useTime;
            Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0.1f;
			Item.value = Item.buyPrice(0, 5, 50, 0);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item24; //spectre boots
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.BlizzardIcicle>(); 
			Item.shootSpeed = 16; //give proj extraupdate and afterimage for 60 velocity
			Item.mana = 12;
		}

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-8, -10);
        }

        public override void HoldItemFrame(Player player)
        {
            Item.scale = 0.8f; //make small while holding
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(10)); // 10 degree spread.
        }

        public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe();//the result is blizzardice
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.FrostyIce>()); //Frosty Ice
			recipe1.AddIngredient(ItemID.BlizzardStaff); //Blizzard Staff
			recipe1.AddIngredient(ItemID.BubbleGun); //Bubble Gun
			recipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 100); //100 starbits
			recipe1.AddIngredient(ModContent.ItemType<Items.RareStone>(), 2); //2 rare stones
            recipe1.AddTile(TileID.MythrilAnvil); //crafted at mythril/orichalcum anvil
			recipe1.Register(); //adds this recipe to the game
		}
	}
}