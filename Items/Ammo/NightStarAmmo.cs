using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Ammo
{
	public class NightStarAmmo : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Star Bullet");
			// Tooltip.SetDefault("Gracefully flies through the air");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 20; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.maxStack = 9999;
			Item.consumable = true;
            Item.value = Item.buyPrice(0, 0, 5, 0);
            Item.rare = ItemRarityID.Pink; 
		    Item.ammo = AmmoID.FallenStar;
			Item.DamageType = DamageClass.Ranged;
			Item.damage = 35;
			Item.shoot = ModContent.ProjectileType<Projectiles.CannonNightStar>();
			Item.shootSpeed = 20f;
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(20);
			recipe.AddIngredient(ModContent.ItemType<NightCloth>(), 1);
			recipe.AddIngredient(ItemID.FallenStar, 5);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}
