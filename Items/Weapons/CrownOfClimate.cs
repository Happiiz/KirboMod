using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class CrownOfClimate : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Crown Of Climate");
			// Tooltip.SetDefault("Summons a burning leo and chilly combo to fight for you");
			ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the player target anywhere on the whole screen while using a controller.
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
        }

		public override void SetDefaults()
		{
			Item.damage = 8;
			Item.knockBack = 2;
			Item.mana = 8;
			Item.width = 30; //for world hitbox
			Item.height = 18; //for world hitbox
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.value = Item.buyPrice(0, 0, 15, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item44;

            // These below are needed for a minion weapon
            Item.noMelee = true;
			Item.DamageType = DamageClass.Summon;
			Item.buffType = ModContent.BuffType<Buffs.MinionBuffs.LeoAndChillyBuff>();
			// No buffTime because otherwise the item tooltip would say something like "1 minute duration"

			
			Item.shoot = ModContent.ProjectileType<Projectiles.DuoBurningLeoMinion>();
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            // Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position.
            position = Main.MouseWorld; //mouse location
        }

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
			player.AddBuff(Item.buffType, 2);

            //spawns minion with scaled damaged automatically
            player.SpawnMinionOnCursor(source, player.whoAmI, Item.shoot, Item.damage, knockback);
            //spawns minion with scaled damaged automatically
            player.SpawnMinionOnCursor(source, player.whoAmI, ModContent.ProjectileType<Projectiles.DuoChillyMinion>(), Item.damage, knockback);

            return false;
		}

        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Starbit>(), 20);
            recipe.AddIngredient(ModContent.ItemType<DreamEssence>(), 20);
			recipe.AddIngredient(ModContent.ItemType<CrownOfFire>());
            recipe.AddIngredient(ModContent.ItemType<CrownOfIce>());
            recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}