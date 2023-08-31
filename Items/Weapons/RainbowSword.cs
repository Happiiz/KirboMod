using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class RainbowSword : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Rainbow Sword"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Holds the light to light up everyone's worlds" +
				"\nWell timed swings can deflect certain dark projectiles" +
                "\nRight click to shoot a non-damaging beam of light"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
		//todo: make able to receive legendary
			Item.damage = 300;//it has a relatively slow base use time so this compensates(+ also shorter range relatively)
			Item.DamageType = DamageClass.Melee; 
			Item.width = 40;
			Item.height = 40;
			Item.useTime = Item.useAnimation = 28; 
			Item.useStyle = ItemUseStyleID.Shoot; 
            Item.knockBack = 8;
			Item.value = Item.buyPrice(0, 15, 50, 0);
			Item.rare = ItemRarityID.Yellow;
			Item.autoReuse = false;
			Item.shoot = ModContent.ProjectileType<Items.RainbowSword.RainbowSwordHeld>();
			Item.noMelee = true; //hitbox reserved for swing and beam
            Item.shootsEveryUse = true;
			Item.channel = true;
			Item.useTurn = true;
			Item.shootSpeed = 1;
			Item.noUseGraphic = true;
			Item.autoReuse = true;
			Item.UseSound = SoundID.Item74 with { MaxInstances = 0, Volume = 0.6f };
		
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ModContent.ProjectileType<Projectiles.RainbowSwordBeam>())
			{
				damage = 0;
			}
			else //slash
			{
				//velocity = new Vector2(player.direction, 0f); //for facing the right direction
				//position = player.MountedCenter + new Vector2(player.direction * 10, 0);
            }
        }

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}


		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if(Main.myPlayer != player.whoAmI)
			return false;
			KirbPlayer mPlayer = player.GetModPlayer<KirbPlayer>();									//todo account for melee speed
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, player.itemAnimationMax, MathHelper.Lerp(6, 4, Main.rand.NextFloat()), mPlayer.NextRainbowSwordSwingDirection);
			return false;
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            /*if (player.altFunctionUse != 2)
            {
				//used to make it rotate around the player when swung
				player.itemRotation = player.direction * (MathHelper.ToRadians(-110) + ((player.itemTimeMax - player.itemTime) * MathHelper.ToRadians(31)));
            }*/
        }

        public override bool AltFunctionUse(Player player)
		{
			return false; //you can right click with this item
		}
		public override bool CanUseItem(Player player)
		{
			return true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Items.RainbowDrops.SnowDrop>());
			recipe.AddIngredient(ModContent.ItemType<Items.RainbowDrops.JungleDrop>());
			recipe.AddIngredient(ModContent.ItemType<Items.RainbowDrops.DesertDrop>());
			recipe.AddIngredient(ModContent.ItemType<Items.RainbowDrops.EvilDrop>());
			recipe.AddIngredient(ModContent.ItemType<Items.RainbowDrops.HellDrop>());
			recipe.AddIngredient(ModContent.ItemType<Items.RainbowDrops.OceanDrop>());
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}