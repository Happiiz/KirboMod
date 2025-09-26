using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class BomberRocket : ModItem
	{
		public override void SetStaticDefaults()
        {
            ItemID.Sets.IsRangedSpecialistWeapon[Type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.width = 10;
			Item.height = 10;
			Item.maxStack = 9999;
			Item.consumable = true;
			Item.value = Item.buyPrice(0, 0, 30, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.DamageType = DamageClass.Ranged;
			Item.useTime = 16;
            Item.useAnimation = Item.useTime;
			Item.useStyle = ItemUseStyleID.Swing;
            Item.damage = 800;
			Item.knockBack = 12;
			Item.shoot = ModContent.ProjectileType<Projectiles.BomberRocketProj>();
			Item.shootSpeed = 15f;
			Item.noMelee = true;
			Item.noUseGraphic = true;
		}
	}
}
