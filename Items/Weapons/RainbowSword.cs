using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.RainbowSword
{
	public class RainbowSword : ModItem
	{
		public override void SetStaticDefaults()
		{
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }
		Effect rainbowSwordShader;
		void SetShaderParams()
		{
			rainbowSwordShader.Parameters["h"].SetValue(Main.GlobalTimeWrappedHourly * 3f);
			rainbowSwordShader.Parameters["s"].SetValue(1);
			rainbowSwordShader.Parameters["l"].SetValue(0.6f);
			rainbowSwordShader.Parameters["uOpacity"].SetValue(1);
			rainbowSwordShader.Parameters["gradientScale"].SetValue(2f);
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			LoadShaderIfNeeded();
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, rainbowSwordShader, Main.UIScaleMatrix);
			SetShaderParams();
			return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
		}
		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			LoadShaderIfNeeded();
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, rainbowSwordShader, Main.Transform);
			SetShaderParams();
			return base.PreDrawInWorld(spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
		}
		private void LoadShaderIfNeeded()
		{
			if (rainbowSwordShader == null)
			{
				rainbowSwordShader = ModContent.Request<Effect>("KirboMod/Items/RainbowSword/RainbowSwordShader", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			}
		}
		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
		}
		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			spriteBatch.End();//this is probably what is used
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
		}
        public override bool MeleePrefix()
        {
			return true;
        }
        public override void SetDefaults()
		{
		//todo: make able to receive legendary
			Item.damage = 300;//it has a relatively slow base use time so this compensates(+ also shorter range relatively)
			Item.DamageType = DamageClass.Melee; 
			Item.width = 40;
			Item.height = 40;
			Item.useTime = Item.useAnimation = 28; 
			Item.useStyle = ItemUseStyleID.Shoot; 
            Item.knockBack = 8;
			Item.value = Item.buyPrice(0, 15, 50, 0);
			Item.rare = ItemRarityID.Yellow;
			Item.autoReuse = false;
			Item.shoot = ModContent.ProjectileType<Items.RainbowSword.RainbowSwordHeld>();
			Item.noMelee = true; //hitbox reserved for swing and beam
            Item.shootsEveryUse = true;
			Item.channel = true;
			Item.useTurn = true;
			Item.shootSpeed = 1;
			Item.noUseGraphic = true;
			Item.autoReuse = true;
			Item.UseSound = SoundID.Item74 with { MaxInstances = 0, Volume = 0.6f };
		
		}
        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if(Main.myPlayer != player.whoAmI)
			return false;
			KirbPlayer mPlayer = player.GetModPlayer<KirbPlayer>();									//todo account for melee speed
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, player.itemAnimationMax, MathHelper.Lerp(6.15f, 4, Main.rand.NextFloat()), mPlayer.NextRainbowSwordSwingDirection);
			return false;
        }
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Items.RainbowDrops.SnowDrop>());
			recipe.AddIngredient(ModContent.ItemType<Items.RainbowDrops.JungleDrop>());
			recipe.AddIngredient(ModContent.ItemType<Items.RainbowDrops.DesertDrop>());
			recipe.AddIngredient(ModContent.ItemType<Items.RainbowDrops.EvilDrop>());
			recipe.AddIngredient(ModContent.ItemType<Items.RainbowDrops.HellDrop>());
			recipe.AddIngredient(ModContent.ItemType<Items.RainbowDrops.OceanDrop>());
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}