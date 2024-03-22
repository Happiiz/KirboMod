using KirboMod.Projectiles.VolcanoFire;
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
			Item.shoot = ModContent.ProjectileType<VolcanoFireFire1>();
			Item.shootSpeed = 10;//1 extraupdate
			Item.mana = 6;
		}

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
			type = VolcanoFireFire1.RandomType;
			Utils.ChaseResults results = Utils.GetChaseResults(position, velocity.Length(), Main.MouseWorld, default);
			velocity = Utils.FactorAcceleration(velocity, results.InterceptionTime, new Vector2(0, 0.12f), 0);
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(10)); // 10 degree spread
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