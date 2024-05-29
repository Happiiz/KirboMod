using KirboMod.Projectiles;
using KirboMod.Projectiles.SwordAuras;
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
    public class MetaKnightSword : ModItem
	{
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Galaxia"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("'Well of course not the actual thing...'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() 
		{
			Item.damage = 102;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = Item.buyPrice(0, 5, 50, 0);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<CresentSlash>();
			Item.shootSpeed = 100f;
			Item.shootsEveryUse = true;
			Item.noMelee = true; //no melee hitbox
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if (Main.myPlayer == player.whoAmI)
			{
				SwordAura.NewAura<MetaKnightSwing>(player, source, damage, knockback, Item);
				SwordSlash.NewSwordSlash<CresentSlash>(source, player, velocity, damage, knockback, .01f);
			}
			return false;
		}
        public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe();//the result is Meta Knight sword
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.GigantSword>()); //Gigant Sword
			recipe1.AddIngredient(ItemID.ChlorophyteClaymore); //Chlorophyte Claymore
			recipe1.AddIngredient(ItemID.InfluxWaver); //Influx Waver
			recipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 100); //100 starbits
			recipe1.AddIngredient(ModContent.ItemType<Items.RareStone>(), 2); //2 rare stones
			recipe1.AddTile(TileID.MythrilAnvil); //crafted at mythril/orichalcum anvil
			recipe1.Register(); //adds this recipe to the game
		}
    }
}