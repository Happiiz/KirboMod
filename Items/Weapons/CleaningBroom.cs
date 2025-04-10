using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using SoundEngine = Terraria.Audio.SoundEngine;

namespace KirboMod.Items.Weapons
{
    public class CleaningBroom : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cleaning Broom");
            /* Tooltip.SetDefault("Swing on the ground to kick up dust at your enemies" +
                "\n'Sweep, sweep, sweep!'"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research 
        }

        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
            Item.width = 20;
            Item.height = 20;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4;
            Item.value = Item.buyPrice(0, 0, 0, 20);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.CleaningBroomDustCloud>();
            Item.shootSpeed = 5f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false; //no vanilla shooting
        }

        public override void HoldItem(Player player)
        {
            //swing only at last quarter while on ground and using item
            if (player.itemTime == player.itemTimeMax / 4 && player.velocity.Y == 0 && !player.ItemTimeIsZero && Main.myPlayer == player.whoAmI)
            {
                Vector2 velocity = Main.MouseWorld - player.Center;
                velocity.Normalize();
                velocity *= Item.shootSpeed;

                Vector2 position = new(player.Center.X + (20 * player.direction), player.Center.Y + 10);

                Projectile.NewProjectile(new EntitySource_ItemUse(Main.player[player.whoAmI], player.HeldItem), position,
                    velocity, Item.shoot, Item.damage, Item.knockBack, player.whoAmI);
                SoundEngine.PlaySound(SoundID.Grass, player.Center);
            }
        }
    }
}