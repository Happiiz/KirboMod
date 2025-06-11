using KirboMod.Items;
using KirboMod.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
    public class Birdon : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Birdon");
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.width = 36;
            NPC.height = 36;
            //drawOffsetY = -18; //make sprite line up with hitbox
            NPC.damage = 40;
            NPC.lifeMax = 360;
            NPC.defense = 18;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = Item.buyPrice(0, 0, 6, 0); // money it drops
            NPC.knockBackResist = 0.5f; //How much of the knockback it receives will actually apply
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<Items.Banners.BirdonBanner>();
            NPC.noGravity = true;
            NPC.aiStyle = 14;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneSkyHeight & Main.hardMode) //if player is within space height and world is in hardmode
            {
                return 0.4f; //return spawn rate
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
            NPC.TargetClosest(true); //face target

            NPC.rotation = NPC.velocity.X * 0.1f;

            NPC.ai[0]++;
            if (NPC.confused)
            {
                return; //don't shoot feathers if confused
            }
            int fireRate = Main.getGoodWorld ? 90 : Main.expertMode ? 120 : 180;
            if (NPC.ai[0] % fireRate == 0)
            {
                Vector2 shootVel = Vector2.Normalize(player.Center - NPC.Center);
                shootVel *= 7;
                if (Main.expertMode)
                {
                    shootVel *= 1.33f;
                    Utils.ChaseResults results = Utils.GetChaseResults(NPC.Center, shootVel.Length(), player.Center, player.velocity);
                    shootVel = results.InterceptionHappens ? results.ChaserVelocity : (Vector2.Normalize(player.velocity) * shootVel.Length());
                }
                int featherAmount = 3;
                if (Main.expertMode)
                {
                    featherAmount = 4;
                }
                if (Main.getGoodWorld)
                {
                    featherAmount = 8;
                }

                float spread = .6f;
                if (Main.expertMode)
                    spread = 1;
                if (Main.getGoodWorld)
                {
                    spread = 1.5f;
                }

                for (int i = -featherAmount / 2; i <= featherAmount / 2; i++)
                {
                    ShootFeather(shootVel.RotatedBy(Utils.Remap(i, -featherAmount / 2, featherAmount / 2, -spread / 2, spread / 2)));
                }
            }
        }
        void ShootFeather(Vector2 velocity)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, ModContent.ProjectileType<BirdonFeatherBad>(), 60 / 2, 1, Main.myPlayer, 0, 0);
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
                    NPC.frame.Y = frameHeight;
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

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.SoulofFlight, 10, 5)); // 1 in 10 (10%) chance in Normal. 1 in 5 (20%) chance in Expert

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DreamEssence>(), 1, 4, 8));

            //more common in normal mode
            new DropBasedOnExpertMode(ItemDropRule.ByCondition(new Conditions.IsCrimsonAndNotExpert(), ModContent.ItemType<SkyBlanket>(), 100, 1, 1),
                ItemDropRule.ByCondition(new Conditions.IsCrimsonAndNotExpert(), ModContent.ItemType<SkyBlanket>(), 50, 1, 1));
        }
    }
}
