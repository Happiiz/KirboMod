using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class Plasma : ModItem
	{
		private bool holdingbutton = false; //checks if you're holding WASD
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Plasma Charge"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Sends out electric shocks with varying strength" +
				"\nStrength depends on how much you charge by pressing any directional input" +
				"\nStronger blasts consume more mana"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.damage = 32;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.width = 34;
			Item.height = 44;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0.1f;
			Item.value = Item.buyPrice(0, 0, 20, 25);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item12; //laser beam
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.PlasmaZap>();
			Item.shootSpeed = 20f;
			Item.mana = 3;
		}

        public override bool CanUseItem(Player player)
        {
			if (player.GetModPlayer<KirbPlayer>().plasmacharge < 3)
			{
                Item.UseSound = SoundID.Item12; //laser beam
            }
			else if (player.GetModPlayer<KirbPlayer>().plasmacharge < 12)
			{
                Item.UseSound = SoundID.Item75; //pulse bow (not boss laser beam because I don't want ptsd)
            }
			else if (player.GetModPlayer<KirbPlayer>().plasmacharge >= 12)
			{
                Item.UseSound = SoundID.Item117; //conjure arcanum 
            }
                return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            if (player.GetModPlayer<KirbPlayer>().plasmacharge < 3)
            {
                type = ModContent.ProjectileType<Projectiles.PlasmaZap>();
                damage *= 1;
                player.GetModPlayer<KirbPlayer>().plasmacharge = 0;
            }
            else if (player.GetModPlayer<KirbPlayer>().plasmacharge < 12)
            {
                type = ModContent.ProjectileType<Projectiles.PlasmaLaser>();
                velocity *= 1.25f; //make velocity 25
				position += velocity * 5; //start away from player
                damage *= 4;
                player.statMana -= 9; //+ -3 is -12 mana
                player.GetModPlayer<KirbPlayer>().plasmacharge = 0;
            }
            else if (player.GetModPlayer<KirbPlayer>().plasmacharge >= 12)
            {
                type = ModContent.ProjectileType<Projectiles.PlasmaBlast>();
                velocity *= 0.5f; //make velocity 10
                position += velocity * 5; //start away from player
                damage *= 16;
                player.statMana -= 37; //+ -3 is -40 mana
                player.GetModPlayer<KirbPlayer>().plasmacharge = 0;
            }
        }

        public override void HoldItem(Player player)
        {
			//if pressing right or left(not holding)
			if ((player.controlRight || player.controlLeft) && holdingbutton == false)
			{
				player.GetModPlayer<KirbPlayer>().plasmacharge++;
                player.GetModPlayer<KirbPlayer>().plasmaTimer = 0; //reset timer

                int dustnumber = Dust.NewDust(player.position, player.width, player.height, DustID.TerraBlade, Main.rand.Next(-10, 10), -10, 200, default, 2f); //dust
				Main.dust[dustnumber].velocity *= 0.3f;
				Main.dust[dustnumber].noGravity = true;

				holdingbutton = true;
			}
			if (player.releaseRight & player.releaseLeft) //release
			{
				holdingbutton = false;
			}
        }
		
		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Texture2D texture = ModContent.Request<Texture2D>("KirboMod/Items/Weapons/Plasma_Glowmask").Value; //Glowmask

			spriteBatch.Draw
			(
				texture,
				new Vector2
			   (
						Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
						Item.position.Y - Main.screenPosition.Y + Item.height - texture.Height * 0.5f
				),
				new Rectangle(0, 0, texture.Width, texture.Height),
				Color.White,
				rotation,
				texture.Size() * 0.5f,
				1f, //size depends on size variable
				SpriteEffects.None,
				0f
			);
		}
	}
}