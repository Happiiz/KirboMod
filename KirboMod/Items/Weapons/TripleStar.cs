using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class TripleStar : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Triple Star Rod"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Shoots out three bouncing stars at once");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

		public override void SetDefaults()
		{
			Item.damage = 172;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 8;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 22; 
			Item.useAnimation = 22; 
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = Item.buyPrice(0, 3, 30, 30);
			Item.rare = ItemRarityID.Yellow;
			Item.autoReuse = true;
			Item.UseSound = SoundID.Item1;
			Item.scale = 1f;
			Item.shoot = ModContent.ProjectileType<Projectiles.TripleStarStar>();
			Item.shootSpeed = 32f;
			Item.crit += 24; //same as star rod

		}

        public override bool AltFunctionUse(Player player)
        {
            return true; //can right click
        }

        public override bool CanUseItem(Player player)
        {
			if (player.altFunctionUse == 2) //right click
			{
                return player.ownedProjectileCounts[Item.shoot] < 1; //has to be none out
            }
			else
			{
				return player.ownedProjectileCounts[Item.shoot] < 3; //only three at a time
			}
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
			position.Y -= 10; //move up slightly

			if (player.altFunctionUse == 2) //right click
			{
				velocity *= 1.2f;

				player.CheckMana(32, true); // + 8 is 40 mana
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2) //right click
            {
				Vector2 velocity2 = velocity.RotatedBy(MathHelper.ToRadians(5));
                Vector2 velocity3 = velocity.RotatedBy(MathHelper.ToRadians(-5));

				Projectile.NewProjectile(source, position, velocity2, type, damage, knockback);
                Projectile.NewProjectile(source, position, velocity3, type, damage, knockback);
            }

            return true;
        }

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}

		public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe();//the result is gigantsword
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.StarRod>()); //Star Rod
			recipe1.AddIngredient(ItemID.HallowedBar, 20); //20 hallowed bars
            recipe1.AddIngredient(ModContent.ItemType<HeartMatter>(), 5); //5 Heart Matter
            recipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 100); //100 starbits
			recipe1.AddIngredient(ModContent.ItemType<Items.DreamEssence>(), 50); //50 dream matter
			recipe1.AddIngredient(ModContent.ItemType<Items.RareStone>(), 5); //5 rare stones
			recipe1.AddTile(TileID.MythrilAnvil); //crafted at hardmode anvil
			recipe1.Register(); //adds this recipe to the game
		}
	}
}