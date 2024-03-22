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
			 // DisplayName.SetDefault("Jamba Staff"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Mighty staff of the Jamba followers" +
				"\nRight click to swing it"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() 
		{
			Item.damage = 52;
			Item.knockBack = 2;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useAnimation = Item.useTime = 10;
			Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
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
    }
}