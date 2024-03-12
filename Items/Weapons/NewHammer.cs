using KirboMod.Projectiles;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class NewHammer : ModItem
	{
		private int uses = 0;
		public override void SetStaticDefaults() 
		{
			 // DisplayName.SetDefault("Ham-O-Matic"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Swing on the ground to cause shockwaves and right click to fire missiles" +
				"\n'Now with 2x the clobbering power!'"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.damage = 38;
			Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.width = 53; //world dimensions
			Item.height = 53; //world dimensions
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 9;
			Item.value = Item.buyPrice( 0, 0, 25, 0);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.None;
			Item.noUseGraphic = false; //use sprite
		}

        public override bool AltFunctionUse(Player player)
        {
			return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
				Item.useStyle = ItemUseStyleID.HoldUp;
				Item.noMelee = true;
				Item.useTime = 30;
				Item.useAnimation = 30;
				Item.shoot = ModContent.ProjectileType<NewHammerMissile>();
				Item.noUseGraphic = true; //dont use sprite
				Item.UseSound = SoundID.Item11; //gun
            }
			else
            {
				Item.useStyle = ItemUseStyleID.Swing;
				Item.noMelee = false;
				Item.useTime = 15;
				Item.useAnimation = 15;
				Item.shoot = ProjectileID.None;
				Item.noUseGraphic = false; //use sprite
				Item.UseSound = SoundID.Item1;
            }

			return true;
        }

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            velocity.X = player.direction * 10;
            velocity.Y = Main.rand.Next(-3, 3);
            position.Y = player.Center.Y - 30;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			//hold up animation
			Projectile.NewProjectile(source, player.Center, new Vector2(0, 0), ModContent.ProjectileType<NewHammerHoldUp>(), 0, 0, player.whoAmI);

			return true;
		}

        public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (player.altFunctionUse != 2)
			{
				for (int i = 0; i < 2; i++) // inital statement ; conditional ; loop
				{
					int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Electric, Scale: 0.75f);
					Main.dust[dust].noGravity = false;
				}

				for (int i = 0; i < 200; i++)
				{
					NPC npc = Main.npc[i];
					bool lineOfSight = Collision.CanHitLine(player.position, player.width, player.height, npc.position, npc.width, npc.height);

					if (hitbox.Intersects(npc.Hitbox) && npc.friendly == false  && lineOfSight == true)
					{
						npc.AddBuff(BuffID.OnFire, 180); //3 seconds 
					}
				}
			}
		}

        public override bool? UseItem(Player player)/* tModPorter Suggestion: Return null instead of false */
        {
			if (uses >= 3 & player.velocity.Y == 0 && player.altFunctionUse != 2)
			{
				SoundEngine.PlaySound(SoundID.Item94, player.Center); // electrosphere stop

                Projectile.NewProjectile(new EntitySource_ItemUse(Main.player[player.whoAmI], player.HeldItem), player.Center.X + player.direction * 40, player.Center.Y,
                        player.direction * 8, 0, ModContent.ProjectileType<Projectiles.NewHammerShockwave>(), player.GetWeaponDamage(Item) / 2, 0, player.whoAmI);

                for (int i = 0; i < 30; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
					Dust d = Dust.NewDustPerfect(player.Center + new Vector2(player.direction * 80, 0), DustID.Electric, speed * 4, 0, default, 0.75f); //Makes dust in a messy circle
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
    }
}