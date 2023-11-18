using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
	public class UFO : ModNPC
	{
        public ref float movement => ref NPC.ai[2];

        private bool seen = false; //determines if the ufo was in range of the player's sight

		private int ranan = Main.rand.Next(0, 4);
		private int subranan = Main.rand.Next(0, 3);
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("UFO");
			Main.npcFrameCount[NPC.type] = 4;
		}

		public override void SetDefaults() {
			NPC.width = 46;
			NPC.height = 44;
			//drawOffsetY = -18; //make sprite line up with hitbox
			NPC.damage = 30;
			NPC.lifeMax = 400;
			NPC.defense = 30;
			NPC.HitSound = SoundID.NPCHit4; //metal
			NPC.DeathSound = SoundID.NPCDeath14; //mech explode
			NPC.value = Item.buyPrice( 0, 50, 0, 0); // money it drops
			NPC.rarity = 4; //1 is dungeon slime, 4 is mimic
			NPC.knockBackResist = 1f; //How much of the knockback it receives will actually apply
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.UFOBanner>();
			NPC.noGravity = true;
			NPC.noTileCollide = true;
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Rarely some may see mysterious constructs floating around the atmosphere. They tend to open fire on suspicious life forms that get too close.")
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) 
		{
			if (spawnInfo.Player.ZoneSkyHeight && Main.hardMode) //if player is within space height and world is in hardmode
			{	
				return 0.025f; //return spawn rate
			}
			else
			{
				return 0f; //no spawn rate
			}
		}

		public override void AI() //constantly cycles each time
        {
			NPC.spriteDirection = NPC.direction;
			NPC.TargetClosest();
			//switch movements(never twice in a row)
			if (NPC.ai[0] == 0)
			{
                int ranan = Main.rand.Next(1, 4);

                if (ranan == 3) //right
                {
                    if (movement == 4) //if already 4
                    {
                        int subranan = Main.rand.Next(1, 3);
                        if (subranan == 3)
                        {
                            movement = 3;
                        }
                        else if (subranan == 2)
                        {
                            movement = 2;
                        }
                        else
                        {
                            movement = 1;
                        }
                    }
                    else
                    {
                        movement = 4;
                    }
                }
                else if (ranan == 2) // left
                {
                    if (movement == 3) //if already 3
                    {
                        int subranan = Main.rand.Next(1, 3);
                        if (subranan == 3)
                        {
                            movement = 4;
                        }
                        else if (subranan == 2)
                        {
                            movement = 2;
                        }
                        else
                        {
                            movement = 1;
                        }
                    }
                    else
                    {
                        movement = 3;
                    }
                }
                else if (ranan == 1) //up
                {
                    if (movement == 2) //if already 2
                    {
                        int subranan = Main.rand.Next(1, 3);
                        if (subranan == 3)
                        {
                            movement = 4;
                        }
                        else if (subranan == 2)
                        {
                            movement = 3;
                        }
                        else
                        {
                            movement = 1;
                        }
                    }
                    else
                    {
                        movement = 2;
                    }
                }
                else //down
                {
                    if (movement == 1) //if already 1
                    {
                        int subranan = Main.rand.Next(1, 3);
                        if (subranan == 3)
                        {
                            movement = 4;
                        }
                        else if (subranan == 2)
                        {
                            movement = 3;
                        }
                        else
                        {
                            movement = 2;
                        }
                    }
                    else
                    {
                        movement = 1;
                    }
                }

                NPC.netUpdate = true;
            }

            //attack
            Player player = Main.player[NPC.target];
            Vector2 distance = player.Center - NPC.Center;

            //this if else statement is here so players don't get shot from where they can't see

            //within dimensions, not in unaccessible area and player not dead
            if (Math.Abs(distance.Y) < 400 && Math.Abs(distance.X) < 800 && !player.dead) 
			{
				seen = true; //prepare attack
			}

			if (seen == true) //prepare attack
			{
                NPC.ai[1]++; //prepare attack
            }
            else
            {
                NPC.ai[1] = 0; //float around 
            }

            if (NPC.ai[1] >= 60) //warning
			{
                //Makes dust slightly above UFO
                Dust d = Dust.NewDustPerfect(NPC.Center + new Vector2(0, -10), ModContent.DustType<Dusts.CyborgArcherLaser>(), Vector2.Zero, Scale: 1f); 
                d.noGravity = true;
            }

			if (NPC.ai[1] >= 120) //shoot
            {
				Vector2 projshoot = player.Center - NPC.Center;
				projshoot.Normalize(); //make into 1
				projshoot *= 40;

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projshoot, ModContent.ProjectileType<Projectiles.UFOLaser>(), 80 / 2, 12, Main.myPlayer);
				}
				SoundEngine.PlaySound(SoundID.Item33, NPC.Center);

				//reset
                NPC.ai[1] = 0;
            }

			//switch directions
			++NPC.ai[0];

			if (NPC.ai[0] >= 120)
			{
				NPC.ai[0] = 0f;
			}

			//movement
			if (movement == 1) //down
			{
				NPC.velocity.X = 0;
				NPC.velocity.Y = 2;
			}
			else if (movement == 2) //up
			{
				NPC.velocity.X = 0;
				NPC.velocity.Y = -2;
			}
			else if (movement == 3) //left
			{
				NPC.velocity.X = -2;
				NPC.velocity.Y = 0;
			}
			else //right
			{
				NPC.velocity.X = 2;
				NPC.velocity.Y = 0;
			}
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
            else
            {
                NPC.frameCounter = 0.0;
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

		/*public override void NPCLoot()
		{
			Item.NewItem(npc.getRect(), ModContent.ItemType<Items.DreamEssence>(), Main.rand.Next(2, 4));

			if (Main.expertMode)
			{
				if (Main.rand.NextBool(2))
				{
					Item.NewItem(npc.getRect(), ItemID.SoulofFlight, 1);
				}
			}
			else
			{
				if (Main.rand.NextBool(4))
				{
					Item.NewItem(npc.getRect(), ItemID.SoulofFlight, 1);
				}
			}
		}*/
	}
}
