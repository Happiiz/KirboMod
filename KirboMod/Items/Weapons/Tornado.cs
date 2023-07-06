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
			Item.damage = 12;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.width = 25;
			Item.height = 30;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.knockBack = 2f;
			Item.value = Item.buyPrice(0, 0, 8, 0);
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item34;
			Item.autoReuse = false;
			Item.shoot = ModContent.ProjectileType<Projectiles.TornadoNado>();
			Item.shootSpeed = 14f;
			Item.mana = 5;
            Item.channel = true;
        }

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            position.Y -= 50f; //spawn above player
        }

        public override bool CanUseItem(Player player)
        {
             return player.ownedProjectileCounts[Item.shoot] < 1;
        }
    }
}