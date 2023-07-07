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
	public class VolcanoFire : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Volcano Pot"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Sprays a shower of molten rocks" +
				"\nExplodes upon contact"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.damage = 35;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.width = 32;
			Item.height = 40;
			Item.useTime = 5;
			Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 7f;
			Item.value = Item.buyPrice(0, 0, 45, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item34;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.VolcanoFireFire>();
			Item.shootSpeed = 12f;
			Item.mana = 6;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(15)); // 15 degree spread.

			Vector2 shootdir = Main.MouseWorld - player.Center; //distance 
			shootdir.Normalize();//reduce to 1
			shootdir *= 8f;//speed
			position = player.Center + shootdir * 4;//move from player apon spawning

			Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y - 6, ModContent.ProjectileType<Projectiles.VolcanoFireFire>(), Item.damage, 0f, player.whoAmI);
			return false;
		}

		public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe();//the result is volcanofire
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.Fire>()); //Fire 
			recipe1.AddIngredient(ItemID.MeteorStaff); //Meteor Staff
			recipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 50); //50 starbits
			recipe1.AddIngredient(ModContent.ItemType<Items.RareStone>(), 2); //2 rare stones
			recipe1.AddTile(TileID.Anvils); //crafted at anvil
			recipe1.Register(); //adds this recipe to the game
		}

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>("KirboMod/Items/Weapons/VolcanoFire_Glowmask").Value; //Glowmask

            spriteBatch.Draw
            (
                texture,
                new Vector2
               (
                        Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                        Item.position.Y - Main.screenPosition.Y + Item.height - texture.Height * 0.5f
                ),
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                rotation,
                texture.Size() * 0.5f,
                1f, //size depends on size variable
                SpriteEffects.None,
                0f
            );
        }
    }
}