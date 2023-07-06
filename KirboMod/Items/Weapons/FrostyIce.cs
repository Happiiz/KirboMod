using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class FrostyIce : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Frosty Ice Pot"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Sprays a flurry of snowballs that form snowmen" +
				"\nEnemies killed by one condense into an ice cube" +
				"\nDoes not work on bosses"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
        }

		public override void SetDefaults()
		{
			Item.damage = 45;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.width = 36;
			Item.height = 40;
			Item.useTime = 3;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4f;
			Item.value = Item.buyPrice(0, 0, 45, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item24; //spectre boots
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.FrostyIceIce>(); 
			Item.shootSpeed = 12f;
			Item.mana = 6;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(30)); // 30 degree spread.

            Vector2 perturbedSpeed2 = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(50)); //50 degree spread for dusts

            //do it before setting velocity to perturbed speed
            Dust.NewDustPerfect(player.Center, ModContent.DustType<Dusts.Flake>(), perturbedSpeed2);

            velocity = perturbedSpeed;

            position += velocity * 2; //move a bit away from the player
        }

		public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe();//the result is volcanofire
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.Ice>()); //Ice
			recipe1.AddIngredient(ItemID.IceRod); //Ice Rod
			recipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 50); //50 starbits
			recipe1.AddIngredient(ModContent.ItemType<Items.RareStone>(), 2); //2 rare stones
			recipe1.AddTile(TileID.Anvils); //crafted at anvil
			recipe1.Register(); //adds this recipe to the game
		}
	}
}