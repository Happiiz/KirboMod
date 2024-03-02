using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class ChainBomb : ModItem
	{
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Chain Bomb"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Detonate after a while or upon contact with an enemy" +
				"\nSets off other nearby chain bombs when detonated"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99; //amount needed to research 
        }

		public override void SetDefaults() 
		{
			Item.damage = 60;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 50;
			Item.height = 50;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5;
			Item.value = Item.buyPrice(0, 0, 1, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.ChainBombProj>();
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
			Recipe chainbombrecipe = CreateRecipe(300);//the result is chain bomb
			chainbombrecipe.AddIngredient(ModContent.ItemType<Items.Weapons.Bomb>()); 
			chainbombrecipe.AddIngredient(ItemID.Wire, 10); //Wire
			chainbombrecipe.AddIngredient(ModContent.ItemType<Items.Starbit>(), 15);
			chainbombrecipe.AddIngredient(ModContent.ItemType<Items.RareStone>(), 1); //1 rare stones
			chainbombrecipe.AddTile(TileID.Anvils); //crafted at anvil
			chainbombrecipe.Register(); //adds this recipe to the game
		}
    }
}