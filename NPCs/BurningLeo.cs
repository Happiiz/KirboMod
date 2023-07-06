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
	public class BurningLeo : ModNPC
	{
		private double counting;

		private int attacktype = 0;
		private int attack = -60; //0 is attack point
		private bool attacking = false;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Burning Leo");
			Main.npcFrameCount[NPC.type] = 8;
		}

		public override void SetDefaults()
		{
			NPC.width = 50;
			NPC.height = 46;
			NPC.damage = 15;
			NPC.defense = 0;
			NPC.lifeMax = 70;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(0, 0, 0, 10);
			NPC.knockBackResist = 1f;
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.BurningLeoBanner>();
			NPC.aiStyle = -1;
			NPC.friendly = false;
			NPC.noGravity = false;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			//if player is in jungle biome and daytime or underground and not in water
			if (spawnInfo.Player.ZoneTowerVortex)
			{
				return 0f;
			}
			else if (spawnInfo.Player.ZoneTowerSolar)
			{
				return 0f;
			}
			else if (spawnInfo.Player.ZoneTowerNebula)
			{
				return 0f;
			}
			else if (spawnInfo.Player.ZoneTowerStardust)
			{
				return 0f;
			}
			else if (spawnInfo.Player.ZoneJungle && (Main.dayTime || spawnInfo.Player.ZoneRockLayerHeight) && !spawnInfo.Water && !spawnInfo.Sky
				&& !Main.eclipse) 
			{
				return spawnInfo.SpawnTileType == TileID.JungleGrass || spawnInfo.SpawnTileType == TileID.Mud ? .4f : 0f; //functions like a mini if else statement
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
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Burning Leos found comfort in the warm jungle, and now they scorch anyone who touches their land!")
            });
        }

        public override void AI() //constantly cycles each time
		{
			NPC.spriteDirection = NPC.direction;
			Player player = Main.player[NPC.target];
			Vector2 distance = player.Center - NPC.Center;

			bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);

            bool inPlayerRangeX = distance.X <= 240f && distance.X >= 0f; //in range of the right

            if (NPC.direction == -1) //facing left
            {
                inPlayerRangeX = distance.X >= -240f && distance.X <= 0f; //in range of the left
            }

            if (inPlayerRangeX && distance.Y > -240 && distance.Y < 50 && lineOfSight && !player.dead) //checks if the leo is in range
			{
				attacking = true; //now attack
			}
			if (attacking == false) //if not attack 
            {
				attacktype = 0; //walk
			}
			else
            {
				attacktype = 1; //attack
            }
			//declaring attacktype values
			if (attacktype == 0)
			{
				Walk();
			}
			if (attacktype == 1)
			{
				Burn();
			}

			//for stepping up tiles
			Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
		}

		public override void FindFrame(int frameHeight) // animation
        {
            if (attacktype == 0) //walking
            {
                NPC.frameCounter++;
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
            if (attacktype == 1) //burning
            {
                NPC.frameCounter++;

                if (attack <= 0) //getting ready
                {
                    if (NPC.frameCounter < 2.0)
                    {
                        NPC.frame.Y = frameHeight * 4;
                    }
                    else if (NPC.frameCounter < 4.0)
                    {
                        NPC.frame.Y = frameHeight * 5;
                    }
                    else
                    {
                        NPC.frameCounter = 0.0;
                    }
                }
                else //BUUURRRRN
                {
                    if (NPC.frameCounter < 3.0)
                    {
                        NPC.frame.Y = frameHeight * 6;
                    }
                    else if (NPC.frameCounter < 6.0)
                    {
                        NPC.frame.Y = frameHeight * 7;
                    }
                    else
                    {
                        NPC.frameCounter = 0.0;
                    }
                }
            }
        }

		private void Walk() //walk towards player
		{
			Player player = Main.player[NPC.target];
			NPC.TargetClosest(true);

			float speed = 1f; //top speed
			float inertia = 10f; //acceleration and decceleration speed

			Vector2 direction = NPC.Center + new Vector2(NPC.direction * 50, 0) - NPC.Center; //start - end 
			//we put this instead of player.Center so it will always be moving top speed instead of slowing down when player is near

			direction.Normalize();
			direction *= speed;
			if (NPC.velocity.Y == 0) //on ground (so it doesn't interfere with knockback)
			{
				NPC.velocity.X = (NPC.velocity.X * (inertia - 1) + direction.X) / inertia; //use .X so it only effects horizontal movement
			}
		}
		private void Burn()
        {
			Player player = Main.player[NPC.target];
			Vector2 projshoot = NPC.Center + new Vector2(NPC.direction * 10, 0) - NPC.Center; //straight ahead
			projshoot.Normalize();
			projshoot *= 10f;

			NPC.velocity.X *= 0.9f; //slow

			if (attack == -60) //point of where attack begins (for some reason)
			{
				NPC.TargetClosest(true); //face target
			}

            attack++; //goes up by 1 each tick (1/60 of a second)

			if (attack >= 0 & attack <= 60) //attack for 60 ticks
			{
				if (attack % 5 == 0) //every 5th tick
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projshoot.RotatedByRandom(MathHelper.ToRadians(50)),
						Mod.Find<ModProjectile>("BadFire").Type, 30 / 2, 1, Main.myPlayer, 0, 0);
					}
				}

				if (attack % 30 == 0) //every 30th tick
				{
                    SoundEngine.PlaySound(SoundID.Item34, NPC.Center); //flamethrower
                }
			}
			if (attack >= 120) //pause for 60 ticks
			{
				attack = -60; //cooldown for 30 ticks
				attacking = false; //can walk if out of range
			}
        }

		/*public override void OnKill()
		{
			if (Main.expertMode)
			{
				weaponchance = Main.rand.Next(1, 20);
			}
			else
			{
				weaponchance = Main.rand.Next(1, 40);
			}
			if (weaponchance == 1)
			{
				Item.NewItem(NPC.getRect(), ModContent.ItemType<Items.Weapons.Fire>());
			}
			Item.NewItem(NPC.getRect(), ModContent.ItemType<Items.Starbit>(), Main.rand.Next(4, 6));
		}*/
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Items.Weapons.Fire>(), 20, 10)); // 1 in 20 (5%) chance in Normal. 1 in 10 (10%) chance in Expert

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
