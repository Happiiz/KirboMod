using KirboMod.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class MetalFighter : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Metal Fighter Glove");
			/* Tooltip.SetDefault("Right click and hold up to combo uppercuts on the ground!" +
				"\nSpike in on the ground and air by just holding right click!"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }
		public static float ShootSpeed => 60f;
		public static int UseTime => 6;
		public override void SetDefaults()
		{
			Item.damage = 22;
			Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.width = 40; //world dimensions
			Item.height = 40; //world dimensions
			Item.useTime = UseTime;
			Item.useAnimation = UseTime;//should be the same
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.knockBack = 1;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.buyPrice(0, 5, 50, 0);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.MetalFistProj>();
			Item.shootSpeed = 50f;
			Item.ArmorPenetration = 36;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            KirbPlayer kplr = player.GetModPlayer<KirbPlayer>();
            if (player.altFunctionUse == 2)
            {
                if (Main.myPlayer == player.whoAmI)
                {
                    FighterUppercut.GetAIValues(player, 0.33f, out float ai1);
                    Projectile.NewProjectile(source, position, velocity, type, damage, knockback, -1, 0, ai1);
                }
                kplr.fighterComboCounter = 0;
                return false;
            }
            return true;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            if (player.altFunctionUse == 2) //right click
            {
				float scaling = player.controlDown ? 1f : 2f;
                damage = FighterGlove.GetDamageScaledByComboCounter(player, damage, scaling);
                position.X += player.direction * 40;
                position.Y += -30;
                knockback = 15;
            }
            else //left click
            {
				velocity = velocity.RotatedByRandom(MathHelper.ToRadians(5)) / ContentSamples.ProjectilesByType[type].MaxUpdates;
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
				Item.shootSpeed = 0.0001f; //make it very small but not immobile

				if (player.controlDown) //hold down 
				{
                    player.mount.Dismount(player);
                    Item.useTime = 50;
                    Item.useAnimation = Item.useTime;
                    Item.shoot = ModContent.ProjectileType<Projectiles.MetalKick>();
                    Item.useStyle = ItemUseStyleID.Shoot;
					player.GetModPlayer<KirbPlayer>().TriggerMetalKick();
                }
				else
                {
                    player.mount.Dismount(player);
                    Item.useTime = 60;
                    Item.useAnimation = 60;
                    Item.shoot = ModContent.ProjectileType<Projectiles.MetalUppercut>();
                    Item.useStyle = ItemUseStyleID.HoldUp;
				}
				return true;
			}
			else //left click
			{
				Item.useTime = UseTime;
				Item.useAnimation = UseTime;
				Item.shoot = ModContent.ProjectileType<Projectiles.MetalFistProj>();
				Item.shootSpeed = ShootSpeed;
                Item.useStyle = ItemUseStyleID.Rapier;

                return player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.MetalKick>()] < 1;
			}
		}

        public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe();
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.HardenedFighter>()); //Hardened Glove
			recipe1.AddIngredient(ItemID.PaladinsHammer); 
			recipe1.AddIngredient(ItemID.GolemFist); 
			recipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 100); 
			recipe1.AddIngredient(ModContent.ItemType<Items.RareStone>(), 2); 
			recipe1.AddTile(TileID.MythrilAnvil); //crafted at mythril/orichalcum anvil
			recipe1.Register(); //adds this recipe to the game
		}
	}
}