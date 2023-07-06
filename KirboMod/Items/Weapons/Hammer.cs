using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class Hammer : ModItem
	{
		private int meleeCharge = 0;
		private int attackTime = 0;
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
			Item.autoReuse = false;
		}

		public override void HoldItem(Player player)
		{
			if (Main.mouseRight == true & attackTime < 1) //holding right & not attacking
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

			if (Main.mouseRight == false & attackTime < 1) //not attacking or charging
			{
				meleeCharge = 0; //reset
			}

			if (meleeCharge >= 60) //cap
			{
				meleeCharge = 60;

				Item.damage = 180; //change damage
				Item.knockBack = 12;

				for (int i = 0; i % 5 == 0; i++) // inital statement ; conditional ; loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
					Dust dust = Dust.NewDustPerfect(player.Center, DustID.Torch, speed * 10, Scale: 2f, newColor: Color.DarkGray); //Makes dust in a messy circle
					dust.noGravity = true;
				}
			}
			else if (meleeCharge < 60)
			{
				Item.damage = 36; //original damage 
				Item.knockBack = 8;
				Item.UseSound = SoundID.Item1;
			}

			attackTime--; //go down

			if (attackTime == 5) //restart when used while charged (2 otherwise there will be window to strong hit again)
			{
				meleeCharge = 0;
			}
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (meleeCharge == 60)
			{
				for (int i = 0; i < 4; i++) // inital statement ; conditional ; loop
				{
					int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Torch, Scale: 2f);
					Main.dust[dust].noGravity = false;
				}

				for (int i = 0; i < 200; i++)
				{
					NPC npc = Main.npc[i];

					if (hitbox.Intersects(npc.Hitbox) && npc.friendly == false)
					{
						npc.AddBuff(BuffID.OnFire, 600); //10 seconds 
					}
				}
			}
		}

		public override bool? UseItem(Player player)/* tModPorter Suggestion: Return null instead of false */
		{
			attackTime = Item.useTime;

			if (meleeCharge == 60)
			{
				SoundEngine.PlaySound(SoundID.Item74, player.Center);  //inferno explosion
			}
			return true;
		}
	}
}