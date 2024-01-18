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
			Item.damage = 21;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.width = 32;
			Item.height = 40;
			Item.useTime = 3;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0.1f;
			Item.value = Item.buyPrice(0, 0, 4, 0);
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item24; //spectre boots
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.IceIce>();
			Item.shootSpeed = 10f;
			Item.mana = 3;
		}

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 rotationoffset = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(50)); //50 degree spread for dusts

            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(50)); //50 degree spread for proj too

            //do it before setting velocity to perturbed speed
            Dust.NewDustPerfect(player.Center, ModContent.DustType<Dusts.Flake>(), rotationoffset);

        }
    }
}