using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class CrystalGun : ModItem
	{

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Crystal Gun"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Consumes magic crystal shards" +
				"\n75% chance to not consume ammo"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
        }

		public override void SetDefaults()
		{
			Item.damage = 215;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.width = 80;
			Item.height = 62;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
            Item.value = Item.buyPrice(0, 25, 0, 0);
            Item.rare = ItemRarityID.Red;
			Item.UseSound = SoundID.Item11; //gunshot
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.CrystalShardProj>();
			Item.shootSpeed = 20f;
			Item.alpha = 50;
			Item.useAmmo = ModContent.ItemType<CrystalShard>(); //use this ammo group
		}

        public override bool CanUseItem(Player player)
        {
			return true;
        }

		public override void AddRecipes()
		{
			Recipe crystalgunrecipe = CreateRecipe();
			crystalgunrecipe.AddIngredient(ModContent.ItemType<CrystalShard>(), 74);
			crystalgunrecipe.AddIngredient(ModContent.ItemType<MiracleMatter>()); //Zero material drop
			crystalgunrecipe.AddTile(TileID.LunarCraftingStation); //Ancient Manipulator
			crystalgunrecipe.Register();
		}

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (Main.rand.Next(1, 100) <= 75) //75/100
            {
				return false; //don't consume
            }
			else
            {
				return true; //consume
            }
        }

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}