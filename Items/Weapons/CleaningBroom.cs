using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using SoundEngine = Terraria.Audio.SoundEngine;
using Terraria.GameContent.Creative;

namespace KirboMod.Items.Weapons
{
	public class CleaningBroom : ModItem
	{
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Cleaning Broom");
			/* Tooltip.SetDefault("Swing on the ground to kick up dust at your enemies" +
                "\n'Sweep, sweep, sweep!'"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
        }

		public override void SetDefaults() 
		{
			Item.damage = 10;
			Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.width = 20;
			Item.height = 20;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 4;
			Item.value = Item.buyPrice(0, 0, 0, 20);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.CleaningBroomDustCloud>();
			Item.shootSpeed = 3f;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false; //no vanilla shooting
        }

		public override void HoldItem(Player player)
		{
            //swing only at last quarter while on ground and using item
            if (player.itemTime == player.itemTimeMax / 4 && player.velocity.Y == 0 && !player.ItemTimeIsZero) 
			{
                Projectile.NewProjectile(new EntitySource_ItemOpen(Main.player[player.whoAmI], Item.type), player.Center.X + (10 * player.direction), player.Center.Y - 10, player.direction * 3, 0,
                    Item.shoot, Item.damage, 2, player.whoAmI);
				SoundEngine.PlaySound(SoundID.Grass, player.Center);
            }
        }
	}
}