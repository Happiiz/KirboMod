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
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using KirboMod.Projectiles;
using KirboMod.ItemDropRules.DropConditions;

namespace KirboMod.NPCs.MidBosses
{
    [AutoloadBossHead]
    public class MrFrosty : ModNPC
	{
        private int attacktype = -1;
        private int lastattack = 2;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Mr. Frosty");
			Main.npcFrameCount[NPC.type] = 15;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                PortraitScale = 1f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 20f,
                PortraitPositionXOverride = 0f,
                Position = new Vector2(20, 40),
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true; //immune because of boss-like behavior
        }

		public override void SetDefaults()
		{
			NPC.width = 80;
			NPC.height = 100;
            DrawOffsetY = 28;
            NPC.damage = Main.hardMode ? (NPC.downedGolemBoss ? 120 : 80) : 40;
            NPC.defense = Main.hardMode ? 30 : 15;
            NPC.lifeMax = Main.hardMode ? (NPC.downedGolemBoss ? 32000 : 16000) : 800;
            NPC.HitSound = SoundID.NPCHit14; //fishron squeal
			NPC.DeathSound = SoundID.NPCDeath8; //grunt
			NPC.value = Main.hardMode ? (NPC.downedGolemBoss ? 200000 : 50000) : 5000; // money it drops (20 gold / 5 gold / 50 silver)
            NPC.knockBackResist = 0f; //how much knockback applies
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<Items.Banners.MrFrostyBanner>();
            NPC.aiStyle = -1; 
			NPC.friendly = false;
			NPC.noGravity = false;
			NPC.rarity = 1; //1 is dungeon slime, 4 is mimic
            NPC.coldDamage = true;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
			if (Main.hardMode)
			{
                Helper.BossHpScalingForHigherDifficulty(ref NPC.lifeMax, balance);
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
            return 0f; //no spawn rate
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("An icy beast that appeared in the tundra from a strange star shaped rift. Does its best to stamp out any threats with style and grace, or lack thereof.")
            });
        }
        public override void AI() //constantly cycles each time
		{
			NPC.spriteDirection = NPC.direction;
			Player player = Main.player[NPC.target];

            if (player.dead) //player has died
            {
                attacktype = 1; //dive dash
                NPC.timeLeft = 40;
            }
            else
            {
                if (NPC.ai[0] == 0) //not attacking
                {
                    attacktype = 0; //standing
                    NPC.ai[3]++; //attack timer
                }
                else
                {
                    NPC.ai[3] = 0; //restart attack timer
                }

                if (NPC.ai[3] >= (Main.expertMode ? 30 : 60)) //times up! (50 if in expertmode)
                {
                    if (attacktype == 0)
                    {
                        NPC.ai[0] += 1; //attack!
                        NPC.ai[3] = 0;
                    }
                }

                if (NPC.ai[0] == 1) //stancing or 
                {
                    if (lastattack == 2) //ice
                    {
                        attacktype = 1; //dash
                        lastattack = 1; //also ddash
                    }
                    else
                    {
                        attacktype = 2; //ice
                        lastattack = 2; //also ice
                    }

                }
            }

            //declaring attacktype values
            if (attacktype == 0)
            {
                Stance(player);
            }
            if (attacktype == 1)
            {
                DiveDash();
            }
            if (attacktype == 2)
            {
                IceToss();
            }
        }

		public override void FindFrame(int frameHeight) // animation
		{
			if (attacktype == 0) //bobbing
			{
                NPC.frameCounter += 1;
                
                if (NPC.frameCounter < 10)
                {
                    NPC.frame.Y = 0;
                }
                else if (NPC.frameCounter < 20)
                {
                    NPC.frame.Y = frameHeight;
                }
                else if (NPC.frameCounter < 35)
                {
                    NPC.frame.Y = frameHeight * 2;
                }
                else
                {
                    NPC.frameCounter = 0;
                }
            }
			if (attacktype == 1) //dive attack
			{
				if (NPC.ai[0] > 180) //the dive
				{
                    NPC.frame.Y = frameHeight * 7; //dive
                }
				else //dashing
				{
                    NPC.frameCounter += 1;

                    if (NPC.frameCounter < 5)
                    {
                        NPC.frame.Y = frameHeight * 3;
                    }
                    else if (NPC.frameCounter < 10)
                    {
                        NPC.frame.Y = frameHeight  * 4;
                    }
                    else if (NPC.frameCounter < 15)
                    {
                        NPC.frame.Y = frameHeight * 5;
                    }
                    else if (NPC.frameCounter < 20)
                    {
                        NPC.frame.Y = frameHeight * 6;
                    }
                    else
                    {
                        NPC.frameCounter = 0;
                    }
                }
			}
			if (attacktype == 2) //ice cube
			{
				if (NPC.ai[0] >= 100)  //throw 3
                {
                    NPC.frame.Y = frameHeight * 14;
                }
                else if (NPC.ai[0] >= 95)  //throw 2
                {
                    NPC.frame.Y = frameHeight * 13;
                }
                else if(NPC.ai[0] >= 90)  //throw 1
                {
                    NPC.frame.Y = frameHeight * 12;
                }
                else if (NPC.ai[0] >= 60)  //prepare ice
                {
                    NPC.frame.Y = frameHeight * 11;
                }
                else //shake that
				{
                    NPC.frameCounter += 1;
					if (NPC.frameCounter < 8)
					{
						NPC.frame.Y = frameHeight * 8;
					}
					else if (NPC.frameCounter < 16)
					{
						NPC.frame.Y = frameHeight *  9;
					}
                    else if (NPC.frameCounter < 24)
                    {
                        NPC.frame.Y = frameHeight * 8;
                    }
                    else if (NPC.frameCounter < 32)
                    {
                        NPC.frame.Y = frameHeight * 10;
                    }
                    else
					{
                        NPC.frameCounter = 0;
					}
				}
			}
		}

        private void Stance(Player player) //stand up straight
		{
			NPC.TargetClosest(true); //face player

            NPC.velocity.X *= 0.5f; //slow

            CheckPlatform(player); //go down platforms when player is low
        }

		private void DiveDash() //dives towards ground
        {
            if (NPC.ai[0] == 1) //short hop to warn
            {
                NPC.velocity.Y = -4; //jump
            }

            NPC.ai[0]++;

            if (NPC.ai[0] > 30)
			{
				if (NPC.ai[0] < 180) //dash towards player
				{
					Player player = Main.player[NPC.target];
                    NPC.TargetClosest(true); //face player

                    CheckPlatform(player); //go down platforms when player is low

                    float speed = Main.expertMode ? 15f : 10f; //top speed (15 in expertmode)
                    speed *= Main.hardMode ? 1.5f : 1;
					float inertia = 20f; //acceleration and decceleration speed

                    ClimbTiles(player);

                    MoveX(player, speed, inertia);

                    Vector2 distance = player.Center - NPC.Center;

                    bool inRangeX = MathF.Abs(distance.X) < (Main.hardMode ? 400 : 10);

                    bool inRangeY = distance.Y > (Main.hardMode ? -600 : -200) &&  distance.Y < 100;

                    if (inRangeX && inRangeY && player.dead == false) //past the player, near player and player not dead
                    {
                        if (Main.hardMode) //launch toward player
                        {
                            Vector2 targetPos = player.Bottom;

                            Vector2 vel = new Vector2(Math.Clamp((targetPos.X - NPC.Bottom.X) / 10, -20, 20),
                            Math.Clamp((targetPos.Y - NPC.Bottom.Y) / 10, -20f, 10f));

                            if (NPC.downedGolemBoss) //post-Golem
                            {
                                //predict player position based on velocity
                                Utils.ChaseResults results = Utils.GetChaseResults(NPC.Bottom, vel.Length(), targetPos, player.velocity);
                                targetPos = results.InterceptionPosition;
                                vel = results.ChaserVelocity;
                            }

                            NPC.velocity = vel;
                        }

						NPC.ai[0] = 180; //dive
                    }

                    if (player.dead) //player is no longer with us unfortunately
					{
						NPC.ai[0] = 31;
					}
				}
				else //dive
				{
					Player player = Main.player[NPC.target];

                    NPC.noTileCollide = false;

                    if (!Main.hardMode)
                        NPC.velocity.X *= 0.95f; //slow 

                    if (NPC.ai[0] == 180) //dive bomb
					{
						SoundEngine.PlaySound(SoundID.Item1, NPC.Center); 
					}

					if (NPC.ai[0] >= 240) //restart after 1 second
					{
                        NPC.ai[0] = 0;
						NPC.ai[1] = 0;
                        attacktype = 0; //I only put this here so it would transition animations correctly
                    }
				}
			}
        }

		private void IceToss() //toss a cold block of ice
        {
            if (NPC.ai[0] < 90)
            {
                NPC.TargetClosest();
            }
            if (NPC.ai[0] < 30)
            {
                NPC.velocity.X *= 0.7f; //slow
            }


            NPC.ai[0]++;

            NPC.velocity.X *= 0.7f; //slow

            if (NPC.ai[0] == 60)
            {
                NPC.velocity.Y = -12; //jump
            }

            if (NPC.ai[0] == 90)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float velX = BadIceChunk.GetIceChunkXVelocity(NPC.direction);
                    Vector2 offset = new Vector2(NPC.direction * -26, -80);
                    float velY = Main.hardMode ? -8 : -5;

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + offset.X, NPC.Center.Y + offset.Y, velX, velY, 
                        ModContent.ProjectileType<BadIceChunk>(), (Main.hardMode ? (NPC.downedGolemBoss ? 120 : 80) : 40) / 2, 5f, Main.myPlayer, 0, 0);
                    if (Main.hardMode)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + offset.X, NPC.Center.Y + offset.Y, -velX, velY,
                        ModContent.ProjectileType<BadIceChunk>(), (Main.hardMode ? (NPC.downedGolemBoss ? 120 : 80) : 40) / 2, 5f, Main.myPlayer, 0, 0);
                    }
                }
                SoundEngine.PlaySound(SoundID.Item1 with { Volume = 2 }, NPC.Center);
            }
            if (NPC.ai[0] >= 150) //restart
            {
                NPC.ai[1] = 0;
                NPC.ai[0] = 0;
                attacktype = 0; //put this here so it will transition animations correctly
            }
        }

        private void ClimbTiles(Player player)
        {
            bool climableTiles = false;

            for (int i = 0; i < NPC.height - 30; i++)
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

                    if (player.Center.Y < NPC.Center.Y && !player.dead) //higher than NPC or dead
                    {
                        NPC.velocity.Y = -4f;
                    }

                    break;
                }
            }
        }

        private void MoveX(Player player, float speed, float inertia) //move X position toward player
        {
            //we put this instead of player.Center so it will always be moving top speed instead of slowing down when player is near
            Vector2 direction = NPC.Center + new Vector2(NPC.direction * 50, 0) - NPC.Center; //start - end 

            direction.Normalize();
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
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapons.Ice>())); // Guaranteed
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Starbit>(), 1, 24, 24));

            //1 for pre-Golem, 1 for post-Golem. Both in Hardmode
            
            PreGolemHardmodeCondition PreGolemCondition = new PreGolemHardmodeCondition();
            IItemDropRule HardmodePreGolem = new LeadingConditionRule(PreGolemCondition);

            PostGolemHardmodeCondition PostGolemCondition = new PostGolemHardmodeCondition();
            IItemDropRule HardmodePostGolem = new LeadingConditionRule(PostGolemCondition);

            //Drop two Rare Stones if post-Golem

            HardmodePreGolem.OnSuccess(ItemDropRule.Common(ModContent.ItemType<RareStone>()));

            HardmodePostGolem.OnSuccess(ItemDropRule.Common(ModContent.ItemType<RareStone>(), 1, 2, 2));

            npcLoot.Add(HardmodePreGolem);
            npcLoot.Add(HardmodePostGolem);
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

        // This npc uses additional textures for drawing
        public static Asset<Texture2D> Ice;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Ice = ModContent.Request<Texture2D>("KirboMod/Projectiles/BadIceChunk");

            if (attacktype == 2 && NPC.ai[0] < 90 && NPC.ai[0] > 60) //about to throw
            {
                Texture2D ice = Ice.Value;
                Vector2 origin = new Vector2(ice.Width / 2, ice.Height / 2); //center
                Vector2 offset = new Vector2(-26, -80); 

                if (NPC.direction == -1)
                {
                    offset = new Vector2(26, -80); //sprite isn't evenly balanced so we have to offset it differently
                }

                spriteBatch.Draw(ice, NPC.Center - Main.screenPosition + offset, null, drawColor, 0f, origin, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}
