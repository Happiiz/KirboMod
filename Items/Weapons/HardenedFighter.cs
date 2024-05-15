using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class HardenedFighter : ModItem
	{
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Hardened Fighter Glove");
			/* Tooltip.SetDefault("Right click to slam the ground and shoot rocks!" +
				"\nSlam onto enemies to boost yourself"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
			ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

		public override void SetDefaults() 
		{
			Item.damage = 55;
			Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.width = 30; //world dimensions
			Item.height = 30; //world dimensions
			Item.useTime = 5;
			Item.useAnimation = 5;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.knockBack = 1;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.buyPrice(0, 0, 45, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.HardenedFistProj>();
			Item.shootSpeed = 35f;
		}

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2) //right click
            {
                damage *= 4;
            }
            else //left click
            {
                velocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
            }
        }

		public override bool AltFunctionUse(Player player)
        {
			return true; //can right click
        }

        public override bool CanUseItem(Player player)
        {
			if (player.altFunctionUse == 2) //right click
			{
				Item.useTime = 12; 
				Item.useAnimation = 12;
				Item.shoot = ModContent.ProjectileType<Projectiles.HardenedSlam>();
				Item.shootSpeed = 0.0001f; //make it very small but not immobile
            }
			else //left click
			{
				Item.useTime = 5;
				Item.useAnimation = 5;
				Item.shoot = ModContent.ProjectileType<Projectiles.HardenedFistProj>();
				Item.shootSpeed = 35f;
            }
			return player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.HardenedSlam>()] < 1;
		}

        public override void AddRecipes()
		{
			Recipe hardenedgloverecipe = CreateRecipe();//the result is gigantsword
			hardenedgloverecipe.AddIngredient(ModContent.ItemType<Items.Weapons.FighterGlove>()); //Fighter Glove
			hardenedgloverecipe.AddIngredient(ItemID.Pwnhammer); //Pwnhammer
			hardenedgloverecipe.AddIngredient(ModContent.ItemType<Items.Starbit>(), 50); //50 starbits
			hardenedgloverecipe.AddIngredient(ModContent.ItemType<Items.RareStone>(), 2); //2 rare stones
			hardenedgloverecipe.AddTile(TileID.Anvils); //crafted at anvil
			hardenedgloverecipe.Register(); //adds this recipe to the game
		}
	}
}