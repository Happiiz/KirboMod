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
		private int meleeCharge = 0;
		private static int maxCharge = 30;
		private int attackTime = 0;
		private Vector2 dash = Main.MouseWorld;
		private int uses = 0;
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
			if (Main.mouseRight == true & player.itemTime == 0) //holding up & not attacking
			{
				meleeCharge++; //go up
				player.velocity.X *= 0.9f; //slow
                player.endurance += 0.35f; //damage reduction of 35%

                for (int i = 0; i % 5 == 0; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
					Dust d = Dust.NewDustPerfect(player.Center, DustID.Smoke, speed * 5, Scale: 2f, newColor: Color.DarkGray); //Makes dust in a messy circle
					d.noGravity = true;
				}
			}

			if (Main.mouseRight == false & player.itemTime == 0) //not attacking or charging
			{
				meleeCharge = 0; //reset
			}

			if (meleeCharge >= maxCharge) //cap
			{
				meleeCharge = maxCharge;

                Item.DamageType = DamageClass.MeleeNoSpeed;
                Item.damage = 512; //change damage
				Item.useTime = 24;
				Item.useAnimation = 24;
				Item.noMelee = true;
				player.dash = 0; //disable dashing

				for (int i = 0; i % 3 == 0; i++) // inital statement ; conditional ; loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
					Dust dust = Dust.NewDustPerfect(player.Center, DustID.SolarFlare, speed * 10, Scale: 2f); //Makes dust in a messy circle
					dust.noGravity = true;
				}
			}
			else if (meleeCharge < maxCharge)
			{
				Item.DamageType = DamageClass.Melee;
                Item.damage = 128; //original damage 
				Item.UseSound = SoundID.Item1;
				Item.useTime = 8;
				Item.useAnimation = 8;
				Item.noMelee = false;
			}

			attackTime--; //go down

			if (attackTime == 5) //restart when used while charged (2 otherwise there will be window to strong hit again)
			{
				meleeCharge = 0;
			}

			if (attackTime > 0 & meleeCharge == maxCharge) //still attacking with charge
            {
				//MELEE TORNADO

				if (attackTime >= 23) //inital strike
				{
					dash = Main.MouseWorld - player.Center;
					dash.Normalize(); //reduce to a unit of 1
					dash *= 15; //make a speed of 15
					player.velocity = dash;

					player.immuneTime = player.itemAnimationMax; //for invincibility timer
				}
				else
                {
					player.velocity = dash; //keep moving the way you were
				}

				//invincibility (timer above)
				player.immune = true;
				player.immuneNoBlink = true;

                //extra...
                player.maxFallSpeed = 20;
                player.noKnockback = true;
                player.canJumpAgain_Blizzard = false;
                player.canJumpAgain_Cloud = false;
                player.canJumpAgain_Sandstorm = false;
                player.canJumpAgain_Sail = false;
                player.canJumpAgain_Fart = false;
                player.dash = 0;

                player.canRocket = false;
                player.carpet = false;
                player.carpetFrame = -1;

                //disable kirby balloon
                player.GetModPlayer<KirbPlayer>().kirbyballoon = false;
                player.GetModPlayer<KirbPlayer>().kirbyballoonwait = 1;

                //double jump effects
                player.hasJumpOption_Blizzard = false;
                player.hasJumpOption_Cloud = false;
                player.hasJumpOption_Sandstorm = false;
                player.hasJumpOption_Sail = false;
                player.hasJumpOption_Fart = false;

                player.isPerformingJump_Blizzard = false;
                player.isPerformingJump_Cloud = false;
                player.isPerformingJump_Sandstorm = false;
                player.isPerformingJump_Sail = false;
                player.isPerformingJump_Fart = false;

                player.DryCollision(true, true); //fall through platforms

                player.mount.Dismount(player); //dismount mounts
			}


			if (Main.mouseLeft == false || Collision.down & player.gravDir == 1f) //not holding down or not in air
			{
				uses = 0;
			}
		}

		public override bool? UseItem(Player player)/* tModPorter Suggestion: Return null instead of false */
		{
			attackTime = Item.useTime;

			if (meleeCharge == maxCharge)
			{
				SoundEngine.PlaySound(SoundID.Item74, player.Center);  //inferno explosion
				Projectile.NewProjectile(new EntitySource_ItemUse(Main.player[player.whoAmI], player.HeldItem), player.Center, player.velocity, ModContent.ProjectileType<Projectiles.MaskedFireTornado>(), Item.damage, Item.knockBack, player.whoAmI);
			}

			bool airborne = player.velocity.Y != 0f; //not not moving

			if (uses >= 3 & !airborne)
			{
				SoundEngine.PlaySound(SoundID.Item14, player.Center);  //inferno explosion
				Projectile.NewProjectile(new EntitySource_ItemUse(Main.player[player.whoAmI], player.HeldItem), player.Center.X + player.direction * 40, player.Center.Y - 40, player.direction * 10, 0, ModContent.ProjectileType<Projectiles.MaskedFireTornadoSmall>(), Item.damage, Item.knockBack, player.whoAmI);

				for (int i = 0; i < 30; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
					Dust d = Dust.NewDustPerfect(player.Center + new Vector2(player.direction * 80, 0), DustID.SolarFlare, speed * 4, 0, default, 2f); //Makes dust in a messy circle
					d.noGravity = true;
				}
				uses = 0;
			}
			else
			{
				uses++; //go up by 1
			}
			return true;
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (Item.noMelee == false)
			{
				for (int i = 0; i < 2; i++) // inital statement ; conditional ; loop
				{
					int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Torch, Scale: 1f);
					Main.dust[dust].velocity *= 0;
					Main.dust[dust].noGravity = false;
				}

				for (int i = 0; i < 200; i++)
				{
					NPC npc = Main.npc[i];

					if (hitbox.Intersects(npc.Hitbox) && npc.friendly == false)
					{
						npc.AddBuff(BuffID.Daybreak, 600); //10 seconds 
					}
				}
			}
		}

		public override void AddRecipes()
		{
			Recipe maskedhammerrecipe = CreateRecipe();//the result is Wild Hammer
			maskedhammerrecipe.AddIngredient(ModContent.ItemType<Items.Weapons.WildHammer>()); //Wild Hammer
			maskedhammerrecipe.AddIngredient(ItemID.PossessedHatchet); //Possessed Hatchet
            maskedhammerrecipe.AddIngredient(ItemID.DD2SquireBetsySword); //Flying Dragon
            maskedhammerrecipe.AddIngredient(ItemID.FragmentSolar, 9); //Solar Fragment
			maskedhammerrecipe.AddIngredient(ModContent.ItemType<Items.Starbit>(), 200); //200 starbits
			maskedhammerrecipe.AddIngredient(ModContent.ItemType<Items.RareStone>(), 8); //8 rare stones
			maskedhammerrecipe.AddTile(TileID.LunarCraftingStation); //crafted at ancient manipulator
			maskedhammerrecipe.Register(); //adds this recipe to the game
		}
	}
}