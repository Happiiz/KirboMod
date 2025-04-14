using KirboMod.Items;
using KirboMod.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using SoundEngine = Terraria.Audio.SoundEngine;

namespace KirboMod.NPCs
{
    public class KnuckleJoe : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Knuckle Joe");
            Main.npcFrameCount[NPC.type] = 11;
        }
        //state 0: walk
        //state 1: sidestep
        //state 2: vulcan jab
        //state 3: blast
        ref float State => ref NPC.ai[0];
        ref float Timer => ref NPC.localAI[1];
        const int StateIDWalk = 0;
        const int StateIDSideStep = 1;
        const int StateIDVulcanJab = 2;
        const int StateIDBlast = 3;
        public override void SetDefaults()
        {
            NPC.width = 44;
            NPC.height = 44;
            NPC.damage = 40;
            NPC.defense = 25;
            NPC.lifeMax = 320;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = Item.buyPrice(0, 0, 0, 10);
            NPC.knockBackResist = 0f; //how much knockback applies
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<Items.Banners.KnuckleJoeBanner>();
            NPC.aiStyle = -1;
            NPC.friendly = false;
            NPC.noGravity = false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneRockLayerHeight) //if player is within cave height
            {
                if (spawnInfo.Player.ZoneJungle)
                {
                    return 0f;
                }
                else if (spawnInfo.Player.ZoneSnow)
                {
                    return 0f;
                }
                else if (spawnInfo.Player.ZoneBeach) //don't spawn on beach
                {
                    return 0f;
                }
                else if (spawnInfo.Player.ZoneDesert) //don't spawn on beach
                {
                    return 0f;
                }
                else if (spawnInfo.Player.ZoneCorrupt) //don't spawn on beach
                {
                    return 0f;
                }
                else if (spawnInfo.Player.ZoneCrimson) //don't spawn on beach
                {
                    return 0f;
                }
                else if (spawnInfo.Player.ZoneDungeon) //don't spawn in dungeon
                {
                    return 0f;
                }
                else if (spawnInfo.Water) //don't spawn in water
                {
                    return 0f;
                }
                else if (spawnInfo.Sky) //don't spawn in space
                {
                    return 0f;
                }
                else //only forest
                {
                    return spawnInfo.SpawnTileType == TileID.Stone || spawnInfo.SpawnTileType == TileID.Dirt ? .03f : 0f; //functions like a mini if else statement
                }
            }
            else
            {
                return 0f; //no spawn rate
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns, 

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Watch out! Knuckle Joe wants a challenge and you seem perfectly fit! They'll bombard you with a flurry of vulcan jabs and smash punches 'til you're down!")
            });
        }
        static float AggroRange => 250;
        static float VulcanJabRate => 3;
        public static float VulcanJabRange =>AggroRange + 16 * 3;
        public static float VulcanJabVelocity => 25;
        public static float VulcanJabSpread => 0.4f;
        static float VulcanJabChargeup => 40;
        static int VulcanJabShotCount => 30;
        static float BlastChargeup => 25;
        static float BlastVelocity => 20;
        static float BlastRate => 35;
        static float WalkSpeed => 3;
        static float JumpSpeed => -8;
        static int VulcanJabDamage => 60 / 2;
        static int BlastShotCount => Main.getGoodWorld ? 3 : Main.expertMode ? 2 : 1;
        static int BlastDamage => 100 / 2;
        public static float BlastRange => 1000;
        public static float BlastVelocityPublic => BlastVelocity;

        public override void AI() //constantly cycles each time
        {
            Timer++;
            NPC.TargetClosest(true);
            NPC.spriteDirection = NPC.direction;
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest(true);
                NPC.spriteDirection = NPC.direction;
            }
            if (!NPC.HasValidTarget)
            {
                NPC.velocity.X *= 0;
                return;
            }
            switch (State)
            {
                case StateIDWalk:
                    State_Walk();
                    break;
                case StateIDVulcanJab:
                    State_VulcanJab();
                    break;
                case StateIDBlast:
                    State_Blast();
                    break;
                default:
                    State = StateIDWalk;
                    Timer = 0;
                    break;
            }

            //for stepping up tiles
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
        }
        float JumpHeight => (JumpSpeed * JumpSpeed) / (2 * NPC.gravity);
        private void Debug_DisplayAggroRangeBox()
        {
            Vector2 tl = NPC.Center + new Vector2(-AggroRange, -AggroRange);
            Vector2 br = NPC.Center + new Vector2(AggroRange, AggroRange);
            Dust.QuickBox(tl, br, (int)(AggroRange / 8), Color.White, null);
        }

        void State_Walk()
        {
            Player player = Main.player[NPC.target];
            NPC.velocity.X = NPC.direction * WalkSpeed;
         
            Vector2 deltaPos = player.Center - NPC.Center;
            if (MathF.Abs(deltaPos.X) < AggroRange && MathF.Abs(deltaPos.Y) < AggroRange)
            {
                Timer = 0;
                State = StateIDVulcanJab;
            }

           
            if (NPC.velocity.Y == 0)
            {
                if (Timer > 400)
                {
                    Timer = 0;
                    State = StateIDBlast;
                    return;
                }
                CheckForJumpOffTiles();
                if (NPC.collideX)
                {
                    NPC.velocity.Y = JumpSpeed;
                }
            }
        }
        void CheckForJumpOffTiles()
        {
            Player player = Main.player[NPC.target];

            Vector2 bottomLeft = NPC.BottomLeft;
            if (bottomLeft.Y < player.BottomLeft.Y)//if above player then don't need to jump
            {
                return;
            }
            bottomLeft.X += NPC.velocity.X;
            bottomLeft.Y += 4;

            if (!Collision.SolidTiles(bottomLeft, NPC.width, 1))
            {
                NPC.velocity.Y = JumpSpeed;
            }
        }
        public override void FindFrame(int frameHeight) // animation
        {
            if (State == 0 || State == 1) //sidestep
            {
                NPC.frameCounter += 1;
                if (NPC.frameCounter < 5)
                {
                    NPC.frame.Y = 0;
                }
                else if (NPC.frameCounter < 10)
                {
                    NPC.frame.Y = frameHeight;
                }
                else if (NPC.frameCounter < 15)
                {
                    NPC.frame.Y = frameHeight * 2;
                }
                else if (NPC.frameCounter < 20)
                {
                    NPC.frame.Y = frameHeight;
                }
                else
                {
                    NPC.frameCounter = 0;
                }
            }
            if (State == 2) //vulcan jabs
            {
                NPC.frameCounter += 1;

                if (Timer >= VulcanJabChargeup) //attacking
                {
                    if (NPC.frameCounter < 3)
                    {
                        NPC.frame.Y = frameHeight * 5; //punch
                    }
                    else
                    {
                        NPC.frame.Y = frameHeight * 6; //punch
                    }
                    if (NPC.frameCounter > 6)
                    {
                        NPC.frameCounter = 0;
                    }
                }
                else if (Timer >= 5) //ready punch
                {
                    NPC.frame.Y = frameHeight * 4;
                }
                else
                {
                    NPC.frame.Y = frameHeight * 3;
                }
            }
            if (State == 3) //smash punch
            {
                NPC.frameCounter += 1;

                if (Timer >= BlastChargeup && (Timer - BlastChargeup) % BlastRate < 10) //blast
                {
                    NPC.frame.Y = frameHeight * 9;

                    if (Timer >= BlastChargeup + 5)
                    {
                        NPC.frame.Y = frameHeight * 10;
                    }
                    else
                    {
                        NPC.frame.Y = frameHeight * 9;
                    }
                }
                else //charge
                {
                    NPC.frameCounter += 1;

                    if (NPC.frameCounter < 5)
                    {
                        NPC.frame.Y = frameHeight * 8;
                    }
                    else if (NPC.frameCounter < 10)
                    {
                        NPC.frame.Y = frameHeight * 7;
                    }
                    else
                    {
                        NPC.frameCounter = 0;
                    }
                }
            }
        }
        void State_VulcanJab()
        {
            NPC.velocity.X *= 0.9f;
            if (Timer < VulcanJabChargeup)
                return;
            else if (Timer < VulcanJabChargeup + VulcanJabShotCount * VulcanJabRate)
            {
                if ((Timer - VulcanJabChargeup) % VulcanJabRate == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item1 with { MaxInstances = 0 }, NPC.Center);//swing
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Player plr = Main.player[NPC.target];
                        Vector2 shootVel = NPC.DirectionTo(plr.Center).RotatedByRandom(VulcanJabSpread) * VulcanJabVelocity;

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, shootVel, ModContent.ProjectileType<VulcanPunch>(), VulcanJabDamage, 1f);
                    }
                }
            }
            else
            {
                State = StateIDBlast;
                Timer = 0;
            }
        }
        void State_Blast()
        {
            NPC.velocity.X *= 0.9f;
            if (Timer < BlastChargeup)
                return;
            else if (Timer < BlastChargeup + BlastShotCount * BlastRate)
            {
                if ((Timer - BlastChargeup) % BlastRate == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item158, NPC.Center);//zapinator pew
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Player plr = Main.player[NPC.target];
                        Vector2 shootVel = NPC.DirectionTo(plr.Center) * BlastVelocity;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, shootVel, ModContent.ProjectileType<JoeBlast>(), BlastDamage, 1f);
                    }
                }
            }
            else
            {
                State = StateIDWalk;
                Timer = 0;
            }
        }
        private void Walk() //walk towards player
        {
            Player player = Main.player[NPC.target];
            Vector2 distance = player.Center - NPC.Center;


            //if (walkTimer % 10 == 0) //turn towards player every 10 ticks
            {
                NPC.TargetClosest(true);
            }
            NPC.velocity.X = NPC.direction * 1.6f;

            if (distance.X > 0) //player is ahead
            {
                //walkDirection = 1; //walk forward
            }
            else //player is behind
            {
                // walkDirection = -1; //walk forward
            }

            Jump();
        }

        private void Sidestep() //back up or move forward randomly
        {
            NPC.TargetClosest(true);

            Player player = Main.player[NPC.target];
            Vector2 distance = player.Center - NPC.Center;

            if (Timer % 10 == 0)
            {
                if (distance.X > 0) //player is ahead
                {
                    if (distance.X > 200) //far enough
                    {
                        // walkDirection = 1; //walk forward
                    }
                    if (distance.X < 100) //close enough
                    {
                        //  walkDirection = -1; //walk backward
                    }
                }
                else //player is behind
                {
                    if (distance.X < -200) //far enough
                    {
                        //walkDirection = -1; //walk forward (reversed)
                    }
                    if (distance.X > -100) //close enough
                    {
                        //  walkDirection = 1; //walk backward (reversed)
                    }
                }
            }

            Jump();

            // NPC.velocity.X = walkDirection * 1.6f;
        }

        private void Jump()
        {
            if (NPC.collideX && NPC.velocity.Y == 0) //hop if touching wall
            {
                NPC.velocity.Y = JumpSpeed;
            }
        }

        private void RapidPunch() //fires punches
        {
            if (Timer < 120 + 60) //stance (add 120 as that's where it starts from)
            {
                NPC.TargetClosest(true); //face player
                NPC.velocity.X *= 0.5f; //slow
            }
            else //punch
            {
                Player player = Main.player[NPC.target];
                Vector2 projshoot = NPC.Center + new Vector2(NPC.direction * 200, 0) - NPC.Center; //200 in the direction it's facing(doesn't have to be 200 because we normalize it)
                projshoot.Normalize();
                projshoot *= 10f;

                NPC.velocity.X *= 0.5f; //slow
                if (Timer >= 120 + 60) //unleash punch
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && Timer % VulcanJabRate == 0)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 20, 0), projshoot.RotatedByRandom(MathHelper.ToRadians(45)), Mod.Find<ModProjectile>("VulcanPunch").Type, 30 / 2, 0.1f, Main.myPlayer, 0, 0);
                    }

                    if (Timer % 5 == 0) //every 5 ticks
                    {
                        SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
                    }
                }
                if (Timer >= 120 + 180) //restart after 2 seconds
                {
                    State = 1;
                    Timer = 0;
                }
            }
        }

        private void ChargeBlast() //blast 
        {
            if (Timer < 120 + 60)
            {
                NPC.TargetClosest(true);
                NPC.velocity.X *= 0.5f; //slow
            }
            Player player = Main.player[NPC.target];
            Vector2 projshoot = NPC.Center.DirectionTo(player.Center);
            projshoot.Normalize();
            projshoot *= 10f;

            if (Timer == 120 + 60) //climax
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projshoot, Mod.Find<ModProjectile>("JoeBlast").Type, 30, 8, Main.myPlayer, 0, 0);
                }
                SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
            }
            if (Timer >= 120 + 120) //restart
            {
                State = 1;
                Timer = 0;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Items.Weapons.FighterGlove>(), 20, 10)); // 1 in 20 (5%) chance in Normal. 1 in 10 (10%) chance in Expert
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Starbit>(), 1, 2, 4));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (NPC.life <= 0)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle edge
                        Gore.NewGorePerfect(NPC.GetSource_FromAI(), NPC.Center, speed, Main.rand.Next(16, 18));
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                        Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, speed, Main.rand.Next(11, 13), Scale: 1f); //double jump smoke
                    }
                }
            }
        }
    }
}
