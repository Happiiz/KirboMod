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
			Item.useTime = 10;
			Item.useAnimation = 12; //starts retracting at 4
			Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.width = 120;
			Item.height = 120;
			Item.value = Item.buyPrice(0, 0, 25, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shootSpeed = 3f;
			//item.shoot = ModContent.ProjectileType<Projectiles.Staffproj>();
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.reuseDelay = 0;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

        public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				Item.useStyle = ItemUseStyleID.Swing;
				Item.useTime = 30;
				Item.useAnimation = 30;
				Item.damage = 104;
				Item.knockBack = 10;
				Item.noMelee = false;
				Item.noUseGraphic = false;
				Item.shoot = ProjectileID.None;
			}
			else
			{
				Item.knockBack = 2;
				Item.useStyle = ItemUseStyleID.Shoot;
				Item.useTime = 12;
				Item.useAnimation = 12; //starts retracting at 4
				Item.damage = 52;
				Item.shoot = ModContent.ProjectileType<Projectiles.Staffproj>();
				Item.noMelee = true;
				Item.noUseGraphic = true;
				Item.autoReuse = true;
			}
			//Ensures no more than one spear can be thrown out, use this when using autoReuse
			return player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Staffproj>()] < 1;
		}
    }
}