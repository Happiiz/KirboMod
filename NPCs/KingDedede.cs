using KirboMod.Bestiary;
using KirboMod.Items.KingDedede;
using KirboMod.Items.Weapons;
using KirboMod.Projectiles;
using KirboMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using ReLogic.Content;
using Stubble.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static KirboMod.NPCs.Zero;

namespace KirboMod.NPCs
{

    [AutoloadBossHead]
    public class KingDedede : ModNPC
    {
        enum DededeAttackType : byte
        {
            Dash,//0
            Hammer,//1
            Gordo,//2
            Slam,//3
            Chomp,//4
            DarkShot,//5
        }

        float attack { get => NPC.ai[0]; set => NPC.ai[0] = value; } //controls dedede's attack pattern

        DededeAttackType attacktype = DededeAttackType.Dash; //which attack dedede will use 

        DededeAttackType lastattacktype = DededeAttackType.Dash; //sets last attack type

        int phase = 1; //determines what phase is dedede on

        int repeathammer = -1; //for multi hammer swings

        int animation = 0; //selection of frames to cycle through

        //for animation (purely visual)
        int Xframe = 0;
        int Yframe = 0;

        //Smash Bros. sounds are very loud :I

        public static SoundStyle Walk => new SoundStyle("KirboMod/Sounds/NPC/KingDedede/walk") with { Volume = 0.8f };

        public static SoundStyle Dive => new SoundStyle("KirboMod/Sounds/NPC/KingDedede/dive") with { Volume = 0.8f };

        public static SoundStyle SuperJumpRise => new SoundStyle("KirboMod/Sounds/NPC/KingDedede/superJumpRise") with { Volume = 0.8f };

        public static SoundStyle HeartyLaugh => new("KirboMod/Sounds/NPC/KingDedede/summon");

        public static SoundStyle HammerWindUp => new SoundStyle("KirboMod/Sounds/NPC/KingDedede/hammerWindUp") with { Volume = 0.8f };

        public static SoundStyle Possession => new SoundStyle("KirboMod/Sounds/NPC/KingDedede/possession") with { Volume = 0.8f };

        public static SoundStyle OrbCharge => new SoundStyle("KirboMod/Sounds/NPC/DarkMatter/DarkMatterSwordsmanSwordBallCharge2");

        public static SoundStyle Defeat => new SoundStyle("KirboMod/Sounds/NPC/KingDedede/defeat") with { Volume = 0.8f };

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("King Dedede");
            Main.npcFrameCount[NPC.type] = 33;

            // Add this in for bosses that have a summon item, requires corresponding code in the item
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "KirboMod/NPCs/BestiaryTextures/KingDededePortrait",
                PortraitScale = 1f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 30,
                Position = new Vector2(20, 70),

            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true; //immune to not mess up movement
        }

        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 100;
            NPC.damage = 50;
            NPC.noTileCollide = false;
            NPC.defense = 16;
            NPC.lifeMax = 8000;
            NPC.HitSound = SoundID.NPCHit1; //organic
            NPC.DeathSound = Defeat;
            NPC.value = Item.buyPrice(0, 3, 0, 0); // money it drops
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.boss = true;
            NPC.noGravity = false;
            NPC.lavaImmune = true;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Music/Happiz_KingDedede");
            }

            NPC.friendly = false;
            NPC.buffImmune[BuffID.Confused] = true;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            Helper.BossHpScalingForHigherDifficulty(ref NPC.lifeMax, balance);
            //NPC.damage = (int)(NPC.damage * 1f);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new SurfaceBackgroundProvider(), //I totally didn't steal this code
				// Sets the spawning conditions of this NPC that is listed in the bestiary.

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("The mighty-yet-greedy ruler of a distant castle! He loves to snack and show those who mistreat him the boot! Especially people who take his precious life-saving brooches!"),
            });
        }

        public override void SendExtraAI(BinaryWriter writer) //for syncing non NPC.ai[] stuff
        {
            writer.Write(animation);
            writer.Write((byte)lastattacktype);
            writer.Write((byte)attacktype);
            writer.Write((sbyte)repeathammer);
            writer.Write((sbyte)phase);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            animation = reader.ReadInt32();
            lastattacktype = (DededeAttackType)reader.ReadSByte();
            attacktype = (DededeAttackType)reader.ReadSByte();
            repeathammer = reader.ReadSByte();
            phase = reader.ReadSByte();
        }

        public override void AI() //cycles through each tick
        {
            Player player = Main.player[NPC.target]; //the player the npc is targeting=
            NPC.spriteDirection = NPC.direction;

            //Despawning
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active) //despawning
            {
                attack = 0;
                NPC.TargetClosest(false);
                NPC.noTileCollide = true;
                NPC.noGravity = false; //make him able to fall normally

                if (phase == 4)
                {
                    NPC.velocity.Y -= 0.2f; //go up
                }

                if (NPC.timeLeft > 60)
                {
                    NPC.timeLeft = 60;
                }
            }
            else
            {
                int phaseThreeSpeedUp = 0;

                if (phase == 3)
                {
                    phaseThreeSpeedUp = 30;
                }

                AttackPattern(phaseThreeSpeedUp);
            }
        }

        private void CheckPlatform(Player player) //referenced from Spirit mod's public source code
        {
            bool onplatform = true;
            for (int i = (int)NPC.position.X; i < NPC.position.X + NPC.width; i += NPC.width / 4)
            { //check tiles beneath the boss to see if they are all platforms
                Tile tile = Framing.GetTileSafely(new Point((int)NPC.position.X / 16, (int)(NPC.position.Y + NPC.height + 8) / 16));
                if (!TileID.Sets.Platforms[tile.TileType])
                    onplatform = false;
            }
            if (onplatform && (NPC.Bottom.Y < player.Bottom.Y - 100)) //if they are and the player is lower than the boss, temporarily let the boss ignore tiles to go through them
            {
                NPC.noTileCollide = true;
            }
            else
            {
                NPC.noTileCollide = false;
            }
        }

        private void AttackPattern(int phaseThreeSpeedUp)
        {
            Player player = Main.player[NPC.target];
            Vector2 distance = player.Center - NPC.Center;

            //if low enough then change phases
            if ((Main.expertMode || NPC.GetLifePercent() <= 0.6) && attack == 0)
            {
                phase = 2;
            }
            if (NPC.GetLifePercent() <= (Main.expertMode ? 0.6 : 0.25) && attack == 0 && phase != 3)
            {
                phase = 3;

                if (attack > 29)
                {
                    attack = 29; //go back to phase 3 attack decision
                }
            }

            //phase 4 change(can happen anytime)
            if (NPC.GetLifePercent() <= 0.3 && Main.expertMode && phase != 4)
            {
                SoundEngine.PlaySound(Possession, NPC.Center);

                attack = 0; //reset
                phase = 4;
            }

            if (attack == 0 & phase >= 2) //for multihammer
            {
                repeathammer = 3;
            }

            attack++; //go up

            if (phase < 4) //not phase 4
            {
                if (attack < 60 - phaseThreeSpeedUp) //face player target
                {
                    NPC.TargetClosest(true);
                    if (attack == 59 - phaseThreeSpeedUp) //random attack
                    {
                        SlamDecide(player, out bool shouldSlam);

                        if (shouldSlam)
                        {
                            attacktype = DededeAttackType.Slam; //slam
                            lastattacktype = DededeAttackType.Slam;

                            attack = phase == 3 ? 60 : 90;
                        }
                        else //random attack
                        {
                            AttackDecideNext();
                        }
                    }
                }
                NPC.noGravity = false;//nogravity caps the fall speed,  will disable on the slam
                NPC.GravityMultiplier = MultipliableFloat.One;
                if (attacktype == DededeAttackType.Dash)
                {
                    CheckPlatform(player);
                    AttackDash(phaseThreeSpeedUp, player, distance);
                }
                if (attacktype == DededeAttackType.Hammer)
                {
                    CheckPlatform(player);
                    NPC.noGravity = true;
                    AttackHammer(phaseThreeSpeedUp, player, distance);
                    NPC.velocity.Y += NPC.gravity;
                    NPC.noGravity = true;
                }
                if (attacktype == DededeAttackType.Gordo)
                {
                    CheckPlatform(player);
                    AttackGordo(phaseThreeSpeedUp, player);
                }
                if (attacktype == DededeAttackType.Slam)
                {
                    NPC.noGravity = true;
                    NPC.GravityMultiplier *= MultipliableFloat.One * 1.5f;
                    AttackSlam(phaseThreeSpeedUp, player, distance);
                    NPC.velocity.Y += NPC.gravity;
                }
            }

            //END OF PHASE 1 - 3

            else if (phase == 4)
            {
                //become normie boss
                NPC.noTileCollide = true;
                NPC.noGravity = true;

                if (attack < 240) //*casually gets possessed*
                {
                    ChooseAnimation(12); //woooaaaahhh my heads spinning!
                    NPC.velocity *= 0.95f;

                    NPC.dontTakeDamage = true; //make immune during transformation
                    NPC.rotation += MathHelper.ToRadians(5); //degrees to radians

                    //dust
                    if (attack % 5 == 0)
                    {
                        Vector2 speed = Main.rand.NextVector2Circular(20, 20);
                        Dust d = Dust.NewDustPerfect(NPC.Center + speed * 20, ModContent.DustType<Dusts.DarkResidue>(), -speed, 0); //Makes dust in a messy circle
                        d.noGravity = true;
                    }
                }
                else if (attack < 300) //rise dedede!
                {
                    if (attack == 240)
                    {
                        for (int i = 0; i < 20; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                        {
                            Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
                            Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 10, 10); //Makes dust in a messy circle
                            d.noGravity = true;
                            SoundEngine.PlaySound(SoundID.Item81, NPC.Center); //spawn slime mount
                        }

                        NPC.rotation = 0; //upright

                        ChooseAnimation(13); //possessed fly
                        NPC.velocity.Y = -0.5f; //go up
                        lastattacktype = DededeAttackType.DarkShot; //go to first attack
                    }
                }
                else if (attack >= 300) //START FROM HERE
                {
                    if (attack == 301)
                    {
                        NPC.dontTakeDamage = false; //make vunerable again

                        ChooseAnimation(13); //possessed fly
                    }

                    if (attack % 30 == 0) //every attack / 10 remainder of 0 
                    {
                        Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                        Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, Scale: 1f); //Makes dust in a messy circle
                        d.noGravity = true;
                    }

                    if (attack < 540)
                    {
                        NPC.TargetClosest(true);

                        float speed = 8f;
                        float inertia = 20;

                        distance.Normalize();
                        distance *= speed;

                        NPC.velocity = (NPC.velocity * (inertia - 1) + distance) / inertia; //fly towards player
                    }

                    if (attack == 449)
                    {
                        AttackDecideNext();
                    }

                    if (attacktype == DededeAttackType.Chomp) //maw
                    {
                        if (attack == 540)
                        {
                            NPC.TargetClosest(false);

                            NPC.frameCounter = 0; //restart animation
                            ChooseAnimation(14); //open maw
                            NPC.velocity *= 0;

                            SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                        }

                        if (attack >= 600 && attack < 720)
                        {
                            ChooseAnimation(15); //chomp loop
                            float speed = Main.expertMode ? 24 : 12; //faster if low on health
                            float inertia = 30; //harder to turn

                            distance.Normalize();
                            distance *= speed;
                            NPC.velocity = (NPC.velocity * (inertia - 1) + distance) / inertia; //fly towards player

                            NPC.TargetClosest(true);

                            if (attack % 10 == 0)
                            {
                                SoundEngine.PlaySound(SoundID.Item2.WithVolumeScale(0.5f), NPC.Center);
                            }
                        }
                        if (attack == 720) //close
                        {
                            NPC.frameCounter = 0; //restart animation

                            ChooseAnimation(16); //close maw

                            NPC.TargetClosest(false);

                            NPC.velocity *= 0;
                        }
                        if (attack >= 780) //end
                        {
                            ChooseAnimation(13); //possessed fly
                            attack = 300; //restart from here
                        }
                    }

                    if (attacktype == DededeAttackType.DarkShot) //eye
                    {

                        if (attack == 540) //do this until closing
                        {
                            NPC.TargetClosest(false);

                            NPC.frameCounter = 0; //restart animation
                            ChooseAnimation(17); //open eye
                            NPC.velocity *= 0;
                        }

                        if (attack == 580 || (attack == 640 && Main.expertMode))
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient) //execute on server( or singleplayer) only
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * 150, NPC.Center.Y, NPC.velocity.X * 0, NPC.velocity.Y * 0, ModContent.ProjectileType<Projectiles.DarkOrb>(), 50 / 2, 10f, Main.myPlayer, 0, NPC.target, 1);
                            }

                            SoundEngine.PlaySound(OrbCharge, NPC.Center);

                            NPC.TargetClosest(false);
                        }

                        if (attack == 680) //close
                        {
                            NPC.TargetClosest(false);

                            NPC.frameCounter = 0; //restart animation
                            ChooseAnimation(18); //close eye
                        }
                        if (attack >= 720) //end
                        {
                            ChooseAnimation(13); //possessed fly
                            attack = 300; //restart from here
                        }
                    }
                }
            }
        }

        private void AttackDash(int phaseThreeSpeedUp, Player player, Vector2 distance)
        {
            if (attack == 60 - phaseThreeSpeedUp)
            {
                ChooseAnimation(1); //run
                NPC.velocity.Y = -4; //short hop to signify start
            }
            if (attack >= 90 - phaseThreeSpeedUp && attack < 270 - phaseThreeSpeedUp)
            {
                if (attack % (phase == 3 ? 15 : 30) == 0) //multiple of 30(or 15)
                {
                    NPC.TargetClosest(true); //face player

                    if (phase == 3) //faster
                    {
                        NPC.velocity.X = NPC.direction * 15; //go 15 in the direction of npc(facing player)

                    }
                    else
                    {
                        NPC.velocity.X = NPC.direction * 10; //go 10 in the direction of npc(facing player)

                    }
                }
                else
                {
                    NPC.TargetClosest(false); //don't face player
                }

                if (attack % 20 == 0) //every 20 ticks
                {
                    SoundEngine.PlaySound(Walk, NPC.Center);
                }

                ClimbTiles(player);

                if (Math.Abs(distance.X) <= 300 && distance.Y <= 100 && distance.Y >= -200) //range
                {
                    attack = 300 - phaseThreeSpeedUp;
                }
            }
            if (attack == 270 - phaseThreeSpeedUp) //3 seconds up
            {
                NPC.velocity.X *= 0f;
                ChooseAnimation(0); //stand	

                attack = 0;
            }
            if (attack == 300 - phaseThreeSpeedUp) //dive
            {
                NPC.TargetClosest(false);

                SoundEngine.PlaySound(Dive, NPC.Center);

                NPC.velocity.Y = MathHelper.Clamp((player.Bottom.Y - NPC.Bottom.Y) / 10, -8f, -4f);//jump depending on player height for dive
                ChooseAnimation(2); //dive


                if (phase == 3) //faster
                {
                    NPC.velocity.X = NPC.direction * 20; //5 units higher than run

                }
                else
                {
                    NPC.velocity.X = NPC.direction * 15; //5 units higher than run

                }
            }
            if (attack > 300 - phaseThreeSpeedUp)
            {
                if (NPC.velocity.Y == 0) //check if not in air
                {
                    NPC.velocity.X *= 0.95f;
                }
            }
            if (attack >= 360 - phaseThreeSpeedUp) //restart from dive
            {
                NPC.velocity.X *= 0f;
                ChooseAnimation(0); //stand
                attack = 0;
            }
        }
        static float HammerFallDownStartSpeed => 10;
        private void AttackHammer(int phaseThreeSpeedUp, Player player, Vector2 distance)
        {
            if (attack == 60 - phaseThreeSpeedUp)
            {
                NPC.velocity.Y = -4; //short hop to signify start
                ChooseAnimation(3); //draw hammer

                SoundEngine.PlaySound(HeartyLaugh, NPC.Center);
            }
            if (attack >= 90 - phaseThreeSpeedUp && attack < 270 - phaseThreeSpeedUp)
            {
                ChooseAnimation(4); //run with hammer

                if (attack % (phase == 3 ? 7 : 15) == 0) //multiple of 15(or 7)
                {
                    NPC.TargetClosest(true); //target player

                    if (phase == 3) //faster
                    {
                        NPC.velocity.X = NPC.direction * 15; //go 15 in the direction of npc(facing player)

                    }
                    else
                    {
                        NPC.velocity.X = NPC.direction * 10; //go 10 in the direction of npc(facing player)

                    }
                }
                else
                {
                    NPC.TargetClosest(false); //don't face player
                }

                if (attack % 20 == 0) //every 20 ticks
                {
                    SoundEngine.PlaySound(Walk, NPC.Center);
                }

                ClimbTiles(player);

                if (Math.Abs(distance.X) <= 150) //range
                {
                    attack = 300 - phaseThreeSpeedUp;
                }
            }
            if (attack == 300 - phaseThreeSpeedUp) //charge swing
            {
                NPC.TargetClosest(false);

                SoundEngine.PlaySound(HammerWindUp, NPC.Center);

                NPC.velocity.X *= 0.001f;
                ChooseAnimation(5); //ready swing   
            }

            //IF ALREADY MULTISWUNG vvv
            if (phase > 1 && repeathammer == -1)
            {
                if (attack == (phase == 3 ? 307 : 315) - phaseThreeSpeedUp) //jump if player is high and multiswing(quicker in phase 3)
                {
                    if (distance.Y < -150) //too high
                    {
                        //jump depending on Y distance(quicker in phase 3)
                        NPC.velocity.Y = MathHelper.Clamp(distance.Y / (phase == 3 ? 5 : 10), -30, -15);
                    }
                }

                if (attack == (phase == 3 ? 315 : 330) - phaseThreeSpeedUp) //swing again after multiswinging
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient) //execute on server( or singleplayer) only
                    {
                        SummonSmash();
                    }
                    NPC.velocity.Y = -4; // stop rising
                    ChooseAnimation(6); //swing	
                }
                else if (attack > (phase == 3 ? 315 : 330) - phaseThreeSpeedUp)
                {
                    if (NPC.velocity.Y != 0 && attack < (phase == 3 ? 315 : 330) - phaseThreeSpeedUp + 3) //wait until hit ground
                    {
                        if (NPC.velocity.Y < HammerFallDownStartSpeed)
                        {
                            NPC.velocity.Y = HammerFallDownStartSpeed;
                        }
                        attack = (phase == 3 ? 315 : 330) - phaseThreeSpeedUp + 1; //go back to right before
                    }
                    if (attack == (phase == 3 ? 315 : 330) - phaseThreeSpeedUp + 3)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient) //execute on server( or singleplayer) only
                        {
                            SummonSmash();
                        }

                        LaunchDropStars();

                        attack = (phase == 3 ? 371 : 401); //just after multiswing section
                    }
                }
            }
            //IF ALREADY MULTISWUNG ^^^

            //regular attack
            if (attack == (phase == 3 ? 315 : 330) - phaseThreeSpeedUp) //jump if player is high(quicker in phase 3)
            {
                if (distance.Y < -150) //too high
                {
                    //jump depending on Y distance(quicker in phase 3)
                    NPC.velocity.Y = MathHelper.Clamp(distance.Y / (phase == 3 ? 5 : 10), -30, -15);
                }
            }
            if (attack == (phase == 3 ? 330 : 360) - phaseThreeSpeedUp) //swing
            {
                if (Main.netMode != NetmodeID.MultiplayerClient) //execute on server( or singleplayer) only
                {
                    SummonSmash();
                }
                ChooseAnimation(6); //swing
            }
            else if (attack > (phase == 3 ? 330 : 360) - phaseThreeSpeedUp)
            {
                if (NPC.velocity.Y != 0 && attack < (phase == 3 ? 330 : 360) - phaseThreeSpeedUp + 3) //wait until hit ground
                {
                    if (NPC.velocity.Y < HammerFallDownStartSpeed)
                    {
                        NPC.velocity.Y = HammerFallDownStartSpeed;
                    }
                    attack = (phase == 3 ? 330 : 360) - phaseThreeSpeedUp + 1; //go back to right before
                }
                if (attack == (phase == 3 ? 330 : 360) - phaseThreeSpeedUp + 3)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient) //execute on server( or singleplayer) only
                    {
                        SummonSmash();
                    }

                    LaunchDropStars();

                    repeathammer -= 1;
                }
            }

            //multiswing
            if (phase > 1) //only on phases 2 or 3
            {
                if (attack == (phase == 3 ? 340 : 370) - phaseThreeSpeedUp)
                {
                    NPC.TargetClosest(false);

                    NPC.velocity.X *= 0f;
                    ChooseAnimation(5); //ready swing    
                }
                if (attack == (phase == 3 ? 350 : 380) - phaseThreeSpeedUp)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient) //execute on server( or singleplayer) only
                    {
                        SummonSmash();
                    }

                    LaunchDropStars();
                    ChooseAnimation(6); //swing
                    repeathammer -= 1;
                }
                if (attack == (phase == 3 ? 360 : 390) - phaseThreeSpeedUp)
                {
                    NPC.TargetClosest(false);

                    NPC.velocity.X *= 0f;
                    ChooseAnimation(5); //ready swing   
                }
                if (attack == (phase == 3 ? 370 : 400) - phaseThreeSpeedUp)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient) //execute on server( or singleplayer) only
                    {
                        SummonSmash();
                    }

                    LaunchDropStars();
                    ChooseAnimation(6); //swing

                    repeathammer -= 1;
                }
            }

            //cycle back again if used repeat hammer swings
            if (attack == (phase == 3 ? 400 : 430) - phaseThreeSpeedUp && repeathammer == 0 && phase >= 2)
            {
                attack = 90;
                repeathammer = -1;
            }
            if (attack >= 430 - phaseThreeSpeedUp && phase == 3) //restart from swing (phase 3)
            {
                attack = 0;

                NPC.velocity.X *= 0f;
                ChooseAnimation(0); //stand    
            }
            if (attack >= 460 - phaseThreeSpeedUp && phase == 2) //restart from swing (phase 2)
            {
                attack = 0;

                NPC.velocity.X *= 0f;
                ChooseAnimation(0); //stand    
            }
            if (attack >= 420 - phaseThreeSpeedUp && phase == 1) //restart from swing (phase 1)
            {
                attack = 0;

                NPC.velocity.X *= 0f;
                ChooseAnimation(0); //stand    
            }
        }

        private void AttackSlam(int phaseThreeSpeedUp, Player player, Vector2 distance)
        {
            Vector2 predictDistance = player.Center + player.velocity * 5 - NPC.Center;

            if (attack >= 60 - phaseThreeSpeedUp && attack < (phase == 3 ? 90: 120) - phaseThreeSpeedUp)
            {
                ChooseAnimation(9); //ready jump
                NPC.TargetClosest(true); //face player

                if (attack == 60)
                {
                    SoundEngine.PlaySound(Dive, NPC.Center);
                }
            }

            if (attack == (phase == 3 ? 90 : 120) - phaseThreeSpeedUp)
            {
                NPC.noTileCollide = true; //don't collide with tiles

                NPC.velocity.Y = MathHelper.Clamp(predictDistance.Y / 2, -35, -30);
                NPC.velocity.X = MathHelper.Clamp(predictDistance.X / 90, -30, 30);
                ChooseAnimation(10); //jump

                SoundEngine.PlaySound(SuperJumpRise, NPC.Center);
            }
            if (attack > (phase == 3 ? 90 : 120) - phaseThreeSpeedUp
                && attack < (phase == 3 ? 390 : 420) - phaseThreeSpeedUp) //fall for 5 seconds
            {
                NPC.GravityIgnoresLiquid = true;
                NPC.GravityIgnoresSpace = true;

                ChooseAnimation(10); //jump

                if (NPC.Bottom.Y < player.Top.Y - 100 || NPC.velocity.Y < 0) //higher than player (adjusts to velocity) or going up
                {
                    NPC.noTileCollide = true; //don't collide with tiles
                }
                else
                {
                    NPC.noTileCollide = false; //collide with tiles

                    if (NPC.velocity.Y == 0 || NPC.oldVelocity.Y == 0) //on ground or stopped rising/falling midjump
                    {
                        NPC.velocity.X = 0;
                        ChooseAnimation(9); //end jump

                        LaunchDropStars();

                        //slam (just for style)
                        if (Main.netMode != NetmodeID.MultiplayerClient) //execute on server( or singleplayer) only
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.position.Y + 150, 0, 0, ModContent.ProjectileType<DededeSlam>(), 0, 8f, Main.myPlayer, 0, 0);
                        }
                        attack = (phase == 3 ? 390 : 420) - phaseThreeSpeedUp; //30 ticks before restart
                    }
                }
            }
            if (attack == (phase == 3 ? 389 : 419) - phaseThreeSpeedUp) //1 tick before rest
            {
                if (NPC.velocity.Y != 0 || NPC.oldVelocity.Y != 0) //go back if not on ground or immobile
                {
                    attack = (phase == 3 ? 91 : 121);
                }
                else //continue
                {
                    NPC.velocity.X *= 0.01f;
                }
            }
            if (attack >= (phase == 3 ? 420 : 450) - phaseThreeSpeedUp)
            {
                ChooseAnimation(0); //stance
                attack = 0;
            }
        }

        private void AttackGordo(int phaseThreeSpeedUp, Player player)
        {
            float timer = attack - 60;

            int numberOfShots = 3 + 2 * (phase - 1);

            float phaseThreeAttackSpeed = 1;

            if (phase == 3)
            {
                timer = attack - 30;
                phaseThreeAttackSpeed = 0.5f;
            }

            if (timer >= (60 - phaseThreeSpeedUp) * phaseThreeAttackSpeed)
            {
                NPC.TargetClosest(true); //target player
            }

            float timerRemainder = timer % (60 * phaseThreeAttackSpeed);

            //remainders on an odd tick else get skipped with phase 3 time reduction

            //every 30th tick, not on any 60th tick
            if (timerRemainder == 30 * phaseThreeAttackSpeed && timerRemainder
                != 60 * phaseThreeAttackSpeed && timer <= (60 * numberOfShots) * phaseThreeAttackSpeed)
            {
                if (phase == 3)
                {
                    ChooseAnimation(19); //ready gordo (fast)
                }
                else
                {

                    ChooseAnimation(7); //ready gordo
                }
            }

            /*//faster ver (5 frames before)
            if (phase == 3 && timerRemainder == 20 * phaseThreeAttackSpeed && timer <= (60 * numberOfShots) * phaseThreeAttackSpeed)
            {
                ChooseAnimation(19); //ready gordo (fast)
            }
            else
            {
                //swing 15 frames before gordo is sent out
                if (timerRemainder == 30 * phaseThreeAttackSpeed && timer <= (60 * numberOfShots) * phaseThreeAttackSpeed)
                {
                    ChooseAnimation(7); //ready gordo

                }
            }*/

            //faster ver (5 frames before)
            if (phase == 3 && timerRemainder == 40 * phaseThreeAttackSpeed && timer <= (60 * numberOfShots) * phaseThreeAttackSpeed)
            {
                ChooseAnimation(11); //swing gordo (fast)
            }
            else
            {
                //swing 15 frames before gordo is sent out
                if (timerRemainder == 45 * phaseThreeAttackSpeed && timer <= (60 * numberOfShots) * phaseThreeAttackSpeed)
                {
                    ChooseAnimation(8); //swing gordo

                }
            }

            //send out gordos in upward arcs
            if (timerRemainder == 0 && timer <= (60 * numberOfShots) * phaseThreeAttackSpeed && timer != 0)
            {
                //float shootSpeed = 22;
                Vector2 shootFrom = NPC.Center + new Vector2(NPC.direction * 100, 0);
                Vector2 distance = player.Center - shootFrom;
                Vector2 velocity;

                float YToReach = GetYVelForParabolaPeakToBeAt(player.Center.Y - 100, BouncyGordo.GordoGravity, shootFrom.Y);
                if (YToReach > -20)
                {
                    YToReach = -20;
                }
                velocity.Y = YToReach;

                float time = TimeToReachYPoint(shootFrom.Y, player.Center.Y, BouncyGordo.GordoGravity, velocity.Y);
                velocity.X = distance.X / time;

                if (Main.netMode != NetmodeID.MultiplayerClient) //execute on server( or singleplayer) only
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom, velocity, ModContent.ProjectileType<BouncyGordo>(), 30 / 2, 4f, Main.myPlayer, 0, 0);
                }
                SoundEngine.PlaySound(SoundID.Item10.WithVolumeScale(2), NPC.Center); //impact
            }

            if (timer >= 60 * (numberOfShots + 1) * phaseThreeAttackSpeed)
            {
                ChooseAnimation(0); //stand

                attack = 0;
            }
        }
        private void SummonSmash()
        {
            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * 130, NPC.Center.Y + 20, NPC.direction * 0.1f, 0,
                ModContent.ProjectileType<BonkersSmash>(), 60 / 2, 8f, Main.myPlayer, 0, NPC.whoAmI, 0);
        }

        static float GetYVelForParabolaPeakToBeAt(float peakPosY, float gravityY, float fromY) //used for finding trajectory of gordo (courtesy of not me)
        {
            float result = MathF.Abs((peakPosY - fromY) * gravityY * 2);

            return -MathF.Sqrt(result);
        }

        static float TimeToReachYPoint(float fromY, float toY, float accelY, float initialVelY) //used for finding trajectory of gordo (courtesy of not me)
        {
            bool hasSolution = Utils.SolveQuadratic(accelY * .5f, initialVelY, fromY - toY, out float result1, out float result2);
            if (!hasSolution)
            {
                return 60f; //just return a second
            }
            float time = MathF.Max(result2, result1);
            return time;
        }

        void AttackDecideNext()
        {

            List<DededeAttackType> possibleAttacks = new() { DededeAttackType.Dash, DededeAttackType.Hammer, DededeAttackType.Gordo, DededeAttackType.Slam };

            if (phase == 4)
            {
                possibleAttacks = new() { DededeAttackType.Chomp, DededeAttackType.DarkShot };
            }

            possibleAttacks.Remove(lastattacktype);

            attacktype = possibleAttacks[Main.rand.Next(possibleAttacks.Count)];
            NPC.netUpdate = true;

            lastattacktype = attacktype;
        }

        void LaunchDropStars()
        {
            SoundEngine.PlaySound(SoundID.Item9, NPC.Center); //star swoosh

            SoundEngine.PlaySound(SoundID.Item14, NPC.Center); //bomb

            if (attacktype == DededeAttackType.Hammer)
            {
                if (repeathammer > -1)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 velocity = new Vector2(NPC.direction * (4f * i + (3 - repeathammer) * 2), -15); //go up evenly in a uneven spread (each extra slam making them go further ahead)

                        if (Main.netMode != NetmodeID.MultiplayerClient) //execute on server( or singleplayer) only
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 120, 20),
                            velocity, ModContent.ProjectileType<DededeDropStar>(), 40 / 2, 4f, Main.myPlayer, 1, 0);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Vector2 velocity = new Vector2(NPC.direction * 2f * i, -15); //go up evenly in a uneven spread

                        if (Main.netMode != NetmodeID.MultiplayerClient) //execute on server( or singleplayer) only
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 120, 20),
                            velocity, ModContent.ProjectileType<DededeDropStar>(), 40 / 2, 4f, Main.myPlayer, 1, 0);
                        }
                    }
                }
            }
            else if (attacktype == DededeAttackType.Slam)
            {
                float maxNumber = 16 + phase * 8;

                for (int i = 0; i < maxNumber; i++)
                {
                    Vector2 velocity = new Vector2(MathF.Cos(MathF.Tau / maxNumber * i), MathF.Sin(MathF.Tau / maxNumber * i));
                    velocity *= 10;

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom,
                        velocity, ModContent.ProjectileType<DededeDropStar>(), 40 / 2, 4f, Main.myPlayer, 0, 0);
                }
            }
        }

        private void SlamDecide(Player player, out bool shouldSlam)
        {
            shouldSlam = false;

            //make a line from dedede through player to see if no tiles
            bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
            float distanceUnit = Vector2.Distance(player.Center, NPC.Center);

            bool stuck = false; //determines if stuck between two tiles

            for (float i = 0; i < NPC.width; i++) //counter for stuck
            {
                Point abovenpc = new Vector2(NPC.position.X + i, NPC.position.Y).ToTileCoordinates(); //all tiles above npc
                Point belownpc = new Vector2(NPC.position.X + i, NPC.position.Y + NPC.height).ToTileCoordinates(); //all tiles below npc

                //head against celling
                if (WorldGen.SolidTile(abovenpc.X, abovenpc.Y) == true && Main.tile[belownpc.X, belownpc.Y].HasTile)
                {
                    stuck = true;
                }
            }

            //too far, can't reach, or too low
            if (NPC.position.Y > player.Bottom.Y + 400 || distanceUnit >= 1500 || stuck)
            {
                shouldSlam = true;
            }
        }
        private void ClimbTiles(Player player)
        {
            bool climableTiles = false;

            for (int i = 0; i < NPC.height; i++)
            {
                if (NPC.direction == 1)
                {
                    //checks for tiles on right side of NPC
                    Tile tile = Main.tile[new Vector2(NPC.Right.X + 1, NPC.position.Y + i).ToTileCoordinates()];
                    climableTiles = WorldGen.SolidOrSlopedTile(tile) || TileID.Sets.Platforms[tile.TileType] || tile.IsHalfBlock;
                }
                else
                {
                    //checks for tiles on left side of NPC
                    Tile tile = Main.tile[new Vector2(NPC.Left.X - 1, NPC.position.Y + i).ToTileCoordinates()];
                    climableTiles = WorldGen.SolidOrSlopedTile(tile) || TileID.Sets.Platforms[tile.TileType] || tile.IsHalfBlock;
                }

                if (climableTiles && MathF.Abs(NPC.Bottom.Y - player.Bottom.Y) > 20f || NPC.velocity.X == 0)
                {
                    NPC.noTileCollide = true;

                    if (player.Bottom.Y < NPC.Bottom.Y && !player.dead) //higher than NPC or not dead
                    {
                        NPC.velocity.Y = -4f;
                    }

                    break;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D dedede = TextureAssets.Npc[Type].Value; //get the texture

            SpriteEffects direction = SpriteEffects.FlipHorizontally; //face right

            if (NPC.direction == -1)
            {
                direction = SpriteEffects.None; //face left
            }

            Rectangle frame = Animation(dedede.Width, dedede.Height);

            //draw!
            spriteBatch.Draw(dedede, NPC.Center - Main.screenPosition - new Vector2(0, 44), frame, drawColor, NPC.rotation,
                frame.Size() / 2, NPC.scale, direction, 0f);

            return false;
        }

        public void ChooseAnimation(int selectAnimation)
        {
            animation = selectAnimation;

            //animations with concrete beginnings
            if (selectAnimation == 2 || selectAnimation == 3 || (selectAnimation >= 6 && selectAnimation <= 11)
                || selectAnimation == 14 || (selectAnimation >= 16 && selectAnimation <= 19))
            {
                NPC.frameCounter = 0; //reset animation counter
            }
        }

        private Rectangle Animation(int textureWidth, int textureHeight) //make the dimensions for the frames
        {

            if (!Main.gamePaused && Main.hasFocus) //not paused or tabbed out
            {
                NPC.frameCounter++;
            }

            if (animation == 0) //stance
            {
                if (NPC.frameCounter < 10)
                {
                    Xframe = 0;
                    Yframe = 0;
                }
                else if (NPC.frameCounter < 20)
                {
                    Xframe = 1;
                    Yframe = 0;
                }
                else if (NPC.frameCounter < 30)
                {
                    Xframe = 2;
                    Yframe = 0;
                }
                else
                {
                    NPC.frameCounter = 0;
                }
            }
            if (animation == 1) //run
            {
                if (NPC.frameCounter < 5)
                {
                    Xframe = 3;
                    Yframe = 0;
                }
                else if (NPC.frameCounter < 10)
                {
                    Xframe = 4;
                    Yframe = 0;
                }
                else if (NPC.frameCounter < 15)
                {
                    Xframe = 5;
                    Yframe = 0;
                }
                else if (NPC.frameCounter < 20)
                {
                    Xframe = 6;
                    Yframe = 0;
                }
                else if (NPC.frameCounter < 25)
                {
                    Xframe = 7;
                    Yframe = 0;
                }
                else if (NPC.frameCounter < 30)
                {
                    Xframe = 6;
                    Yframe = 0;
                }
                else if (NPC.frameCounter < 35)
                {
                    Xframe = 5;
                    Yframe = 0;
                }
                else if (NPC.frameCounter < 40)
                {
                    Xframe = 4;
                    Yframe = 0;
                }
                else
                {
                    NPC.frameCounter = 0;
                }
            }
            if (animation == 2) //dive
            {
                if (NPC.frameCounter < 10)
                {
                    Xframe = 0;
                    Yframe = 1;
                }
                else if (NPC.frameCounter < 20)
                {
                    Xframe = 1;
                    Yframe = 1;
                }
                else
                {
                    Xframe = 2;
                    Yframe = 1;
                }
            }
            if (animation == 3) //pull hammer
            {
                if (NPC.frameCounter < 10)
                {
                    Xframe = 3;
                    Yframe = 1;
                }
                else if (NPC.frameCounter < 20)
                {
                    Xframe = 4;
                    Yframe = 1;
                }
                else
                {
                    Xframe = 5;
                    Yframe = 1;
                }
            }
            if (animation == 4) //run with hammer
            {
                if (NPC.frameCounter < 5)
                {
                    Xframe = 6;
                    Yframe = 1;
                }
                else if (NPC.frameCounter < 10)
                {
                    Xframe = 7;
                    Yframe = 1;
                }
                else if (NPC.frameCounter < 15)
                {
                    Xframe = 0;
                    Yframe = 2;
                }
                else if (NPC.frameCounter < 20)
                {
                    Xframe = 1;
                    Yframe = 2;
                }
                else if (NPC.frameCounter < 25)
                {
                    Xframe = 2;
                    Yframe = 2;
                }
                else if (NPC.frameCounter < 30)
                {
                    Xframe = 1;
                    Yframe = 2;
                }
                else if (NPC.frameCounter < 35)
                {
                    Xframe = 0;
                    Yframe = 2;
                }
                else if (NPC.frameCounter < 40)
                {
                    Xframe = 7;
                    Yframe = 1;
                }
                else
                {
                    NPC.frameCounter = 0;
                }
            }
            if (animation == 5) //ready swing hammer
            {
                if (NPC.frameCounter < 2)
                {
                    Xframe = 3;
                    Yframe = 2;
                }
                else if (NPC.frameCounter < 4)
                {
                    Xframe = 4;
                    Yframe = 2;
                }
                else
                {
                    NPC.frameCounter = 0;
                }
            }
            if (animation == 6) //swing hammer
            {
                if (NPC.frameCounter < 5)
                {
                    Xframe = 5;
                    Yframe = 2;
                }
                else
                {
                    Xframe = 6;
                    Yframe = 2;
                }
            }
            if (animation == 7) //ready swing gordo
            {
                if (NPC.frameCounter < 4)
                {
                    Xframe = 7;
                    Yframe = 2;
                }
                else if (NPC.frameCounter < 8)
                {
                    Xframe = 0;
                    Yframe = 3;
                }
                else
                {
                    Xframe = 1;
                    Yframe = 3;
                }
            }
            if (animation == 8) //swing gordo
            {
                if (NPC.frameCounter < 5)
                {
                    Xframe = 2;
                    Yframe = 3;
                }
                else if (NPC.frameCounter < 10)
                {
                    Xframe = 3;
                    Yframe = 3;
                }
                else if (NPC.frameCounter < 15)
                {
                    Xframe = 4;
                    Yframe = 3;
                }
                else if (NPC.frameCounter < 20)
                {
                    Xframe = 5;
                    Yframe = 3;
                }
                else
                {
                    Xframe = 6;
                    Yframe = 3;
                }
            }
            if (animation == 9) //slam
            {
                if (NPC.frameCounter < 10)
                {
                    Xframe = 7;
                    Yframe = 3;
                }
                else
                {
                    Xframe = 0;
                    Yframe = 4;
                }
            }
            if (animation == 10) //rise
            {
                if (NPC.frameCounter < 10)
                {
                    Xframe = 1;
                    Yframe = 4;
                }
                else
                {
                    Xframe = 2;
                    Yframe = 4;
                }
            }
            if (animation == 11) //swing gordo fast
            {
                if (NPC.frameCounter < 2)
                {
                    Xframe = 2;
                    Yframe = 3;
                }
                else if (NPC.frameCounter < 4)
                {
                    Xframe = 3;
                    Yframe = 3;
                }
                else if (NPC.frameCounter < 6)
                {
                    Xframe = 4;
                    Yframe = 3;
                }
                else if (NPC.frameCounter < 8)
                {
                    Xframe = 5;
                    Yframe = 3;
                }
                else
                {
                    Xframe = 6;
                    Yframe = 3;
                }
            }
            if (animation == 12) //ouch!
            {
                Xframe = 3;
                Yframe = 4;
                NPC.frameCounter = 0;
            }
            if (animation == 13) //possessed float
            {
                if (NPC.frameCounter < 15)
                {
                    Xframe = 4;
                    Yframe = 4;
                }
                else if (NPC.frameCounter < 30)
                {
                    Xframe = 5;
                    Yframe = 4;
                }
                else if (NPC.frameCounter < 45)
                {
                    Xframe = 6;
                    Yframe = 4;
                }
                else if (NPC.frameCounter < 60)
                {
                    Xframe = 5;
                    Yframe = 4;
                }
                else
                {
                    NPC.frameCounter = 0;
                }
            }
            if (animation == 14) //possessed open maw
            {
                if (NPC.frameCounter < 3)
                {
                    Xframe = 7;
                    Yframe = 4;
                }
                else if (NPC.frameCounter < 6)
                {
                    Xframe = 0;
                    Yframe = 5;
                }
                else if (NPC.frameCounter < 9)
                {
                    Xframe = 1;
                    Yframe = 5;
                }
                else if (NPC.frameCounter < 12)
                {
                    Xframe = 2;
                    Yframe = 5;
                }
                else if (NPC.frameCounter < 15)
                {
                    Xframe = 3;
                    Yframe = 5;
                }
                else if (NPC.frameCounter < 18)
                {
                    Xframe = 4;
                    Yframe = 5;
                }
            }
            if (animation == 15) //possessed chomp loop
            {
                if (NPC.frameCounter < 2)
                {
                    Xframe = 3;
                    Yframe = 5;
                }
                else if (NPC.frameCounter < 4)
                {
                    Xframe = 4;
                    Yframe = 5;
                }
                else if (NPC.frameCounter < 6)
                {
                    //clamp
                    Xframe = 2;
                    Yframe = 5;
                }
                else
                {
                    NPC.frameCounter = 0;
                }
            }
            if (animation == 16) //possessed close maw
            {
                if (NPC.frameCounter < 3)
                {
                    Xframe = 3;
                    Yframe = 5;
                }
                else if (NPC.frameCounter < 6)
                {
                    Xframe = 2;
                    Yframe = 5;
                }
                else if (NPC.frameCounter < 9)
                {
                    Xframe = 1;
                    Yframe = 5;
                }
                else if (NPC.frameCounter < 12)
                {
                    Xframe = 0;
                    Yframe = 5;
                }
                else if (NPC.frameCounter < 15)
                {
                    Xframe = 7;
                    Yframe = 4;
                }
            }
            if (animation == 17) //possessed open eye
            {
                if (NPC.frameCounter < 2)
                {
                    Xframe = 7;
                    Yframe = 4;
                }
                else if (NPC.frameCounter < 4)
                {
                    Xframe = 0;
                    Yframe = 5;
                }
                else if (NPC.frameCounter < 6)
                {
                    Xframe = 5;
                    Yframe = 5;
                }
                else if (NPC.frameCounter < 8)
                {
                    Xframe = 6;
                    Yframe = 5;
                }
                else if (NPC.frameCounter < 10)
                {
                    Xframe = 7;
                    Yframe = 5;
                }
                else if (NPC.frameCounter < 30)
                {
                    Xframe = 0;
                    Yframe = 6;
                }
                else if (NPC.frameCounter < 32)
                {
                    Xframe = 1;
                    Yframe = 6;
                }
                else if (NPC.frameCounter < 34)
                {
                    Xframe = 2;
                    Yframe = 6;
                }
                else if (NPC.frameCounter < 36)
                {
                    Xframe = 3;
                    Yframe = 6;
                }
            }
            if (animation == 18) //possessed close eye
            {
                if (NPC.frameCounter < 2)
                {
                    Xframe = 2;
                    Yframe = 6;
                }
                else if (NPC.frameCounter < 4)
                {
                    Xframe = 1;
                    Yframe = 6;
                }
                else if (NPC.frameCounter < 6)
                {
                    Xframe = 0;
                    Yframe = 6;
                }
                else if (NPC.frameCounter < 16)
                {
                    Xframe = 7;
                    Yframe = 5;
                }
                else if (NPC.frameCounter < 18)
                {
                    Xframe = 5;
                    Yframe = 5;
                }
                else if (NPC.frameCounter < 20)
                {
                    Xframe = 0;
                    Yframe = 5;
                }
                else
                {
                    Xframe = 7;
                    Yframe = 4;
                }
            }
            if (animation == 19) //ready swing gordo (fast)
            {
                if (NPC.frameCounter < 2)
                {
                    Xframe = 7;
                    Yframe = 2;
                }
                else if (NPC.frameCounter < 4)
                {
                    Xframe = 0;
                    Yframe = 3;
                }
                else
                {
                    Xframe = 1;
                    Yframe = 3;
                }
            }

            int frameWidth = textureWidth / 8;
            int frameHeight = textureHeight / 7;

            return new Rectangle(Xframe * frameWidth, Yframe * frameHeight, frameWidth, frameHeight);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D gordo = TextureAssets.Projectile[ModContent.ProjectileType<Gordo>()].Value;


            float xOffset = NPC.direction * 100;
            Vector2 position = NPC.Center - Main.screenPosition + new Vector2(xOffset, -100);

            Vector2 origin = gordo.Size() / 2;

            //only draw if in holding up while holding hammer frame or the first part of the swinging frames

            if (animation == 7 && NPC.frameCounter >= 8)
            {
                Main.EntitySpriteDraw(gordo, position, null, drawColor, 0, origin, 1, SpriteEffects.None);
            }
            else if (animation == 8) 
            {
                if (NPC.frameCounter >= 0 && NPC.frameCounter < 5)
                {
                    position = NPC.Center - Main.screenPosition + new Vector2(xOffset, -80);

                    Main.EntitySpriteDraw(gordo, position, null, drawColor, MathHelper.PiOver2 / 8, origin, 1, SpriteEffects.None);
                }
                else if (NPC.frameCounter >= 5 && NPC.frameCounter < 10)
                {
                    position = NPC.Center - Main.screenPosition + new Vector2(xOffset, -65);

                    Main.EntitySpriteDraw(gordo, position, null, drawColor, MathHelper.PiOver2 / 6, origin, 1, SpriteEffects.None);
                }
                else if (NPC.frameCounter >= 10 && NPC.frameCounter < 15)
                {
                    position = NPC.Center - Main.screenPosition + new Vector2(xOffset, -30);

                    Main.EntitySpriteDraw(gordo, position, null, drawColor, MathHelper.PiOver2 / 4, origin, 1, SpriteEffects.None);
                }
            }
            else if (animation == 11) //same thing as previous except faster
            {
                if (NPC.frameCounter >= 0 && NPC.frameCounter < 2)
                {
                    position = NPC.Center - Main.screenPosition + new Vector2(xOffset, -80);

                    Main.EntitySpriteDraw(gordo, position, null, drawColor, MathHelper.PiOver2 / 8, origin, 1, SpriteEffects.None);
                }
                else if (NPC.frameCounter >= 2 && NPC.frameCounter < 4)
                {
                    position = NPC.Center - Main.screenPosition + new Vector2(xOffset, -65);

                    Main.EntitySpriteDraw(gordo, position, null, drawColor, MathHelper.PiOver2 / 6, origin, 1, SpriteEffects.None);
                }
                else if (NPC.frameCounter >= 4 && NPC.frameCounter < 6)
                {
                    position = NPC.Center - Main.screenPosition + new Vector2(xOffset, -30);

                    Main.EntitySpriteDraw(gordo, position, null, drawColor, MathHelper.PiOver2 / 4, origin, 1, SpriteEffects.None);
                }
            }
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedKingDededeBoss, -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<KingDededeBag>())); //only drops in expert

            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert()); //checks if not expert
            LeadingConditionRule masterMode = new LeadingConditionRule(new Conditions.IsMasterMode()); //checks if master mode

            notExpertRule.OnSuccess(new OneFromRulesRule(1, ItemDropRule.Common(ModContent.ItemType<Items.Weapons.NewHammer>(), 1)
                , ItemDropRule.Common(ModContent.ItemType<Items.Weapons.Blado>(), 1, 300, 300)
                , ItemDropRule.Common(ModContent.ItemType<Items.Weapons.OrnateChest>(), 1)
                , ItemDropRule.Common(ModContent.ItemType<Items.Weapons.DooStaff>(), 1)));

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.KingDedede.KingDededeMask>(), 7));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.KingDedede.KingDededeTrophy>(), 10));

            //master mode stuff
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeables.BossRelics.KingDededeRelic>()));

            masterMode.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.KingDedede.KingDededePetItem>(), 4));

            // add the rules
            npcLoot.Add(notExpertRule);
            npcLoot.Add(masterMode);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "King Dedede";
            potionType = ItemID.HealingPotion;
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    // go around in a octogonal pattern
                    Vector2 speed = new Vector2((float)Math.Cos(MathHelper.ToRadians(i * 45)) * 25, (float)Math.Sin(MathHelper.ToRadians(i * 45)) * 25);

                    Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.BoldStar>(), speed, Scale: 3f); //Makes dust in a messy circle
                    d.noGravity = true;
                }
                for (int i = 0; i < 20; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
                    Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, speed, Main.rand.Next(11, 13), Scale: 2f); //double jump smoke
                }

                if (NPC.direction == -1) //die left
                {
                    Dust.NewDustPerfect(NPC.position, ModContent.DustType<Dusts.KingDededead>(), new Vector2(NPC.direction * -8, -8), 1);
                }
                else if (NPC.direction == 1)//die right
                {
                    Dust.NewDustPerfect(NPC.position, ModContent.DustType<Dusts.KingDededeadRight>(), new Vector2(NPC.direction * -8, -8), 1);
                }
            }
            else
            {
                if (phase != 4) //no dark residue
                {
                    Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                    Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, speed, Main.rand.Next(11, 13), Scale: 0.5f); //double jump smoke
                }
                else
                {
                    Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
                    Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, Scale: 0.5f); //Makes dust in a messy circle
                    d.noGravity = false;
                }
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            Player player = Main.player[NPC.target]; //the player the npc is targeting


            SlamDecide(player, out bool shouldSlam);

            if (shouldSlam && phase != 4)
            {
                NPC.dontTakeDamage = true; //invunerable

                return new Color(255, 128, 128); //warning tint (light red)
            }
            NPC.dontTakeDamage = false; //vunerable

            return drawColor;
        }

        public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
        {
            boundingBox = NPC.Hitbox;
        }
    }
}
