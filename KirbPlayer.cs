using KirboMod.Items;
using KirboMod.Items.Weapons;
using KirboMod.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace KirboMod
{
    public class KirbPlayer : ModPlayer
    {
        public bool whispbush; //for whispy shrub accesory true or false

        public bool kirbyballoon; //for kirby ballon accesory true or false
        public int kirbyballoonwait; //for kirby balloon timer
        public bool kirbyballoonactivated; //if space has been pressed

        public bool nightcloak; // for night cloak accesory true or false

        public bool[] HasTripleStars = {false, false, false}; //targeting
        public int tripleStarRotationCounter = 0;
        public int[] tripleStarIndexes = new int[3];

        public bool nightcrown; //for nightmare crown accesory true or false
        public bool nightmareeffect; // nightmare crown effect

        public bool darkShield; //checking if wearing dark shield at all times
        public int darkDashHitboxTimer = 0; // duration of hitbox
        public int darkDashTimeWindow = 0; // the window a player has in input a second time, activating the dash. Just set to 15
        public int darkDashDelay = 0; // how long before you can dash again, becomes 20 when -1
        public bool darkDash; //check if have dark dash availiable
        public int darkDashTime; //how long do afterimages linger
        public bool hasdarkShield; //additional check that only updates when not dashing

        public int fighterComboCounter = 0; //combo Counter to determine strength of uppercut in regular fighter glove
        public int fighterComboResetDelay = 0; //delay until combo Counter resets

        public bool personalcloud; //checks if have personal cloud accesory
        public bool personalcloudalive = false; //checks if personal cloud is alive

        public bool royalslippers;
        public int falltime = 0;

        public bool hyperzone;

        public bool zeroEyePet;

        public bool whispyPet;
        public bool krackoPet;
        public bool kingDededePet;
        public bool nightmarePet;
        public bool darkMatterPet;
        public bool zeroPet;

        public bool badgeofgloom; //for badge of gloom accesory true or false
        public int gloombadgeattackcount = 0;

        public int plasmaTimer = 0; //if 60 then takes plasma charge down a bit
        public int plasmacharge = 0;  //charge amount for plasma weapon

        public int rainbowSwordSwingCounter;
        public int NextRainbowSwordSwingDirection { get => rainbowSwordSwingCounter++ % 2 * 2 - 1; }
        public override void ResetEffects() //restart accesory stats so if not wearing one then it stops doing the effects
        {
            whispbush = false;
            kirbyballoon = false;
            nightcloak = false;
            nightcrown = false;
            nightmareeffect = false;
            darkShield = false;
            personalcloud = false;
            royalslippers = false;
            badgeofgloom = false;

            zeroEyePet = false;
            whispyPet = false;
            krackoPet = false;
            zeroPet = false;
            nightmarePet = false;
            kingDededePet = false;
            darkMatterPet = false;
            zeroPet = false;
        }

        public override void PreUpdate()
        {
            kirbyballoonwait -= 1;

            //fighter glove
            if (fighterComboResetDelay > 0) //go down 'til 0
            {
                fighterComboResetDelay -= 1;
            }
            
            if (fighterComboResetDelay == 0 && fighterComboCounter != 0) //go down
            {
                fighterComboCounter--;
            }

            //plasmacharge minimum
            if (plasmacharge < 0)
            {
                plasmacharge = 0;
            }            

            darkDashTime -= 1;

            if (darkDashTime < 0)
            {
                darkDashTime = 0;
            }
        }
        public TripleStarStar GetAvailableTripleStarStar()
        {
            TripleStarStar availableStar;
            for (int i = 0; i < tripleStarIndexes.Length; i++)
            {
                if (tripleStarIndexes[i] != -1)
                {
                    availableStar = Main.projectile[tripleStarIndexes[i]].ModProjectile as TripleStarStar;
                    if (availableStar.AvailableForUse)
                    {
                        return availableStar;
                    }
                }
            }
            return null;
        }
        public override void PostUpdate()
        {
            Player player = Main.player[Main.myPlayer];

            if (darkDashDelay < 0) //dashing
            {
                player.armorEffectDrawShadow = true; //afterimages
            }

            //ground pounding with royal slippers
            if (royalslippers == true && player.controlDown && player.velocity.Y != 0 && player.controlUp == false && player.controlJump == false)
            {
                player.armorEffectDrawShadow = true; //afterimages
            }

            //TRIPLE STAR STARS
            tripleStarRotationCounter += 1;
           
            int tripleStarID = ModContent.ItemType<TripleStar>();
            if (player.HeldItem.type == tripleStarID && !player.dead && player.active)
            {
                float finalDamage = player.GetTotalDamage(Player.HeldItem.DamageType).ApplyTo(Player.HeldItem.damage); //final damage calculated
                for (int i = 0; i < tripleStarIndexes.Length; i++)
                {   
                    if (tripleStarIndexes[i] == -1 || !Main.projectile[tripleStarIndexes[i]].active)
                    {
                        tripleStarIndexes[i] = Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                                ModContent.ProjectileType<TripleStarStar>(), (int)finalDamage, 0, player.whoAmI, 0, 0);
                    }
                    else
                    {
                        Projectile tripleStar = Main.projectile[tripleStarIndexes[i]];
                        tripleStar.timeLeft = 2;
                    }
                }
            }
            else
            {
                for (int i = 0; i < tripleStarIndexes.Length; i++)
                {
                    if (tripleStarIndexes[i] != -1)
                    {
                        Main.projectile[tripleStarIndexes[i]].Kill();
                        tripleStarIndexes[i] = -1;
                    }
                }
            }
            //holding Triple Star Rod
            //if (player.HeldItem.type == tripleStar && !player.dead && player.active) //holding Triple Star Rod
            //{
            //    for (int i = 0; i < HasTripleStars.Length; i++)
            //    {
            //        tripleStarIndexes[i] = -1;
            //    }
            //    //for (int i = 0; i < player.ownedProjectileCounts[tripleStarStar]; i++)
            //    //{
            //    //    HasTripleStars[i] = false;
            //    //}
            //    for (int i = 0; i < 3; i++) //to 3
            //    {
            //        float finalDamage = player.GetTotalDamage(Player.HeldItem.DamageType).ApplyTo(Player.HeldItem.damage); //final damage calculated

            //        if (tripleStarIndexes[i] == -1) //checks each one to see if it has the triple stars
            //        {

            //            tripleStarIndexes[i] = Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
            //                ModContent.ProjectileType<CyclingStar>(), (int)finalDamage, 0, player.whoAmI, 0, i);
            //        }
            //    }
            //}
            //else //not holding
            //{
            //    //no stars circle
            //    HasTripleStars[0] = false;
            //    HasTripleStars[0] = false;
            //    HasTripleStars[0] = false;
            //}

            //Plasma

            //Go up
            plasmaTimer++;
            if (plasmaTimer >= 60) //every second not tapping
            {
                plasmacharge--;
                plasmaTimer = 0;
            }

            //beefy shield
            if (plasmacharge >= 12)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, player.velocity * 0, ModContent.ProjectileType<Projectiles.PlasmaShield>(), 64, 0f, player.whoAmI, 0, 0);
            }
            //manlet shield
            else if (plasmacharge >= 3 && plasmacharge < 12)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, player.velocity * 0, ModContent.ProjectileType<Projectiles.PlasmaOrb>(), 32, 0f, player.whoAmI, 0, 0);
            }

            //cap
            if (player.GetModPlayer<KirbPlayer>().plasmacharge >= 15)
            {
                player.GetModPlayer<KirbPlayer>().plasmacharge = 15;
            }
        }

        public override void PostUpdateEquips()
        {
            Player player = Main.player[Main.myPlayer];

            bool airborne = player.velocity.Y != 0f;

            DarkDashMovement(); //dark dash

            //KIRBY BALLOON (checks if already used all double jumps and rockets and player doesn't have mount)
            if (kirbyballoon == true && airborne & player.GetJumpState(ExtraJump.BlizzardInABottle).Available == false & player.GetJumpState(ExtraJump.CloudInABottle).Available == false 
                & player.GetJumpState(ExtraJump.FartInAJar).Available == false & player.GetJumpState(ExtraJump.TsunamiInABottle).Available == false & player.GetJumpState(ExtraJump.SandstormInABottle).Available == false
                & !player.mount.CanHover())
            {
                kirbyballoonwait -= 1; //go down

                player.canRocket = false;
                player.rocketTime = 0;
                player.wingTime = 0;
                player.noFallDmg = true;

                if (kirbyballoonwait <= 0)
                {
                    if (player.controlJump & player.releaseJump) //if jumped and wasn't holding space
                    {
                        player.velocity.Y = -7.5f;
                        SoundEngine.PlaySound(SoundID.SplashWeak, player.Center);
                        kirbyballoonwait = 10;
                    }
                }

                //disable personal cloud if die
                if (personalcloud == true && (player.dead || !player.active))
                {
                    personalcloud = false;
                }
            }

            //Personal Cloud
            if (personalcloud == true && personalcloudalive == false && !player.dead && player.active)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center + new Vector2(0, -50), player.velocity * 0, ModContent.ProjectileType<PersonalCloud>(), 0, 0, player.whoAmI);
               personalcloudalive = true; //no more spawning
            }
            if (personalcloud == false)
            {
                player.GetModPlayer<KirbPlayer>().personalcloudalive = false; //can spawn again
            }

            // NIGHTMARE EFFECT
            if (player.HasBuff(ModContent.BuffType<Buffs.Nightmare>()))
            {
                nightmareeffect = true;
            }

            // NIGHTMARE CROWN
            if (nightcrown == true)
            {
                Point mouselocation = Main.MouseWorld.ToTileCoordinates();

                Point playerlocation = player.position.ToTileCoordinates();

                //teleport if no tiles under mouse and not on a mount and no lava or lihizhard brick wall
                if (!NPC.downedGolemBoss)
                {
                    if (player.controlUp && player.releaseUp && WorldGen.SolidOrSlopedTile(Main.tile[mouselocation.X, mouselocation.Y]) == false
                        && player.mount.Active == false
                        && (Main.tile[mouselocation.X, mouselocation.Y].LiquidType == LiquidID.Lava) == false
                        && (Main.tile[mouselocation.X, mouselocation.Y].WallType != WallID.LihzahrdBrickUnsafe))
                    {
                        //before effect
                        Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<NightCrownEffect>(), 0, 0, player.whoAmI);

                        player.Teleport(Main.MouseWorld, -1);

                        //after effect
                        for (int i = 0; i < 10; i++)
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(5f, 10f); //circle
                            Dust.NewDustPerfect(player.Center, ModContent.DustType<Dusts.NightStar>(), speed, Scale: 1f); //Makes dust in a messy circle
                        }

                        player.AddBuff(ModContent.BuffType<Buffs.Nightmare>(), 720); //gives player nightmare effect for 12 seconds

                        if (nightmareeffect) //has nightmare effect
                        {
                            player.statLife -= player.statLifeMax2 / 10; //subtract life
                            if (player.statLife <= 0) //if dead
                            {
                                PlayerDeathReason deathmessage = PlayerDeathReason.ByCustomReason($"{player.name} couldn't handle the bad thoughts"); //custom death message

                                player.KillMe(deathmessage, 1.0, 0); //kill player with death message
                            }
                        }
                    }
                }
                else //everything else but now you can teleport through lihizhard walls
                {
                    if (player.controlUp && player.releaseUp && WorldGen.SolidOrSlopedTile(Main.tile[mouselocation.X, mouselocation.Y]) == false
                        && player.mount.Active == false
                        && (Main.tile[mouselocation.X, mouselocation.Y].LiquidType == LiquidID.Lava) == false)
                    {
                        //before effect
                        Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<NightCrownEffect>(), 0, 0, player.whoAmI);

                        player.Teleport(Main.MouseWorld, -1);
                        
                        player.AddBuff(ModContent.BuffType<Buffs.Nightmare>(), 720); //gives player nightmare effect for 12 seconds

                        //after effect
                        for (int i = 0; i < 10; i++)
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(5f, 10f); //circle
                            Dust.NewDustPerfect(player.Center, ModContent.DustType<Dusts.NightStar>(), speed, Scale: 1f); //Makes dust in a messy circle
                        }

                        if (nightmareeffect) //has nightmare effect
                        {
                            player.statLife -= player.statLifeMax2 / 10; //subtract life
                            if (player.statLife <= 0) //if dead
                            {
                                PlayerDeathReason deathmessage = PlayerDeathReason.ByCustomReason($"{player.name} couldn't handle the bad thoughts"); //custom death message

                                player.KillMe(deathmessage, 1.0, 0); //kill player with death message
                            }
                        }
                    }
                }
            }

            //ROYAL SLIPPERS

            if (royalslippers == true)
            {
                //holding down ,not holding up and jump, in the air, and not grappling
                if (player.controlDown && player.velocity.Y != 0 && player.controlUp == false && player.controlJump == false)
                {
                    player.velocity.Y = 20;
                    player.noFallDmg = true; //disable fall damage

                    player.DryCollision(true, true); //fall through platforms
                    player.SlopingCollision(true, true); //fall through slopes(?)

                    //stop grappling
                    for (int k = 0; k < 1000; k++)
                    {
                        if (Main.projectile[k].active && Main.projectile[k].owner == player.whoAmI && Main.projectile[k].aiStyle == 7)
                        {
                            Main.projectile[k].Kill(); //kill grapple hook
                        }
                    }

                    falltime++; //go up

                    for (float i = 0; i < player.width; i++) //counter for stuck
                    {
                        Point belowplayer = new Vector2(player.position.X + i, player.position.Y + player.height).ToTileCoordinates(); //all tiles below npc

                        //touching ground while groundpounding
                        if (WorldGen.SolidOrSlopedTile(Main.tile[belowplayer.X, belowplayer.Y]) && falltime >= 5)
                        {
                            Projectile.NewProjectile(null, player.Center.X, player.position.Y + player.height, 0, 0, Mod.Find<ModProjectile>("PlayerSlam").Type, 100, 8f, Main.myPlayer, 0, 0);
                            SoundEngine.PlaySound(SoundID.Item14, player.Center); //bomb

                            falltime = 0; //reset
                        }
                    }
                }

                if (player.velocity.Y == 0)
                {
                    falltime = 0; //reset
                }
            }
            
            //BADGE OF GLOOM

            if (badgeofgloom == true)
            {
                //using item
                if (player.controlUseItem && player.HeldItem.damage > 0 && player.itemTime != 0)
                {
                    if (gloombadgeattackcount >= 20) //reset and shoot projectile
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Dust d = Dust.NewDustPerfect(player.Center, Mod.Find<ModDust>("DarkResidue").Type, Main.rand.NextVector2Circular(5f, 5f), Scale: 1f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }
                        Projectile.NewProjectile(player.GetSource_FromThis(), player.Center + new Vector2( Main.rand.Next(-100, 100), Main.rand.Next(-100, 100)), new Vector2(player.direction * 0.05f, 0), Mod.Find<ModProjectile>("SmallDarkMatterShot").Type, 80, 8f, Main.myPlayer, 0, player.whoAmI);

                        gloombadgeattackcount = 0;
                    }
                    else //add 1
                    {
                        gloombadgeattackcount++;
                    }
                }
            }     
            else //also reset
            {
                gloombadgeattackcount = 0;
            }

            //small things

            if (darkShield == true) //wearing dark shield (keep this at the bottom because it's small)
            {
                if (player.dashType > 0) //this disables vanila dashes
                {
                    player.dashType = 0;
                }
            }
        }

        private void DarkDashMovement()
        {
            Player player = Main.player[Main.myPlayer];

            if (darkDashDelay == 0)
            {
                hasdarkShield = darkShield; //checks if has dark shield
            }
            if (hasdarkShield == false)
            {
                darkDashTime = 0;
                darkDashDelay = 0;
            }

            //collision 
            if (darkDashDelay < 0 && player.whoAmI == Main.myPlayer) //if dashing while using dark shield
            {
                Rectangle rectangle = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - 4.0), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
                for (int i = 0; i < 200; i++)
                {
                    NPC npc = Main.npc[i];

                    //cycle for loop again
                    if (!npc.active || npc.dontTakeDamage || npc.friendly || npc.immune[player.whoAmI] > 0 || (npc.aiStyle == 112 && !(npc.ai[2] <= 1f)) || !player.CanNPCBeHitByPlayerOrPlayerProjectile(npc))
                    {
                        continue; //continue going
                    }

                    Rectangle rect = npc.getRect();
                    if (rectangle.Intersects(rect) && (npc.noTileCollide || player.CanHit(npc)))
                    {
                        float damage = player.GetTotalDamage(DamageClass.Melee).ApplyTo(35f);
                        float knockback = player.GetTotalKnockback(DamageClass.Melee).ApplyTo(9f);
                        bool crit = false;
                        if (player.kbGlove)
                        {
                            knockback *= 2f;
                        }
                        if (player.kbBuff)
                        {
                            knockback *= 1.5f;
                        }
                        if (Main.rand.Next(100) < player.GetTotalCritChance(DamageClass.Melee))
                        {
                            crit = true;
                        }

                        //face direction of X
                        if (player.velocity.X < 0f)
                        {
                            player.direction = -1;
                        }
                        if (player.velocity.X > 0f)
                        {
                            player.direction = 1;
                        }
                        if (player.whoAmI == Main.myPlayer) //deal contact damage
                        {
                            player.ApplyDamageToNPC(npc, (int)damage, knockback, player.direction, crit);
                        }
                    }
                }
            }

            if (darkDashDelay > 0) //dash delay more than zero
            {
                darkDashDelay--; //go down by 1 every frame
            }
            else if (darkDashDelay < 0) //dash delay less than zero 
            {

                player.StopVanityActions();

                //dusts
                for (int i = 0; i < 2; i++)
                {
                    int d = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, ModContent.DustType<Dusts.DarkResidue>(), player.velocity.X * -0.5f, 0f, 0, default(Color), 0.5f);
                    Main.dust[d].position.X += Main.rand.Next(-5, 6);
                    Main.dust[d].position.Y += Main.rand.Next(-5, 6);
                    Main.dust[d].velocity *= 0.2f;
                }

                player.statDefense += 60; //increase defense by 60 for dash duration
                player.noKnockback = true; //disable knockback
                player.vortexStealthActive = false; //turn off vortex stealth

                //slow

                if (player.velocity.X > 12 || player.velocity.X < -12)
                {
                    player.velocity.X *= 0.985f;
                    return;
                }
                float maxspeed = Math.Max(player.accRunSpeed, player.maxRunSpeed);
                if (player.velocity.X > maxspeed || player.velocity.X < -maxspeed)
                {
                    player.velocity.X *= 0.94f; //lasts too long when using Soaring Insignia with no speed boots but hey it happens to SoC too so...
                    return;
                }

                darkDashDelay = 20; //set delay cooldown I guess

                //idk what this solves
                if (player.velocity.X < 0f)
                {
                    player.velocity.X = -maxspeed;
                }
                else if (player.velocity.X > 0f)
                {
                    player.velocity.X = maxspeed;
                }
            }
            else //dash delay equal to 0 (inital)
            {
                if (player.mount.Active || player.dashDelay != 0) //riding mount or (not dark) dashing already
                {
                    return; //end code here
                }

                if (hasdarkShield == true) //only run this code if have dark shield
                {
                    DashHandling(out int dir, out bool initaldash);

                    if (initaldash == true)
                    {
                        player.velocity.X = 16f * dir;

                        //when on ground
                        Point point = (player.Center + new Vector2(player.direction * player.width / 2 + 2, player.gravDir * (float)(-player.height) / 2f + player.gravDir * 2f)).ToTileCoordinates();
                        Point point2 = (player.Center + new Vector2(player.direction * player.width / 2 + 2, 0f)).ToTileCoordinates();
                        if (WorldGen.SolidOrSlopedTile(point.X, point.Y) || WorldGen.SolidOrSlopedTile(point2.X, point2.Y))
                        {
                            player.velocity.X /= 2f;
                        }

                        darkDashDelay = -1;
                        darkDashTime = 10;

                        //dusts
                        for (int i = 0; i < 8; i++)
                        {
                            int d = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, ModContent.DustType<Dusts.DarkResidue>(), player.direction * -5f, Main.rand.Next(-5, 5), 0, default(Color), 0.75f);
                        }
                    }
                }
            }
        }

        private void DashHandling(out int dir, out bool initialdash)
        {
            Player player = Main.player[Main.myPlayer];

            initialdash = false;
            dir = 1;
            if (darkDashTimeWindow > 0)
            {
                darkDashTimeWindow--;
            }
            if (darkDashTimeWindow < 0)
            {
                darkDashTimeWindow++;
            }
            if (player.controlRight && player.releaseRight) //right
            {
                if (darkDashTimeWindow > 0)
                {
                    initialdash = true;
                    dir = 1;
                    darkDashTimeWindow = 0;
                }
                else
                {
                    darkDashTimeWindow = 15;
                }
            }
            else if (player.controlLeft && player.releaseLeft) //left
            {
                if (darkDashTimeWindow < 0)
                {
                    initialdash = true;
                    dir = -1;
                    darkDashTimeWindow = 0;
                }
                else
                {
                    darkDashTimeWindow = -15;
                }
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (whispbush == true)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(3f, 3f); //oval
                    Projectile.NewProjectile(null, Player.Center, speed, ModContent.ProjectileType<SmallApple>(), 10, 6, Player.whoAmI);
                }
            }

            //NIGHT CLOAK
            if (nightcloak == true)
            {
                //Pretty self explanatory
                int damage = Player.HeldItem.damage;
                if (damage < 50)
                {
                    damage = 50;
                }

                //down right
                Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, 7, 7, Mod.Find<ModProjectile>("GoodNightStar").Type, damage, 3f, Player.whoAmI, 0, 0);
                //right
                Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, 10, 0, Mod.Find<ModProjectile>("GoodNightStar").Type, damage, 3f, Player.whoAmI, 0, 0);
                //up right
                Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, 7, -7, Mod.Find<ModProjectile>("GoodNightStar").Type, damage, 3f, Player.whoAmI, 0, 0);
                //up
                Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, 0, -10, Mod.Find<ModProjectile>("GoodNightStar").Type, damage, 3f, Player.whoAmI, 0, 0);
                //up left
                Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, -7, -7, Mod.Find<ModProjectile>("GoodNightStar").Type, damage, 3f, Player.whoAmI, 0, 0);
                //left
                Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, -10, 0, Mod.Find<ModProjectile>("GoodNightStar").Type, damage, 3f, Player.whoAmI, 0, 0);
                //down left
                Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, -7, 7, Mod.Find<ModProjectile>("GoodNightStar").Type, damage, 3f, Player.whoAmI, 0, 0);
                //down
                Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, 0, 10, Mod.Find<ModProjectile>("GoodNightStar").Type, damage, 3f, Player.whoAmI, 0, 0);

                SoundEngine.PlaySound(SoundID.Item4, Player.Center); //life crystal
            }
        }
    }

}
