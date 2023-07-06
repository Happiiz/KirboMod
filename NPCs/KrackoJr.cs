using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{ 
	public class KrackoJr : ModNPC
	{
		private float cloudsrotation = 0;

        public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Kracko Jr.");
			Main.npcFrameCount[NPC.type] = 1;

            // Add this in for bosses(in this case minibosses) that have a summon item, requires corresponding code in the item
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
			{
				CustomTexturePath = "KirboMod/NPCs/BestiaryTextures/KrackoJrPortrait",
				PortraitScale = 0.75f, // Portrait refers to the full picture when clicking on the icon in the bestiary
				PortraitPositionYOverride = 0f,
				Rotation = MathHelper.ToRadians(45),
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

		public override void SetDefaults() {
			NPC.width = 58;
			NPC.height = 58;
			NPC.damage = 15;
			NPC.defense = 3;
			NPC.noTileCollide = true;
			NPC.lifeMax = 300;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
			NPC.aiStyle = -1;
			NPC.noGravity = true;
			NPC.lavaImmune = true;
			NPC.friendly = false;
			NPC.rarity = 2; //groom/pinky rarity
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneSkyHeight && NPC.downedBoss2 && !Main.hardMode) //if player is within space height, not in hardmode and defeated evil boss
			{
				return !NPC.AnyNPCs(Mod.Find<ModNPC>("Kracko").Type) ? 0.15f : 0; //return spawn rate if kracko isn't here
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
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("A Kracko that shrunk itself in size to be more mobile, but will grow and become stronger if threatened!")
            });
        }

        public override void AI() //constantly cycles each time
		{
			Player player = Main.player[NPC.target];
			NPC.TargetClosest();

			//rotato
			// First, calculate a Vector pointing towards what you want to look at
			Vector2 distance = player.Center - NPC.Center;
			// Second, use the ToRotation method to turn that Vector2 into a float representing a rotation in radians.
			float desiredRotation = distance.ToRotation();

			NPC.rotation = desiredRotation;

			//DESPAWNING
			if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
			{
				

				NPC.velocity.Y = NPC.velocity.Y - 0.2f;
				NPC.ai[0] = 0;

				if (NPC.timeLeft > 60)
				{
					NPC.timeLeft = 60;
					return;
				}
			}
			else if (NPC.life <= 0) //Transformation
			{
				NPC.ai[1]++;
			}
			else //regular attack
			{
				AttackPattern();
			}
		}

		private void AttackPattern()
		{
			Player player = Main.player[NPC.target];
			Vector2 distance = player.Center - NPC.Center; //start - end
            Vector2 randomDistanceFromPlayer = player.Center + new Vector2(Main.rand.Next(-200, 200), Main.rand.Next(-200, 200)) - NPC.Center;

            NPC.ai[0]++;

			if (NPC.ai[0] >= 120 & NPC.ai[0] <= 179)
            {
				if (NPC.ai[0] == 120)
				{
					NPC.velocity = randomDistanceFromPlayer / 10;
					NPC.netUpdate = true; //sync cause random
				}
				else
				{
					NPC.velocity *= 0.9f;

					if (NPC.ai[0] == 179)
                    {
						NPC.velocity *= 0;
                    }
				}

				NPC.damage = 0;
            }

			if (NPC.ai[0] == 180)
            {
				NPC.damage = 25;

				distance.Normalize();
				distance *= 10;
				NPC.velocity = distance;
			}

			if (NPC.ai[0] >= 180 & NPC.ai[0] % 5 == 0)
            {
				Dust d = Dust.NewDustPerfect(NPC.Center, DustID.Cloud, NPC.velocity * 0, Scale: 1.5f);
			}

			if (NPC.ai[0] >= 240)
            {
				NPC.velocity *= 0;
				NPC.ai[0] = 0;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
				for (int i = 0; i < 50; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
					Dust d = Dust.NewDustPerfect(NPC.Center, DustID.Cloud, speed * 20, Scale: 3f); //Makes dust in a circle
					d.noGravity = true;
				}

				//summon kracko
				int boss = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 3, ModContent.NPCType<NPCs.Kracko>(), 0, 0, 0, 0, 0, NPC.target);

				if (Main.netMode == NetmodeID.SinglePlayer)
				{
					Main.NewText(Language.GetTextValue("Announcement.HasAwoken", Main.npc[boss].TypeName), 175, 75);
				}
				else if (Main.netMode == NetmodeID.Server)
				{
					ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", Main.npc[boss].GetTypeNetName()), new Color(175, 75, 255));
				}
			}	
			else
            {
				for (int i = 0; i < 10; i++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(3f, 3f); //circle
					Dust d = Dust.NewDustPerfect(NPC.Center, DustID.Cloud, speed * 2, Scale: 1.5f); //Makes dust in a messy circle
					d.noGravity = false;
				}
			}
        }

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; //make it unaffected by light
		}

        // This npc uses an additional texture for drawing
        public static Asset<Texture2D> Clouds;

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Clouds = ModContent.Request<Texture2D>("KirboMod/NPCs/KrackoJrClouds");

			cloudsrotation += 0.1f; //go up

            //Draw clouds
            Texture2D clouds = Clouds.Value;
            spriteBatch.Draw(clouds, NPC.Center - Main.screenPosition, null, new Color(255, 255, 255), cloudsrotation, new Vector2(55, 55), 1f, SpriteEffects.None, 0f);
        }
    }
}
