using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class ToyHammer : ModItem
	{
		private int meleeCharge = 0;
		private int attackTime = 0;
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Toy Hammer"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Hold right to slow and charge a powerful swing" +
				"\nLeft click to release when at full power"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.damage = 115;
			Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.width = 47;
			Item.height = 47;
			Item.useTime = 8;
			Item.useAnimation = 8;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = Item.buyPrice(0, 0, 45, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
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

				Item.damage = 1150; //change damage
				Item.scale = 1.5f;
				Item.knockBack = 14;

				for (int i = 0; i % 5 == 0; i++) // inital statement ; conditional ; loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
					Dust dust = Dust.NewDustPerfect(player.Center, DustID.Torch, speed * 10, Scale: 2f, newColor: Color.DarkGray); //Makes dust in a messy circle
					dust.noGravity = true;
				}
			}
			else if (meleeCharge < 60)
			{
				Item.damage = 115; //original damage 
				Item.UseSound = SoundID.Item1;
				Item.scale = 1;
				Item.knockBack = 6;
			}

			attackTime--; //go down

			if (attackTime == 5) //restart when used while charged (2 otherwise there will be window to strong hit again)
			{
				meleeCharge = 0;
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

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 5; i++) // inital statement ; conditional ; loop
            {
                Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                Dust dust = Dust.NewDustPerfect(target.Center, ModContent.DustType<Dusts.LilStar>(), speed, Main.rand.Next(1, 2)); //Makes dust in a messy circle
                dust.noGravity = true;
            }

            SoundStyle Squeak = new SoundStyle("KirboMod/Sounds/Item/Squeak");
            Squeak.Pitch = Main.rand.NextFloat(0.5f);
			Squeak.Volume = 0.5f;
            SoundEngine.PlaySound(Squeak, target.Center);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            for (int i = 0; i < 5; i++) // inital statement ; conditional ; loop
            {
                Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                Dust dust = Dust.NewDustPerfect(target.Center, ModContent.DustType<Dusts.LilStar>(), speed, Main.rand.Next(1, 2)); //Makes dust in a messy circle
                dust.noGravity = true;
            }

            SoundStyle Squeak = new SoundStyle("KirboMod/Sounds/Item/Squeak");
            Squeak.Pitch = Main.rand.NextFloat(0.5f);
            Squeak.Volume = 0.5f;
            SoundEngine.PlaySound(Squeak, target.Center);
        }

        public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe();//the result is gigantsword
			recipe1.AddIngredient(ModContent.ItemType<Hammer>()); //Hammer
			recipe1.AddIngredient(ItemID.KOCannon); //KO Cannon
			recipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 50); //50 starbits
			recipe1.AddIngredient(ModContent.ItemType<Items.RareStone>(), 2); //2 rare stones
			recipe1.AddTile(TileID.Anvils); //crafted at anvil
			recipe1.Register(); //adds this recipe to the game
		}
	}
}