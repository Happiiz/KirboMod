using KirboMod.NPCs.NewWhispy;
using KirboMod.Projectiles;
using KirboMod.Projectiles.NewWhispy.NewWhispyWind;
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
			Item.damage = 10;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.width = 30;
			Item.height = 20;
			Item.useTime = 60;
			Item.useAnimation = Item.useTime;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.knockBack = 2f;
			Item.value = 516;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = NewWhispyBoss.AirShotSFX; //blowpipe
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.GoodWhisp>();
			Item.shootSpeed = 15f;
			Item.mana = 10;
		}
		public static int SplitProjCount => 6;
		public static float SplitRadius => 350f;
		public static int SplitProjDuration => 120;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if(Main.myPlayer == player.whoAmI)
			{
				NewWhispySplittingWind.GetAIValues(position, Main.MouseWorld, velocity.Length(), out float ai0, out Vector2 projVelocity);
				Projectile.NewProjectile(source, position, projVelocity, type, damage, knockback, player.whoAmI, ai0);
            }
			return false;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            position.X = player.Center.X;
            position.Y = player.Center.Y - 25f;
        }
	}
}