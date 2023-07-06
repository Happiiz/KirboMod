using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class RainbowSword : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Rainbow Sword"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Holds the light to light up everyone's worlds" +
				"\nWell timed swings can deflect certain dark projectiles" +
                "\nRight click to shoot a non-damaging beam of light"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.damage = 195; //1995
			Item.DamageType = DamageClass.MeleeNoSpeed; //attack speed doesn't scale
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 10; 
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Swing; 
            Item.knockBack = 8;
			Item.value = Item.buyPrice(0, 15, 50, 0);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.RainbowSlash>();
			Item.noMelee = true; //hitbox reserved for swing and beam
            //item.shootSpeed = 6f;
            Item.shootsEveryUse = true;

        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ModContent.ProjectileType<Projectiles.RainbowSwordBeam>())
			{
				damage = 0;
			}
			else //slash
			{
				velocity = new Vector2(player.direction, 0f); //for facing the right direction
				position = player.MountedCenter + new Vector2(player.direction * 10, 0);
            }
        }

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            /*if (player.altFunctionUse != 2)
            {
				//used to make it rotate around the player when swung
				player.itemRotation = player.direction * (MathHelper.ToRadians(-110) + ((player.itemTimeMax - player.itemTime) * MathHelper.ToRadians(31)));
            }*/
        }

        public override bool AltFunctionUse(Player player)
		{
			return true; //you can right click with this item
		}
		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2) //right click
			{
				Item.staff[Item.type] = true; //staff not gun
				Item.useStyle = ItemUseStyleID.Shoot; //gun
				Item.useTime = 60;
				Item.useAnimation = 60;
				Item.UseSound = SoundID.Item25; //fairy bell
				Item.shoot = ModContent.ProjectileType<Projectiles.RainbowSwordBeam>();
				Item.shootSpeed = 10f;
				return !NPC.AnyNPCs(ModContent.NPCType<NPCs.DarkMatter>()); //checks if dark matter isn't alive
			}
			else
			{
				Item.staff[Item.type] = false;
				Item.useStyle = ItemUseStyleID.Swing; //sword
				Item.useTime = 10;
				Item.useAnimation = 10;
				Item.UseSound = SoundID.Item1; //swing
				Item.shoot = ModContent.ProjectileType<Projectiles.RainbowSlash>();
				Item.shootSpeed = 0;
				return player.ownedProjectileCounts[Item.shoot] < 1;
			}
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