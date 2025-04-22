using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class OrnateChest : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Ornate Chest"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Contains dark secrets" +
				"\nUse to unleash the dark fragments within"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.damage = 23;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.width = 19;
			Item.height = 19;
			Item.useTime = 5;
			Item.useAnimation = 5;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.knockBack = 2f;
			Item.value = Item.buyPrice(0, 0, 2, 20);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.CoinPickup;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.NebulaStar>();
			Item.shootSpeed = 8f;
			Item.mana = 2;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			position.Y -= 14;
			position.X += player.direction * 14;

			velocity = velocity.RotatedByRandom(1f);

			velocity *= Main.rand.NextFloat(0.75f, 1.25f); //increase speed by a random amount
        }
    }
}