using KirboMod.Buffs;
using KirboMod.Items.Accesories;
using KirboMod.Items.Armor.Photonic0Boots;
using KirboMod.Items.Weapons;
using KirboMod.Mounts;
using KirboMod.NPCs;
using KirboMod.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace KirboMod
{
    public class KirbPlayer : ModPlayer
    {
        /// <summary>
        /// for checking right clicks of each player because player.channel does not exist for right click.
        /// </summary>
        public static bool[] playerRightClicks = new bool[Main.maxPlayers];
        public bool RightClicking { get => playerRightClicks[Player.whoAmI]; }

        public int hammerCharge = 0;

        public bool whispbush; //for whispy shrub accesory true or false

        public bool kirbyballoon; //for kirby ballon accesory true or false
        public int kirbyballoonwait; //for kirby balloon timer
        public bool kirbyballoonactivated; //if space has been pressed

        public bool nightcloak; // for night cloak accesory true or false

        public bool[] HasTripleStars = { false, false, false }; //targeting
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
        public const int MaxFighterComboCounter = 100;
        public byte fighterKickAndUppercutTimer = 0;//initialize this stuff on spawn of the projectile
        public byte fighterKickAndUppercutType;
        public const byte KickAndUppercutIDFighterGloveUppercut = 0;
        public const byte KickAndUppercutIDHardenedFighterUppercut = 1;
        public const byte KickAndUppercutIDMetalFighterUppercut = 2;
        public const byte KickAndUppercutIDMetalFighterKick = 3;
        /// <summary>
        /// Exists to prevent flying with Fighter Glove in Pre-Hardmode
        /// </summary>
        public int fighterGloveLevelOneCooldown = 0; //cooldown between fighter glove uses in the air for first level only

        public bool airWalkerSet = false;
        public bool airWalkerJump = false;
        public bool blockAirWalkerJump = false;

        public bool personalcloud; //checks if have personal cloud accesory
        public bool personalcloudalive = false; //checks if personal cloud is alive

        public bool royalslippers;
        public int falltime = 0;
        public bool dededeSlam = false; //determines if Player is stomping from mid-air

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

        int plasmaChargeDecayTimer = 0; //if 60 then takes plasma charge down a bit
        int plasmaCharge = 0;  //charge amount for plasma weapon
        public const int maxPlasmaCharge = 20;
        public const int plasmaShieldRadiusSmall = 48;
        public const int plasmaShieldRadiusLarge = 80;
        bool plasmaReleaseLeft = false;
        bool plasmaReleaseRight = false;
        bool plasmaReleaseUp = false;
        bool plasmaReleaseDown = false;
        public int PlasmaShieldRadius { get => plasmaCharge < 3 ? 0 : plasmaCharge < 12 ? plasmaShieldRadiusSmall : plasmaShieldRadiusLarge; }
        /// <summary>
        /// 0: none. 1: small. 2: max
        /// </summary>
        public int PlasmaShieldLevel { get => plasmaCharge < 3 ? 0 : plasmaCharge < 12 ? 1 : 2; }
        public int PlasmaCharge => plasmaCharge;//can't alter outside the method. only read.
        public int customSwordSwingCounter;
        public int NextCustomSwingDirection { get => customSwordSwingCounter++ % 2 * 2 - 1; }
        public int finalCutterAnimationCounter = 0;
        public int finalCutterDamageCounter = 0;//should cap out at 5
        public List<int> currentFinalCutterTargets = new();

        public int ufoMountShootTimer = 0;

        public bool talkedToMarx = false;

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

            airWalkerSet = false;
            blockAirWalkerJump = false;
        }
        public override void PreUpdate()
        {
            kirbyballoonwait -= 1;
            //-1 to compensate when you detect right click it adds to the counter
            if (finalCutterAnimationCounter > -1)
                finalCutterAnimationCounter--;

            //plasmacharge minimum
            if (plasmaCharge < 0)
            {
                plasmaCharge = 0;
            }

            darkDashTime -= 1;

            if (darkDashTime < 0)
            {
                darkDashTime = 0;
            }

            if (fighterComboCounter > MaxFighterComboCounter)
            {
                fighterComboCounter = MaxFighterComboCounter;
            }

            if (Player.dead) //reset fighter counter if dead
            {
                fighterComboCounter = 0;
            }

            fighterGloveLevelOneCooldown--;

            if (Player.mount.Type == ModContent.MountType<UFOMount>() && Player.mount.Active)
            {
                ufoMountShootTimer++;
            }
            else
            {
                ufoMountShootTimer = 0;
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
            //UpdateRightClicksArray();
            if (darkDashDelay < 0) //dashing
            {
                Player.armorEffectDrawShadow = true; //afterimages
            }

            //ground pounding with royal slippers
            if (dededeSlam == true || Player.mount.Type == ModContent.MountType<WheelieBike>())
            {
                Player.armorEffectDrawShadow = true; //afterimages
            }

            if (Player.velocity.Y == 0) //reset
            {
                dededeSlam = false;
                fighterGloveLevelOneCooldown = 0;
            }


            //TRIPLE STAR STARS
            tripleStarRotationCounter += 1;

            int tripleStarID = ModContent.ItemType<TripleStar>();
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                if (Player.HeldItem.type == tripleStarID && !Player.dead && Player.active)
                {
                    float finalDamage = Player.GetTotalDamage(Player.HeldItem.DamageType).ApplyTo(Player.HeldItem.damage); //final damage calculated
                    for (int i = 0; i < tripleStarIndexes.Length; i++)
                    {
                        if (tripleStarIndexes[i] == -1 || !Main.projectile[tripleStarIndexes[i]].active)
                        {
                            tripleStarIndexes[i] = Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                                    ModContent.ProjectileType<TripleStarStar>(), (int)finalDamage, 0, Player.whoAmI, 0, 0);
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
            }
            else if (Player.HeldItem.type == tripleStarID && !Player.dead && Player.active)
            {
                if ((int)Main.timeForVisualEffects % 20 == 0 && Player.whoAmI == Main.myPlayer)//hits are local anyway so
                {
                    int finalDamage = (int)Player.GetTotalDamage(Player.HeldItem.DamageType).ApplyTo(Player.HeldItem.damage);
                    bool crit = Main.rand.Next(100) < Player.GetCritChance(Player.HeldItem.DamageType) + Player.HeldItem.crit;
                    float kb = Player.GetKnockback(Player.HeldItem.DamageType).ApplyTo(Player.HeldItem.knockBack);
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (Helper.CheckCircleCollision(npc.Hitbox, Player.Center, 100))
                        {
                            npc.SimpleStrikeNPC(finalDamage, MathF.Sign(npc.Center.X - Player.Center.X), crit, kb, Player.HeldItem.DamageType, false, Player.luck);
                        }
                    }
                }
                Dust d = Dust.NewDustPerfect(Player.Center + Main.rand.NextFloat(MathF.Tau).ToRotationVector2() * 100, DustID.BlueTorch, Player.velocity, 0, default, 2);
                d.noGravity = true;
            }
            //Plasma

            UpdatePlasmaCharge();

            UpdateRightClicksArray();
            UpdateFinalCutter();
        }
        public override void PreUpdateMovement()
        {
            UpdateFighterManeuvers();
        }
        private void UpdateRightClicksArray()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                playerRightClicks[Main.myPlayer] = Main.mouseRight;
            }
            NetMethods.SyncPlayerRightClick(Player);

        }

        public override void PostUpdateEquips()
        {
            bool airborne = Player.velocity.Y != 0f;

            if (Player.controlUseItem)
            {
                finalCutterAnimationCounter++;
            }

            DarkDashMovement(); //dark dash

            //KIRBY BALLOON (checks if already used all double jumps and rockets and player doesn't have mount)
            if (kirbyballoon == true && airborne && Player.AnyExtraJumpUsable() == false && !Player.mount.CanHover())
            {
                kirbyballoonwait -= 1; //go down

                Player.canRocket = false;
                Player.rocketTime = 0;
                Player.wingTime = 0;
                Player.noFallDmg = true;

                if (kirbyballoonwait <= 0)
                {
                    if (Player.controlJump && Player.releaseJump) //if jumped and wasn't holding space
                    {
                        Player.velocity.Y = -7.5f;
                        SoundEngine.PlaySound(SoundID.SplashWeak, Player.Center);
                        kirbyballoonwait = 10;
                    }
                }


            }
            //disable personal cloud if die
            if (personalcloud && (Player.dead || !Player.active))
            {
                personalcloud = false;
            }
            //Personal Cloud

            if (personalcloud && !Player.dead && Player.active && Player.ownedProjectileCounts[ModContent.ProjectileType<PersonalCloud>()] == 0)
            {
                if (Main.myPlayer == Player.whoAmI)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + new Vector2(0, -50), Player.velocity * 0, ModContent.ProjectileType<PersonalCloud>(), 0, 0, Player.whoAmI);
                }
            }


            // NIGHTMARE EFFECT
            if (Player.HasBuff(ModContent.BuffType<Buffs.Nightmare>()))
            {
                nightmareeffect = true;
            }

            // NIGHTMARE CROWN
            if (nightcrown && Main.myPlayer == Player.whoAmI)
            {
                Point mouselocation = Main.MouseWorld.ToTileCoordinates();

                Point playerlocation = Player.position.ToTileCoordinates();

                //teleport if no tiles under mouse and not on a mount and no lava or lihizhard brick wall
                if (!NPC.downedGolemBoss)
                {
                    if (Player.controlUp && Player.releaseUp && WorldGen.SolidOrSlopedTile(Main.tile[mouselocation.X, mouselocation.Y]) == false
                        && Player.mount.Active == false
                        && (Main.tile[mouselocation.X, mouselocation.Y].LiquidType == LiquidID.Lava) == false
                        && (Main.tile[mouselocation.X, mouselocation.Y].WallType != WallID.LihzahrdBrickUnsafe))
                    {
                        NightCrownTeleport();
                    }
                }
                else //everything else but now you can teleport through lihizhard walls
                {
                    if (Player.controlUp && Player.releaseUp && WorldGen.SolidOrSlopedTile(Main.tile[mouselocation.X, mouselocation.Y]) == false
                        && Player.mount.Active == false
                        && (Main.tile[mouselocation.X, mouselocation.Y].LiquidType == LiquidID.Lava) == false)
                    {
                        NightCrownTeleport();
                    }
                }
            }



            //ROYAL SLIPPERS

            if (royalslippers == true)
            {
                //pressed down, not holding up or jump, and in the air
                if (Player.controlDown && Player.releaseDown && Player.velocity.Y != 0 && Player.controlUp == false && Player.controlJump == false)
                {
                    dededeSlam = true;
                }

                if (dededeSlam)
                {
                    Player.velocity.Y = 20;
                    Player.maxFallSpeed = 20;
                    Player.noFallDmg = true; //disable fall damage

                    //stop grappling
                    for (int k = 0; k < 1000; k++)
                    {
                        if (Main.projectile[k].active && Main.projectile[k].owner == Player.whoAmI && Main.projectile[k].aiStyle == 7)
                        {
                            Main.projectile[k].Kill(); //kill grapple hook
                        }
                    }

                    falltime++; //go up

                    for (float i = 0; i < Player.width; i++)
                    {
                        Point belowPlayer = new Vector2(Player.position.X + i, Player.position.Y + Player.height).ToTileCoordinates(); //all tiles below npc

                        Tile tile = Main.tile[belowPlayer.X, belowPlayer.Y];

                        //touching ground while groundpounding
                        if ((WorldGen.SolidOrSlopedTile(tile) || TileID.Sets.Platforms[tile.TileType]) && falltime >= 5)
                        {
                            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center.X, Player.position.Y + Player.height, Player.direction * 0.01f, 0, ModContent.ProjectileType<PlayerSlam>(), 100 + (falltime - 5), 8f, Main.myPlayer, 0, 0);
                            SoundEngine.PlaySound(SoundID.Item14, Player.Center); //bomb

                            falltime = 0; //reset damage
                        }
                    }

                    if (Player.controlUp || Player.controlJump) //cancel slam
                    {
                        dededeSlam = false;
                    }
                }
            }

            //BADGE OF GLOOM

            if (badgeofgloom == true)
            {
                //using item
                if (Player.controlUseItem && Player.HeldItem.damage > 0 && Player.itemTime != 0)
                {
                    if (gloombadgeattackcount >= 10) //reset and shoot projectile
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Dust d = Dust.NewDustPerfect(Player.Center, Mod.Find<ModDust>("DarkResidue").Type, Main.rand.NextVector2Circular(5f, 5f), Scale: 1f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }

                        int damage = 40 + Player.statDefense / 2;
                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                            Main.rand.NextVector2Circular(20, 20), ModContent.ProjectileType<SmallDarkMatterShot>(), damage, 8f, Main.myPlayer, 0, Player.whoAmI);

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

            //AIR WALKER JUMP

            if (airWalkerSet)
            {
                if (Player.velocity.Y == 0)
                {
                    airWalkerJump = true;
                }
                else
                {
                    //do air walker jump
                    if (Player.controlJump && Player.releaseJump && airWalkerJump == true && blockAirWalkerJump == false 
                        && !Player.blockExtraJumps && !(Player.mount.CanHover() && Player.mount.Active))
                    {
                        Player.velocity.Y = -7.5f;
                        airWalkerJump = false;
                        Player.blockExtraJumps = true; //temporarily disallow other jumps

                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 position = Player.Bottom + new Vector2(-45 + 30 * i, 0); //circle
                            Gore.NewGorePerfect(Player.GetSource_FromThis(), position, Vector2.Zero, Main.rand.Next(11, 13), Scale: 1f); //double jump smoke
                        }

                        for (int i = 0; i < 12; i++)
                        {
                            Vector2 position = Player.Bottom + new Vector2(Main.rand.NextFloat(-50, 50), 0); //circle
                            Dust.NewDustPerfect(position, DustID.Electric); //sparks
                        }

                        SoundEngine.PlaySound(SoundID.DoubleJump, Player.Center);
                    }
                }
            }

            //small things

            if (darkShield) //wearing dark shield (keep this at the bottom because it's small)
            {
                if (Player.dashType > 0) //this disables vanila dashes
                {
                    Player.dashType = 0;
                }
            }
        }

        private void NightCrownTeleport()
        {
            //before effect
            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<NightCrownEffect>(), 0, 0, Player.whoAmI);

            Player.Teleport(Main.MouseWorld, -1);

            Player.AddBuff(ModContent.BuffType<Buffs.Nightmare>(), 720); //gives player nightmare effect for 12 seconds

            //after effect
            for (int i = 0; i < 10; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(5f, 10f); //circle
                Dust.NewDustPerfect(Player.Center, ModContent.DustType<Dusts.NightStar>(), speed, Scale: 1f); //Makes dust in a messy circle
            }

            if (nightmareeffect) //has nightmare effect
            {
                Player.statLife -= Player.statLifeMax2 / 10; //subtract life
                if (Player.statLife <= 0) //if dead
                {
                    PlayerDeathReason deathmessage = PlayerDeathReason.ByCustomReason($"{Player.name} couldn't handle the bad thoughts"); //custom death message

                    Player.KillMe(deathmessage, 1.0, 0); //kill player with death message
                }
            }
        }

        public void GetDarkSwordSwingStats(out int direction, out Items.DarkSword.DarkSword.ProjectileShootType projToShoot)
        {
            projToShoot = (Items.DarkSword.DarkSword.ProjectileShootType)(customSwordSwingCounter % 3);
            direction = customSwordSwingCounter++ % 2 * 2 - 1;
        }
        public bool TryStartingFinalCutter()
        {
            if (finalCutterAnimationCounter > 0)
                return false;
            int horizontalRange = 16 * 4;//4 tiles
            Rectangle hitbox = new((int)Player.Center.X, (int)Player.position.Y, horizontalRange, Player.height);
            if (Player.direction == -1)
            {
                hitbox.X -= horizontalRange;
            }
            currentFinalCutterTargets = new List<int>();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy() && npc.knockBackResist > .2f && !npc.boss && hitbox.Intersects(npc.Hitbox))
                {
                    currentFinalCutterTargets.Add(i);
                }
            }
            if (currentFinalCutterTargets.Count <= 0)
                return false;//failed to start final cutter.
            //ModPacket packet;
            if (currentFinalCutterTargets.Count == 1)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    //StartFinalCutter();
                    return true;
                }
                //packet = Mod.GetPacket(3);
                //packet.Write((byte)KirboMod.ModPacketType.StartFinalCutter);
                //packet.Write((byte)Main.myPlayer);
                //packet.Write((byte)currentFinalCutterTargets[0]);
                //packet.Send(-1, Main.myPlayer);
                return true;
            }
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                //StartFinalCutter();
                return true;
            }
            //packet = Mod.GetPacket();
            //packet.Write((byte)KirboMod.ModPacketType.StartFinalCutterMultiNPC);
            //packet.Write((byte)Main.myPlayer);
            //packet.Write((byte)currentFinalCutterTargets.Count);
            //for (int i = 0; i < currentFinalCutterTargets.Count; i++)
            //{
            //    packet.Write((byte)currentFinalCutterTargets[i]);
            //}
            //packet.Send(-1, Main.myPlayer);
            return false;
        }
        //public void StartFinalCutter()
        //{
        //    finalCutterAnimationCounter = 80;
        //    for (int i = 0; i < currentFinalCutterTargets.Count; i++)
        //    {
        //        //cursed line of code
        //        Main.npc[currentFinalCutterTargets[i]].GetGlobalNPC<KirbNPC>().StartFinalCutter(finalCutterAnimationCounter + 10);
        //    }

        //}
        void UpdateFinalCutter()
        {

            if (finalCutterAnimationCounter <= 0 || currentFinalCutterTargets.Count <= 0)
                return;
            //perhaps dont damage npc and just spawn combat text and play hit sfx?
            for (int i = 0; i < currentFinalCutterTargets.Count; i++)
            {
                NPC target = Main.npc[currentFinalCutterTargets[i]];
                target.Bottom = Player.Bottom;//might not work well with mounts
                target.position.X = Player.Center.X + Player.direction * target.width - target.width / 2;
            }
            int finalCutterJumpStart = 60;
            if (finalCutterAnimationCounter < finalCutterJumpStart)
            {
                float progress = Utils.Remap(finalCutterAnimationCounter, finalCutterJumpStart, 0, -1.53f, 1);

                Player.position.Y += progress * 30;
            }
            if (finalCutterAnimationCounter == 1)//last frame
            {
                Player.velocity = new Vector2(Player.direction * 10, 10);
                for (int i = 0; i < currentFinalCutterTargets.Count; i++)
                {
                    NPC npc = Main.npc[currentFinalCutterTargets[i]];
                    npc.position.Y += 8;
                    npc.SimpleStrikeNPC(100, Player.direction, true, 60, Player.HeldItem.DamageType, true, Player.luck);//boom
                }
                if (Player.whoAmI == Main.myPlayer)
                {
                    Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, new Vector2(Player.direction * 20, 0), ModContent.ProjectileType<Projectiles.Star>(), 100, 11);
                }
            }
        }
        void UpdatePlasmaCharge()
        {
            if (Main.myPlayer != Player.whoAmI)
            {
                return;
            }
            sbyte plasmaChargeAmount = 0;
            plasmaChargeDecayTimer++;
            if (plasmaChargeDecayTimer >= 60)
            {
                plasmaChargeAmount--;
                plasmaChargeDecayTimer = 0;
            }
            if (Player.HeldItem.type != ModContent.ItemType<Plasma>())
            {
                ResetPlasmaCharge();
                return;
            }
            if (Player.controlRight && plasmaReleaseRight)
                plasmaChargeAmount++;
            if (Player.controlLeft && plasmaReleaseLeft)
                plasmaChargeAmount++;
            if (Player.controlUp && plasmaReleaseUp)
                plasmaChargeAmount++;
            if (Player.controlDown && plasmaReleaseDown)
                plasmaChargeAmount++;
            ModifyPlasmaCharge(plasmaChargeAmount);
            plasmaReleaseLeft = Player.releaseLeft;
            plasmaReleaseRight = Player.releaseRight;
            plasmaReleaseUp = Player.releaseUp;
            plasmaReleaseDown = Player.releaseDown;
            if (plasmaCharge > 20)
            {
                plasmaCharge = 20;
            }
            if (plasmaCharge < 3)
            {
                return;//don't try to spawn shield
            }
            bool hasPlasmaShield = Player.ownedProjectileCounts[ModContent.ProjectileType<PlasmaShield>()] > 0;
            if (!hasPlasmaShield)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<PlasmaShield>(), 2, 20, Main.myPlayer, 2);
            }
        }
        public void ModifyPlasmaCharge(sbyte changeAmount)
        {
            if (changeAmount < 0 && plasmaCharge < 0)
            {
                return;
            }
            if (Main.myPlayer == Player.whoAmI && Main.netMode == NetmodeID.MultiplayerClient && changeAmount != 0)
            {
                plasmaCharge += changeAmount;
                NetMethods.SyncPlasmaChargeChange(Player, changeAmount);
                plasmaChargeDecayTimer = 0;
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                plasmaChargeDecayTimer = 0;
                plasmaCharge += changeAmount;
            }
        }
        public void ModifyPlasmaChargeAndResetPlasmaChargeDecayTimer_NoNetMessageSend(sbyte changeAmount)
        {
            plasmaCharge += changeAmount;
            plasmaChargeDecayTimer = 0;
            if (plasmaCharge > 20)
            {
                plasmaCharge = 20;
            }
            if (plasmaCharge < 0)
            {
                plasmaCharge = 0;
            }
        }
        public void ResetPlasmaCharge()
        {
            ModifyPlasmaCharge((sbyte)-plasmaCharge);
        }
        public void ResetPlasmaCharge_NoNetSync()
        {
            plasmaCharge = 0;
            plasmaChargeDecayTimer = 0;
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
                Rectangle rectangle = new((int)(player.position.X + player.velocity.X * 0.5 - 4.0), (int)(player.position.Y + player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
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
                        float damage = player.GetTotalDamage(DamageClass.Melee).ApplyTo(70f);
                        bool crit = false;

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
                            player.ApplyDamageToNPC(npc, (int)damage, 0f, player.direction, crit);
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

                player.noKnockback = true; //disable knockback so it can go through enemies
                player.doorHelper.AllowOpeningDoorsByVelocityAloneForATime(60); //what it says on the tin
                player.vortexStealthActive = false; //turn off vortex stealth

                //slow if going above a certain speed

                if (player.velocity.X > 12 || player.velocity.X < -12)
                {
                    player.velocity.X *= 0.992f; //same slowdown as tabi
                    return;
                }
                float maxspeed = Math.Max(player.accRunSpeed, player.maxRunSpeed);
                if (player.velocity.X > maxspeed || player.velocity.X < -maxspeed)
                {
                    player.velocity.X *= 0.96f; //same slowdown as tabi
                    return;
                }

                darkDashDelay = 20; //set delay cooldown 

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
                        SoundEngine.PlaySound(PureDarkMatter.DashSFX.WithVolumeScale(0.4f), Player.Center);
                        player.velocity.X = 16.9f * dir;

                        //when on ground
                        Point point = (player.Center + new Vector2(player.direction * player.width / 2 + 2, player.gravDir * -player.height / 2f + player.gravDir * 2f)).ToTileCoordinates();
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
            if (whispbush && Player.whoAmI == Main.myPlayer)
            {
                for (int i = 0; i < 4; i++)
                {
                    int damage = 10 + Player.statDefense / 2;
                    Vector2 speed = Main.rand.NextVector2CircularEdge(5f, 5f); //circular spread with constant speed
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, speed, ModContent.ProjectileType<SmallApple>(), damage, 6, Player.whoAmI);
                }
            }

            //NIGHT CLOAK
            if (nightcloak)
            {
                if (info.PvP)
                {
                    NightCloakEffect_Pvp(info);
                }
                else
                {
                    NightCloakEffect_Normal(info);
                }
            }
        }

        public override void SaveData(TagCompound tag)
        {
            if (talkedToMarx)
            {
                tag.Add("TalkedToMarx", talkedToMarx);
            }
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("TalkedToMarx"))
            {
                talkedToMarx = tag.Get<bool>("TalkedToMarx");
            }
        }

        private void NightCloakEffect_Pvp(Player.HurtInfo info)
        {
            if (info.CooldownCounter == ImmunityCooldownID.Lava || info.CooldownCounter == ImmunityCooldownID.TileContactDamage || info.CooldownCounter == ImmunityCooldownID.WrongBugNet)
                return;
            if (Player.whoAmI == Main.myPlayer)
            {
                GetNightCloakValues(info, out int damage, out float numStars, out float starShootSpeed, out int type, out int numStarsCeil, out float ai2);
                for (int i = 0; i < numStarsCeil; i++)
                {
                    float rotation = Utils.Remap(i, 0, numStarsCeil, 0, MathF.Tau);
                    Vector2 vel = rotation.ToRotationVector2() * starShootSpeed;
                    if (i == numStarsCeil - 1)
                    {
                        damage = (int)(damage * (numStars % 1));
                    }

                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.MountedCenter - vel, vel, type, damage, 7f, Main.myPlayer, -1, vel.Length(), ai2);

                }
            }
            NightCloakVisual();
        }
        private void NightCloakEffect_Normal(Player.HurtInfo info)
        {
            if (info.CooldownCounter == ImmunityCooldownID.Lava || info.CooldownCounter == ImmunityCooldownID.TileContactDamage || info.CooldownCounter == ImmunityCooldownID.WrongBugNet)
                return;
            if (Player.whoAmI == Main.myPlayer)
            {
                GetNightCloakValues(info, out int damage, out float numStars, out float starShootSpeed, out int type, out int numStarsCeil, out float ai2);
                for (int i = 0; i < numStarsCeil; i++)
                {
                    float rotation = Utils.Remap(i, 0, numStarsCeil, 0, MathF.Tau);
                    Vector2 vel = rotation.ToRotationVector2() * starShootSpeed;
                    if (i == numStarsCeil - 1)
                    {
                        damage = (int)(damage * (numStars % 1));
                    }
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.MountedCenter - vel, vel, type, damage, 7f, Main.myPlayer, -1, vel.Length(), ai2);

                }
            }
            NightCloakVisual();
        }
        private void NightCloakVisual()
        {
            //visual
            float starSize = 200;
            float amountOfDots = Utils.Remap(Main.maxDustToDraw, 0, Main.maxDust, 15f, 45f);
            float extraRotation = MathF.Tau * .2f * Main.rand.NextFloat();
            for (int i = 0; i < 5; i++)
            {
                for (float j = -1; j < 1; j += 1 / amountOfDots)
                {
                    Vector2 pos = new Vector2(j, -.33f).RotatedBy(Utils.Remap(i, 0, 5, 0, MathF.Tau) + extraRotation) * starSize;
                    Dust.NewDustPerfect(pos + Player.Center, DustID.ShadowbeamStaff, Vector2.Zero);
                }
            }
            NightmareWizard.PlayBodyStarSoundEffect(Player.Center);
        }
        private static void GetNightCloakValues(Player.HurtInfo info, out int damage, out float numStars, out float starShootSpeed, out int type, out int numStarsCeil, out float starAi2)
        {
            damage = 40;
            starShootSpeed = 20;
            int hitDamage = info.Damage;
            numStars = (hitDamage * 3f) / damage;
            if (info.PvP)
            {
                damage = hitDamage * 3;
                starShootSpeed = 35;
            }
            type = ModContent.ProjectileType<GoodNightStar>();
            numStarsCeil = (int)numStars + 1;
            starAi2 = info.PvP ? 0 : 1;
            if (numStars > 50)
            {
                numStars = 50;
                numStarsCeil = 50;
            }
        }
        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            Photonic0BootsDrawLayer.ModifyDrawInfo(ref drawInfo);
        }
        public void StartMetalFighterKick()
        {
            MetalKick kick = (MetalKick)ContentSamples.ProjectilesByType[ModContent.ProjectileType<MetalKick>()].ModProjectile;
            fighterKickAndUppercutTimer = (byte)kick.AnimationDuration;
        }
        public void UpdateFighterManeuvers()
        {
            if (fighterKickAndUppercutTimer > 0)
            {
                switch (fighterKickAndUppercutType)
                {
                    case KickAndUppercutIDFighterGloveUppercut:
                        break;
                    case KickAndUppercutIDHardenedFighterUppercut:
                        break;
                       
                    case KickAndUppercutIDMetalFighterUppercut:
                        UpdateMetalUppercut();
                        break;
                    case KickAndUppercutIDMetalFighterKick:
                        UpdateMetalKick();
                        break;
                    default:
                        break;
                }
                fighterKickAndUppercutTimer--;
            }
        }

        private void UpdateMetalUppercut()
        {
        }
        public void TriggerMetalKick()
        {
            fighterKickAndUppercutTimer = (byte)MetalKick.SampleInstance.AnimationDuration;
            fighterKickAndUppercutType = KickAndUppercutIDMetalFighterKick;
        }
        public void UpdateMetalKick()
        {
            Player.mount.Dismount(Player);
            Player.velocity = MetalKick.GetSpeed(Player, Player.velocity.Y == 0, MetalKick.SampleInstance, fighterKickAndUppercutTimer);
        }
        public static void IncreaseComboCounter(int owner)
        {
            Main.player[owner].GetModPlayer<KirbPlayer>().fighterComboCounter++;
        }
    }

}
