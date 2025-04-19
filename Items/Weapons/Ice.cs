using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class Ice : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Ice Pot"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Sprays a flurry of snowballs" +
				"\nEnemies killed by one condense into an ice cube" +
				"\nDoes not work on bosses"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.damage = 11;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.width = 32;
			Item.height = 40;
			Item.useTime = 13;
			Item.useAnimation = Item.useTime;
            Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2f;
			Item.value = Item.buyPrice(0, 0, 4, 0);
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item24; //spectre boots
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.IceIce>();
			Item.shootSpeed = 16f;
			Item.mana = 14;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 rotationoffset = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(20)); //20 degree spread for dusts

            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(30)); //30 degree spread for proj too

            Dust.NewDustPerfect(player.Center, ModContent.DustType<Dusts.Flake>(), rotationoffset);

        }
    }
}