using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;

namespace KirboMod.Items.Weapons
{
	public class MaskedHammer : ModItem
	{
		const int chargeNeededForTornado = 120;

		int useCounter = 0;
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Wild Fire Hammer"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Hold left on the ground to swing fire tornados in the direction you're facing" +
				"\nHold right click to charge a firey spin" +
				"\nLeft click to release when at full power"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.damage = 128;
			Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.width = 40; //make it half to make world hitbox better
			Item.height = 40; //make it half to make world hitbox better
			Item.useTime = 8; //default only
			Item.useAnimation = 8; //default only
            Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 10;
			Item.value = Item.buyPrice( 0, 11, 0, 0);
			Item.rare = ItemRarityID.Red;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.noMelee = false;
			Item.noUseGraphic = false;
        }
		public override void HoldItem(Player player)
		{
			Item.noMelee = player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.MaskedFireTornado>()] > 0;
			KirbPlayer kplr = player.GetModPlayer<KirbPlayer>();
			if (kplr.RightClicking & player.ItemTimeIsZero) //holding up & not attacking
			{
				if(++kplr.hammerCharge >= chargeNeededForTornado)
				{
					kplr.hammerCharge = chargeNeededForTornado;
					for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
					{
						Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
						Dust d = Dust.NewDustPerfect(player.Center + Main.rand.NextVector2Circular(32,32), DustID.SolarFlare, speed * 5, Scale: 2f, newColor: Color.White); //Makes dust in a messy circle
						d.noGravity = true;
					}
				}
				
				player.velocity.X *= 0.99f; //slow
                for (int i = 0; i % 5 == 0; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
					Dust d = Dust.NewDustPerfect(player.Center, DustID.Smoke, speed * 5, Scale: 2f, newColor: Color.DarkGray); //Makes dust in a messy circle
					d.noGravity = true;
				}
				
			}
            if (!kplr.RightClicking)
            {
				kplr.hammerCharge = 0;
            }
		}

        public override void UpdateInventory(Player player)
        {
            Item.noMelee = player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.MaskedFireTornado>()] > 0;
            KirbPlayer kplr = player.GetModPlayer<KirbPlayer>();
            if (kplr.RightClicking && player.ItemTimeIsZero) //holding right & not attacking
            {
                player.endurance += 0.35f; //damage reduction of 35% (put it here since it won't work in HoldItem()
            }
        }

        public override bool? UseItem(Player player)
		{
			KirbPlayer kplr = player.GetModPlayer<KirbPlayer>();
			if (kplr.hammerCharge >= chargeNeededForTornado)
			{
				Item.noMelee = true;
				kplr.hammerCharge = 0;
				Item.noUseGraphic = true;
				SoundEngine.PlaySound(SoundID.Item74, player.Center);  //inferno explosion
				if (Main.myPlayer == player.whoAmI)
				{
					Projectile.NewProjectile(new EntitySource_ItemUse(Main.player[player.whoAmI], player.HeldItem), player.Center, player.velocity, ModContent.ProjectileType<Projectiles.MaskedFireTornado>(), Item.damage * 10, Item.knockBack, player.whoAmI);
				}
				return true;
			}
			Item.noUseGraphic = false;
			kplr.hammerCharge = 0;
			bool hasGroundToSlamHammer = false;

            for (int i = 0; i < 32; i++)//width
            {
                for (int j = 0; j < 32; j++) //height
                {
					if (player.direction == 1)
					{
						//checks for tiles on right side of player
						Tile tile = Main.tile[(player.Center + new Vector2((64 + i), j)).ToTileCoordinates()];
                        hasGroundToSlamHammer = WorldGen.SolidOrSlopedTile(tile) || TileID.Sets.Platforms[tile.TileType];
                    }
					else
					{
                        //checks for tiles on left side of player
                        Tile tile = Main.tile[(player.Center - new Vector2((64 + i), -j)).ToTileCoordinates()];
                        hasGroundToSlamHammer = WorldGen.SolidOrSlopedTile(tile) || TileID.Sets.Platforms[tile.TileType];
                    }
                }

                if (hasGroundToSlamHammer && useCounter % 3 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item14, player.Center);  //inferno explosion
                    if (Main.myPlayer == player.whoAmI)
                    {
                        Projectile.NewProjectile(new EntitySource_ItemUse(Main.player[player.whoAmI], player.HeldItem), player.Center.X + player.direction * 80, player.Center.Y - 40, player.direction * 10, 0, ModContent.ProjectileType<Projectiles.MaskedFireTornadoSmall>(), Item.damage, Item.knockBack, player.whoAmI);
                    }
                    for (int k = 0; k < 30; k++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                    {
                        Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
                        Dust d = Dust.NewDustPerfect(player.Center + new Vector2(player.direction * 80, 0), DustID.SolarFlare, speed * 4, 0, default, 2f); //Makes dust in a messy circle
                        d.noGravity = true;
                    }

                    break;
                }
            }

            useCounter++; //go up by 1

            return true;
		}
		public override void AddRecipes()
		{
			Recipe maskedhammerrecipe = CreateRecipe();//the result is Wild Hammer
			maskedhammerrecipe.AddIngredient(ModContent.ItemType<Items.Weapons.WildHammer>()); //Wild Hammer
			maskedhammerrecipe.AddIngredient(ItemID.PossessedHatchet); //Possessed Hatchet
            maskedhammerrecipe.AddIngredient(ItemID.TheHorsemansBlade); //The Horseman's Blade
            maskedhammerrecipe.AddIngredient(ItemID.FragmentSolar, 9); //Solar Fragment
			maskedhammerrecipe.AddIngredient(ModContent.ItemType<Items.Starbit>(), 200); //200 starbits
			maskedhammerrecipe.AddIngredient(ModContent.ItemType<Items.RareStone>(), 3); //3 rare stones
			maskedhammerrecipe.AddTile(TileID.LunarCraftingStation); //crafted at ancient manipulator
			maskedhammerrecipe.Register(); //adds this recipe to the game
		}
	}
}