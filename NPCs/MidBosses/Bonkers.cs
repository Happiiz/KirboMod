using KirboMod.Items;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using SoundEngine = Terraria.Audio.SoundEngine;
using KirboMod.Projectiles;
using Terraria.ModLoader.Utilities;

namespace KirboMod.NPCs.MidBosses
{
    [AutoloadBossHead]
    public class Bonkers : ModNPC
	{
		private int attacktype = -1;
		private int lastattack = 2;

        private int coconutRounds = 3;

        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Bonkers");
			Main.npcFrameCount[NPC.type] = 8;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                //CustomTexturePath = "ExampleMod/Assets/Textures/Bestiary/MinionBoss_Preview",
                PortraitScale = 1f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 10f,
                Position = new Vector2(40, 50),
                PortraitPositionXOverride = 0,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

		public override void SetDefaults()
		{
			NPC.width = 100;
			NPC.height = 100;
			DrawOffsetY = 70;
			NPC.damage = Main.hardMode ? 100 : 50;
			NPC.defense = 15;
			NPC.lifeMax = Main.hardMode ? 2500 : 400;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(0, 0, 50, 0); // money it drops
			NPC.knockBackResist = 0f; //how much knockback applies
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<Items.Banners.BonkersBanner>();
            NPC.aiStyle = -1; 
			NPC.friendly = false;
			NPC.noGravity = false;
			NPC.rarity = 1; //1 is dungeon slime, 4 is mimic
			NPC.lavaImmune = true;
		}

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
		{
			/*(if (Main.hardMode)
			{
				NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance);
				NPC.damage = (int)(NPC.damage * 0.75f);
			}*/
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
            return 0f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("A giant armored monkey that came from a strange star shaped rift, prepared to smash down on any threat that approaches with its hammer!")
            });
        }
        public override void AI() //constantly cycles each time
		{
			NPC.spriteDirection = NPC.direction;
			Player player = Main.player[NPC.target];

            NPC.ai[0]++; //attack delay timer

            if (NPC.ai[0] < 120) //not attacking
            {
                attacktype = 0; //walking
            }
            else
            {
                if (NPC.ai[0] == 120)
                {
                    NPC.noTileCollide = false; //don't phase through tiles

                    if (lastattack == 2) //coconut was last
                    {
                        attacktype = 1; //hammer
                        lastattack = 1; //next is coconut
                    }
                    else
                    {
                        coconutRounds = Main.expertMode ? 6 : 3; //3 or 6 coconut throws

                        attacktype = 2; //coconut
                        lastattack = 2; //next is hammer
                        int delayBeforeFirstCoconut = (int)Utils.Remap(NPC.Distance(player.Center), 100, 600, -30, 0);
                        NPC.ai[1] = delayBeforeFirstCoconut;
                    }
                }

                NPC.ai[1]++; //attack timer
            }

            if (player.dead) //player has died
            {
                attacktype = 0; //walk
                NPC.ai[0] = 0;
                NPC.ai[1] = 0;
            }
            NPC.noGravity = attacktype == 1;
            NPC.GravityMultiplier = MultipliableFloat.One;
            //declaring attacktype values
            if (attacktype == 0)
            {
                Walk();
            }
            if (attacktype == 1)
            {
                if (Main.hardMode)
                {
                    NPC.GravityMultiplier = MultipliableFloat.One * 2;
                }
                Hammer();
                NPC.velocity.Y += NPC.gravity;
                if(NPC.velocity.Y > 16)
                {
                    NPC.velocity.Y = 16;
                }
            }
            if (attacktype == 2)
            {
                ExplosiveCoconut();
            }
        }

		public override void FindFrame(int frameHeight) // animation
		{
            if (attacktype == -1) //just spawned
            {
                NPC.frame.Y = 0;
            }
            if (attacktype == 0) //walk
			{
				NPC.frameCounter += 1;
				if (NPC.frameCounter < 10)
				{
					NPC.frame.Y = frameHeight; 
				}
				else if (NPC.frameCounter < 20)
				{
					NPC.frame.Y = frameHeight * 2; 
				}
				else
				{
                    NPC.frameCounter = 0;
				}
			}
			else if (attacktype == 1) //hammer
			{
				if (NPC.ai[1] >= 60) //attacking
				{
					NPC.frame.Y = frameHeight * 4; //swing
				}
				else //stancing
				{
					NPC.frame.Y = frameHeight * 3; //ready hammer
				}
			}
			else if (attacktype == 2) //cocunut
			{
                if (NPC.ai[1] >= (Main.expertMode ? 15 : 30)) //attacking
				{
					NPC.frame.Y = frameHeight * 7; //toss
				}
                else if (NPC.ai[1] >= (Main.expertMode ? 7 : 15))
                {
                    NPC.frame.Y = frameHeight * 6; //hand behind back
                }
                else //stancing
				{
					NPC.frame.Y = frameHeight * 5; //hand behind back
				}
			}
		}

        private void Walk() //walk towards player
		{
			Player player = Main.player[NPC.target];
			if (player.dead == false)
			{
				NPC.TargetClosest(true);
			}

			CheckPlatform(player); //go down platforms when player is low

            float speed = Main.expertMode ? 12f : 8f; 
			float inertia = 15f; //acceleration and decceleration speed

            ClimbTiles(player);

            MoveX(player, speed, inertia);
            Rectangle hitbox = Utils.CenteredRectangle(NPC.Center, new Vector2(NPC.width + 140, NPC.height + 500));
            if (hitbox.Intersects(player.Hitbox))
            {
                NPC.ai[0] = 119;//reached player, stop chasing to avoid stunlock
            }
        }
        static float TimeToReachYPoint(float fromY, float toY, float accelY, float initialVelY)
        {
            bool hasSolution = Utils.SolveQuadratic(accelY * .5f, initialVelY, fromY - toY, out float result1, out float result2);
            if (!hasSolution)
            {
                return 99999f;
            }
            float time = MathF.Max(result2, result1);
            return time;
        }
        private void Hammer() //slams hammer
        {
            Player player = Main.player[NPC.target];
            Vector2 distance = player.Center - NPC.Center;
            if (NPC.ai[1] < 60) //charge
            {
                NPC.TargetClosest(true); //face player

                if (NPC.ai[1] < 30)
                {
                    NPC.velocity.X *= 0.8f; //slow
                }
                else
                {
                    if (NPC.ai[1] == 30) //jump if player is high
                    {
                        float vel = GetYVelForParabolaPeakToBeAt(player.Center.Y - 320, NPC.gravity, NPC.Center.Y);
                        if(vel > -14)
                        {
                            vel = -14;
                        }
                        float time = TimeToReachYPoint(NPC.Center.Y, player.Center.Y, NPC.gravity, vel);
                        distance.X += player.velocity.X * time;
                        NPC.velocity.X = distance.X / time;
                        NPC.velocity.Y = vel;
                        NPC.ai[2] = MathF.Abs(NPC.velocity.X);
                    }

                    float speed = NPC.ai[2];
                    float inertia = 25f; //acceleration and decceleration speed
                    MoveX(player, speed, inertia);

                    NPC.noTileCollide = true;

                    if (NPC.Bottom.Y < player.Center.Y || NPC.velocity.Y < 0) //higher than player or going up
                    {
                        NPC.ai[1] = 31; //reset
                    }
                    else
                    {
                        NPC.ai[1] = 59; //skip ahead
                    }
                }
            }
            else //slam
            {
				NPC.noTileCollide = false;

                NPC.velocity.X *= 0.8f; //slow

                if (NPC.ai[1] == 60) //unleash hammer
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 130, -10), default, 
							ModContent.ProjectileType<BonkersSmash>(), (Main.hardMode ? 100 : 50) / 2, 8f, Main.myPlayer, 0, NPC.whoAmI); 
                    }

                    SoundEngine.PlaySound(SoundID.Item1, NPC.Center); //we dont define the stuff after coordinates because legacy sound style
                }
                if (NPC.ai[1] >= 120) //restart
                {
                    NPC.ai[0] = 0;
                    NPC.ai[1] = 0;
                }
            }
        }
        static float GetYVelForParabolaPeakToBeAt(float peakPosY, float gravityY, float fromY)
        {
            float result = MathF.Abs((peakPosY - fromY) * gravityY * 2);

            return -MathF.Sqrt(result);
        }
        static Vector2 PredictForAcceleratingProj(Player target, float shootSpeed, Vector2 acceleration, Vector2 targetPos, Vector2 from)
        {
            Utils.ChaseResults results = Utils.GetChaseResults(from, shootSpeed, targetPos, target.velocity);
            targetPos = results.InterceptionPosition;
            Vector2 velocity = results.ChaserVelocity;
            float timeToReachX = (targetPos.X - from.X) / velocity.X;
            Vector2 posWhenReachesX = from + velocity * timeToReachX + acceleration * timeToReachX * timeToReachX * .5f;
            float heightOffset = targetPos.Y - posWhenReachesX.Y;
            velocity.Y += heightOffset / timeToReachX;
            return velocity;
        }
        private void ExplosiveCoconut() //explosive coconut throw
		{
            Player player = Main.player[NPC.target];
            NPC.velocity.X *= 0.8f; //slow
            NPC.TargetClosest(true); //face player
         

            if (NPC.ai[1] == (Main.expertMode ? 15 : 30))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 shootFrom = NPC.Center;
                    Vector2 projVel = PredictForAcceleratingProj(player, 15, new Vector2(0, Projectiles.ExplosiveCoconut.yAcceleration), player.Center, shootFrom);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom, projVel, 
                        ModContent.ProjectileType<ExplosiveCoconut>(), (Main.hardMode ? 50 : 25) / 2, 0f, Main.myPlayer, 0, 0, projVel.Y); 
                }
                SoundEngine.PlaySound(SoundID.Item1, NPC.Center);

                coconutRounds--; //go down
            }

            if (NPC.ai[1] == (Main.expertMode ? 30 : 60) && coconutRounds > 0)
            {
                NPC.ai[1] = 0; //reset attack
            }

            if (NPC.ai[1] >= (Main.expertMode ? 90 : 120)) //restart
            {
                NPC.ai[1] = 0;
                NPC.ai[0] = 0;
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
                    Tile tile = Main.tile[(new Vector2((NPC.Right.X), NPC.position.Y + i)).ToTileCoordinates()];
                    climableTiles = WorldGen.SolidOrSlopedTile(tile) || TileID.Sets.Platforms[tile.TileType] || tile.IsHalfBlock;
                }
                else
                {
                    //checks for tiles on left side of NPC
                    Tile tile = Main.tile[(new Vector2((NPC.Left.X), NPC.position.Y + i)).ToTileCoordinates()];
                    climableTiles = WorldGen.SolidOrSlopedTile(tile) || TileID.Sets.Platforms[tile.TileType] || tile.IsHalfBlock;
                }

                if (climableTiles || NPC.velocity.X == 0)
                {
                    NPC.noTileCollide = true;

                    if (player.Center.Y < NPC.Center.Y || !player.dead) //higher than NPC or dead
                    {
                        NPC.velocity.Y = -4f;
                    }

                    break;
                }
            }
        }

        private void MoveX(Player player, float speed, float inertia) //move X position toward player
        {
            Vector2 direction = new Vector2(NPC.direction, 0);
            direction *= speed;
            NPC.velocity.X = (NPC.velocity.X * (inertia - 1) + direction.X) / inertia; //use .X so it only effects horizontal movement
        }

        private void CheckPlatform(Player player) //trust me this is totally unique and original code and definitely not stolen from Spirit Mod's public source code(thx so much btw you don't know the hell I went through with this)
        {
            bool onplatform = true;
            for (int i = (int)NPC.position.X; i < NPC.position.X + NPC.width; i += NPC.width / 4)
            { //check tiles beneath the boss to see if they are all platforms
                Tile tile = Framing.GetTileSafely(new Point((int)NPC.position.X / 16, (int)(NPC.position.Y + NPC.height + 8) / 16));
                if (!TileID.Sets.Platforms[tile.TileType])
                    onplatform = false;
            }
            if (onplatform && (NPC.Center.Y < player.position.Y - 75)) //if they are and the player is lower than the boss, temporarily let the boss ignore tiles to go through them
            {
                NPC.noTileCollide = true;
            }
            else
            {
                NPC.noTileCollide = false;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Items.Weapons.Hammer>(), 1, 1)); // Guaranteed in all difficulties
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Starbit>(), 1, 24, 24));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<Items.RareStone>(), 1, 1, 1));
        }

        public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life <= 0)
            {
                for (int i = 0; i < 8; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                {
                    // go around in a octogonal pattern
                    Vector2 speed = new Vector2((float)Math.Cos(MathHelper.ToRadians(i * 45)) * 20, (float)Math.Sin(MathHelper.ToRadians(i * 45)) * 20);

                    Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.BoldStar>(), speed, Scale: 1.5f); //Makes dust in a messy circle
                    d.noGravity = true;
                }
                for (int i = 0; i < 10; i++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                    Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, speed, Main.rand.Next(11, 13), Scale: 1.5f); //double jump smoke
                }
            }
        }

        public override void ModifyHoverBoundingBox(ref Rectangle boundingBox) //box where NPC name and health is shown
        {
            boundingBox = NPC.Hitbox;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            position.Y = NPC.position.Y + NPC.height + 20;

			return true;
        }

    }
}
