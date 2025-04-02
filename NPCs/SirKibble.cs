using KirboMod.Items;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using KirboMod.Bestiary;
using KirboMod.Projectiles;

namespace KirboMod.NPCs
{
	public class SirKibble : ModNPC 
	{
		private int frame = 0; //different types of animation cycles / frames
        public ref float AttackTimer => ref NPC.localAI[0]; //the attack timer
        private bool attacking = false; //controls if in attacking state
		public ref float TimeWhenCutterBladeReachesKibbleAgain => ref NPC.ai[1];
		public ref float MostRecentCutterYVelocity => ref NPC.ai[2];
		float moveSpeedMultiplier = 1;
        private bool jumped = false;
		bool jumpToGrabCutterBack = false;

        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Sir Kibble");
			Main.npcFrameCount[NPC.type] = 7;
		}

		public override void SetDefaults()
		{
			NPC.width = 48;
			NPC.height = 48;
			NPC.damage = 6;
			NPC.defense = 4;
			NPC.lifeMax = 40;
			NPC.HitSound = SoundID.NPCHit4; //metal
			NPC.DeathSound = SoundID.NPCDeath14; //explosive metal
			NPC.value = Item.buyPrice(0, 0, 0, 5); // money it drops
			NPC.knockBackResist = 0.5f; //how much knockback applies
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.SirKibbleBanner>();
			NPC.aiStyle = -1;
			NPC.friendly = false;
			NPC.noGravity = false;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneOverworldHeight && Main.dayTime && !spawnInfo.Invasion && !Main.eclipse) //if player is within surface height & daytime
            {
                if (spawnInfo.Player.ZoneJungle)
                {
                    return spawnInfo.SpawnTileType == TileID.JungleGrass || spawnInfo.SpawnTileType == TileID.Mud ? 0.075f : 0f;
                }
                else if (spawnInfo.Player.ZoneSnow)
                {
                    return spawnInfo.SpawnTileType == TileID.SnowBlock ? .15f : 0f;
                }
                else if (spawnInfo.Player.ZoneForest) //if forest
                {
                    return spawnInfo.SpawnTileType == TileID.Grass || spawnInfo.SpawnTileType == TileID.Dirt ? 0.15f : 0f;
                }
                else
                {
                    return 0f; //no spawn rate
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
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                new SurfaceBackgroundProvider(),

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("A fearless warrior that refuses to back out of a battle, even when the odds are stacked against them.")
            });
        }
        public override void AI() //constantly cycles each time
		{
			NPC.spriteDirection = NPC.direction;
			Player player = Main.player[NPC.target];
			Vector2 distance = player.Center - NPC.Center;
			bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
			float range = 265;
			if (Main.expertMode)
				range *= 1.33f;
			if (distance.X < range & distance.X > -range & distance.Y > -range & distance.Y < range && lineOfSight && !player.dead && NPC.velocity.Y == 0) //checks if the kibble is in range
			{
				attacking = true;
				jumpToGrabCutterBack = false;
			}

			//declaring attacktype values
			if (attacking == false)
			{
				Walk();
			}
			else
			{
				Throw();
			}

			//for stepping up tiles
			Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
		}

		public override void FindFrame(int frameHeight) // animation
		{
			if (jumpToGrabCutterBack)
			{
				NPC.frame.Y = frameHeight;
				return;
			}
			if (attacking == false) //walking
			{
                NPC.frameCounter += 1.0;
                if (NPC.frameCounter < 8) 
                {
                    NPC.frame.Y = 0; 
                }
                else if (NPC.frameCounter < 16) 
                {
                    NPC.frame.Y = frameHeight; 
                }
                else if (NPC.frameCounter < 24)
                {
                    NPC.frame.Y = frameHeight * 2;
                }
                else if (NPC.frameCounter < 32)
                {
                    NPC.frame.Y = frameHeight * 3;
                }
				else
				{
                    NPC.frameCounter = 0; //reset
                }
            }
			else //attacking
			{
                NPC.frameCounter += 1.0;
                if (NPC.frameCounter < 60) //before throw
				{
					NPC.frame.Y = frameHeight * 4; //lean backward
				}
                else if (NPC.frameCounter < 65)//after throw
                {
                    NPC.frame.Y = frameHeight * 5; //lean forward
                }
                else //second throw frame
                {
                    NPC.frame.Y = frameHeight * 6; //lean forward
                }
            }
		}

		private void Walk() //walk towards player
		{
			Player player = Main.player[NPC.target];
			NPC.TargetClosest(true); //face target

			float speed = 1f; //top speed
			float inertia = 10f; //acceleration and decceleration speed

			Vector2 direction = NPC.Center + new Vector2(NPC.direction * 50, 0) - NPC.Center; //start - end 
			//we put this instead of player.Center so it will always be moving top speed instead of slowing down when player is near

			direction.Normalize(); //reduce to 1
			direction *= speed; //equal speed
            if (jumpToGrabCutterBack)
            {
				if (NPC.velocity.Y == 0)
					jumpToGrabCutterBack = false;
				return;
            }
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
		private void Throw()
        {
			//this instead of player.Center so it throws in the direction it's facing
			AttackTimer++;

			//facing
			if (AttackTimer == 1)
            {
				NPC.TargetClosest(true); //don't face target
                NPC.frameCounter = 0; //reset frame counter
            }

			NPC.velocity.X *= 0.9f; //slow

			//make player index be the target to follow
			//or maybe just calculate a Y velocity based on height difference(probably better)
			//calculate time to reach x
			const int attackThreshold = 60;
			float cutterAcceleration = 1;
			if (AttackTimer == attackThreshold) //throw
			{
				float shotVelocity = 13;
				if (Main.expertMode)
					shotVelocity *= 1.333f;
				//this also sets TimeWhenCutterBladeReachesKibbleAgain
				BadCutter.ShootBadCutter(NPC, shotVelocity, cutterAcceleration);
			}
			float heightWhenReachesKibbleAgain = MostRecentCutterYVelocity;
			float relativeHeightWhenReachesKibbleAgain = heightWhenReachesKibbleAgain - NPC.Center.Y;
			if(relativeHeightWhenReachesKibbleAgain > -200 && relativeHeightWhenReachesKibbleAgain < -16)
            {
				if(AttackTimer >= TimeWhenCutterBladeReachesKibbleAgain  / 2 + 60 && TimeWhenCutterBladeReachesKibbleAgain > 0 && NPC.velocity.Y >= 0 && Collision.CanHitLine(NPC.position,NPC.width,NPC.height, NPC.position + new Vector2(0,relativeHeightWhenReachesKibbleAgain), NPC.width, NPC.height))
                {
					NPC.velocity.Y = GetVelFromMRUV(NPC.Bottom.Y, heightWhenReachesKibbleAgain, TimeWhenCutterBladeReachesKibbleAgain / 2, NPC.gravity);
					jumpToGrabCutterBack = true;
					attacking = false;
					AttackTimer = 0;
                }

			}
			if (AttackTimer >= TimeWhenCutterBladeReachesKibbleAgain + attackThreshold && TimeWhenCutterBladeReachesKibbleAgain > 0)
            {
				AttackTimer = 0; //ready next attack
				jumpToGrabCutterBack = false;
				attacking = false; //stop attacking
            }
        }
		static float GetVelFromMRUV(float initialSpace, float targetSpace, float time, float acceleration)
		{
			return (targetSpace - initialSpace - .5f * acceleration * time * time) / time;
		}
		public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Items.Weapons.Cutter>(), 40, 20)); // 1 in 40 (2.5%) chance in Normal. 1 in 20 (5%) chance in Expert
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
