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
	public class HunterArcherBow : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Tri-Killer"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Converts all arrows into three jester arrows" +
				"\nConverts star arrows into five charged variants"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
        }

		public override void SetDefaults()
		{
			Item.damage = 17;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.width = 48;
			Item.height = 62;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4;
			Item.value = Item.buyPrice(0, 0, 45, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item5; //bow shot
			Item.autoReuse = true;
			Item.shoot = ItemID.JestersArrow;
			Item.shootSpeed = 10f;
			Item.useAmmo = AmmoID.Arrow;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{

			if (type != ModContent.ProjectileType<Projectiles.StarArrowProj>())
			{
				type = ProjectileID.JestersArrow;
				if (ContentSamples.ProjectilesByType[type].localNPCHitCooldown == -2 || !ContentSamples.ProjectilesByType[type].usesLocalNPCImmunity)
				{
					//check if uses local because of some other mod
					damage *= 3;//compensate for static/global immunity because relogic is STUPID
				}
			}
			else
			{
				type = ModContent.ProjectileType<Projectiles.ChargedArrowProj>();
				damage = (int)(damage * (3f/5f) * 1.2f);
				velocity *= 1.2f;
			}
        }
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			Vector2 projshoot = Main.MouseWorld - player.Center;
			projshoot.Normalize();
            bool useChargedArrows = type == ModContent.ProjectileType<Projectiles.ChargedArrowProj>();
            int arrowCount = useChargedArrows ? 5 : 3;
            float totalSpread = 0.2f;
            for (int i = 0; i < arrowCount; i++)
            {
                float angle = Utils.Remap(i, 0, arrowCount - 1, -totalSpread / 2f, totalSpread / 2f);
                Vector2 vel = velocity.RotatedBy(angle);
                Projectile.NewProjectile(source, position, vel, type, damage, knockback, Item.playerIndexTheItemIsReservedFor);
            }
			return false;
		}

		public override void AddRecipes()
		{
			Recipe hunterArcherBow1 = CreateRecipe();//the result is tri-kill bow
            hunterArcherBow1.AddIngredient(ModContent.ItemType<Items.Weapons.ArcherBow>()); //Archer Bow
            hunterArcherBow1.AddIngredient(ItemID.TitaniumRepeater); //Titanium Repeater
            hunterArcherBow1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 50); //50 starbits
            hunterArcherBow1.AddIngredient(ModContent.ItemType<RareStone>(), 1); //1 rare stone
            hunterArcherBow1.AddTile(TileID.Anvils); //crafted at anvil
            hunterArcherBow1.Register(); //adds this recipe to the game

            Recipe hunterArcherBow2 = CreateRecipe();//the result is tri-kill bow
            hunterArcherBow2.AddIngredient(ModContent.ItemType<Items.Weapons.ArcherBow>()); //Archer Bow
            hunterArcherBow2.AddIngredient(ItemID.AdamantiteRepeater); //Adamantite Repeater
            hunterArcherBow2.AddIngredient(ModContent.ItemType<Items.Starbit>(), 50); //50 starbits
            hunterArcherBow2.AddIngredient(ModContent.ItemType<RareStone>(), 1); //1 rare stone
            hunterArcherBow2.AddTile(TileID.Anvils); //crafted at anvil
            hunterArcherBow2.Register(); //adds this recipe to the game
        }
	}
}