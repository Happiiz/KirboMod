using KirboMod.Items;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace KirboMod.NPCs
{
	public class Twister : ModNPC
	{
		private int frame = 0;
		private double counting;
		
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Twister");
			Main.npcFrameCount[NPC.type] = 4;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "KirboMod/NPCs/BestiaryTextures/TwisterPortrait",
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

		public override void SetDefaults()
		{
			NPC.width = 34;
			NPC.height = 34;
            NPC.damage = 30;
			NPC.defense = 10;
			NPC.lifeMax = 50;
			NPC.HitSound = SoundID.NPCHit4; //metal
			NPC.DeathSound = SoundID.NPCDeath14; //also metal
			NPC.value = Item.buyPrice(0, 0, 0, 10);
			NPC.knockBackResist = 0f;
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.TwisterBanner>();
			NPC.aiStyle = -1; 
			NPC.noGravity = false;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneDirtLayerHeight || spawnInfo.Player.ZoneRockLayerHeight) //if player is within cave height
			{
				return spawnInfo.SpawnTileType == TileID.Marble ? .4f : 0f; //functions like a mini if else statement
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
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Marble,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Look out! This little yellow top has the knack to spin violently towards intruders! One could wonder how it goes about doing so without eyes.")
            });
        }

        public override void AI() //constantly cycles each time
		{
			NPC.spriteDirection = 1; //only spin in 1 direction
			Player player = Main.player[NPC.target];
			Vector2 distance = player.Center - NPC.Center;

			if (player.dead == false)
			{
				NPC.TargetClosest(true);
			}

			float speed = 10f; //top speed
			float inertia = 80f; //acceleration and decceleration speed

			Vector2 direction = NPC.Center + new Vector2(NPC.direction * 50, 0) - NPC.Center; //start - end 
			//we put this instead of player.Center so it will always be moving top speed instead of slowing down when player is near

			direction.Normalize();
			direction *= speed;
			NPC.velocity.X = (NPC.velocity.X * (inertia - 1) + direction.X) / inertia; //use .X so it only effects horizontal movement

			//for stepping up tiles
			Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
		}

		public override void FindFrame(int frameHeight) 
		{
			counting += 1.0;
			if (counting < 5.0)
			{
				NPC.frame.Y = 0;
			}
			else if (counting < 10.0)
			{
				NPC.frame.Y = frameHeight;
			}
			else if (counting < 15.0)
			{
				NPC.frame.Y = frameHeight * 2;
			}
			else if (counting < 20.0)
			{
				NPC.frame.Y = frameHeight * 3;
			}
			else
			{
				counting = 0.0;
			}
		}

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Items.Weapons.Tornado>(), 20, 10)); // 1 in 20 (5%) chance in Normal. 1 in 10 (10%) chance in Expert
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

        public static Asset<Texture2D> Twisty;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Main.instance.LoadNPC(NPC.type);
            Twisty = ModContent.Request<Texture2D>(Texture);
            Texture2D texture = Twisty.Value;

            Vector2 drawOrigin = new Vector2(texture.Width / 2, 40);
            Vector2 drawPos = NPC.Center - Main.screenPosition + new Vector2(0f, 20 + NPC.gfxOffY);

            Main.EntitySpriteDraw(texture, drawPos, NPC.frame, drawColor, MathHelper.ToRadians(NPC.velocity.X * 2), drawOrigin, 1f, SpriteEffects.None);

            return false;
        }

    }
}
