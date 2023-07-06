using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class DarkSword : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dark Sword"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Fire dark beams within a short range");
			Item.staff[Item.type] = true; //fire like staff (sword sprite but gun holding)
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
        }

		public override void SetDefaults()
		{
			Item.damage = 105;
			Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Shoot; //gun/staff
			Item.UseSound = SoundID.Item15; //phaseblade
			Item.knockBack = 9;
			Item.value = Item.buyPrice(0, 2, 75, 0);
			Item.rare = ItemRarityID.Yellow;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.GoodDarkBeam>();
			Item.shootSpeed = 48f;
			Item.noMelee = true;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			return true;
        }
        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (Main.rand.NextBool(6))
			{
				//Emit dusts when the sword is swung
				Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<Dusts.DarkResidue>(), 0, 0, 0, default, 0.5f);
			}
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();//the result is darksword
			recipe.AddIngredient(ItemID.Excalibur); //excalibur
			recipe.AddIngredient(ModContent.ItemType<DarkMaterial>(), 15); //15 dark material
			recipe.AddTile(TileID.MythrilAnvil); //crafted at mythril anvil
			recipe.Register(); //adds this recipe to the game
		}
    }
}