using KirboMod.Projectiles.Artist;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace KirboMod.Items.Weapons
{
    public class ArtistsBrush : ModItem
    {
        List<Color> availableColors = [Color.Red, Color.Orange, Color.Yellow, Color.DeepPink, Color.Lime, Color.Blue, Color.BlueViolet];

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

        public override void SetDefaults()
        {
            Item.damage = 32;
            Item.DamageType = DamageClass.Melee;
            Item.width = 80;
            Item.height = 80;
            Item.useTime = 15;
            Item.useAnimation = Item.useTime; 
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7; //6 is average
            Item.value = Item.buyPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PaintBlob>();
            Item.shootSpeed = 8f;
            Item.useTurn = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if(Main.myPlayer != player.whoAmI)
            {
                return false;
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2 newVel = velocity.RotatedByRandom(MathF.PI / 4);
                Projectile.NewProjectile(source, position + (newVel * 4), newVel, type, damage, 1f);
            }

            return false;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(6f, 6f); //circle
                Dust d = Dust.NewDustPerfect(target.Center, DustID.Marble, speed, 
                    Scale: 1f, newColor: availableColors[Main.rand.Next(availableColors.Count)]); //Makes dust in a messy circle
            }

            Vector2 velocity = Main.rand.NextVector2Circular(14f, 14f); //circle
            Projectile.NewProjectile(player.GetSource_OnHit(target), target.Center, velocity, 
                ModContent.ProjectileType<Paintings>(), damageDone * 3, 5f);

            SoundEngine.PlaySound(SoundID.Item177, target.Center);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(6f, 6f); //circle
                Dust d = Dust.NewDustPerfect(target.Center, DustID.Marble, speed,
                    Scale: 1f, newColor: availableColors[Main.rand.Next(availableColors.Count)]); //Makes dust in a messy circle
            }

            Vector2 velocity = Main.rand.NextVector2Circular(14f, 14f); //circle
            Projectile.NewProjectile(player.GetSource_OnHit(target), target.Center, velocity,
                ModContent.ProjectileType<Paintings>(), hurtInfo.Damage * 3, 5f);

            SoundEngine.PlaySound(SoundID.Item177, target.Center);
        }

        public override void AddRecipes()
        {
            Recipe artistsbrush = CreateRecipe();
            artistsbrush.AddIngredient(ModContent.ItemType<DreamEssence>(), 20);
            artistsbrush.AddIngredient(ItemID.SoulofLight, 10);
            artistsbrush.AddIngredient(ItemID.Paintbrush);
            artistsbrush.AddTile(TileID.MythrilAnvil);
            artistsbrush.Register();
        }
    }
}