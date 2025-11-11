using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class Staff : ModItem
	{
		public override void SetStaticDefaults() 
		{
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() 
		{
			Item.damage = 52;
			Item.knockBack = 2;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useAnimation = Item.useTime = 10;
			Item.DamageType = DamageClass.MeleeNoSpeed;
			Item.width = 62;
			Item.height = 62;
			Item.value = Item.buyPrice(0, 0, 25, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = false;
			Item.shootSpeed = 3f;
			Item.channel = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.Staffproj>();
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.ArmorPenetration = 20;
		}

        /*public override void AddRecipes()
        {
			Recipe recipe = CreateRecipe()
				.AddIngredient(ModContent.ItemType<DreamEssence>(), 20)
				.AddIngredient(ItemID.SoulofNight, 15)
				.AddIngredient(ItemID.HallowedBar, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }*/
    }
}