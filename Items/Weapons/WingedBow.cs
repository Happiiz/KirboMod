using KirboMod.Projectiles.WingedBow;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
    public class WingedBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.width = 30;
            Item.height = 50;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item5; //bow shot
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.WingedBow.WingedBullet>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if(Main.myPlayer != player.whoAmI)
            {
                return false;
            }

            if (Main.rand.NextBool(5))
            {
                type = ModContent.ProjectileType<WingedSlash>();
                velocity *= 2f;
                damage *= 3;
                knockback /= 2;

                int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, player.direction);

                //this won't sync in multiplayer
                //use AI slot instead
                //WingedSlash proj = Main.projectile[p].ModProjectile as WingedSlash;
                //proj.initialShootDirection = player.direction;
            }
            else
            {
                velocity = velocity.RotatedByRandom(MathF.PI / 8);

                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            }

            return false;
        }

        public override void AddRecipes()
        {
            Recipe wingedBow = CreateRecipe();
            wingedBow.AddIngredient(ItemID.HellwingBow);
            wingedBow.AddIngredient(ModContent.ItemType<BirdonFeather>(), 15);
            wingedBow.AddTile(TileID.MythrilAnvil);
            wingedBow.Register();
        }
    }
}