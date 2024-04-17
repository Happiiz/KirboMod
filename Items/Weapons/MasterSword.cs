using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
    public class MasterSword : ModItem
    {
        private Vector2 dash = Main.MouseWorld;
        private bool canUseDash = false;
        private int dashCooldown = 0;
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

        public override void SetDefaults()
        {
            Item.damage = 310;
            Item.crit += 10;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(0, 25, 0, 0);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.MasterSwing>();
            Item.shootSpeed = 80f; //only used for distance of second attack
            Item.noUseGraphic = true; //kinda like arkhailis
            Item.noMelee = true; //dont have a melee hitbox(the projectile IS the melee hitbox)
            Item.channel = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Swing like arkhailis
            if (player.altFunctionUse != 2)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.MasterSwing>()] < 1)//if one is not already out
                {
                    //Swing
                    Projectile.NewProjectile(source, player.Center.X, player.Center.Y, 0, 0, type, damage, knockback, player.whoAmI);
                }
            }
            else //Right click
            {
                //Sword
                Projectile.NewProjectile(source, player.Center.X, player.Center.Y, velocity.X, velocity.Y, type, 0, knockback, player.whoAmI);

                //Dash fire 
                Projectile.NewProjectile(source, player.Center, player.velocity, ModContent.ProjectileType<Projectiles.MasterDash>(), damage * 10, knockback, player.whoAmI);
            }
            return false;
        }

        public override bool AltFunctionUse(Player player)
        {
            if (canUseDash) //is jumping while not have used dash yet
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2) //right click
            {
                Item.useTime = 60; //30 seconds attack, 30 second cooldown
                Item.useAnimation = 60; //30 seconds attack, 30 second cooldown
                Item.knockBack = 12;
                Item.UseSound = SoundID.Item74; //inferno explosion
                Item.shoot = ModContent.ProjectileType<Projectiles.MasterSwordProj>();
                Item.channel = false;
            }
            else
            {
                Item.useTime = 5;
                Item.useAnimation = 5;
                Item.knockBack = 6;
                Item.UseSound = SoundID.Item1;
                Item.shoot = ModContent.ProjectileType<Projectiles.MasterSwing>();
                Item.channel = true;
            }
            //Ensures no more than one sword can be thrown out, use this when using autoReuse
            return player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.MasterSwordProj>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.MasterDash>()] < 1;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                dashCooldown = 60;
            }

            return true;
        }

        public override void UpdateInventory(Player player)
        {
            if (dashCooldown > 0) //go down
            {
                dashCooldown--;
            }

            if (player.itemAnimation >= player.itemAnimationMax / 2 && player.altFunctionUse == 2)
            {
                //damage reduction
                player.endurance += 0.5f; //+50%

                DisableExtraMovement(player);
            }
        }

        public override void HoldItem(Player player)
        {
            //stuff for checking if jumpSpeed or velocity Y is not stagnent
            bool airborne = player.velocity.Y != 0f;

            //criteria for dashing

            if (airborne == true && dashCooldown <= 0) //able to use dash
            {
                canUseDash = true;
            }
            else //on ground 
            {
                canUseDash = false;
            }

            //ACTUAL DASH FUNCTION

            if (player.itemAnimation > 0 && player.altFunctionUse == 2) //still attacking
            {
                //RIGHT CLICK DASH

                if (player.ItemAnimationJustStarted) //inital strike (one less than useAnimation)       
                {
                    dash = Main.MouseWorld - player.Center;
                    dash.Normalize(); //reduce to a unit of 1
                    dash *= 24; //make a speed of 24
                    player.velocity = dash;

                    player.immuneTime = player.itemAnimationMax / 2; //for invincibility timer
                }
                else if (player.itemAnimation >= player.itemAnimationMax / 2) //only happen during attack (not including inital)
                {
                    //detect if touching surface
                    for (int i = 0; i < player.width + 2; i++)
                    {
                        bool foundCollision = false;

                        for (int j = 0; j < player.height + 2; j++)
                        {
                            Point tileposition = new Vector2(player.position.X - 1 + i, player.position.Y - 1 + j).ToTileCoordinates();

                            if (WorldGen.SolidOrSlopedTile(Main.tile[tileposition]))
                            {
                                for (int k = 0; k < 120; k++)
                                {
                                    Vector2 speed = Main.rand.NextVector2Circular(50f, 50f);

                                    Dust d = Dust.NewDustPerfect(player.Center, DustID.SolarFlare, speed, Scale: 2f);
                                    d.noGravity = true;

                                    SoundEngine.PlaySound(SoundID.Item100, player.Center);
                                }

                                player.itemAnimation = player.itemAnimationMax / 2; //stop main attack

                                foundCollision = true;

                                break;
                            }
                        }

                        if (foundCollision == true)
                        {
                            break;
                        }
                    }

                    player.velocity = dash; //keep moving the way you were

                    if (player.dash != 0)
                    {
                        player.dash = 0; //disable dashing
                    }

                    player.maxFallSpeed = 24;

                    if (player.itemAnimation % 5 == 0) //every multiple of 5
                    {
                        SoundEngine.PlaySound(SoundID.Item20, player.Center); //fire cast
                    }
                }
                else if (player.itemAnimation < player.itemAnimationMax / 2) //after attack
                {
                    player.fullRotation = 0;
                }
            }
        }

        private void DisableExtraMovement(Player player)
        {
            //vvv Disabling other movement options vvv

            player.maxFallSpeed = 20;
            player.noKnockback = true;
            player.dashType = 0;

            player.canRocket = false;
            player.carpet = false;
            player.carpetFrame = -1;

            //disable kirby balloon
            player.GetModPlayer<KirbPlayer>().kirbyballoon = false;
            player.GetModPlayer<KirbPlayer>().kirbyballoonwait = 1;

            //double jump effects
            player.blockExtraJumps = true;

            player.DryCollision(true, true); //fall through platforms

            player.mount.Dismount(player); //dismount mounts

            //stop grappling
            player.grappling[0] = 0;
            player.grapCount = 0;
            for (int k = 0; k < 1000; k++)
            {
                if (Main.projectile[k].active && Main.projectile[k].owner == player.whoAmI && Main.projectile[k].aiStyle == 7)
                {
                    Main.projectile[k].Kill(); //kill grapple hook
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; // Makes it uneffected by light
        }

        public override void AddRecipes()
        {
            Recipe recipe1 = CreateRecipe();//the result is mastersword
            recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.MetaKnightSword>()); //Galaxia
            recipe1.AddIngredient(ModContent.ItemType<Items.RainbowSword.RainbowSword>()); //Rainbow Sword
            recipe1.AddIngredient(ModContent.ItemType<Items.DarkSword.DarkSword>()); //Dark Sword
            recipe1.AddIngredient(ModContent.ItemType<MiracleMatter>()); //Zero material drop
            recipe1.AddTile(TileID.LunarCraftingStation); //crafted at ancient manipulator
            recipe1.Register(); //adds this recipe to the game
        }
    }
}