using KirboMod.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
    public class FighterGlove : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fighter Glove"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("Right click on the ground to do a uppercut, sending you upwards" +
				"\nPunching many times RIGHT before uppercutting will make it stronger"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
        }
        static int UseTime => 5;
        static float ShootSpeed => 20f;
        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
            Item.width = 20; //world dimensions
            Item.height = 20;
            Item.useTime = UseTime;
            Item.useAnimation = Item.useTime;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 1;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.buyPrice(0, 0, 6, 30);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.FlyingPunch>();
            Item.shootSpeed = ShootSpeed;
            Item.ArmorPenetration = 5;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            KirbPlayer kplr = player.GetModPlayer<KirbPlayer>();
            if (player.altFunctionUse == 2)
            {
                if (Main.myPlayer == player.whoAmI)
                {
                    FighterUppercut.GetAIValues(player, 0.5f, out float ai1);
                    Projectile.NewProjectile(source, position, velocity, type, damage, knockback, -1, 0, ai1);
                }
                kplr.fighterComboCounter = 0;
                return false;
            }
            return true;
        }
        public static int GetDamageScaledByComboCounter(Player player, int damage, float scalingMult)
        {
            KirbPlayer kplr = player.GetModPlayer<KirbPlayer>();
            return (int)MathF.Max(damage,((kplr.fighterComboCounter + 1) * scalingMult * damage));
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2) //right click
            {
                damage = GetDamageScaledByComboCounter(player, damage, 0.5f);
                position.X += player.direction * 40;
                position.Y += -30;
                knockback = 12;
            }
            else //left click
            {
                position.Y -= 2;
                velocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
            }
        }

        public override bool AltFunctionUse(Player player)
        {
            return true; //can right click
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useTime = 50;
                Item.useAnimation = 50;
                Item.shoot = ModContent.ProjectileType<Projectiles.FighterUppercut>();
                Item.shootSpeed = 0.0001f; //make it very small but not immobile
                Item.useStyle = ItemUseStyleID.HoldUp;
                player.mount.Dismount(player); //dismount mounts
                                               //player.velocity.Y = -10;
                                               //player velocity is handled in projectile
                return true; // usedUppercut = false; //checks if used uppercut is false	
            }
            else //left click
            {
                Item.useTime = UseTime;
                Item.useAnimation = UseTime;
                Item.shoot = ModContent.ProjectileType<Projectiles.FlyingPunch>();
                Item.shootSpeed = ShootSpeed;
                Item.useStyle = ItemUseStyleID.Rapier;
                return true;
            }
        }
    }
}