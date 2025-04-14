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
	public class HardenedFighter : ModItem
	{
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Hardened Fighter Glove");
			/* Tooltip.SetDefault("Right click to slam the ground and shoot rocks!" +
				"\nSlam onto enemies to boost yourself"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
        }
		static int UseTime => 5;
		static float ShootSpeed => 35f;
		public override void SetDefaults() 
		{
			Item.damage = 18;
			Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.width = 30; //world dimensions
			Item.height = 30; //world dimensions
			Item.useTime = UseTime;
			Item.useAnimation = UseTime;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.knockBack = 1;
			Item.useStyle = ItemUseStyleID.Rapier;
			Item.value = Item.buyPrice(0, 0, 45, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.HardenedFistProj>();
			Item.shootSpeed = ShootSpeed;
			Item.ArmorPenetration = 15;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            KirbPlayer kplr = player.GetModPlayer<KirbPlayer>();
            if (player.altFunctionUse == 2)
            {
                if (Main.myPlayer == player.whoAmI)
                {
                    FighterUppercut.GetAIValues(player, 0.5f, out float ai1);
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
                damage = FighterGlove.GetDamageScaledByComboCounter(player, damage, 0.5f);
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
				Item.useTime = 50; 
				Item.useAnimation = 50;
				Item.shoot = ModContent.ProjectileType<Projectiles.HardenedFighterUppercut>();
				Item.shootSpeed = 0.0001f; //make it very small but not immobile
                Item.useStyle = ItemUseStyleID.HoldUp;
            }
            else //left click
			{
				Item.useTime = 5;
				Item.useAnimation = 5;
				Item.shoot = ModContent.ProjectileType<Projectiles.HardenedFistProj>();
				Item.shootSpeed = ShootSpeed;
                Item.useStyle = ItemUseStyleID.Rapier;
				Item.autoReuse = true;
            }
            return true;
		}

        public override void AddRecipes()
		{
			Recipe hardenedgloverecipe = CreateRecipe();//the result is gigantsword
			hardenedgloverecipe.AddIngredient(ModContent.ItemType<Items.Weapons.FighterGlove>()); //Fighter Glove
			hardenedgloverecipe.AddIngredient(ItemID.Pwnhammer); //Pwnhammer
			hardenedgloverecipe.AddIngredient(ModContent.ItemType<Items.Starbit>(), 50); //50 starbits
			hardenedgloverecipe.AddIngredient(ModContent.ItemType<RareStone>(), 1); //1 rare stone
			hardenedgloverecipe.AddTile(TileID.Anvils); //crafted at anvil
			hardenedgloverecipe.Register(); //adds this recipe to the game
		}
	}
}