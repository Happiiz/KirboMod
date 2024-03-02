using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.DarkSword
{
	public class DarkSword : ModItem
	{
        public enum ProjectileShootType
        {
			DarkOrb,
			DarkBeam,
			DarkWave
        }
		public override void SetStaticDefaults()
		{
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
        }

		public override void SetDefaults()
		{
			Item.damage = 150;
			Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Shoot; //gun/staff
			Item.UseSound = SoundID.Item15; //phaseblade
			Item.knockBack = 9;
			Item.value = Item.buyPrice(0, 2, 75, 0);
			Item.rare = ItemRarityID.Yellow;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<DarkSwordHeld>();
			Item.shootSpeed = 48f;
			Item.noMelee = true;
			Item.noUseGraphic = true;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if (Main.myPlayer != player.whoAmI)
				return false;
			KirbPlayer mPlayer = player.GetModPlayer<KirbPlayer>();
			mPlayer.GetDarkSwordSwingStats(out int direction, out ProjectileShootType projToShoot);
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, player.itemAnimationMax, MathHelper.Lerp(6.15f, 4, Main.rand.NextFloat()), direction);
			float delayMultiplier = player.itemAnimationMax / 2f;


            switch (projToShoot)
            {
                case ProjectileShootType.DarkOrb:
					int amountOfOrbs = 4;
					delayMultiplier *= .5f; //orbs fire at a smaller angle so needs less delay to look smooth
					for (float i = 0; i < 1.01f; i += 1f / amountOfOrbs)
                    {
						Vector2 shootVel = velocity.RotatedBy(MathHelper.Lerp(-1, 1, i));
						float delay = -i * delayMultiplier;
						if (direction == -1)
							delay = Utils.Remap(delay, 0, -delayMultiplier, -delayMultiplier, 0, false);
						Projectile.NewProjectile(source, position, shootVel, ModContent.ProjectileType<DarkSwordOrb>(), damage, knockback, player.whoAmI, delay);
                    }
                    break;
                case ProjectileShootType.DarkBeam:
					int amountOfBeams = 10;
					for (float i = 0; i < 1.01f; i += 1f / amountOfBeams)
					{
						Vector2 shootVel = velocity.RotatedBy(MathHelper.Lerp(-1.6f, 1.6f, i));
						float delay = -i * delayMultiplier;
						if (direction == -1)
							delay = Utils.Remap(delay, 0, -delayMultiplier, -delayMultiplier, 0, false);
						Projectile.NewProjectile(source, position, shootVel * 0.5f, ModContent.ProjectileType<DarkSwordBeam>(), damage, knockback, player.whoAmI, delay);
					}
					break;
                case ProjectileShootType.DarkWave:
					Projectile.NewProjectile(source, position + velocity * .6f, velocity, ModContent.ProjectileType<DarkSwordWave>(), damage, knockback, player.whoAmI, -delayMultiplier / 3, direction);
                    break;
            }
            return false;
        }
        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (Main.rand.NextBool(6))
			{
				//Emit dusts when the sword is swung
				Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<Dusts.DarkResidue>(), 0, 0, 0, default, 0.5f);
			}
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();//the result is darksword
			recipe.AddIngredient(ItemID.BrokenHeroSword); //Broken Hero Sword
			recipe.AddIngredient(ModContent.ItemType<DarkMaterial>(), 15); //15 dark material
			recipe.AddTile(TileID.MythrilAnvil); //crafted at mythril anvil
			recipe.Register(); //adds this recipe to the game
		}
    }
}