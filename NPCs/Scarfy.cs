using KirboMod.Items;
using KirboMod.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
    public class Scarfy : ModNPC
    {
        public bool Angry { get => NPC.ai[2] == 1f; set => NPC.ai[2] = value ? 1f : 0f; } //initially set to false because it doesn't start at 1f

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Scarfy");
            Main.npcFrameCount[NPC.type] = 6;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Direction = -1,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true; //immune to not mess up movement
        }

        public override void SetDefaults()
        {
            NPC.width = 38;
            NPC.height = 38;
            NPC.lifeMax = 180;
            NPC.defense = 10;
            NPC.damage = 0;//damage will be from the explosion. Don't deal damage while passive
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = Item.buyPrice(0, 0, 0, 15);
            NPC.knockBackResist = 0f; //how much knockback applies
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<Items.Banners.ScarfyBanner>();
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.direction = Main.rand.Next(0, 1 + 1) == 1 ? 1 : -1; //determines whether to go left or right initally
            NPC.chaseable = false; //initally

            //Prevent becoming angry due to flying in lava, but also be able to easily travel through lava when angry
            NPC.lavaImmune = true;
            NPC.GravityIgnoresLiquid = true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            //if player is within underworld height and spawn is not outside of the world
            if (spawnInfo.Player.ZoneUnderworldHeight && spawnInfo.SpawnTileY < Main.maxTilesY && spawnInfo.SpawnTileX < Main.maxTilesX && spawnInfo.SpawnTileX > 0)
            {
                return .15f; //returns spawn rate
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
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("A cute little face that stands out amongst the demons and flames, but only if you keep it that way!")
            });
        }
        public override void AI() //constantly cycles each time
        {
            NPC.spriteDirection = NPC.direction;
            CheckPlatform();

            if (Angry == false) //if neutral
            {
                //float
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

                if (NPC.life < NPC.lifeMax) //not at max health
                {
                    Angry = true;
                    NPC.ai[0] = 0;
                }
            }
            else //angry
            {
                NPC.chaseable = true; //now homable
                NPC.ai[0]++; //go up by 1 each tick

                if (NPC.ai[0] <= 30)
                {
                    if (NPC.ai[0] == 1)
                    {
                        NPC.TargetClosest(true); //face perpetrator for one tick
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.velocity.X = NPC.direction * -2; //backup
                        NPC.velocity.Y = 0;
                        NPC.netUpdate = true;
                    }
                }
                else
                {
                    NPC.TargetClosest();
                    Player player = Main.player[NPC.target];
                    if (player.Hitbox.Intersects(NPC.Hitbox))
                    {
                        Boom();
                    }

                    float speed = Helper.RemapEased(NPC.ai[0], 30, 70, 0, 15, Easings.EaseInOutSine);
                    float inertia = 20f;

                    Vector2 moveTo = player.Center;
                    Vector2 direction = moveTo - NPC.Center; //start - end
                    direction.Normalize();
                    direction *= speed;

                    if (player.dead == false) //only go towards player if alive
                    {
                        NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia; //follow player
                    }
                    else
                    {
                        NPC.velocity = NPC.velocity; //keep going one direction
                    }
                }
            }
        }
        static float GetExplosionSizeMultiplier()
        {
            if (Main.getGoodWorld)
            {
                return 4;
            }
            if (Main.expertMode)
            {
                return 2;
            }
            return 1;
        }
        Vector2 RndCircleOffset { get => Main.rand.NextVector2Circular(NPC.width, NPC.height); }
        void Boom()
        {
            int max = (int)(3 * GetExplosionSizeMultiplier() * GetExplosionSizeMultiplier());
            for (int i = 0; i < max; i++)
            {
                Vector2 scale = new Vector2(1, 1).RotatedByRandom(.5f) * 1.2f;
                Vector2 offset = RndCircleOffset / 2f;
                Sparkle sparkle = new(NPC.Center + offset, Color.OrangeRed, Vector2.Zero, scale * 2, scale * 2, 3);
                sparkle.rotation = Main.rand.NextBool() ? 0 : MathHelper.PiOver4;
                sparkle.Confirm();
                Ring ring = Ring.EmitRing(NPC.Center + offset, Color.Lerp(Color.Orange * .7f, Color.Black, Main.rand.NextFloat()));
                ring.squish = scale;
                Gore gore = Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center + Main.rand.NextVector2Circular(4, 4) * GetExplosionSizeMultiplier(), Main.rand.NextVector2Circular(4, 4), Main.rand.NextFromList(GoreID.Smoke1, GoreID.Smoke2, GoreID.Smoke2), Main.rand.NextFloat() * .3f + .9f);
                gore.rotation = Main.rand.NextFloat() * MathF.Tau;
                Dust.NewDustPerfect(NPC.Center + RndCircleOffset / 3, DustID.Torch, -Vector2.UnitY.RotatedByRandom(1) * (Main.rand.NextFloat() * 4 + 2), 0, default, 3);
            }
            //die from explosion
            NPC.active = false;
            SoundEngine.PlaySound(SoundID.Item38 with { MaxInstances = 0 }, NPC.Center);
            NPC.Hitbox = Utils.CenteredRectangle(NPC.Center, NPC.Size * GetExplosionSizeMultiplier());
            int dmg = NPC.GetAttackDamage_ScaledByStrength(100);
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player plr = Main.player[i];
                if (plr.active && !plr.dead && plr.Hitbox.Intersects(NPC.Hitbox))
                    Main.player[NPC.target].Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), dmg, NPC.direction);
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
            if (Angry == false)
            {
                NPC.frameCounter += 1;
                if (NPC.frameCounter < 12)
                {
                    NPC.frame.Y = 0;
                }
                else if (NPC.frameCounter < 24)
                {
                    NPC.frame.Y = frameHeight;
                }
                else if (NPC.frameCounter < 36)
                {
                    NPC.frame.Y = frameHeight * 2;
                }
                else if (NPC.frameCounter < 48)
                {
                    NPC.frame.Y = frameHeight * 3;
                }
                else
                {
                    NPC.frameCounter = 0;
                }
            }
            else
            {
                NPC.frameCounter += 1;
                if (NPC.frameCounter < 12)
                {
                    NPC.frame.Y = frameHeight * 4;
                }
                else if (NPC.frameCounter < 24)
                {
                    NPC.frame.Y = frameHeight * 5;
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

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Starbit>(), 1, 4, 8));
        }
    }
}
