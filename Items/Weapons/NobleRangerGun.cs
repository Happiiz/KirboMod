using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class NobleRangerGun : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Noble Fury"); // display name
			/* Tooltip.SetDefault("Shoots a flurry of star bubbles" + //first line
				"\nConverts star bullets into faster star bubbles"); */ //second line
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.damage = 55;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.width = 70;
			Item.height = 40;
			Item.useTime = 5; //make less than animation to attack again
			Item.useAnimation = 15;
			Item.reuseDelay = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 10;
            Item.value = Item.buyPrice(0, 0, 45, 0);
            Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item11; //basic gun shot
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.RangerStar>();
			Item.shootSpeed = 12f;
			Item.useAmmo = AmmoID.Bullet;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            if (type != ModContent.ProjectileType<Projectiles.StarBulletProj>()) //if not star bullet
            {
                type = ModContent.ProjectileType<Projectiles.RangerStar>();
            }

            position = player.Center + velocity * 3;//move from player apon spawning
        }

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if (type != ModContent.ProjectileType<Projectiles.StarBulletProj>()) //if not star bullet
			{
				return true;
			}
			else //multi ranger star
            {
				Projectile.NewProjectile(source, position, velocity * 2, ModContent.ProjectileType<Projectiles.PinkRangerStar>(), damage, 10, player.whoAmI, 0, 0);
				return false;
			}
		}

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return !(player.itemAnimation < Item.useAnimation - 2); //not lower than the first shot
        }

        public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe();//the result is noble fury gun
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.RangerGun>()); //Ranger Gun
			recipe1.AddIngredient(ItemID.ClockworkAssaultRifle); //Clockwork Assult Rifle
			recipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 50); //50 starbits
			recipe1.AddIngredient(ModContent.ItemType<RareStone>(), 1); //1 rare stone
			recipe1.AddTile(TileID.Anvils); //crafted at anvil
			recipe1.Register(); //adds this recipe to the game
		}
	}
}