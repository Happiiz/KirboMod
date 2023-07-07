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
	public class SpaceRangerGun : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Space Blaster"); // display name
			/* Tooltip.SetDefault("Converts all bullets into plasma shots" + //first line
				"\nConverts star bullets into condensed balls of electricity"); */ //second line
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.damage = 95;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.width = 102;
			Item.height = 46;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 8;
            Item.value = Item.buyPrice(0, 5, 50, 0);
            Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item75; //pulse bow
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.SpaceRangerBlast>(); //doesn't matter
			Item.shootSpeed = 16f;
			Item.useAmmo = AmmoID.Bullet;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            Vector2 shootdir = Main.MouseWorld - player.Center; //distance 
            shootdir.Normalize();//reduce to 1
            shootdir *= 40f; //how far
            position = player.Center + shootdir;//move from player apon spawning


            if (type != ModContent.ProjectileType<Projectiles.StarBulletProj>()) //if not star bullet
			{
				type = ModContent.ProjectileType<Projectiles.SpaceRangerBlast>();
			}
			else
			{
				type = ModContent.ProjectileType<Projectiles.SpaceRangerOrb>();
				damage *= 4; //moar damage
                knockback = 0f;
				position.Y -= 15; //move slightly up a little
            }
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (Main.rand.Next(1, 100) <= 25) //25/100
            {
                return false; //don't consume
            }
            else
            {
                return true; //consume
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			return true;
		}

        // Help, my gun isn't being held at the handle! Adjust these 2 numbers until it looks right.
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override void AddRecipes()
		{
			//You can use both Zapinators!

			Recipe recipe1 = CreateRecipe();//the result is gigantsword
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.NobleRangerGun>()); //Noble ranger gun
			recipe1.AddIngredient(ItemID.ChlorophyteShotbow); //Chlorophyte Shotbow
			recipe1.AddIngredient(ItemID.ElectrosphereLauncher); //Electrosphere Launcher
			recipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 100); //100 starbits
			recipe1.AddIngredient(ModContent.ItemType<Items.RareStone>(), 5); //5 rare stones
			recipe1.AddTile(TileID.MythrilAnvil); //crafted at mythril/orichalcum anvil
			recipe1.Register(); //adds this recipe to the game
        }
	}
}