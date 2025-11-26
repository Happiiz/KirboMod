using KirboMod.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace KirboMod.Items.Ammo
{
    public class NightStarAmmo : ModItem
    {
        public override void SetStaticDefaults()
        {
            AmmoID.Sets.IsSpecialist[Type] = true;
            // DisplayName.SetDefault("Star Bullet");
            // Tooltip.SetDefault("Gracefully flies through the air");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 20; //amount needed to research
            Main.RegisterItemAnimation(Type, new VFX.NightmareItemAnimation());
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CannonNightStar.StarCannonSlashDamageMult);
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.value = Item.buyPrice(0, 0, 5, 0);
            Item.rare = ItemRarityID.Pink;
            Item.ammo = AmmoID.FallenStar;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 1;
            Item.shoot = ModContent.ProjectileType<Projectiles.CannonNightStar>();
            Item.shootSpeed = 20f;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(6);
            recipe.AddIngredient(ModContent.ItemType<NightCloth>(), 1);
            recipe.AddIngredient(ItemID.FallenStar, 6);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
