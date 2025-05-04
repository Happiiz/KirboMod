using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class SwayingBranch : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Twitchy Twig");
			/* Tooltip.SetDefault("Summons a whispy woods sentry to spew apples at your enemies" +
				"\nApples will momentarily spawn weak"); */
			ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the player target anywhere on the whole screen while using a controller.
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.damage = 17;
			Item.knockBack = 1;
			Item.mana = 15;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.buyPrice(0, 0, 8, 5);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item44;

			// These below are needed for a minion weapon
			Item.noMelee = true;
			Item.DamageType = DamageClass.Summon;
			//item.buffType = ModContent.BuffType<Buffs.ParosolBuff>(); //its a sentry sooo
			// No buffTime because otherwise the item tooltip would say something like "1 minute duration"
			Item.shoot = ModContent.ProjectileType<Projectiles.MiniWhispy>();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			player.FindSentryRestingSpot(type, out int worldX, out int worldY, out int pushYUp); //This returns the coordinates for the sentry to be placed on solid ground below the cursor position
			//adjust pushYUp here depending on how high your projectile hitbox is
			pushYUp = 71;
			type = ModContent.ProjectileType<Projectiles.MiniWhispy>();
			//damage = Item.damage;
			//knockback = Item.knockBack;
            int index = Projectile.NewProjectile(source, worldX, worldY - pushYUp, 0f, 0f, type, damage, knockback, Main.myPlayer);
            Main.projectile[index].originalDamage = damage;
            player.UpdateMaxTurrets(); //This despawns old sentries so spawning new ones will not go above player.maxTurrets


			return false;
		}
	}
}