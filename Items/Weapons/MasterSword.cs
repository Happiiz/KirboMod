using Microsoft.Xna.Framework;
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
        private bool usedDash = false;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Master Sword"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("While airborne, right-click a direction to do a invincible dash in that direction" +
				"\nCan only dash in the air once" +
				"\n'Is this the sword that seals the darkness?'"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

        public override void SetDefaults()
        {
            Item.damage = 204; //april 2004
            Item.crit += 10;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(0, 25, 0, 0);
            Item.rare = ItemRarityID.Red;
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

        public override void HoldItem(Player player)
        {
            //stuff for checking if jumpSpeed or velocity Y is not stagnent (this took waaaaaay too long to figure out!)
            bool airborne = player.velocity.Y != 0f;

            //criteria for dashing

            if (airborne == false) //not in air
            {
                usedDash = false; //reset usedDash
            }

            if (airborne == true & usedDash == false) //jumping while not have used dash yet
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

                if (player.itemAnimation >= player.itemAnimationMax - 1) //inital strike (one less than useAnimation)       
                {
                    dash = Main.MouseWorld - player.Center;
                    dash.Normalize(); //reduce to a unit of 1
                    dash *= 12; //make a speed of 12
                    player.velocity = dash;

                    usedDash = true; //can't use dash anymore 'til on ground

                    player.immuneTime = player.itemAnimationMax - (player.itemAnimationMax / 2); //for invincibility timer
                }
                else if (player.itemAnimation >= player.itemAnimationMax - (player.itemAnimationMax / 2)) //only happen during attack (not including inital)
                {
                    player.velocity = dash; //keep moving the way you were
                }
                else if (player.itemAnimation < player.itemAnimationMax - (player.itemAnimationMax / 2)) //after attack
                {
                    player.fullRotation = 0;
                }
                //if max 60, then this 30
                if (player.itemAnimation >= player.itemAnimationMax - (player.itemAnimationMax / 2)) //only happen during attack (including inital)
                {
                    if (player.dash != 0)
                    {
                        player.dash = 0; //disable dashing
                    }

                    //invincibility (timer above)
                    player.immune = true;
                    player.immuneNoBlink = true;

                    //extra...
                    player.maxFallSpeed = 20;
                    player.noKnockback = true;
                    player.GetJumpState(ExtraJump.BlizzardInABottle).Available = false;
                    player.GetJumpState(ExtraJump.CloudInABottle).Available = false;
                    player.GetJumpState(ExtraJump.SandstormInABottle).Available = false;
                    player.GetJumpState(ExtraJump.TsunamiInABottle).Available = false;
                    player.GetJumpState(ExtraJump.FartInAJar).Available = false;
                    player.dash = 0;

                    player.canRocket = false;
                    player.carpet = false;
                    player.carpetFrame = -1;

                    //disable kirby balloon
                    player.GetModPlayer<KirbPlayer>().kirbyballoon = false;
                    player.GetModPlayer<KirbPlayer>().kirbyballoonwait = 1;

                    //double jump effects
                    player.GetJumpState(ExtraJump.BlizzardInABottle).Disable();/* tModPorter Suggestion: Call Enable() if setting this to true, otherwise call Disable(). */;
                    player.GetJumpState(ExtraJump.CloudInABottle).Disable()/* tModPorter Suggestion: Call Enable() if setting this to true, otherwise call Disable(). */;
                    player.GetJumpState(ExtraJump.SandstormInABottle).Disable()/* tModPorter Suggestion: Call Enable() if setting this to true, otherwise call Disable(). */;
                    player.GetJumpState(ExtraJump.TsunamiInABottle).Disable()/* tModPorter Suggestion: Call Enable() if setting this to true, otherwise call Disable(). */;
                    player.GetJumpState(ExtraJump.FartInAJar).Disable()/* tModPorter Suggestion: Call Enable() if setting this to true, otherwise call Disable(). */;

                    player.DryCollision(true, true); //fall through platforms

                    player.mount.Dismount(player); //dismount mounts

                    //player.fullRotationOrigin
                    //player.fullRotation = player.velocity.ToRotation() + MathHelper.ToRadians(90); //rotate towards dash

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

                    if (player.itemAnimation % 5 == 0) //every multiple of 5
                    {
                        SoundEngine.PlaySound(SoundID.Item1, player.Center);
                    }
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
            recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.MetaKnightSword>()); //Knight Hero Sword
            recipe1.AddIngredient(ModContent.ItemType<Items.RainbowSword.RainbowSword>()); //Epilespy Hero Sword
            recipe1.AddIngredient(ModContent.ItemType<Items.DarkSword.DarkSword>()); //Edgy Hero Sword
            recipe1.AddIngredient(ModContent.ItemType<MiracleMatter>()); //Zero material drop
            recipe1.AddTile(TileID.LunarCraftingStation); //crafted at ancient manipulator
            recipe1.Register(); //adds this recipe to the game
        }
    }
}