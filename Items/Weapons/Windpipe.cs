using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class WindPipe : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Wind Pipe"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Shoots whisps that explode into smaller whisps");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.damage = 18;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.width = 30;
			Item.height = 20;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.knockBack = 4f;
			Item.value = 516;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item63; //blowpipe
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.GoodWhisp>();
			Item.shootSpeed = 5f;
			Item.mana = 8;
		}
		//makes it shoot in front of the player's direction

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            position.X = player.Center.X;
            position.Y = player.Center.Y - 25f;
        }
	}
}