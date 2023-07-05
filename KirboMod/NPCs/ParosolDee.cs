using KirboMod.Items;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using KirboMod.Bestiary;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace KirboMod.NPCs
{
	public class ParosolDee : ModNPC
	{
        public ref float ranan => ref NPC.ai[1];

        public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Parasol Waddle Dee");
			Main.npcFrameCount[NPC.type] = 8;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
				
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

        }

		public override void SetDefaults()
        {
            NPC.width = 36;
            NPC.height = 32;
            NPC.damage = 5;
			NPC.defense = 1; 
			NPC.lifeMax = 20;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = 0f;
			NPC.knockBackResist = 1f;
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.ParosolWaddleDeeBanner>();
			NPC.aiStyle = -1;
			NPC.noGravity = false;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) 
		{
			if (spawnInfo.Player.ZoneOverworldHeight && Main.dayTime) //if player is within surface height & daytime
			{
				if (spawnInfo.Player.ZoneJungle)
				{
					return spawnInfo.SpawnTileType == TileID.JungleGrass || spawnInfo.SpawnTileType == TileID.Mud ? .15f : 0f; //functions like a mini if else statement
				}
				else if (spawnInfo.Player.ZoneSnow)
				{
					return spawnInfo.SpawnTileType == TileID.SnowBlock ? .15f : 0f; //functions like a mini if else statement
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
				else if (spawnInfo.Invasion) //don't spawn during invasions
				{
					return 0f;
				}
				else if (spawnInfo.Player.ZoneMeteor) //don't spawn on meteor
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
                else if (Main.eclipse) //don't spawn during eclipse
                {
                    return 0f;
                }
                else //only forest
				{
					return spawnInfo.SpawnTileType == TileID.Grass || spawnInfo.SpawnTileType == TileID.Dirt ? .3f : 0f; //functions like a mini if else statement
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
				new FlavorTextBestiaryInfoElement("A waddle dee that likes to take it's umbrella out for walks, even when it isn't raining.")
			}); 
        }

        public override void AI() //constantly cycles each time
        {
			NPC.spriteDirection = NPC.direction;

            //reduce gravity by one third 
            NPC.GravityMultiplier /= 4; 
			NPC.MaxFallSpeedMultiplier /= 4;

            if (NPC.ai[0] == 0)
            {
                ranan = Main.rand.Next(0, 10);
                NPC.netUpdate = true;
            }

            if (ranan > 5)
            {
				NPC.direction = 1;
			}
            else
            {
				NPC.direction = -1;
			}
            
			//reroll direction
			++NPC.ai[0];

			if (NPC.ai[0] >= 300)
            {
				NPC.ai[0] = 0f;
            }

			//movement
			float speed = 0.7f;
			float inertia = 20f;

			Vector2 moveTo = NPC.Center + new Vector2(NPC.direction * 200, 0);
			Vector2 direction = moveTo - NPC.Center; //start - end
			direction.Normalize();
			direction *= speed;
			if (NPC.velocity.Y == 0) //on ground (so it doesn't interfere with knockback)
			{
				NPC.velocity.X = (NPC.velocity.X * (inertia - 1) + direction.X) / inertia; //use .X so it only effects horizontal movement
			}

			//for stepping up tiles
			Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
		}

        public override void FindFrame(int frameHeight) // animation
        {
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter < 10.0)
            {
                NPC.frame.Y = 0;
            }
            else if (NPC.frameCounter < 20.0)
            {
                NPC.frame.Y = frameHeight;
            }
            else if (NPC.frameCounter < 30.0)
            {
                NPC.frame.Y = frameHeight * 2;
            }
            else if (NPC.frameCounter < 40.0)
            {
                NPC.frame.Y = frameHeight * 3;
            }
            else if (NPC.frameCounter < 50.0)
            {
                NPC.frame.Y = frameHeight * 4;
            }
            else if (NPC.frameCounter < 60.0)
            {
                NPC.frame.Y = frameHeight * 5;
            }
            else if (NPC.frameCounter < 70.0)
            {
                NPC.frame.Y = frameHeight * 6;
            }
            else if (NPC.frameCounter < 80.0)
            {
                NPC.frame.Y = frameHeight * 7;
            }
            else
            {
                NPC.frameCounter = 0.0;
            }
        }

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life <= 0)
			{
                for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                {
                    Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
                    Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.LilStar>(), speed * 5, Scale: 1f); //Makes dust in a messy circle
                }
                for (int i = 0; i < 5; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                    Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, speed, Main.rand.Next(11, 13), Scale: 1f); //double jump smoke
                }
            }
		}

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Items.Weapons.Parosol>(), 40, 20)); // 1 in 40 (2.5%) chance in Normal. 1 in 20 (5%) chance in Expert
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Starbit>(), 1, 1, 2));
        }
    }
}
