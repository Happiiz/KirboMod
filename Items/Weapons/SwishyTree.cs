using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
    public class SwishyTree : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Swishy Tree"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("'Swishy Swishy'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
            Item.width = 80;
            Item.height = 80;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 10; //6 is average
            Item.value = Item.buyPrice(0, 0, 0, 60);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.SmolApple>();
            Item.shootSpeed = 12f;
            Item.useTurn = false; //can't turn around while using
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if(Main.myPlayer != player.whoAmI)
            {
                return false;
            }

            Projectile.NewProjectile(source, position, new Vector2(velocity.X, velocity.Y), type, damage, 3f);
            Projectile.NewProjectile(source, position, new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.ToRadians(15f)), type, damage, 3f);
            Projectile.NewProjectile(source, position, new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.ToRadians(-15f)), type, damage, 3f);
            return false;
        }
    }
}