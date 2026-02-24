using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace KirboMod.NPCs
{
    public class BrontoBurt : ModNPC
    {
        private int frame = 0;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bronto Burt");
            Main.npcFrameCount[NPC.type] = 4;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
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
            NPC.defense = 3;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 40; // money it drops
            NPC.knockBackResist = 0.6f; //how much knockback applies
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<Items.Banners.BrontoBurtBanner>();
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = false;

            //bad idea, will desync in multiplayer. -Photonic0
            //NPC.direction = Main.rand.Next(0, 1 + 1) == 1 ? 1: -1; //determines whether to go left or right initally
            //will just make it initially have the direction towards the player target.

        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            //if player is within surface height, daytime, not raining, no invasions, and in forest/purity
            if (spawnInfo.Player.ZoneOverworldHeight && Main.dayTime && !Main.raining && spawnInfo.Player.ZoneForest && !spawnInfo.Invasion && !Main.eclipse)
            {
                return spawnInfo.SpawnTileType == TileID.Grass || spawnInfo.SpawnTileType == TileID.Dirt ? (SpawnCondition.OverworldDay.Chance * .15f) : 0f;
            }
            else
            {
                return 0f; //no spawn rate
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            //uses AddRange to add multiple things instead of Add for simplicity
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				//set spawning conditions of NPC in bestiary
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				//bestiary description
				new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.KirboMod.NPCs.Bestiary.BrontoBurt"))
            });
        }

        public override void AI() //constantly cycles each time
        {
            NPC.TargetClosest(false);
            if (NPC.localAI[0] == 0)
            {
                if (NPC.HasValidTarget)
                {
                    NPC.direction = MathF.Sign(Main.player[NPC.target].Center.X - NPC.Center.X);
                }
                else
                {
                    NPC.direction = Main.rand.NextBool() ? 1 : -1;
                    NPC.netUpdate = true;
                }
                NPC.localAI[0] = 1;
            }
            NPC.spriteDirection = NPC.direction;
            CheckPlatform();

            NPC.ai[0]++;

            //float
            if (NPC.ai[0] < 60)
            {
                NPC.velocity.Y = -1.2f; //rise up initally
            }
            else
            {
                NPC.velocity.Y = NPC.Center.Y + MathF.Sin(MathF.Tau / 60 * (NPC.ai[0] - 60)) * 2 - NPC.Center.Y;

                //switching directions
                if (NPC.collideX)
                {
                    NPC.ai[1]++;

                    if (NPC.ai[1] >= 60)
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

            //movement
            float speed = 1f;
            float inertia = 20f;

            Helper.BasicEnemyWalk(ref NPC.velocity.X, speed, inertia, NPC.direction);
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
