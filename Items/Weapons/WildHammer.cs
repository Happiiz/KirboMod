using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class WildHammer : ModItem
	{
		private int meleeCharge = 0;
		private int attackTime = 0;
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Wild Hammer"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Hold right to slow and charge a firey and powerful swing" +
				"\nLeft click to release when at full power"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.damage = 225;
			Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.width = 59;
			Item.height = 59;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 12;
			Item.value = Item.buyPrice(0, 5, 50, 0);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
		}

		public override void HoldItem(Player player)
		{
			if (Main.mouseRight == true && attackTime < 1) //holding right & not attacking
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

				Item.damage = 1800; //change damage
				Item.knockBack = 16;
				Item.scale = 1.5f;

				for (int i = 0; i % 5 == 0; i++) // inital statement ; conditional ; loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
					Dust dust = Dust.NewDustPerfect(player.Center, DustID.Torch, speed * 10, Scale: 2f, newColor: Color.DarkGray); //Makes dust in a messy circle
					dust.noGravity = true;
				}
			}
            else if (meleeCharge < 60)
            {
				Item.damage = 225; //original damage 
				Item.knockBack = 12;
				Item.UseSound = SoundID.Item1;
				Item.scale = 1;
			}

			attackTime--; //go down

			if (attackTime == 5) //restart when used while charged (2 otherwise there will be window to strong hit again)
			{
				meleeCharge = 0;
			}
		}

        public override void UpdateInventory(Player player)
        {
            if (Main.mouseRight == true && attackTime < 1) //holding right & not attacking
            {
                player.endurance += 0.35f; //damage reduction of 35% (put it here since it won't work in HoldItem()
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
				SoundEngine.PlaySound(SoundID.Item74 , player.Center);  //inferno explosion
			}
			return true;
        }

		public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe();//the result is Wild Hammer
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.ToyHammer>()); //Toy Hammer
			recipe1.AddIngredient(ItemID.ChlorophyteGreataxe); //Chlorophyte Greataxe
			recipe1.AddIngredient(ItemID.DeathSickle); //Death Sickle
            recipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 100); //100 starbits
			recipe1.AddIngredient(ModContent.ItemType<Items.RareStone>(), 5); //5 rare stones
			recipe1.AddTile(TileID.MythrilAnvil); //crafted at mythril/orichalcum anvil
			recipe1.Register(); //adds this recipe to the game
		}
	}
}