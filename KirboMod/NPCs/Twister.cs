using KirboMod.Items;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

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
		}

		public override void SetDefaults()
		{
			NPC.width = 33;
			NPC.height = 33;
			DrawOffsetY = -2; //make sprite line up with hitbox
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
				new FlavorTextBestiaryInfoElement("Look out! This little yellow top has the knack to spin violently towards intruders! But how does it go about without eyes or a mouth?")
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

		public override void FindFrame(int frameHeight) // SPEEN
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
		/*public override void OnKill()
		{
			if (Main.expertMode)
			{
				weaponchance = Main.rand.Next(1, 10);
			}
			else
			{
				weaponchance = Main.rand.Next(1, 20);
			}
			if (weaponchance == 1)
			{
				Item.NewItem(NPC.getRect(), ModContent.ItemType<Items.Weapons.Tornado>());
			}
			Item.NewItem(NPC.getRect(), ModContent.ItemType<Items.Starbit>(), Main.rand.Next(4, 5));
		}*/

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
        }
	}
}
