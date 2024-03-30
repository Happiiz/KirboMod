using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class Hammer : ModItem
	{
		private int meleeCharge = 0;

        const int chargeCap = 60;
        public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Bonking Hammer"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Hold right to slow and charge a firey swing" +
				"\nLeft click to release when at full power"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
        }

		public override void SetDefaults()
		{
			Item.damage = 36;
			Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 8;
			Item.value = Item.buyPrice( 0, 0, 12, 0);
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
		}

		public override void HoldItem(Player player)
		{
            KirbPlayer kplr = player.GetModPlayer<KirbPlayer>();

            if (player.ItemTimeIsZero)
			{
				if (kplr.RightClicking) //holding right & not attacking
                {
					meleeCharge++; //go up
					player.velocity.X *= 0.9f; //slow

					for (int i = 0; i % 5 == 0; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
					{
						Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
						Dust d = Dust.NewDustPerfect(player.Center, DustID.Smoke, speed * 5, Scale: 2f, newColor: Color.DarkGray); //Makes dust in a messy circle
						d.noGravity = true;
					}
				}
				else
				{
                    meleeCharge = 0; //reset
                }
            }
            else
            {
                meleeCharge = 0; //reset
            }

            if (meleeCharge >= chargeCap) //cap
			{
				meleeCharge = chargeCap;

				Item.knockBack = 12;

				for (int i = 0; i % 5 == 0; i++) // inital statement ; conditional ; loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
                    Dust dust = Dust.NewDustPerfect(player.Center, DustID.Torch, speed * 10, Scale: 2f);
                    dust.noGravity = true;
				}
			}
			else
			{
				Item.knockBack = 8;
			}

			if (player.itemTime == 5) //restart when used while charged
			{
				meleeCharge = 0;
			}
		}

		public override bool? UseItem(Player player)/* tModPorter Suggestion: Return null instead of false */
		{
			if (meleeCharge >= chargeCap)
			{
                Item.noMelee = true;
                Item.noUseGraphic = true;

                SoundEngine.PlaySound(SoundID.Item74, player.Center);  //inferno explosion

                Projectile.NewProjectile(new EntitySource_ItemUse(Main.player[player.whoAmI], player.HeldItem), player.Center, player.velocity, 
					ModContent.ProjectileType<Projectiles.HammerSwings.HammerSwing>(), player.GetWeaponDamage(player.HeldItem), Item.knockBack, player.whoAmI);
            }
			else
			{
                Item.noMelee = false;
            }
			return true;
		}
    }
}