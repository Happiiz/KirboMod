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
    public class BioSpark : ModNPC
    {
        int AttackTimer { get => (int)NPC.ai[0]; set => NPC.ai[0] = value; }
        int WalkTimer { get => (int)NPC.ai[1]; set => NPC.ai[1] = value; }
        int WalkDirection { get => (int)NPC.ai[2]; set => NPC.ai[2] = value; } //determines whether the enemy will walk forward or backward
        private int Attacktype { get => (int)NPC.ai[3]; set => NPC.ai[3] = value; }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bio Spark");
            Main.npcFrameCount[NPC.type] = 16;
        }
        public override void SetDefaults()
        {
            NPC.width = 54;
            NPC.height = 40;
            DrawOffsetY = 4; //make sprite line up with hitbox
            NPC.damage = 60;
            NPC.defense = 35;
            NPC.lifeMax = 650;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = Item.buyPrice(0, 5, 0, 0); // money it drops
            NPC.knockBackResist = 0f; //how much knockback applies
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<Items.Banners.BioSparkBanner>();
            NPC.aiStyle = -1;
            NPC.friendly = false;
            NPC.noGravity = false;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneRockLayerHeight && Main.hardMode) //if player is within cave height
            {
                return spawnInfo.SpawnTileType == TileID.Dirt || spawnInfo.SpawnTileType == TileID.Stone ? .015f : 0f; //functions like a mini if else statement
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
				new FlavorTextBestiaryInfoElement("An elite ninja that hides deep within the caverns, training to become stronger. Ambushes targets with a flurry of attacks.")
            });
        }
        static int StartAttackTime => 90;
        static int TimeSpentWalkingToForceAttack => 100;
        public override void AI() //constantly cycles each time
        {
            if (NPC.localAI[0] == 0)
            {
                WalkDirection = 1;
                NPC.localAI[0] = 1;
            }
            NPC.spriteDirection = NPC.direction;
            Player player = Main.player[NPC.target];
            Vector2 distance = player.Center - NPC.Center;
            bool inRange = Math.Abs(distance.X) < 500 && Math.Abs(distance.Y) < 400 && !player.dead;

            if (Attacktype > 1 || inRange) //attacking or in range
            {
                AttackTimer++; //attack timer
            }
            if (inRange || WalkTimer > TimeSpentWalkingToForceAttack) //checks if the spark is in range
            {
                if (AttackTimer < StartAttackTime && WalkTimer <= TimeSpentWalkingToForceAttack) //not attacking
                {
                    Attacktype = 0; //side stepping
                }
                else if (AttackTimer <= StartAttackTime) //times up!
                {
                    bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
                    if (Math.Abs(distance.Y) < 100 && distance.Y <= 0 && MathF.Abs(distance.X) < 250) //close enough && above or leveled with player && the slash can actually hit the player before decelerating
                    {
                        Attacktype = 2; //slash
                        WalkTimer = 0; //reset walk timer
                        AttackTimer = StartAttackTime;
                    }
                    else if (lineOfSight || WalkTimer > TimeSpentWalkingToForceAttack) //can see player
                    {
                        Attacktype = 3; //kunai
                        WalkTimer = 0; //reset walk timer
                        AttackTimer = StartAttackTime;
                    }
                    else //wait until can see player within range
                    {
                        Attacktype = 0;
                        AttackTimer = StartAttackTime - 1; //reset attack timer to before time's up
                    }
                }
            }
            else if (Attacktype == 1) //sidestepping when out of range
            {
                Attacktype = 0; //walk
                AttackTimer = 0; //reset attack timer
            }

            //declaring attacktype values
            switch (Attacktype)
            {
                case 0:
                    Walk();
                    break;
                case 1:
                    Sidestep();
                    break;
                case 2:
                    Slash();
                    break;
                case 3:
                    Daggers();
                    break;
            }

            //for stepping up tiles
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
        }
        private void Walk() //walk towards player
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];
            Vector2 distance = player.Center - NPC.Center;

            WalkTimer++;
            NPC.velocity.X = NPC.direction * 5f;
            if(distance.Length() < 56)
            {
                AttackTimer = StartAttackTime - 1;
            }
            if (distance.X > 0) //player is ahead
            {
                WalkDirection = 1; //walk forward
            }
            else //player is behind
            {
                WalkDirection = -1; //walk forward
            }

            Jump();
        }
        private void Sidestep()
        {
            NPC.TargetClosest(true);

            Player player = Main.player[NPC.target];
            Vector2 distance = player.Center - NPC.Center;

            if (AttackTimer % 10 == 0)
            {
                if (distance.X > 0) //player is ahead
                {
                    if (distance.X > 200) //far enough
                    {
                        WalkDirection = 1; //walk forward
                    }
                    if (distance.X < 100) //close enough
                    {
                        WalkDirection = -1; //walk backward
                    }
                }
                else //player is behind
                {
                    if (distance.X < -200) //far enough
                    {
                        WalkDirection = -1; //walk forward (reversed)
                    }
                    if (distance.X > -100) //close enough
                    {
                        WalkDirection = 1; //walk backward (reversed)
                    }
                }
            }
            Jump();

            NPC.velocity.X = WalkDirection * 5f;
        }
        private void Jump()
        {
            if (NPC.velocity.Y == 0)
            {
                CheckForJumpOffTiles();
                if (NPC.collideX) //hop if touching wall
                {
                    NPC.velocity.Y = -7;
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
            bottomLeft.Y += 4;
            bottomLeft.X += NPC.velocity.X;
            if (!Collision.SolidTiles(bottomLeft, NPC.width, 1))
            {
                NPC.velocity.Y = -7f;
            }
        }
        private void Slash() //draws sword
        {
            if (AttackTimer < StartAttackTime + 30) //stance
            {
                NPC.TargetClosest(true); //face player

                NPC.velocity.X *= 0.5f; //stop
            }
            else //slash
            {

                Player player = Main.player[NPC.target];
                if (AttackTimer == StartAttackTime + 30) //unleash slash
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        //hitbox
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BioSparkSlashHitbox>(), 55 / 2, 8, Main.myPlayer, NPC.whoAmI, 0);
                    }
                    float moveSpeed = 25;
                    NPC.velocity.X = NPC.direction * moveSpeed;
                    NPC.velocity.Y = -MathF.Abs((player.Center.Y - NPC.Center.Y) / moveSpeed) * 3;
                    SoundEngine.PlaySound(SoundID.Item1, NPC.Center); //we dont define the stuff after coordinates because legacy sound style
                }
                if (AttackTimer > StartAttackTime + 30 & AttackTimer < StartAttackTime + 60)
                {
                    if (NPC.velocity.Y < -2)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(NPC.Bottom + Main.rand.NextVector2Circular(32, 32), DustID.RainbowTorch, Vector2.Zero);
                            dust.noGravity = true;
                            dust.color = Color.Yellow;
                        }
                    }
                    NPC.velocity.X *= 0.92f;
                }
                if (AttackTimer >= StartAttackTime + 60) //restart
                {
                    Attacktype = 0;
                    AttackTimer = 0;
                }
            }
        }
        private void Daggers() //slashes
        {
            Player player = Main.player[NPC.target];
            Vector2 projshoot = player.Center - NPC.Center;
            float shootSpeed = 20;

            NPC.TargetClosest(true);

            NPC.velocity.X *= 0.5f; //slow down

            if (AttackTimer % 5 == 0 && AttackTimer > StartAttackTime + 30 && AttackTimer <= StartAttackTime + 45) //unleash daggers every 5 ticks within 15 ticks
            {
                if (Main.getGoodWorld)
                {
                    Utils.ChaseResults results = Utils.GetChaseResults(NPC.Center, shootSpeed, player.Center, player.velocity);
                    if (results.InterceptionHappens)
                    {
                        projshoot = results.ChaserVelocity;
                    }
                    else
                    {
                        projshoot = projshoot.Normalized(shootSpeed);
                    }
                }
                else
                {
                    projshoot.Normalize();
                    projshoot *= shootSpeed;
                }

                for (int i = 0; i < 4; i++)
                {
                    float velocity = Utils.Remap(i, 0, 3, 2, 7);
                    float scale = Utils.Remap(i, 0, 3, 2.5f, .5f);
                    Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(16, 16), DustID.SilverCoin, projshoot.Normalized(velocity), Scale: scale);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projshoot, ModContent.ProjectileType<BioDagger>(), 45 / 2, 4, Main.myPlayer, 0, 0);
                }
                SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
            }
            if (AttackTimer >= StartAttackTime + 60) //restart
            {
                Attacktype = 0;
                AttackTimer = 0;
            }
        }
        public override void FindFrame(int frameHeight) // animation
        {
            if (Attacktype == 0 || Attacktype == 1) //sidestep
            {
                NPC.frameCounter += 1.0;
                if (NPC.frameCounter < 5.0)
                {
                    NPC.frame.Y = 0; //frame 1
                }
                else if (NPC.frameCounter < 10.0)
                {
                    NPC.frame.Y = frameHeight; //frame 2
                }
                else if (NPC.frameCounter < 15.0)
                {
                    NPC.frame.Y = frameHeight * 2; //frame 3
                }
                else if (NPC.frameCounter < 20.0)
                {
                    NPC.frame.Y = frameHeight * 3; //frame 4
                }
                else
                {
                    NPC.frameCounter = 0.0;
                }
            }
            if (Attacktype == 2) //slash
            {
                if (AttackTimer < StartAttackTime + 30)
                {
                    NPC.frame.Y = frameHeight * 6; //frame 7
                }
                else if (AttackTimer < StartAttackTime + 34)
                {
                    NPC.frame.Y = frameHeight * 7; //frame 8
                }
                else if (AttackTimer < StartAttackTime + 38)
                {
                    NPC.frame.Y = frameHeight * 8; //frame 9
                }
                else if (AttackTimer < StartAttackTime + 42)
                {
                    NPC.frame.Y = frameHeight * 9; //frame 10
                }
                else if (AttackTimer < StartAttackTime + 46)
                {
                    NPC.frame.Y = frameHeight * 12; //frame 13
                }
                else if (AttackTimer < StartAttackTime + 50)
                {
                    NPC.frame.Y = frameHeight * 15; //frame 16
                }
                else
                {
                    NPC.frameCounter = 0.0;
                }
            }
            else if (Attacktype == 3) //kunai
            {
                if (AttackTimer < StartAttackTime + 30)
                {
                    NPC.frame.Y = frameHeight * 4; //frame 5
                }
                else
                {
                    NPC.frame.Y = frameHeight * 5; //frame 6
                }
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Items.Weapons.ShinobiScroll>(), 10, 5)); // 1 in 20 (5%) chance in Normal. 1 in 10 (10%) chance in Expert
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DreamEssence>(), 1, 2, 16));
        }
        public override void HitEffect(NPC.HitInfo hit)
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
        public override void ModifyHoverBoundingBox(ref Rectangle boundingBox) => boundingBox = NPC.Hitbox;
    }
}
