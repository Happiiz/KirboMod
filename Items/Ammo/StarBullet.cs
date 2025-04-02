using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Ammo
{
	public class StarBullet : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Star Bullet");
			// Tooltip.SetDefault("Gracefully flies through the air");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.maxStack = 9999;
			Item.consumable = true; //You need to set the item consumable so that the ammo would automatically consumed
            Item.value = Item.buyPrice(0, 0, 2, 0);
            Item.rare = ItemRarityID.White; 
		    Item.ammo = AmmoID.Bullet;
			Item.DamageType = DamageClass.Ranged;
			Item.damage = 12;
			Item.shoot = ModContent.ProjectileType<Projectiles.StarBulletProj>();
			Item.shootSpeed = 3f;
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(10);
			recipe.AddIngredient(ModContent.ItemType<Items.Starbit>(), 15);
			recipe.AddIngredient(ItemID.MusketBall, 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
