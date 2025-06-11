using KirboMod.Items;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using KirboMod.Items.Weapons;
using Steamworks;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using KirboMod.Projectiles;

namespace KirboMod.NPCs
{
	public class PoppyBrosJr : ModNPC
	{
        public ref float AttackTimer => ref NPC.ai[0];
		public ref float Attacktype => ref NPC.ai[1];
        static float BombYLaunchVelocity { get => Main.expertMode ? -10 : -8; }
        
        private bool jumped = false;

        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Poppy Bros. Jr.");
			Main.npcFrameCount[NPC.type] = 12;
		}

		public override void SetDefaults()
		{
			NPC.width = 50;
			NPC.height = 54;
			DrawOffsetY = -2; //make sprite line up with hitbox
			NPC.damage = 10;
			NPC.defense = 5;
			NPC.lifeMax = 60;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(0, 0, 0, 5);
			NPC.knockBackResist = 1f;
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.PoppyBrosJrBanner>();
			NPC.aiStyle = -1;
			NPC.friendly = false;
			NPC.noGravity = false;
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("This happy little demolitionist loves to throw, toss, and hurl bombs! All while keep a wide grin on its face that never seems to wash off!")
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			//if player is in snow biome and daytime and spawn is not in water
			if (spawnInfo.Player.ZoneTowerVortex || spawnInfo.Player.ZoneTowerSolar
                || spawnInfo.Player.ZoneTowerNebula || spawnInfo.Player.ZoneTowerStardust)
			{
				return 0f;
			}
			else if (spawnInfo.Player.ZoneSnow && spawnInfo.Player.ZoneOverworldHeight && Main.dayTime && !spawnInfo.Water && !spawnInfo.Sky
                && !Main.eclipse)
			{
				return spawnInfo.SpawnTileType == TileID.SnowBlock || spawnInfo.SpawnTileType == TileID.IceBlock ? .2f : 0f; //functions like a mini if else statement
			}
			else
			{
				return 0f; //no spawn rate
			}
		}
		public override void AI() //constantly cycles each time
		{
			NPC.spriteDirection = NPC.direction;
			Player player = Main.player[NPC.target];
			Vector2 distance = player.Center - NPC.Center;

			bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);

			if (distance.X < 480 & distance.X > -480 & distance.Y > -120 & distance.Y < 120 && lineOfSight && !player.dead) //checks if da bomb is in range
			{
                Attacktype = 1; //now start attacking
			}

			//declaring attacktype values
			if (Attacktype == 0)
			{
				Walk();
			}
			if (Attacktype == 1)
			{
				Bomb();
			}

			//for stepping up tiles
			Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
		}

		public override void FindFrame(int frameHeight) // animation
		{
			if (Attacktype == 0)
			{
                NPC.frameCounter += 1.0;
                if (NPC.frameCounter < 8.0)
                {
                    NPC.frame.Y = 0;
                }
                else if (NPC.frameCounter < 16.0)
                {
                    NPC.frame.Y = frameHeight;
                }
                else if (NPC.frameCounter < 24.0)
                {
                    NPC.frame.Y = frameHeight * 2;
                }
                else if (NPC.frameCounter < 32.0)
                {
                    NPC.frame.Y = frameHeight * 3;
                }
                else if (NPC.frameCounter < 40.0)
                {
                    NPC.frame.Y = frameHeight * 4;
                }
                else if (NPC.frameCounter < 48.0)
                {
                    NPC.frame.Y = frameHeight * 5;
                }
                else if (NPC.frameCounter < 56.0)
                {
                    NPC.frame.Y = frameHeight * 6;
                }
                else if (NPC.frameCounter < 64.0)
                {
                    NPC.frame.Y = frameHeight * 7;
                }
                else
                {
                    NPC.frameCounter = 0.0;
                }
            }
			if (Attacktype == 1) //throwing bomb
			{
				if (AttackTimer < 30)
                {
                    NPC.frame.Y = frameHeight * 8; //throw 1
                    NPC.frameCounter = 0.0;
                }
                else
                {
                    NPC.frameCounter += 1.0;
                    if (AttackTimer > 90)
                    {
                        NPC.frame.Y = frameHeight * 5;
                    }
                    if (NPC.frameCounter < 10.0)
                    {
                        NPC.frame.Y = frameHeight * 9;
                    }
                    else if (NPC.frameCounter < 20.0)
                    {
                        NPC.frame.Y = frameHeight * 10;
                    }
                    else if (NPC.frameCounter < 30.0)
                    {
                        NPC.frame.Y = frameHeight * 11;
                    }
                }
            }
		}
        static float YPointInTrajectory(float initialPosY, float initialVelY, float accelY, float time)
        {
            return initialPosY + initialVelY * time +  accelY * time * time * 0.5f; 
        }
        static float TimeToReachYVel(float targetVelY, float initialVelY, float accelY)
        {
            return (targetVelY - initialVelY) / accelY;
        }
        static float TimeToReachYPoint(float fromY, float toY, float accelY, float initialVelY)
        {
            float bombTerminalYVel = Projectiles.PoppyBomb.BombMaxSpeed;
            float timeToReachTerminalVel = (bombTerminalYVel - initialVelY) / accelY;
            bool hasSolution = Utils.SolveQuadratic(accelY * .5f, initialVelY, fromY - toY, out float result1, out float result2);
            if (!hasSolution)
            {
                return 99999f;
            }
            float time = MathF.Max(result2, result1);
            if (time > timeToReachTerminalVel)
            {
                time = timeToReachTerminalVel;
                float yPointReachTerminalVelocity = fromY + initialVelY * timeToReachTerminalVel + accelY * timeToReachTerminalVel * timeToReachTerminalVel * 0.5f;
                time -= (yPointReachTerminalVelocity - toY) / bombTerminalYVel;
            }
            return time;
        }

		private void Walk() //walk towards player
		{

            NPC.TargetClosest();

            float speed = 1f; //top speed
            float inertia = 10f; //acceleration and decceleration speed

            Vector2 direction = NPC.Center + new Vector2(NPC.direction * 50, 0) - NPC.Center; //start - end 
                                                                                              //we put this instead of player.Center so it will always be moving top speed instead of slowing down when player is near
            direction.Normalize();
            direction *= speed;
			if (NPC.velocity.Y == 0 || jumped == true) //walking/jumping (so it doesn't interfere with knockback)
            {
				NPC.velocity.X = (NPC.velocity.X * (inertia - 1) + direction.X) / inertia; //use .X so it only effects horizontal movement	
			}

            if (NPC.collideX && NPC.velocity.Y == 0) //hop if touching wall
            {
                NPC.velocity.Y = -5;
                jumped = true;
            }

            if (NPC.velocity.Y == 0) //on ground
            {
                jumped = false;
            }
        }
		private void Bomb()
        {
            int attackDuration = Main.expertMode ? 90 : 160;

			NPC.velocity.X *= 0.9f; //slow

            AttackTimer++; //goes up by 1 each tick

            NPC.TargetClosest(true);
     
            if (AttackTimer == 1) //when begin
            {
                //convert vector floats to point inches

                for (int i = 0; i < NPC.width; i++)
                {
                    Point rightbelow = new Vector2(NPC.position.X + i, NPC.position.Y + NPC.height).ToTileCoordinates();

                    if (Main.tile[rightbelow.X, rightbelow.Y].HasTile) //ground has tile
                    {
                        NPC.velocity.Y = -10f; //jump...bum..bum..bum..bum might as weeell jump
                        return; //end loop here
                    }
                }
            }
            if (AttackTimer == 30) //when halfway through attack
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Player player = Main.player[NPC.target];
                    float Xprojshoot = player.Center.X - NPC.Center.X;

                    float timeToReach = TimeToReachYPoint(NPC.Center.Y, player.Center.Y, Projectiles.PoppyBomb.BombAcceleration, BombYLaunchVelocity);
                    Xprojshoot /= timeToReach;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, Xprojshoot, 0, ModContent.ProjectileType<PoppyBomb>(), 20 / 2, 0, Main.myPlayer, 0, BombYLaunchVelocity);
                }
            }
            
			if (AttackTimer > attackDuration) //end attack after a second
            {
                Attacktype = 0; //can walk if out of range
                AttackTimer = 0; //ready next attack
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // 1 in 20 (5%) chance in Normal. 1 in 10 (10%) chance in Expert
			DropBasedOnExpertMode drop = new DropBasedOnExpertMode(ItemDropRule.Common(ModContent.ItemType<Bomb>(), 20, 75, 75), ItemDropRule.Common(ModContent.ItemType<Bomb>(), 10, 75, 75));

            npcLoot.Add(drop);
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Starbit>(), 1, 2, 4));
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

        // This npc uses additional textures for drawing
        static Asset<Texture2D> poppyBomb;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            if (Attacktype == 1 && AttackTimer < 30) //about to throw
            {
                Texture2D bomb = poppyBomb.Value;
                Vector2 origin = new Vector2(bomb.Width / 2, bomb.Height / 2); //center
                Vector2 offset = new Vector2(-14, -30);
                float rotation = NPC.direction * MathHelper.ToRadians(-45);

                if (NPC.direction == -1)
                {
                    offset = new Vector2(14, -30); //sprite isn't evenly balanced so we have to offset it differently
                }
                
                spriteBatch.Draw(bomb, NPC.Center - Main.screenPosition + offset, null, drawColor, rotation, origin, 1f, SpriteEffects.None, 0f);
            }
        }
        public override void Load()
        {
            poppyBomb = ModContent.Request<Texture2D>("KirboMod/Projectiles/PoppyBomb");
        }
        public override void Unload()
        {
            poppyBomb = null;
        }
    }
}
