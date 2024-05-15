using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class FighterGlove : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Fighter Glove"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Right click on the ground to do a uppercut, sending you upwards" +
				"\nPunching many times RIGHT before uppercutting will make it stronger"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
        }

		public override void SetDefaults()
		{
			Item.damage = 17;
			Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.width = 20; //world dimensions
			Item.height = 20;
			Item.useTime = 5;
			Item.useAnimation = 5;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.knockBack = 1;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.buyPrice(0, 0, 6, 30);
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.FlyingPunch>();
			Item.shootSpeed = 20f;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            if (player.altFunctionUse == 2) //right click
            {
                damage += damage * player.GetModPlayer<KirbPlayer>().fighterComboCounter / 2; //do more damage

                if (damage >= Item.damage * 100) //cap
                {
                    damage = Item.damage * 100;
                }

                position.X += player.direction * 40;
                position.Y += -30;
                knockback = 12;

                player.GetModPlayer<KirbPlayer>().fighterComboCounter = 0; //reset combo counter
            }
            else //left click
            {
				velocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
            }
        }

		public override bool AltFunctionUse(Player player)
		{
			return true; //can right click
		}

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
                Item.useTime = 50;
                Item.useAnimation = 50;
                Item.shoot = ModContent.ProjectileType<Projectiles.FighterUppercut>();
                Item.shootSpeed = 0.0001f; //make it very small but not immobile
                Item.useStyle = ItemUseStyleID.HoldUp;

                player.mount.Dismount(player); //dismount mounts
                player.velocity.Y = -10;

                return true; // usedUppercut = false; //checks if used uppercut is false	
            }
			else //left click
			{
				Item.useTime = 5;
				Item.useAnimation = 5;
				Item.shoot = ModContent.ProjectileType<Projectiles.FlyingPunch>();
				Item.shootSpeed = 20f;
				Item.useStyle = ItemUseStyleID.Swing;

				return true;
			}
		}
    }
}