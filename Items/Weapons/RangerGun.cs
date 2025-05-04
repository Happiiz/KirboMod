using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
    public class RangerGun : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

        public override void SetDefaults()
        {
            Item.damage = 33;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.width = 35;
            Item.height = 20;
            Item.useTime = 31;
            Item.useAnimation = Item.useTime;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 8;
            Item.value = Item.buyPrice(0, 0, 30, 5);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.RangerStar>();
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type != ModContent.ProjectileType<Projectiles.StarBulletProj>())
            {
                type = ModContent.ProjectileType<Projectiles.RangerStar>();
            }
            else
            {
                position.Y -= 10; //start higher when charged
                velocity *= 1.3f;
            }

            position = player.Center + velocity * 3;//move from player apon spawning
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundEngine.PlaySound(SoundID.Item14 with { Pitch = 1f, MaxInstances = 0 }, position);
            if (type != ModContent.ProjectileType<Projectiles.StarBulletProj>())
            {
                return true;
            }
            else
            {
                Projectile.NewProjectile(source, position.X, position.Y - 15, velocity.X, velocity.Y, ModContent.ProjectileType<Projectiles.BigRangerStar>(), (int)(damage * 1.5), 5, Item.playerIndexTheItemIsReservedFor, 0, 0);
                return false;
            }
        }

        public override void AddRecipes()
        {
            Recipe rangergunrecipe1 = CreateRecipe();//the result is rangergun
            rangergunrecipe1.AddIngredient(ItemID.Musket); //Musket
            rangergunrecipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 20); //20 starbits
            rangergunrecipe1.AddTile(TileID.Anvils); //crafted at anvil
            rangergunrecipe1.Register(); //adds this recipe to the game

            //alternate recipe for crimson worlds
            Recipe rangergunrecipe2 = CreateRecipe();//the result is rangergun
            rangergunrecipe2.AddIngredient(ItemID.TheUndertaker); //Undertaker
            rangergunrecipe2.AddIngredient(ModContent.ItemType<Items.Starbit>(), 20); //20 starbits
            rangergunrecipe2.AddTile(TileID.Anvils); //crafted at anvil
            rangergunrecipe2.Register(); //adds this recipe to the game
        }
    }
}