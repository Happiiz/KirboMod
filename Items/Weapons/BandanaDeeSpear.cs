using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class BandanaDeeSpear : ModItem
	{
		public override void SetStaticDefaults()
		{
			ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the player target anywhere on the whole screen while using a controller.
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.damage = 13;
			Item.knockBack = 4f;
			Item.mana = 7;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 36;
			Item.useAnimation = 24;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.buyPrice(0, 1, 20, 0);
			Item.rare = ItemRarityID.Lime;
			Item.UseSound = SoundID.Item44;

			// These below are needed for a minion weapon
			Item.noMelee = true;
			Item.DamageType = DamageClass.Summon;
			Item.buffType = ModContent.BuffType<Buffs.MinionBuffs.BandanaDeeBuff>();
			Item.shoot = ModContent.ProjectileType<Projectiles.BandanaDee.BandanaWaddleDee>();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			player.AddBuff(Item.buffType, 2);

			position = Main.MouseWorld; //mouse location

            player.SpawnMinionOnCursor(source, player.whoAmI, Item.shoot, Item.damage, knockback);
            return false;
		}

		public override void AddRecipes()
		{
			Recipe bandanaSpear = CreateRecipe();
            bandanaSpear.AddIngredient(ModContent.ItemType<Parosol>());
            bandanaSpear.AddIngredient(ModContent.ItemType<DooStaff>());
            bandanaSpear.AddIngredient(ItemID.SoulofSight, 1);
            bandanaSpear.AddIngredient(ItemID.SoulofMight, 1);
            bandanaSpear.AddIngredient(ItemID.SoulofFright, 1);
            bandanaSpear.AddIngredient(ModContent.ItemType<HeartMatter>(), 3);
            bandanaSpear.AddTile(TileID.MythrilAnvil);
            bandanaSpear.Register();
		}
	}
}