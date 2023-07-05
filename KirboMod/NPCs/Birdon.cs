using KirboMod.Items;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace KirboMod.NPCs
{
	public class Birdon : ModNPC
	{
		private int frame = 0;
		private double counting;
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Birdon");
			Main.npcFrameCount[NPC.type] = 3;
		}

		public override void SetDefaults() {
			NPC.width = 36;
			NPC.height = 36;
			//drawOffsetY = -18; //make sprite line up with hitbox
			NPC.damage = 40;
			NPC.lifeMax = 220;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(0, 0, 2, 50); // money it drops
			NPC.knockBackResist = 0.5f; //How much of the knockback it receives will actually apply
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.BirdonBanner>();
			NPC.aiStyle = 14; //flying ai
			NPC.noGravity = true;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) 
		{
			if (spawnInfo.Player.ZoneSkyHeight & Main.hardMode) //if player is within space height and world is in hardmode
			{	
				return 0.5f; //return spawn rate
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
				new FlavorTextBestiaryInfoElement("This high-flying bird just loves to see how far can they fly! Got along very well with the harpies, and now they protect their blue feathery friends!")
            });
        }
        public override void AI() //constantly cycles each time
        {
			NPC.spriteDirection = NPC.direction;
			Player player = Main.player[NPC.target];
			Vector2 distance = player.Center - NPC.Center;
            NPC.TargetClosest(true); //face target

            NPC.rotation = NPC.velocity.X * 0.1f;

            NPC.ai[0]++;
			if (NPC.ai[0] % 120 == 0)
            {
				distance.Normalize();
				distance *= 5;

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, distance, Mod.Find<ModProjectile>("BirdonFeatherBad").Type, 60 / 2, 1, Main.myPlayer, 0, 0);
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, distance.RotatedBy(MathHelper.ToRadians(20)), Mod.Find<ModProjectile>("BirdonFeatherBad").Type, 60 / 2, 1, Main.myPlayer, 0, 0);
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, distance.RotatedBy(MathHelper.ToRadians(-20)), Mod.Find<ModProjectile>("BirdonFeatherBad").Type, 60 / 2, 1, Main.myPlayer, 0, 0);
				}
			}
		}

        public override void FindFrame(int frameHeight) // animation
        {
			if (frame == 0)
			{
				counting += 1.0;
				if (counting < 10.0)
				{
					NPC.frame.Y = 0;
				}
				else if (counting < 20.0)
				{
					NPC.frame.Y = frameHeight;
				}
                else if (counting < 30.0)
                {
                    NPC.frame.Y = frameHeight * 2;
                }
                else if (counting < 40.0)
                {
                    NPC.frame.Y = frameHeight;
                }
                else
				{
					counting = 0.0;
				}
			}
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

		/*public override void OnKill()
		{
			Item.NewItem(NPC.getRect(), ModContent.ItemType<Items.DreamEssence>(), Main.rand.Next(2, 4));

			if (Main.expertMode)
			{
				if (Main.rand.NextBool(2))
				{
					Item.NewItem(NPC.getRect(), ItemID.SoulofFlight, 1);
				}

				if (Main.rand.NextBool(50)) //drop sky blanket because kracko jr no longer spawns naturally in hardmode
				{
					Item.NewItem(NPC.getRect(), ModContent.ItemType<Items.SkyBlanket>(), 1);
				}
			}
			else
			{
				if (Main.rand.NextBool(4))
				{
					Item.NewItem(NPC.getRect(), ItemID.SoulofFlight, 1);
				}

				if (Main.rand.NextBool(100)) //drop sky blanket because kracko jr no longer spawns naturally in hardmode
				{
					Item.NewItem(NPC.getRect(), ModContent.ItemType<Items.SkyBlanket>(), 1);
				}
			}
		}*/

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.SoulofFlight, 10, 5)); // 1 in 10 (10%) chance in Normal. 1 in 5 (20%) chance in Expert

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DreamEssence>(), 1, 2, 4));

			//more common in normal mode
			new DropBasedOnExpertMode(ItemDropRule.ByCondition(new Conditions.IsCrimsonAndNotExpert(), ModContent.ItemType<SkyBlanket>(), 100, 1, 1), 
				ItemDropRule.ByCondition(new Conditions.IsCrimsonAndNotExpert(), ModContent.ItemType<SkyBlanket>(), 50, 1, 1));
        }
    }
}
