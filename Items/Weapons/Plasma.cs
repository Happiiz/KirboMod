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
			Item.damage = 25;
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
			Item.mana = 5;
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
			int plasmaChargeLevel = player.GetModPlayer<KirbPlayer>().PlasmaShieldLevel;
			if(plasmaChargeLevel == 1)
            {
				reduce = -2;
            }
			else if(plasmaChargeLevel == 2)
            {
				reduce = -5;
            }
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
                type = ModContent.ProjectileType<PlasmaZap>();
				timeToPressKey = Item.useTime; //cancels out to become 1
            }
            else if (chargeFromShot < 12)
            {
                type = ModContent.ProjectileType<PlasmaLaser>();
                velocity *= 2;
				position += velocity;
				chargeBonus = 1;
            }
            else
            {
                type = ModContent.ProjectileType<PlasmaBlast>();
                velocity *= 3;
                position += velocity;
				chargeBonus = 1;
            }
			//higher time to press key = higher damage
			//higher use time = less damage because then dthe time taken to charge is less of a difference compared to normal firing
			//higher charge from shot = higher damage bonus 
			//time to press key is an estimate of how much time on average fast people can mash keys while also mantaining the general movement they have.
			damage = (int)(damage * chargeFromShot * (timeToPressKey / Item.useTime) * chargeBonus);
			player.GetModPlayer<KirbPlayer>().ResetPlasmaCharge();
			velocity /= ContentSamples.ProjectilesByType[type].MaxUpdates;
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
			int orb = ModContent.ProjectileType<PlasmaShield>();
			int shiel = ModContent.ProjectileType<PlasmaOrb>();
			bool bigCharge = mplr.plasmaCharge > 12;
			tex = TextureAssets.Projectile[bigCharge ? orb : shiel].Value;
			float scale = (bigCharge ? KirbPlayer.plasmaShieldRadiusLarge : KirbPlayer.plasmaShieldRadiusSmall) / (float)tex.Width * 2f;
			drawInfo.DrawDataCache.Add(new DrawData(tex, drawInfo.Center - Main.screenPosition, null, Color.White, 0, tex.Size() / 2, scale, SpriteEffects.None));
        }
    }
}