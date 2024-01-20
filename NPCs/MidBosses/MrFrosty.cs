using KirboMod.Items;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using SoundEngine = Terraria.Audio.SoundEngine;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using KirboMod.Projectiles;

namespace KirboMod.NPCs.MidBosses
{
    [AutoloadBossHead]
    public class MrFrosty : ModNPC
	{
        private int attacktype = -1;
        private int lastattack = 2;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Mr. Frosty");
			Main.npcFrameCount[NPC.type] = 8;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                PortraitScale = 1f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 20f,
                PortraitPositionXOverride = 0f,
                Position = new Vector2(20, 80),
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

		public override void SetDefaults()
		{
			NPC.width = 100;
			NPC.height = 136;
			NPC.damage = Main.hardMode ? 80 : 40;
			NPC.defense = 20;
            NPC.lifeMax = Main.hardMode ? 2500 : 500;
            NPC.HitSound = SoundID.NPCHit14; //fishron squeal
			NPC.DeathSound = SoundID.NPCDeath8; //grunt
			NPC.value = Item.buyPrice(0, 0, 50, 0); // money it drops
			NPC.knockBackResist = 0f; //how much knockback applies
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<Items.Banners.MrFrostyBanner>();
            NPC.aiStyle = -1; 
			NPC.friendly = false;
			NPC.noGravity = false;
			NPC.rarity = 1; //1 is dungeon slime, 4 is mimic
            NPC.coldDamage = true;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
			if (Main.hardMode)
			{
				NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance);
				NPC.damage = (int)(NPC.damage * 0.75f);
			}
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
            return 0f; //no spawn rate
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundSnow,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("An icy beast that appeared in the tuntra from a strange star shaped rift. Does it's best to stamp out any threats with it's grace and style, or lack thereof.")
            });
        }
        public override void AI() //constantly cycles each time
		{
			NPC.spriteDirection = NPC.direction;
			Player player = Main.player[NPC.target];
			Vector2 distance = player.Center - NPC.Center;

            if (NPC.ai[0] == 0) //not attacking
            {
                attacktype = 0; //standing
                NPC.ai[3]++; //attack timer
            }
            else
            {
                NPC.ai[3] = 0; //restart attack timer
            }

            if (NPC.ai[3] >= (Main.expertMode ? 50 : 100)) //times up! (50 if in expertmode)
            {
                if (attacktype == 0)
                {
                    NPC.ai[0] += 1; //attack!
                    NPC.ai[3] = 0;
                }
            }

            if (NPC.ai[0] == 1) //stancing or 
            {
                if (lastattack == 2) //ice
                {
                    attacktype = 1; //dash
                    lastattack = 1; //also ddash
                }
                else
                {
                    attacktype = 2; //ice
                    lastattack = 2; //also ice
                }

            }

            if (player.dead) //player has died
            {
                attacktype = 1; //dive dash
            }

            //declaring attacktype values
            if (attacktype == 0)
            {
                Stance();
            }
            if (attacktype == 1)
            {
                DiveDash();
            }
            if (attacktype == 2)
            {
                IceToss();
            }

            //for stepping up tiles
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
        }

		public override void FindFrame(int frameHeight) // animation
		{
			if (attacktype == 0) //stand still
			{
				NPC.frame.Y = 0; //stance
            }
			if (attacktype == 1) //dive attack
			{
				if (NPC.ai[0] > 180) //the dive
				{
                    NPC.frame.Y = frameHeight * 3; //dive
                }
				else //dashing
				{
					NPC.frameCounter += 1;
					if (NPC.frameCounter < 10)
					{
						NPC.frame.Y = frameHeight; //walk 1
					}
					else if (NPC.frameCounter <= 20) 
					{
						NPC.frame.Y = frameHeight * 2; //walk 2
					}
					else
					{
                        NPC.frameCounter = 0;
					}
				}
			}
			if (attacktype == 2) //ice cube
			{
				if (NPC.ai[0] > 90) //attacking
				{
                    NPC.frame.Y = frameHeight * 7; //toss
                }
                else if (NPC.ai[0] > 60) //attacking
                {
                    NPC.frame.Y = frameHeight * 6; //prepare ice
                }
                else //shake that
				{
                    NPC.frameCounter += 1;
					if (NPC.frameCounter < 8)
					{
						NPC.frame.Y = frameHeight * 4; //aww
					}
					else if (NPC.frameCounter < 16)
					{
						NPC.frame.Y = frameHeight *  5; //yeah
					}
					else
					{
                        NPC.frameCounter = 0;
					}
				}
			}
		}

        private void Stance() //stand up straight
		{
			Player player = Main.player[NPC.target];
			NPC.TargetClosest(true); //face player

            NPC.velocity.X *= 0.5f; //slow

            CheckPlatform(player); //go down platforms when player is low
        }

		private void DiveDash() //dives towards ground
        {
            if (NPC.ai[0] == 1) //short hop to warn
            {
                NPC.velocity.Y = -4; //jump
            }

            NPC.ai[0]++;

            if (NPC.ai[0] > 30)
			{
				if (NPC.ai[0] < 180) //dash towards player
				{
					
					Player player = Main.player[NPC.target];

                    CheckPlatform(player); //go down platforms when player is low

                    float speed = Main.expertMode ? 5f : 3f; //top speed(5 in expertmode)
					float inertia = 10f; //acceleration and decceleration speed

					Vector2 direction = NPC.Center + new Vector2(NPC.direction * 50, 0) - NPC.Center; //start - end 
																									  //we put this instead of player.Center so it will always be moving top speed instead of slowing down when player is near

					direction.Normalize();
					direction *= speed;
					NPC.velocity.X = (NPC.velocity.X * (inertia - 1) + direction.X) / inertia; //use .X so it only effects horizontal movement

					Vector2 distance = player.Center - NPC.Center;

					bool pastPlayer = distance.X < 5; //checks if past the player because it doesn't turn

					if (NPC.direction == -1)
					{
                        pastPlayer = distance.X > -5; //check for if facing left
                    }

                    //tile in front of npc
                    if (NPC.collideX && NPC.velocity.Y == 0)
                    {
                        NPC.velocity.Y = -8; //jump over
                    }

                    if (pastPlayer && player.dead == false) //past the player and player not dead
                    {
						NPC.ai[0] = 180; //dive
                    }

                    if (player.dead) //player is no longer with us unfortunately
					{
						NPC.ai[0] = 1;
					}
				}
				else //dive
				{
					Player player = Main.player[NPC.target];
					NPC.ai[0]++;
					
					NPC.velocity.X *= 0.9f; //slow 
					
					if (NPC.ai[0] == 180) //dive bomb
					{
						SoundEngine.PlaySound(SoundID.Item1, NPC.Center); 
					}
					if (NPC.ai[0] >= 240) //restart after 1 second
					{
                        NPC.ai[0] = 0;
						NPC.ai[1] = 0;
                        attacktype = 0; //I only put this here so it would transition animations correctly
                    }
				}
			}
        }

		private void IceToss() //toss a cold block of ice
		{
			if (NPC.ai[0] > 0)
			{
				if (NPC.ai[0] < 60)
				{
					NPC.TargetClosest(true);
					NPC.velocity.X *= 0.7f; //slow
				}
				Player player = Main.player[NPC.target];


                float Xoffset = -26;

                if (NPC.direction == -1) //so it matches projectile starting position
                {
                   Xoffset = 26;
                }

                Vector2 projshoot = player.Center - (NPC.Center + new Vector2(Xoffset, -80)); 
				projshoot.X /= 70;
				//limit
				if (projshoot.X > 10f)
                {
					projshoot.X = 10f;
                }
				if (projshoot.X < -10f)
				{
					projshoot.X = -10f;
				}

                Vector2 offset = new Vector2(-26, -80);

                if (NPC.direction == -1) //when facing left
                {
                    offset = new Vector2(26, -80); 
                }

                NPC.ai[0]++;
				
				NPC.velocity.X *= 0.7f; //slow

				if (NPC.ai[0] == 61)
				{
                    NPC.velocity.Y = -12; //jump
                }

				if (NPC.ai[0] == 91) //climax
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + offset.X, NPC.Center.Y + offset.Y, projshoot.X, -5, ModContent.ProjectileType<BadIceChunk>(), NPC.damage / 4, 5f, Main.myPlayer, 0, 0); //actual damage has to be divided by 2
					}
					SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
				}
				if (NPC.ai[0] >= 150) //restart
				{
                    NPC.ai[1] = 0;
					NPC.ai[0] = 0;
					attacktype = 0; //I only put this here so it would transition animations correctly
                }
			}
		}

        private void CheckPlatform(Player player) //trust me this is totally unique and original code and definitely not stolen from Spirit Mod's public source code(thx so much btw you don't know the hell I went through with this)
        {
            bool onplatform = true;
            for (int i = (int)NPC.position.X; i < NPC.position.X + NPC.width; i += NPC.width / 4)
            { //check tiles beneath the boss to see if they are all platforms
                Tile tile = Framing.GetTileSafely(new Point((int)NPC.position.X / 16, (int)(NPC.position.Y + NPC.height + 8) / 16));
                if (!TileID.Sets.Platforms[tile.TileType])
                    onplatform = false;
            }
            if (onplatform && (NPC.Center.Y < player.position.Y - 75)) //if they are and the player is lower than the boss, temporarily let the boss ignore tiles to go through them
            {
                NPC.noTileCollide = true;
            }
            else
            {
                NPC.noTileCollide = false;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Items.Weapons.Ice>(), 1, 1)); // Guaranteed in all difficulties
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Starbit>(), 1, 24, 24));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<Items.RareStone>(), 1, 1, 1));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 8; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                {
                    // go around in a octogonal pattern
                    Vector2 speed = new Vector2((float)Math.Cos(MathHelper.ToRadians(i * 45)) * 20, (float)Math.Sin(MathHelper.ToRadians(i * 45)) * 20);

                    Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.BoldStar>(), speed, Scale: 1.5f); //Makes dust in a messy circle
                    d.noGravity = true;
                }
                for (int i = 0; i < 10; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                    Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, speed, Main.rand.Next(11, 13), Scale: 1.5f); //double jump smoke
                }
            }
        }

        // This npc uses additional textures for drawing
        public static Asset<Texture2D> Ice;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Ice = ModContent.Request<Texture2D>("KirboMod/Projectiles/BadIceChunk");

            if (attacktype == 2 && NPC.ai[0] < 90 && NPC.ai[0] > 60) //about to throw
            {
                Texture2D ice = Ice.Value;
                Vector2 origin = new Vector2(ice.Width / 2, ice.Height / 2); //center
                Vector2 offset = new Vector2(-26, -80); 

                if (NPC.direction == -1)
                {
                    offset = new Vector2(26, -80); //sprite isn't evenly balanced so we have to offset it differently
                }

                spriteBatch.Draw(ice, NPC.Center - Main.screenPosition + offset, null, drawColor, 0f, origin, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}
