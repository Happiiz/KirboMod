using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
    public class DragonFire : ModItem
	{
		public override void SetStaticDefaults()
		{
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
        }

        static int ArmPen = 15;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ArmPen);

        public override void SetDefaults()
		{
			Item.damage = 45;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.width = 25;
			Item.height = 25;
			Item.useTime = 7;
			Item.useAnimation = Item.useTime * 5;
            Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0.2f;
			Item.value = Item.buyPrice(0, 5, 50, 0);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item34;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.Flames.DragonFireFire>();
			Item.shootSpeed = 7; //proj has 3 extraupdates
			Item.mana = 12;
			Item.ArmorPenetration = ArmPen;
		}

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, -10);
        }

        public override void HoldItemFrame(Player player)
        {
            Item.scale = 0.8f; //make small while holding
        }

        //Draw Flame
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Texture2D texture = ModContent.Request<Texture2D>("KirboMod/Items/Weapons/DragonFire_Glowmask").Value; //GlowMask (flame)

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

		public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe();//the result is dragonfire
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.VolcanoFire>()); //Volcano Fire
			recipe1.AddIngredient(ItemID.ShadowbeamStaff); //Shadowbeam Staff
			recipe1.AddIngredient(ItemID.LaserMachinegun); //Laser Machinegun
			recipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 100); //100 starbits
			recipe1.AddIngredient(ModContent.ItemType<Items.RareStone>(), 5); //5 rare stones
			recipe1.AddTile(TileID.MythrilAnvil); //crafted at mythril/orichalcum anvil
			recipe1.Register(); //adds this recipe to the game
		}
	}
}