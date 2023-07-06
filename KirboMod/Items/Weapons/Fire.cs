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
	public class Fire : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Fire Pot"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Sprays a flurry of fireballs" +
				"\nEnemies hit by one will catch on fire"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
        }

		public override void SetDefaults()
		{
			Item.damage = 16;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.width = 32;
			Item.height = 40;
			Item.useTime = 5;
			Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0.1f;
            Item.value = Item.buyPrice(0, 0, 4, 0);
            Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item34;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.FireFire>();
			Item.shootSpeed = 10f;
			Item.mana = 3;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(50)); // 50 degree spread.

			Vector2 shootdir = Main.MouseWorld - player.Center; //distance 
			shootdir.Normalize();//reduce to 1
			shootdir *= 8f;//speed
			position = player.Center + shootdir * 4;//move from player apon spawning

			velocity.X = perturbedSpeed.X;
			velocity.Y = perturbedSpeed.Y;
		}

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>("KirboMod/Items/Weapons/Fire_Glowmask").Value; //Glowmask

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