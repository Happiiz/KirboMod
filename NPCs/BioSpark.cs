using KirboMod.Items;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using SoundEngine = Terraria.Audio.SoundEngine;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using KirboMod.Projectiles;

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
			NPC.damage = 70;
			NPC.defense = 30;
			NPC.lifeMax = 400;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(0, 0, 2, 50); // money it drops
			NPC.knockBackResist = 0f; //how much knockback applies
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.BioSparkBanner>();
			NPC.aiStyle = -1; 
			NPC.friendly = false;
			NPC.noGravity = false;
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneRockLayerHeight & Main.hardMode) //if player is within cave height and world is in hardmode
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
				else //only forest
                {
					return spawnInfo.SpawnTileType == TileID.Stone || spawnInfo.SpawnTileType == TileID.Dirt ? .06f : 0f; //functions like a mini if else statement
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
				new FlavorTextBestiaryInfoElement("An elite ninja that hides deep within the caverns, training to become stronger. Ambushes targets with a flurry of attacks.")
            });
        }
        public override void AI() //constantly cycles each time
		{
            if(NPC.localAI[0] == 0)
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
            if (inRange) //checks if the spark is in range
            {
                WalkTimer = 0; //reset walk timer

                if (AttackTimer < 120) //not attacking
                {
                    Attacktype = 1; //side stepping
                }
                else if (AttackTimer == 120) //times up!
                {
                    bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
                    if (Math.Abs(distance.Y) < 100 && distance.Y <= 0 && MathF.Abs(distance.X) < 250) //close enough && above or leveled with player && the slash can actually hit the player before decelerating
                    {
                        Attacktype = 2; //slash
                    }
                    else if (lineOfSight) //can see player
                    {
                        Attacktype = 3; //kunai
                    }
                    else //wait until can see player within range
                    {
                        Attacktype = 1;
                        AttackTimer = 119; //reset attack timer to before time's up
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
            Player player = Main.player[NPC.target];
            Vector2 distance = player.Center - NPC.Center;

            WalkTimer++;

            if (WalkTimer % 10 == 0) //turn towards player every 10 ticks
            {
                NPC.TargetClosest(true);
            }
            NPC.velocity.X = NPC.direction * 2.5f;

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

            NPC.velocity.X = WalkDirection * 2.5f;
        }
        private void Jump()
        {
            if (NPC.collideX && NPC.velocity.Y == 0) //hop if touching wall
            {
                NPC.velocity.Y = -5;
            }
        }
        private void Slash() //draws sword
        {
            if (AttackTimer < 120 + 30) //stance
            {
                NPC.TargetClosest(true); //face player

                NPC.velocity.X *= 0.5f; //stop
            }
            else //slash
            {

                Player player = Main.player[NPC.target];
                if (AttackTimer == 120 + 30) //unleash slash
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient) 
                    {
                        //hitbox
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BioSparkSlashHitbox>(), 70 / 2, 8, Main.myPlayer, NPC.whoAmI, 0);
                    }
                    float moveSpeed = 25;
                    NPC.velocity.X = NPC.direction * moveSpeed;
                    NPC.velocity.Y = -MathF.Abs((player.Center.Y - NPC.Center.Y) / moveSpeed) * 4;
                    SoundEngine.PlaySound(SoundID.Item1, NPC.Center); //we dont define the stuff after coordinates because legacy sound style
                }
                if (AttackTimer > 120 + 30 & AttackTimer < 120 + 60)
                {
                    if(NPC.velocity.Y < -2)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(NPC.Bottom + Main.rand.NextVector2Circular(32,32), DustID.RainbowTorch, Vector2.Zero);
                            dust.noGravity = true;
                            dust.color = Color.Yellow;
                        }
                    }
                    NPC.velocity.X *= 0.92f;
                }
                if (AttackTimer >= 120 + 60) //restart
                {
                    Attacktype = 1;
                    AttackTimer = 0;
                }
            }
        }
		private void Daggers() //slashes
		{
			Player player = Main.player[NPC.target];
			Vector2 projshoot = player.Center - NPC.Center;
            float shootSpeed = 20;
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
            NPC.TargetClosest(true);

			NPC.velocity.X *= 0.5f; //slow

            if (AttackTimer % 5 == 0 && AttackTimer > 120 + 30 && AttackTimer <= 120 + 45) //unleash daggers every 5 ticks within 15 ticks
            {
                for (int i = 0; i < 4; i++)
                {
                    float velocity = Utils.Remap(i, 0, 3, 2, 7);
                    float scale = Utils.Remap(i, 0, 3, 2.5f, .5f);
                    Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(16,16), DustID.SilverCoin, projshoot.Normalized(velocity), Scale: scale);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projshoot, ModContent.ProjectileType<BioDagger>(), 45 / 2, 4, Main.myPlayer, 0, 0);
                }
                SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
            }
            if (AttackTimer >= 120 + 60) //restart
            {
                Attacktype = 1;
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
                if (AttackTimer < 120 + 30)
                {
                    NPC.frame.Y = frameHeight * 6; //frame 7
                }
                else if (AttackTimer < 124 + 30)
                {
                    NPC.frame.Y = frameHeight * 7; //frame 8
                }
                else if (AttackTimer < 128 + 30)
                {
                    NPC.frame.Y = frameHeight * 8; //frame 9
                }
                else if (AttackTimer < 132 + 30)
                {
                    NPC.frame.Y = frameHeight * 9; //frame 10
                }
                else if (AttackTimer < 136 + 30)
                {
                    NPC.frame.Y = frameHeight * 10; //frame 11
                }
                else if (AttackTimer < 140 + 30)
                {
                    NPC.frame.Y = frameHeight * 11; //frame 12
                }
                else if (AttackTimer < 144 + 30)
                {
                    NPC.frame.Y = frameHeight * 12; //frame 13
                }
                else if (AttackTimer < 148 + 30)
                {
                    NPC.frame.Y = frameHeight * 13; //frame 14
                }
                else if (AttackTimer < 152 + 30)
                {
                    NPC.frame.Y = frameHeight * 14; //frame 15
                }
                else if (AttackTimer < 156 + 30)
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
                if (AttackTimer < 120 + 30)
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
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Items.Weapons.ShinobiScroll>(), 20, 10)); // 1 in 20 (5%) chance in Normal. 1 in 10 (10%) chance in Expert
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DreamEssence>(), 1, 2, 4));
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
