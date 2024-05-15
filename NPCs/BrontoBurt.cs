using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
	public class BrontoBurt : ModNPC
	{
		private int frame = 0;

		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Bronto Burt");
			Main.npcFrameCount[NPC.type] = 4;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Direction = -1,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }

		public override void SetDefaults() 
		{
			NPC.width = 32;
			NPC.height = 32;
			NPC.lifeMax = 20;
			NPC.damage = 10;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = 0f; // money it drops
			NPC.knockBackResist = 0.6f; //how much knockback applies
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.BrontoBurtBanner>();
			NPC.aiStyle = -1;
			NPC.noGravity = true;
			NPC.noTileCollide = false;
			NPC.direction = Main.rand.Next(0, 1 + 1) == 1 ? 1: -1; //determines whether to go left or right initally
        }

		public override float SpawnChance(NPCSpawnInfo spawnInfo) 
		{
            //if player is within surface height, daytime, not raining, no invasions, and in forest/purity
            if (spawnInfo.Player.ZoneOverworldHeight && Main.dayTime && !Main.raining && spawnInfo.Player.ZoneForest && !spawnInfo.Invasion)
            {
                return spawnInfo.SpawnTileType == TileID.Grass || spawnInfo.SpawnTileType == TileID.Dirt ? .5f : 0f;
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

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Flutters through the air in a wave pattern. Will push others out of the way to keep on it's path.")
            });
        }

        public override void AI() //constantly cycles each time
        {
			NPC.spriteDirection = NPC.direction;
			CheckPlatform();

			NPC.ai[0]++;

            if (NPC.velocity.Y == 0) //not flying
            {
				NPC.ai[0] = 0; //reset
            }

            //float
            if (NPC.ai[0] < 60)
            {
				NPC.velocity.Y = -1f; //rise up initally

				NPC.velocity.X *= 0.01f;
            }
            else
			{
                NPC.velocity.Y = (float)Math.Sin(NPC.position.X / 20) * 2;
				
                //movement
                float speed = 1f;
                float inertia = 20f;

                Vector2 moveTo = NPC.Center + new Vector2(NPC.direction * 200, 0);
                Vector2 direction = moveTo - NPC.Center; //start - end
                direction.Normalize();
                direction *= speed;
                NPC.velocity.X = (NPC.velocity.X * (inertia - 1) + direction.X) / inertia; //use .X so it only effects horizontal movement


                //switching directions
                Point tileNPCIsOn = NPC.Center.ToTileCoordinates();
                Tile frontOfNPC = Main.tile[tileNPCIsOn.X + NPC.direction, tileNPCIsOn.Y];

                //tile in front of npc
                if (WorldGen.SolidOrSlopedTile(frontOfNPC))
                {
                    NPC.ai[1]++;

                    if (NPC.ai[1] >= 120)
                    {
                        NPC.direction *= -1; //reverse direction
                        NPC.ai[1] = 0;
                    }
                }
                else
                {
                    NPC.ai[1] = 0;
                }
            }
		}

		private void CheckPlatform() //trust me this is totally unique and original code and definitely not stolen from Spirit Mod's public source code(thx so much btw you don't know the hell I went through with this)
		{
			bool onplatform = true;
			for (int i = (int)NPC.position.X; i < NPC.position.X + NPC.width; i += NPC.width / 4)
			{ //check tiles beneath the boss to see if they are all platforms
				Tile tile = Framing.GetTileSafely(new Point((int)NPC.position.X / 16, (int)(NPC.position.Y + NPC.height + 8) / 16));
				if (!TileID.Sets.Platforms[tile.TileType])
					onplatform = false;
			}
			if (onplatform) //if they are on platform
				NPC.noTileCollide = true;
			else
				NPC.noTileCollide = false;
		}

		public override void FindFrame(int frameHeight) // animation
        {
			if (frame == 0)
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 2.0)
				{
					NPC.frame.Y = 0;
				}
				else if (NPC.frameCounter < 4.0)
				{
					NPC.frame.Y = frameHeight;
				}
				else if (NPC.frameCounter < 6.0)
				{
					NPC.frame.Y = frameHeight * 2;
				}
				else
				{
                    NPC.frameCounter = 0;
				}
			}
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
