using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
    public class Cutter : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsRangedSpecialistWeapon[Type] = true;
            // DisplayName.SetDefault("Cutter"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("Flies in the opposite direction" +
				"\nOnly two can be out at a time"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
        }

        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(0, 0, 0, 20);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.CutterBlade>();
            Item.shootSpeed = 8;
            Item.noUseGraphic = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;
            //KirbPlayer kPlr = player.GetModPlayer<KirbPlayer>();
            //         if (kPlr.TryStartingFinalCutter() || kPlr.finalCutterAnimationCounter > 0)
            //         {
            //	return false;
            //         }
            float acceleration = .15f;
            int direction = player.direction;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, direction, acceleration, 20);
            return false;
        }
        public override void AddRecipes()
        {
            Recipe cutter = CreateRecipe();//the result is cutter
            cutter.AddIngredient(ModContent.ItemType<Starbit>(), 10); //10 starbits
            cutter.AddIngredient(ItemID.WoodenBoomerang);
            cutter.AddTile(TileID.Anvils); //crafted at anvil
            cutter.Register(); //adds this recipe to the game

            cutter = CreateRecipe();//the result is cutter
            cutter.AddIngredient(ModContent.ItemType<Starbit>(), 10); //10 starbits
            cutter.AddIngredient(ItemID.EnchantedBoomerang);
            cutter.AddTile(TileID.Anvils); //crafted at anvil
            cutter.Register(); //adds this recipe to the game
        }
    }
}