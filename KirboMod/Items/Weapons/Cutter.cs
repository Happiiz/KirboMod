using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class Cutter : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Cutter"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Flies in the opposite direction" +
				"\nOnly two can be out at a time"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
        }

		public override void SetDefaults()
		{
			Item.damage = 9;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 30;
			Item.height = 30;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = Item.buyPrice(0, 0, 0, 20);
			Item.rare = ItemRarityID.White;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = false;
			Item.shoot = ModContent.ProjectileType<Projectiles.CutterBlade>();
			Item.shootSpeed = 15f; //doesn't matter
			Item.noUseGraphic = true;
		}

		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[Item.shoot] < 2; //only two at a time
		}

		public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe();//the result is cutter
			recipe1.AddIngredient(ModContent.ItemType<Starbit>(), 20); //20 starbits
			recipe1.AddTile(TileID.Anvils); //crafted at anvil
			recipe1.Register(); //adds this recipe to the game
		}
	}
}