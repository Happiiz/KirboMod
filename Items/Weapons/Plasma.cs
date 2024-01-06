using KirboMod.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
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
			if (player.GetModPlayer<KirbPlayer>().plasmaCharge < 3)
			{
                Item.UseSound = SoundID.Item12; //laser beam
            }
			else if (player.GetModPlayer<KirbPlayer>().plasmaCharge < 12)
			{
                Item.UseSound = SoundID.Item75; //pulse bow (not boss laser beam because I don't want ptsd)
            }
			else if (player.GetModPlayer<KirbPlayer>().plasmaCharge >= 12)
			{
				SoundEngine.PlaySound(SoundID.Item38, player.Center);//tactical shotgun
                Item.UseSound = SoundID.Item117; //conjure arcanum 
            }
                return true;
        }
        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            base.ModifyManaCost(player, ref reduce, ref mult);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			float timeToPressKey = 16;
			float chargeBonus = 1;
			float chargeFromShot = player.GetModPlayer<KirbPlayer>().plasmaCharge;
            if (chargeFromShot < 3)
            {
				chargeFromShot = 1;
				velocity *= 1.3f;
                type = ModContent.ProjectileType<Projectiles.PlasmaZap>();
            }
            else if (chargeFromShot < 12)
            {
                type = ModContent.ProjectileType<Projectiles.PlasmaLaser>();
                velocity *= 2;
				position += velocity;
				chargeBonus = 3;
            }
            else
            {
                type = ModContent.ProjectileType<Projectiles.PlasmaBlast>();
                velocity *= 3;
                position += velocity; //start away from player
				chargeBonus = 7;
            }
			//higher time to press key = higher damage
			//higher use time = less damage because then dthe time taken to charge is less of a difference compared to normal firing
			//higher charge from shot = higher damage bonus 
			//
			damage = (int)(damage * chargeFromShot * (timeToPressKey / Item.useTime) * chargeBonus);
			player.GetModPlayer<KirbPlayer>().ResetPlasmaCharge();//make reset charge method with packet handling
			velocity /= ContentSamples.ProjectilesByType[type].MaxUpdates;
		}

		public override void HoldItem(Player player)
        {

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
    class PlasmaDrawLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
			KirbPlayer mplr = drawInfo.drawPlayer.GetModPlayer<KirbPlayer>();
			return mplr.plasmaCharge >= 3; 
        }
        public override Position GetDefaultPosition()
        {
			return PlayerDrawLayers.BeforeFirstVanillaLayer;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
			if (drawInfo.shadow != 0)
				return;
			KirbPlayer mplr = drawInfo.drawPlayer.GetModPlayer<KirbPlayer>();
			Texture2D tex;
			int orb = ModContent.ProjectileType<PlasmaOrb>();
			int shiel = ModContent.ProjectileType<PlasmaShield>();
			bool bigCharge = mplr.plasmaCharge > 12;
			tex = TextureAssets.Projectile[bigCharge ? orb : shiel].Value;
			float scale = (bigCharge ? KirbPlayer.plasmaShieldRadiusLarge : KirbPlayer.plasmaShieldRadiusSmall) / (float)tex.Width;
			drawInfo.DrawDataCache.Add(new DrawData(tex, drawInfo.Center - Main.screenPosition, null, Color.White, 0, tex.Size() / 2, scale, SpriteEffects.None));
        }
    }
}