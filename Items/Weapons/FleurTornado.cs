using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
    public class FleurTornado : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fleur Tornado Ring"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("Control a tornado by holding down the mouse" +
				"\nSplits into multiple feathers upon death"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
        }

        public override void SetDefaults()
        {
            Item.damage = 24;
            Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
            Item.width = 50;
            Item.height = 42;
            Item.useTime = 90;
            Item.useAnimation = 90;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(0, 0, 45, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item34;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<Projectiles.FleurTornadoNado>();
            Item.shootSpeed = 0f;
            Item.mana = 10;
            Item.channel = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if(Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, Item.mana, 0);
            }
            return false;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position.Y -= 50f; //spawn above player
        }

        public override bool CanUseItem(Player player)
        {
            return player.statMana >= player.statManaMax2 && player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void AddRecipes()
        {
            Recipe recipe1 = CreateRecipe();//the result is gigantsword
            recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.Tornado>()); //Tornado
            recipe1.AddIngredient(ItemID.CrystalVileShard); //Crystal Vile Shard
            recipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 50); //50 starbits
            recipe1.AddIngredient(ModContent.ItemType<RareStone>(), 1); //1 rare stone
            recipe1.AddTile(TileID.Anvils); //crafted at anvil
            recipe1.Register(); //adds this recipe to the game
        }
    }
}