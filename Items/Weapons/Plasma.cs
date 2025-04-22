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
			Item.damage = 42;
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
			int plasmaCharge = player.GetModPlayer<KirbPlayer>().PlasmaCharge;

			//todo: change these to be played on the projectile AI.
            if (plasmaCharge < 3)
			{
                Item.UseSound = SoundID.Item12; //laser beam
            }
			else if (plasmaCharge < 12)
			{
                Item.UseSound = SoundID.Item75; //pulse bow (not boss laser beam because I don't want ptsd)
            }
			else if (plasmaCharge >= 12)
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
			float timeToPressKey = 10;
			float chargeBonus = 1;
			float chargeFromShot = player.GetModPlayer<KirbPlayer>().PlasmaCharge;
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
			return mplr.PlasmaCharge >= 3 /*&&drawInfo.drawPlayer.HeldItem.type == ModContent.ItemType<Plasma>()*/; 
        }
        public override Position GetDefaultPosition()
        {
			return PlayerDrawLayers.BeforeFirstVanillaLayer;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
			if (drawInfo.shadow != 0)
				return;
            Texture2D main = ModContent.Request<Texture2D>("KirboMod/Projectiles/PlasmaOrb/PlasmaOrbBase").Value;
            Texture2D strands1 = ModContent.Request<Texture2D>("KirboMod/Projectiles/PlasmaOrb/PlasmaOrbStrands1").Value;
            Texture2D strands2 = ModContent.Request<Texture2D>("KirboMod/Projectiles/PlasmaOrb/PlasmaOrbStrands2").Value;
            KirbPlayer mplr = drawInfo.drawPlayer.GetModPlayer<KirbPlayer>();
			bool bigCharge = mplr.PlasmaCharge > 12;
			float scale = (bigCharge ? KirbPlayer.plasmaShieldRadiusLarge : KirbPlayer.plasmaShieldRadiusSmall) / (float)main.Width * 2f;
			Vector2 drawCenter = drawInfo.Center - Main.screenPosition;
            Color col = Color.White;
            col.A = 0;
            drawInfo.DrawDataCache.Add(new DrawData(main, drawCenter, null, Color.White with { A = 128 }, 0, main.Size() / 2, scale, SpriteEffects.None));
            float time = (float)(Main.GlobalTimeWrappedHourly * 100f);
            int strand1FlashCycleLength = 30;
            int strand2FlashCycleLength = (int)(strand1FlashCycleLength * Helper.Phi);
            float strand1Opacity = Utils.GetLerpValue(0, strand1FlashCycleLength / 2f, time % strand1FlashCycleLength, true) * Utils.GetLerpValue(strand1FlashCycleLength - 1, strand1FlashCycleLength / 2f, time % strand1FlashCycleLength, true);
            int strand1FlashCycleIndex = (int)(time / strand1FlashCycleLength);
            float rotation = (int)(strand1FlashCycleIndex * Helper.Phi) * MathF.PI * 0.5f;
            SpriteEffects fx = (SpriteEffects)(strand1FlashCycleIndex % 3);
			drawInfo.DrawDataCache.Add(new DrawData(strands1, drawCenter, null, col * strand1Opacity, rotation, strands1.Size() / 2, scale, fx));
            float strand2Opacity = Utils.GetLerpValue(0, strand2FlashCycleLength / 2f, time % strand2FlashCycleLength, true) * Utils.GetLerpValue(strand2FlashCycleLength - 1, strand2FlashCycleLength / 2f, time % strand2FlashCycleLength, true);
            int strand2FlashCycleIndex = (int)((time) / strand2FlashCycleLength);
            rotation = (int)(strand2FlashCycleIndex * Helper.Phi) * MathF.PI * 0.5f;
            fx = (SpriteEffects)(strand2FlashCycleIndex % 3);
			drawInfo.DrawDataCache.Add(new DrawData(strands2, drawCenter, null, col * strand2Opacity, rotation, strands2.Size() / 2, scale, fx));
        }
    }
}