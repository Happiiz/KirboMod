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

		public override void SetDefaults()
		{
			Item.damage = 76;
			Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.width = 40; //world dimensions
			Item.height = 40; //world dimensions
			Item.useTime = 24;
			Item.useAnimation = 24;
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
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            if (player.altFunctionUse == 2) //right click
            {
                if (type == ModContent.ProjectileType<Projectiles.MetalUppercut>()) //uppercut
                {
                    damage += damage * player.GetModPlayer<KirbPlayer>().fighterComboCounter; //do much more damage

                    if (damage >= Item.damage * 100) //cap
                    {
                        damage = Item.damage * 100;
                    }

                    position.X += player.direction * 50;
                    position.Y += -60;
                    knockback = 24;

                    player.GetModPlayer<KirbPlayer>().fighterComboCounter = 0; //reset combo counter

                    player.velocity.Y = -12; //launch player (put here because it lets player uppercut again when holding down mouse instead of slam
                }
                else
                {
                    damage *= 3;
                }

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
				Item.shootSpeed = 0.0001f; //make it very small but not immobile

				if (player.controlUp) //hold up 
				{
                    Item.useTime = 60;
                    Item.useAnimation = 60;
                    Item.shoot = ModContent.ProjectileType<Projectiles.MetalUppercut>();
					Item.useStyle = ItemUseStyleID.HoldUp;

					player.mount.Dismount(player); //dismount mounts
				}
				else
				{
                    Item.useTime = 12;
                    Item.useAnimation = 12;
                    Item.shoot = ModContent.ProjectileType<Projectiles.MetalSlam>();
					Item.useStyle = ItemUseStyleID.Swing;
				}
				return player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.MetalSlam>()] < 1;
			}
			else //left click
			{
				Item.useTime = 5;
				Item.useAnimation = 5;
				Item.shoot = ModContent.ProjectileType<Projectiles.MetalFistProj>();
				Item.shootSpeed = 45f;
                Item.useStyle = ItemUseStyleID.Swing;

                return player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.MetalSlam>()] < 1;
			}
		}

        public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe();//the result is Meta Knight sword
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.HardenedFighter>()); //Hardened Glove
			recipe1.AddIngredient(ItemID.PaladinsHammer); //Paladin's Hammer
			recipe1.AddIngredient(ItemID.GolemFist); //Golem Fist
			recipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 100); //100 starbits
			recipe1.AddIngredient(ModContent.ItemType<Items.RareStone>(), 2); //2 rare stones
			recipe1.AddTile(TileID.MythrilAnvil); //crafted at mythril/orichalcum anvil
			recipe1.Register(); //adds this recipe to the game
		}
	}
}