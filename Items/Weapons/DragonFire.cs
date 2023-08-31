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
	public class DragonFire : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dragon Pot"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Unleashes a flurry of shadow fire" +
				"\nEnemies hit by one will be set on strong shadow flames"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
        }

		public override void SetDefaults()
		{
			Item.damage = 45;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.width = 56;
			Item.height = 50;
			Item.useTime = 4;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0f;
			Item.value = Item.buyPrice(0, 5, 50, 0);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item34;
			Item.autoReuse = true;
			//item.shoot = ModContent.ProjectileType<Projectiles.FireFire>();
			Item.shootSpeed = 16f; 
			Item.mana = 12;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

			Vector2 shootdir = Main.MouseWorld - player.Center; //distance 
			shootdir.Normalize();//reduce to 1
			shootdir *= 25f;//speed
			position = player.Center + shootdir / 2;//move 8 units away from player apon spawning

			//uses it two times for each supporting projectile
			Vector2 shootoffset1 = new Vector2(shootdir.X, shootdir.Y).RotatedBy(MathHelper.ToRadians(2)); // 2 degree offset
			Vector2 shootoffset2 = new Vector2(shootdir.X, shootdir.Y).RotatedBy(MathHelper.ToRadians(-2)); // -2 degree offset

			Projectile.NewProjectile(source, position.X, position.Y, shootdir.X, shootdir.Y, ModContent.ProjectileType<Projectiles.DragonFireFire>(), Item.damage, 0.2f, player.whoAmI);
			Projectile.NewProjectile(source, position.X, position.Y, shootoffset1.X, shootoffset1.Y, ModContent.ProjectileType<Projectiles.DragonFireFire>(), Item.damage, 0.2f, player.whoAmI);
			Projectile.NewProjectile(source, position.X, position.Y, shootoffset2.X, shootoffset2.Y, ModContent.ProjectileType<Projectiles.DragonFireFire>(), Item.damage, 0.2f, player.whoAmI);
			return false;
		}

        public override bool CanUseItem(Player player)
        {
			if (player.altFunctionUse == 2)
            {
				Item.useStyle = ItemUseStyleID.HoldUp;
				Item.shoot = ModContent.ProjectileType<Projectiles.FireSphere>();
				Item.shootSpeed = 0f;
			}
			else
			{
				Item.useStyle = ItemUseStyleID.Shoot;
				Item.shoot = ModContent.ProjectileType<Projectiles.FireFire>();
				Item.shootSpeed = 8f;
			}
			return player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.FireSphere>()] < 1;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-10, 0);
		}

		//Draw Flame
		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Texture2D texture = ModContent.Request<Texture2D>("KirboMod/Items/Weapons/DragonFire_Glowmask").Value; //GlowMask (flame)

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

		public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe();//the result is dragonfire
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.VolcanoFire>()); //Volcano Fire
			recipe1.AddIngredient(ItemID.ApprenticeStaffT3); //Betsy's Wrath
			recipe1.AddIngredient(ItemID.LaserMachinegun); //Laser Machinegun
			recipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 100); //100 starbits
			recipe1.AddIngredient(ModContent.ItemType<Items.RareStone>(), 5); //5 rare stones
			recipe1.AddTile(TileID.MythrilAnvil); //crafted at mythril/orichalcum anvil
			recipe1.Register(); //adds this recipe to the game
		}
	}
}