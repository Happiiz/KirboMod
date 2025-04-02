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
using KirboMod.Bestiary;
using System.IO;
using KirboMod.Projectiles;

namespace KirboMod.NPCs
{
	public class BladeKnight : ModNPC
	{
		int AttackTimeDecrease { get => Main.expertMode ? 20 : 0; }

		private byte attacktype = 0;
		private bool jumped = false;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Blade Knight");
			Main.npcFrameCount[NPC.type] = 13;
		}
		public override void SetDefaults()
		{
			NPC.width = 33;
			NPC.height = 33;
			DrawOffsetY = 6;
			NPC.damage = 4;
			NPC.defense = 4;
			NPC.lifeMax = 40;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(0, 0, 0, 5);
			NPC.knockBackResist = .3f;
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.BladeKnightBanner>();
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
                    return spawnInfo.SpawnTileType == TileID.SnowBlock ? 0.075f : 0f;
                }
                else if (spawnInfo.Player.ZoneForest) //if forest
                {
                    return spawnInfo.SpawnTileType == TileID.Grass || spawnInfo.SpawnTileType == TileID.Dirt ? .13f : 0f;
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
				new FlavorTextBestiaryInfoElement("A fearless warrior constantly searching for a challenge to appease his desire for battle. Despite being an ametaur, anyways.")
            });
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(attacktype); //send non NPC.ai array info to servers
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            attacktype = reader.ReadByte(); //sync in multiplayer
        }
        public override void AI() //constantly cycles each time
		{
			NPC.spriteDirection = NPC.direction;
			Player player = Main.player[NPC.target];
			Vector2 distance = player.Center - NPC.Center;
			bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
			float rangeX = 100;
			float rangeY = 60;
			if (distance.X < rangeX & distance.X > -rangeX & distance.Y > -rangeY & distance.Y < rangeY && lineOfSight && !player.dead) //checks if the knight is in range
			{
				NPC.ai[1] = 1; //starts attacking if in range
			}
			if (NPC.ai[0] >= 60 - AttackTimeDecrease)
			{
				attacktype = 2;
			}
			else
			{
				attacktype = 1;
			}
			if (NPC.ai[1] == 0) //checks if not slash and in range
			{
				attacktype = 0;
			}
			//declaring attacktype values
			if (attacktype == 0)
			{
				Walk();
			}
			if (attacktype == 1)
			{
				Stance();
			}
			if (attacktype == 2)
            {
				Slash();
            }

			//for stepping up tiles
			Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
		}

		public override void FindFrame(int frameHeight) // animation
		{
            if (attacktype == 0) //walk cycle
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
                else
                {
                    NPC.frameCounter = 0.0;
                }
            }
            if (attacktype == 1) //charge
            {
                NPC.frameCounter += 1.0;
                if (NPC.frameCounter < 5.0)
                {
                    NPC.frame.Y = frameHeight * 4;
                }
                else if (NPC.frameCounter < 10.0)
                {
                    NPC.frame.Y = frameHeight * 5;
                }
                else
                {
                    NPC.frameCounter = 0.0;
                }
            }
            if (attacktype == 2) //slash
            {
                NPC.frameCounter += 1.0;
                if (NPC.frameCounter < 4.0)
                {
                    NPC.frame.Y = frameHeight * 6;
                }
                else if (NPC.frameCounter < 8.0)
                {
                    NPC.frame.Y = frameHeight * 7;
                }
                else if (NPC.frameCounter < 12.0)
                {
                    NPC.frame.Y = frameHeight * 8;
                }
                else if (NPC.frameCounter < 16.0)
                {
                    NPC.frame.Y = frameHeight * 9;
                }
                else if (NPC.frameCounter < 20.0)
                {
                    NPC.frame.Y = frameHeight * 12;
                }
            }
        }

		private void Walk() //walk towards player
		{
			Player player = Main.player[NPC.target];
			NPC.TargetClosest(true);

            float speed = 1f; //top speed
            if (Main.expertMode)
				speed *= 1.3334f;
			float inertia = 10f; //acceleration and decceleration speed
			float jumpSpeed = 7;
			Vector2 direction = NPC.Center + new Vector2( NPC.direction * 50, 0) - NPC.Center; //start - end 
			//we put this instead of player.Center so it will always be moving top speed instead of slowing down when player is near

			direction.Normalize();
			direction *= speed;
			if (NPC.velocity.Y == 0 || jumped == true) //walking/jumping (so it doesn't interfere with knockback)
			{
				NPC.velocity.X = (NPC.velocity.X * (inertia - 1) + direction.X) / inertia; //use .X so it only effects horizontal movement
			}

            if (NPC.collideX && NPC.velocity.Y == 0) //hop if touching wall
            {
                NPC.velocity.Y = -jumpSpeed;
				jumped = true; 
            }

			if (NPC.velocity.Y == 0) //on ground
			{
				jumped = false;
			}
        }

		private void Stance() //readies attack
        {
			if (NPC.ai[1] == 1)
			{
				NPC.ai[0]++;
				NPC.TargetClosest(true); //face player
				NPC.velocity.X *= 0.9f; //slow
			}
        }

		private void Slash() //attacks
        {
			Player player = Main.player[NPC.target];
            NPC.ai[0]++;

            NPC.velocity.X *= 0.9f; //slow

			if (NPC.ai[0] == 61 - AttackTimeDecrease) //unleash slash
			{
                NPC.frameCounter = 0; //reset frame counter

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + (NPC.direction * 20), NPC.Center.Y, NPC.direction * 0.01f, 0, ModContent.ProjectileType<BioSparkSlashHitbox>(), 16 / 2, 5f, Main.myPlayer, NPC.whoAmI, 0);
				}
				SoundEngine.PlaySound(SoundID.Item1, NPC.Center); 
			}
			if (NPC.ai[0] >= 120 - AttackTimeDecrease) //restart
            {
				NPC.ai[0] = 0;
				NPC.ai[1] = 0;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Items.Weapons.HeroSword>(), 40, 20)); // 1 in 40 (2.5%) chance in Normal. 1 in 20 (5%) chance in Expert
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Starbit>(), 1, 2, 4));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 10; i++) //first section makes inital statement once //second declares the conditional they must follow // third declares the loop
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
