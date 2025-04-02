using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class GordoItem : ModItem
	{
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Gordo"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Wears down the more hits it racks up" +
				"\n'Side b lol'"); */ //its a smash reference
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99; //amount needed to research 
        }

		public override void SetDefaults() 
		{
			Item.damage = 10;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 25;
			Item.height = 25;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5;
			Item.value = 20;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.GoodGordo>();
			Item.shootSpeed = 12f;
			Item.consumable = true;
			Item.maxStack = 9999;
			Item.noMelee = true;
			Item.noUseGraphic = true;
		}

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position.X = player.Center.X;
            position.Y = player.Center.Y - 25f;
        }
    }
}