using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class Blado : ModItem
	{
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Blado"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Lands on the ground and then spins forward");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99; //amount needed to research
        }

		public override void SetDefaults() 
		{
			Item.damage = 24;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 29;
			Item.height = 29;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
			Item.value = 40; //measured in copper coins
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.BladoProj>();
			Item.shootSpeed = 12f;
			Item.consumable = true;
			Item.maxStack = 9999;
			Item.noMelee = true;
			Item.noUseGraphic = true;
		}
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            position.X = player.Center.X;
            position.Y = player.Center.Y - 25f;
        }
    }
}