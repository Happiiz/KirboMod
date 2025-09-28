using KirboMod.ItemDropRules.DropConditions;
using KirboMod.Items;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.NPCs.MidBosses
{
    [AutoloadBossHead]

    public class Batafire : ModNPC
    {
        float attackTimer { get => NPC.localAI[0]; set => NPC.localAI[0] = value; }
        ref float attackType => ref NPC.localAI[1];

        int animation = 0;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 12;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true; //immune because of boss-like behavior
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 100;
            DrawOffsetY = 70;
            NPC.damage = Main.hardMode ? (NPC.downedGolemBoss ? 150 : 100) : 50; //all stats scale with progression
            NPC.defense = Main.hardMode ? 30 : 15;
            NPC.lifeMax = Main.hardMode ? (NPC.downedGolemBoss ? 32000 : 10000) : 1000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath4;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Main.hardMode ? (NPC.downedGolemBoss ? Item.buyPrice(gold: 20) : Item.buyPrice(gold: 5)) : Item.buyPrice(silver: 50);
            NPC.npcSlots = 5f;
            NPC.lavaImmune = true;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<Items.Banners.BatafireBanner>();
            NPC.rarity = 1; //1 is dungeon slime, 4 is mimic

            attackTimer = -30; //start with intro
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
            new FlavorTextBestiaryInfoElement("When this fiery fiend appeared from the star-shaped rift it immediately found solace where it made sense the most: the only other places with bats and heat!"),
            ]);
        }

        public override void AI()
        {
            NPC.TargetClosest(false);
            NPC.spriteDirection = NPC.direction;
            Player player = Main.player[NPC.target];

            Vector2 distanceFromPlayer = player.Center - NPC.Center;

            //constant dust
            int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, Scale: 2f);
            Main.dust[d].noGravity = true;

            if (player.active && !player.dead)
            {
                if (attackType > 1)
                {
                    attackType = 0;
                }

                if (attackTimer > 180)
                {
                    if (attackType == 0)
                    {
                        Dive(attackTimer - 180, player, distanceFromPlayer);
                    }

                    if (attackType == 1)
                    {
                        FireSpew(attackTimer - 180);
                    }
                }
                else if (attackTimer >= 0)
                {
                    animation = 0;
                    NPC.TargetClosest(true);

                    float speed = Main.hardMode ? (NPC.downedGolemBoss ? 20 : 12) : 8;
                    float inertia = Main.hardMode ? (NPC.downedGolemBoss ? 40 : 30) : 20;

                    distanceFromPlayer.Normalize();
                    distanceFromPlayer *= speed;

                    NPC.velocity = (NPC.velocity * (inertia - 1) + distanceFromPlayer) / inertia;
                }

                attackTimer++;
            }
            else
            {
                NPC.velocity.Y += 0.5f;
            }
        }

        void Dive(float timer, Player player, Vector2 distance)
        {
            float YSpeedMult = Main.hardMode ? (NPC.downedGolemBoss ? 2 : 1.5f) : 1;
            float diveSpeedMult = Main.hardMode ? (NPC.downedGolemBoss ? 1.6f : 1.3f) : 1;

            float dashThreshold = 300;

            if (timer < dashThreshold)  //go up until above player
            {
                NPC.TargetClosest(true);
                NPC.velocity.Y -= 0.2f * YSpeedMult;
                NPC.velocity.X = 0;

                if (NPC.Center.Y < player.Center.Y - 500)
                {
                    attackTimer = 179 + dashThreshold; //update attackTimer to properly update timer
                }
            }
            if (timer >= dashThreshold)
            {
                animation = 1;

                if (timer == dashThreshold)
                {
                    SoundEngine.PlaySound(SoundID.Item100, player.Center);

                    Vector2 vel = distance / 20 * diveSpeedMult;

                    if (NPC.downedGolemBoss)
                        vel = (distance + player.velocity * 6) / 20 * diveSpeedMult; //not predict perfectly, but become more accurate to build tension

                    NPC.velocity = vel;
                }

                NPC.velocity *= 0.965f; //slow
            }

            float endTime = Main.hardMode ? (NPC.downedGolemBoss ? dashThreshold + 60 : dashThreshold + 60) : dashThreshold + 120;

            if (timer >= endTime)
            {
                attackTimer = 0;
                attackType += 1;
                NPC.velocity *= 0.1f; //stunt velocity so it stops flying up
            }
        }

        void FireSpew(float timer)
        {
            NPC.velocity *= 0.9f;

            animation = 2;
            if (timer == 1)
            {
                NPC.velocity.X = 0;
                NPC.velocity.Y = -2;
            }

            float shootTime = Main.hardMode ? (NPC.downedGolemBoss ? 40 : 80) : 80;
            float shootStart = Main.hardMode ? (NPC.downedGolemBoss ? 30 : 30) : 60;
            float shootInterval = Main.hardMode ? (NPC.downedGolemBoss ? 5 : 10) : 10;

            if (timer >= shootStart && timer % shootInterval == 0 && timer <= shootStart + shootTime)
            {
                Vector2 position = NPC.Center;
                int damage = NPC.damage / 2;
                float knockback = 5f;
                int type = ModContent.ProjectileType<Projectiles.Flames.BatafireFire>();
                //rotate starting down diagonally and then turning up

                float trajectory = (MathF.PI / 4) - MathF.PI / 2 * ((timer - shootStart) / shootTime);

                if (NPC.downedGolemBoss)
                    trajectory = (MathF.PI / 2.5f) - (MathF.PI / 2.5f + MathF.PI / 2.5f) * ((timer - shootStart) / shootTime);

                float shootSpeed = Main.hardMode ? (NPC.downedGolemBoss ? 30 : 25) : 20;

                Vector2 projVel = new Vector2(-MathF.Cos(trajectory), MathF.Sin(trajectory)) * shootSpeed;
                int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projVel, type, damage / 2, knockback, Main.myPlayer);
                Main.projectile[p].tileCollide = false;
                Main.projectile[p].friendly = false;
                Main.projectile[p].hostile = true;

                //another projectile with reversed X

                projVel.X *= -1;
                int p2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), position, projVel, type, damage / 2, knockback, Main.myPlayer);
                Main.projectile[p2].tileCollide = false;
                Main.projectile[p2].friendly = false;
                Main.projectile[p].hostile = true;

                for (int i = 0; i < 20; i++) //spew dust every shot
                {
                    Dust.NewDustPerfect(NPC.Center, DustID.Torch, Main.rand.NextVector2Circular(10f, 10f), Scale: 2f);
                }

                SoundEngine.PlaySound(SoundID.Item20, NPC.Center); //fire cast
            }

            float endTime = Main.hardMode ? (NPC.downedGolemBoss ? shootTime + shootStart + 30 : shootTime + shootStart + 30) : shootTime + shootStart + 60;

            if (timer >= endTime)
            {
                attackTimer = 0;
                attackType += 1;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            int denom = 3;
            if (Main.expertMode)
                denom = 1;

            if (Main.rand.NextBool(denom))
            {
                target.AddBuff(BuffID.OnFire, 180);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //TODO: implement exclusive rare stone drop

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapons.CrownOfFire>())); // Guaranteed
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Starbit>(), 1, 24, 24));

            //1 for pre-Golem, 1 for post-Golem. Both in Hardmode

            PreGolemHardmodeCondition PreGolemCondition = new PreGolemHardmodeCondition();
            IItemDropRule HardmodePreGolem = new LeadingConditionRule(PreGolemCondition);

            PostGolemHardmodeCondition PostGolemCondition = new PostGolemHardmodeCondition();
            IItemDropRule HardmodePostGolem = new LeadingConditionRule(PostGolemCondition);

            //Drop two Rare Stones if post-Golem

            HardmodePreGolem.OnSuccess(ItemDropRule.Common(ModContent.ItemType<TreasureStone>()));

            HardmodePostGolem.OnSuccess(ItemDropRule.Common(ModContent.ItemType<TreasureStone>(), 1, 2, 2));

            npcLoot.Add(HardmodePreGolem);
            npcLoot.Add(HardmodePostGolem);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            int startFrame = 0;
            int endFrame = 5;
            if (animation == 0)
            {
                startFrame = 0;
                endFrame = 5;
            }
            else if (animation == 1)
            {
                startFrame = 6;
                endFrame = 8;
            }
            else if (animation == 2)
            {
                startFrame = 9;
                endFrame = 11;
            }
            else //invalid animation so set to 0
            {
                animation = 0;
            }

            //code below referenced from example mod because it's conveinient for Batafire
            if (NPC.frameCounter > 5)
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y > endFrame * frameHeight || NPC.frame.Y < startFrame * frameHeight)
            {
                NPC.frame.Y = startFrame * frameHeight;
            }
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

        public override Color? GetAlpha(Color lightColor)
        {
            Lighting.AddLight(NPC.Center, TorchID.Torch);
            return Color.White;
        }
    }
}