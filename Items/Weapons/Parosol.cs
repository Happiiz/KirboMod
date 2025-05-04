using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class Parosol : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Parasol");
			// Tooltip.SetDefault("Summons a parasol waddle dee to fight for you");
			ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the player target anywhere on the whole screen while using a controller.
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.damage = 7;
			Item.knockBack = 3f;
			Item.mana = 8;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.buyPrice(0, 0, 0, 20);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item44;

			// These below are needed for a minion weapon
			Item.noMelee = true;
			Item.DamageType = DamageClass.Summon;
			Item.buffType = ModContent.BuffType<Buffs.MinionBuffs.ParosolBuff>();
			// No buffTime because otherwise the item tooltip would say something like "1 minute duration"
			Item.shoot = ModContent.ProjectileType<Projectiles.ParosolMinion>();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
			player.AddBuff(Item.buffType, 2);

			// Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position.
			position = Main.MouseWorld; //mouse location

			//spawns minion with scaled damaged automatically
            player.SpawnMinionOnCursor(source, player.whoAmI, Item.shoot, Item.damage, knockback);
            return false;
		}

		public override void AddRecipes()
		{
			Recipe parasol = CreateRecipe();
            parasol.AddIngredient(ModContent.ItemType<Starbit>(), 10);
            parasol.AddIngredient(ItemID.Umbrella);
            parasol.AddTile(TileID.Anvils);
            parasol.Register();
		}
	}
}