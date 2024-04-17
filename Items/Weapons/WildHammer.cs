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
	public class WildHammer : ModItem
	{
		private int meleeCharge = 0;
		private int attackTime = 0;

        const int chargeCap = 90;
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
                Item.knockBack = 16;

                for (int i = 0; i % 5 == 0; i++) // inital statement ; conditional ; loop
                {
                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
                    Dust dust = Dust.NewDustPerfect(player.Center, DustID.Torch, speed * 10, Scale: 2f);
                    dust.noGravity = true;
                }
            }
            else
            {
                Item.knockBack = 12;
            }
        }

        public override void UpdateInventory(Player player)
        {
            if (Main.mouseRight == true && player.ItemTimeIsZero) //holding right & not attacking
            {
                player.endurance += 0.35f; //damage reduction of 35% (put it here since it won't work in HoldItem()
            }

            //I put this here since it wouldn't register in the projectile code itself
            if (!player.ItemTimeIsZero && player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.HammerSwings.WildHammerSwing>()] > 0)
            {
                player.maxFallSpeed = 40;
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
                    ModContent.ProjectileType<Projectiles.HammerSwings.WildHammerSwing>(), player.GetWeaponDamage(player.HeldItem) * 10, Item.knockBack, player.whoAmI);
            }
            else
            {
                Item.noMelee = false;
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