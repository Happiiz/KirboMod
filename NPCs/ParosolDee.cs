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
using KirboMod.NPCs.NPCConfusionHelper;

namespace KirboMod.NPCs
{
	public class ParosolDee : ModNPC
	{

        public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Parasol Waddle Dee");
			Main.npcFrameCount[NPC.type] = 8;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
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
            NPC.direction = Main.rand.NextBool(2) == true ? 1 : -1;
        }

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneOverworldHeight && Main.dayTime && !spawnInfo.Invasion && !Main.eclipse) //if player is within surface height & daytime
            {
                if (spawnInfo.Player.ZoneJungle)
                {
                    return spawnInfo.SpawnTileType == TileID.JungleGrass || spawnInfo.SpawnTileType == TileID.Mud ? .15f : 0f;
                }
                else if (spawnInfo.Player.ZoneSnow)
                {
                    return spawnInfo.SpawnTileType == TileID.SnowBlock ? .15f : 0f;
                }
                else if (spawnInfo.Player.ZoneForest) //if forest
                {
                    return spawnInfo.SpawnTileType == TileID.Grass || spawnInfo.SpawnTileType == TileID.Dirt ? .3f : 0f;
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
				new FlavorTextBestiaryInfoElement("A waddle dee that likes to take it's umbrella out for walks, even when it isn't raining.")
			}); 
        }

        public override void AI() //constantly cycles each time
        {
			NPC.spriteDirection = NPC.direction;

            //reduce gravity by one fourth 
            NPC.GravityMultiplier /= 4; 
			NPC.MaxFallSpeedMultiplier /= 4;

            //reroll direction
            ++NPC.ai[0];

            if (NPC.ai[0] >= 300)
            {
                if (NPC.ai[0] >= 300)
                {
                    int ranan = Main.rand.Next(0, 10);

                    if (ranan > 5)
                    {
                        NPC.direction = 1;
                    }
                    else
                    {
                        NPC.direction = -1;
                    }
                    NPC.netUpdate = true;

                    NPC.ai[0] = 0f;
                }

                NPC.ai[0] = 0f;
            }

            //turn around if touching wall for a while
            if (NPC.collideX && NPC.velocity.Y == 0)
            {
                NPC.ai[1]++;

                if (NPC.ai[1] >= 180) //turn around
                {
                    NPC.direction *= -1;
                    NPC.ai[1] = 0f;
                    NPC.ai[0] = 1f;
                }
            }
            else //reset
            {
                NPC.ai[1] = 0;
            }

            //movement
            float speed = 0.7f;
            if (Main.expertMode)
                speed = 1;
            if(NPC.confused)
            {
                speed *= -1;
            }
			float inertia = 20f;
            Confusion.InvertDirection(NPC);
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

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Items.Weapons.Parosol>(), 40, 20)); // 1 in 40 (2.5%) chance in Normal. 1 in 20 (5%) chance in Expert
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Starbit>(), 1, 1, 2));
        }
    }
}
