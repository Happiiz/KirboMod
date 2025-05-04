using KirboMod.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class Tornado : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Tornado Ring"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Control a tornado by holding down the mouse");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.damage = 18;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.width = 25;
			Item.height = 30;
			Item.useTime = 90;
			Item.useAnimation = 90;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.knockBack = 2f;
			Item.value = Item.buyPrice(0, 0, 8, 0);
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item34;
			Item.autoReuse = false;
			Item.shoot = ModContent.ProjectileType<Projectiles.TornadoNado>();
			Item.shootSpeed = 14f;
			Item.mana = 10;
            Item.channel = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, Item.mana, 0);
            }
            return false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            position.Y -= 50f; //spawn above player
        }

        public override bool CanUseItem(Player player)
        {
             return player.ownedProjectileCounts[Item.shoot] < 1 && player.statMana >= player.statManaMax2;
        }
    }
}