using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
	public class Cappy : ModNPC
	{
		private int animation = 0;

		private int ranan = Main.rand.Next(0, 10);

		private int Hitamount = 0; //times hit
		private int jump = 0; //if above 0 cappy will go up
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Cappy");
			Main.npcFrameCount[NPC.type] = 4;
		}

		public override void SetDefaults() {
			NPC.width = 20;
			NPC.height = 20;
			NPC.defense = 0; 
			NPC.lifeMax = 20;
			NPC.damage = 5;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = 0f; // money it drops
			NPC.knockBackResist = 1f; //how much knockback applies
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.CappyBanner>();
			NPC.aiStyle = -1;
			NPC.noGravity = false;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) 
		{
            //if player is within surface height, daytime, not raining, no invasions, and in forest/purity
            if (spawnInfo.Player.ZoneOverworldHeight && Main.dayTime && !Main.raining && spawnInfo.Player.ZoneForest && !spawnInfo.Invasion)
            {
                return spawnInfo.SpawnTileType == TileID.Grass || spawnInfo.SpawnTileType == TileID.Dirt ? .5f : 0f; //functions like a mini if else statement
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
				new FlavorTextBestiaryInfoElement("This shy creature keeps itself concealed with a mushroom like hat. Uncovering it reveals a face that doesn't look as cheery as they move.")
            });
        }
		//todo: test confusion functionality
        public override void AI() //constantly cycles each time
        {
			if(NPC.confused)
			{
				NPC.direction = MathF.Sign(NPC.Center.X - Main.player[NPC.target].Center.X);

			}

			NPC.spriteDirection = NPC.direction;
			jump--;

			//movement
			if (!NPC.confused)
			{
				if (ranan > 5)
				{
					NPC.direction = 1;
				}
				else
				{
					NPC.direction = -1;
				}
			}
			//switch directions
			++NPC.ai[0];
			if (NPC.ai[0] >= 300)
			{
				ranan = Main.rand.Next(0, 10);
				NPC.ai[0] = 0f;
			}

			//movement
			
			float speed = 0.7f;
			if (Main.expertMode) 
			{
				speed = 1.4f;
			}

			float inertia = 20f;

			Vector2 moveTo = NPC.Center + new Vector2(NPC.direction * 200, 0);
			Vector2 direction = moveTo - NPC.Center; //start - end
			direction.Normalize();
			direction *= speed;

			//Don't negate X movement in the air else Cappy flies large distances
            NPC.velocity.X = (NPC.velocity.X * (inertia - 1) + direction.X) / inertia; //use .X so it only effects horizontal movement

            //Jump When land

            if (NPC.velocity.Y == 0) 
            {
				jump = 5;
				if (animation == 0)
                {
					animation = 1;

				}
				else
                {
			        animation = 0;
                }
			}
			if (jump > 0)
            {
				NPC.velocity.Y = -2f;
			}

			//for stepping up tiles
			Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
		}

        public override void FindFrame(int frameHeight) // animation
        {
			if (animation == 0)
			{
				if (NPC.life > NPC.lifeMax * 0.95)
				{
					NPC.frame.Y = 0; //cap 1
				}
				else
				{
					NPC.frame.Y = frameHeight * 2; // nocap 1
				}
			}
			if (animation == 1)
			{
				if (NPC.life > NPC.lifeMax * 0.95)
				{
					NPC.frame.Y = frameHeight; //cap 2
				}
				else
				{
					NPC.frame.Y = frameHeight * 3; // nocap 2
				}
			}
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life <= NPC.lifeMax * 0.95 & Hitamount == 0) //checks if lost some health
            {
                Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity * -1, ModContent.GoreType<Gores.CappyCap>(), 1f);
				Hitamount += 1;
			}
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
