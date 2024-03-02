using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class DreamRod : ModItem
	{
		public override void SetStaticDefaults()
		{
			ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the player target anywhere on the whole screen while using a controller.
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
        }

		public override void SetDefaults()
		{
			Item.damage = 183;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Summon;
			Item.mana = 6;
			Item.width = 22; //make small for better world hitbox
			Item.height = 22;
			Item.useTime = 5;
			Item.useAnimation = 5;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.knockBack = 6;
            Item.value = Item.buyPrice(0, 25, 0, 0);
            Item.rare = ItemRarityID.Purple;
			Item.autoReuse = true;
			Item.UseSound = SoundID.Item9; //fallen star
			Item.scale = 1f;
			Item.shoot = ModContent.ProjectileType<Projectiles.DreamedFriend>();
			Item.shootSpeed = 25f;
			Item.crit += 24;
			Item.buffType = ModContent.BuffType<Buffs.HopesAndDreams>();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			player.AddBuff(ModContent.BuffType<Buffs.HopesAndDreams>(), 300); //give hopes and dreams buff

			return true;
		}

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            float spreadAngle = (120 - player.maxMinions * 20) < 20 ? 20 : 120 - player.maxMinions * 20f; //can't be lower than 20

			int damageMultiplier = 20 * player.maxMinions;

            damage += player.maxMinions < 8 ? 20 * player.maxMinions:
                20 * 8; //can only increase by 160 damage max

            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(spreadAngle)); // 120 degree spread max and 20 degree min 

            position = (player.Center + new Vector2(0, -5));//go sightly above player upon spawning
        }

        public override void HoldItem(Player player)
        {
            Item.shootSpeed = 25f + player.maxMinions * 2.5f; //increase
        }

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-30, 0); //-30 units x offset from holding position (doesn't work as of now)
        }

        public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe();//the result is dream rod
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.Parosol>()); //parosol dee
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.WeirdParosol>()); //gooey
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.ShinobiScroll>()); //bio spark
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.CrownOfClimate>()); //burning leo and chilly
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.DooStaff>()); //waddle doo
			recipe1.AddIngredient(ModContent.ItemType<MiracleMatter>()); //Zero material drop
			recipe1.AddTile(TileID.LunarCraftingStation); //crafted at ancient manipulator
			recipe1.Register(); //adds this recipe to the game
		}
	}
}