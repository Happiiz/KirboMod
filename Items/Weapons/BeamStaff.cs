using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class BeamStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Beam Staff"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Item.staff[Item.type] = true; //staff not gun
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.damage = 15;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 4; //lower than use animation to repeat projectiles
			Item.useAnimation = 40;
			Item.reuseDelay = 40;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = -3;
			Item.value = Item.buyPrice(0, 0, 0, 20);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item93; //electro zap
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.BeamBall>();
			Item.shootSpeed = 17;
			Item.mana = 8;
		}

		public override void AddRecipes()
		{
			Recipe beamstaff = CreateRecipe();//the result is staff
            beamstaff.AddIngredient(ModContent.ItemType<Starbit>(), 20); //20 starbits
            beamstaff.AddRecipeGroup(RecipeGroupID.IronBar, 10); //10 iron/lead bars
            beamstaff.AddTile(TileID.Anvils); //crafted at anvil
            beamstaff.Register(); //adds this recipe to the game
		}

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            position = player.Center + (velocity * 1.5f);
        }
    }
}