using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class KirbyBall : ModItem
	{
		public override void SetStaticDefaults() 
		{
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99; //amount needed to research
        }

		public override void SetDefaults() 
		{
			Item.damage = 90;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 28;
			Item.height = 26;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5;
			Item.value = 20;
			Item.rare = ItemRarityID.Lime;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.KirbyBallProj>();
			Item.shootSpeed = 18f;
			Item.consumable = true;
			Item.maxStack = 9999;
			Item.noMelee = true;
			Item.noUseGraphic = true;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            position = new Vector2(player.Center.X, player.Center.Y - 50); 
        }

        public override void AddRecipes()
        {
            Recipe kirbyballrecipe = CreateRecipe(600);
			kirbyballrecipe.AddIngredient(ModContent.ItemType<Starbit>(), 10);
			kirbyballrecipe.AddIngredient(ModContent.ItemType<HeartMatter>());
			kirbyballrecipe.AddTile(TileID.MythrilAnvil);
			kirbyballrecipe.Register();
		}
    }
}