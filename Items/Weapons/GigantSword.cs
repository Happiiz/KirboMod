using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class GigantSword : ModItem
	{
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Gigant Sword"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() 
		{
			Item.damage = 82;
			Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.width = 78;
			Item.height = 78;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 9;
			Item.value = Item.buyPrice(0, 0, 45, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item1.WithPitchOffset(-0.5f); //lower pitch to emphasis power
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.GigantSlash>();
			Item.shootSpeed = 60;
		}

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
			if (player.velocity.Y == 0)
			{
				velocity *= 1.5f ; //go 60 units per tick in shoot direction
				damage = (int)(damage * 1.5);
            }
        }

        public override void AddRecipes()
		{
			Recipe gigantsword = CreateRecipe();//the result is gigantsword
            gigantsword.AddIngredient(ModContent.ItemType<HeroSword>()); //Hero Sword
            gigantsword.AddIngredient(ItemID.BreakerBlade); //Breaker Blade
            gigantsword.AddIngredient(ModContent.ItemType<Starbit>(), 25); //25 starbits
			gigantsword.AddIngredient(ModContent.ItemType<RareStone>(), 1); //1 rare stone
			gigantsword.AddTile(TileID.Anvils); //crafted at anvil
			gigantsword.Register(); //adds this recipe to the game
		}
    }
}