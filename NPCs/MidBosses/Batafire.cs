using KirboMod.ItemDropRules.DropConditions;
using KirboMod.Items;
using KirboMod.Projectiles.Flames;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace KirboMod.NPCs.MidBosses
{
    [AutoloadBossHead]

    public class Batafire : ModNPC
    {

        float AttackTimer { get => NPC.localAI[0]; set => NPC.localAI[0] = value; }
        ref float AttackType => ref NPC.localAI[1];

        int Animation { get => (int)NPC.localAI[2]; set => NPC.localAI[2] = value; }
        ref float DiveFireballDistTracker => ref NPC.localAI[3];

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
            NPC.lifeMax = Main.hardMode ? (NPC.downedGolemBoss ? 32000 : 5000) : 1500;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath4;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Main.hardMode ? (NPC.downedGolemBoss ? Item.buyPrice(gold: 20) : Item.buyPrice(gold: 5)) : Item.buyPrice(gold: 1);
            NPC.npcSlots = 5f;
            NPC.lavaImmune = true;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<Items.Banners.BatafireBanner>();
            ItemID.Sets.KillsToBanner[BannerItem] = 10;
            NPC.rarity = 1; //1 is dungeon slime, 4 is mimic

            AttackTimer = -30; //start with intro


        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
            new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.KirboMod.NPCs.Bestiary.Batafire")),
            ]);
        }
        public static int PostGolemShootTime => 40;
        public static int HardmodeShootTime => 80;
        public static int PreHardmodeShootTime => 80;
        public static int PostGolemShootStart => 30;
        public static int HardmodeShootStart => 30;
        public static int PreHardmodeShootStart => 60;
        public static int PostGolemShootCount => 8;
        public static int HardmodeShootCount => 8;
        public static int PrehardmodeShootCount => 8;

        public static int PostGolemShootInterval => 5;
        public static int HardmodeShootInterval => 10;
        public static int PrehardmodeShootInterval => 10;
        public static int PostGolemShootSpeed => 30;
        public static int HardmodeShootSpeed => 25;
        public static int PrehardmodeShootSpeed => 20;
        public static float PostGolemChaseSpeed => 20;
        public static float HardmodeChaseSpeed => 12;
        public static float PrehardmodeChaseSpeed => 8;
        public static float PostGolemChaseInertia => 40;
        public static float HardmodeChaseInertia => 30;
        public static float PrehardmodeChaseInertia => 20;
        public static float PostGolemDiveYSpeedMult => 2;
        public static float HardmodeDiveYSpeedMult => 1.5f;
        public static float PrehardmodeDiveYSpeedMult => 1f;
        public static float PostGolemDiveSpeedMult => 1.6f;
        public static float HardmodeDiveSpeedMult => 1.3f;
        public static float PrehardmodeDiveSpeedMult => 1f;
        public static float PostGolemDiveEndTime => 60;
        public static float HardmodeDiveEndTime => 60;
        public static float PrehardmodeDiveEndTime => 120;
        static float DashThreshold => 300;

        readonly int projDamage = Main.hardMode ? (NPC.downedGolemBoss ? 60 : 30) : 15;

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
                if (AttackType > 1)
                {
                    AttackType = 0;
                }

                if (AttackType == 0)
                {
                    if (AttackTimer - 180 > DashThreshold)
                    {
                        DiveFireballs();
                    }
                }
                if (AttackTimer > 180)
                {
                    if (AttackType == 0)
                    {
                        Dive(AttackTimer - 180, player, distanceFromPlayer);
                    }

                    if (AttackType == 1)
                    {
                        FireSpew(AttackTimer - 180);
                    }
                }
                else if (AttackTimer >= 0)
                {
                    Animation = 0;
                    NPC.TargetClosest(true);

                    float speed = Main.hardMode ? (NPC.downedGolemBoss ? 20 : 12) : 8;
                    float inertia = Main.hardMode ? (NPC.downedGolemBoss ? 40 : 30) : 20;

                    distanceFromPlayer.Normalize();
                    distanceFromPlayer *= speed;

                    int fireballRate = 100;
                    if (AttackTimer % fireballRate == 0 && AttackTimer != 0)
                    {
                        IdleFireballBurst();
                    }

                    NPC.velocity = (NPC.velocity * (inertia - 1) + distanceFromPlayer) / inertia;
                }

                AttackTimer++;
            }
            else
            {
                NPC.velocity.Y += 0.5f;
            }
        }
        void Dive(float timer, Player player, Vector2 distance)
        {
            float YSpeedMult = Main.hardMode ? (NPC.downedGolemBoss ? PostGolemDiveYSpeedMult : HardmodeDiveYSpeedMult) : PrehardmodeDiveYSpeedMult;
            float diveSpeedMult = Main.hardMode ? (NPC.downedGolemBoss ? PostGolemDiveSpeedMult : HardmodeDiveSpeedMult) : PrehardmodeDiveSpeedMult;

            float dashThreshold = DashThreshold;

            if (timer < dashThreshold)  //go up until above player
            {
                NPC.TargetClosest(true);
                NPC.velocity.Y -= 0.2f * YSpeedMult;
                NPC.velocity.X = 0;

                if (NPC.Center.Y < player.Center.Y - 500)
                {
                    AttackTimer = 179 + DashThreshold; //update attackTimer to properly update timer
                }
            }
            if (timer >= dashThreshold)
            {
                Animation = 1;

                if (timer == dashThreshold)
                {
                    SoundEngine.PlaySound(SoundID.Item100, player.Center);

                    Vector2 vel = distance / 20 * diveSpeedMult;

                    if (NPC.downedGolemBoss)
                        vel = (distance + player.velocity * 6) / 20 * diveSpeedMult; //not predict perfectly, but become more accurate to build tension

                    NPC.velocity = vel;
                }
                else
                {
                }
                NPC.velocity *= 0.965f; //slow
            }

            float endTime = Main.hardMode ? (NPC.downedGolemBoss ? dashThreshold + PostGolemDiveEndTime : dashThreshold + HardmodeDiveEndTime) : dashThreshold + PrehardmodeDiveEndTime;

            if (timer >= endTime)
            {
                DiveFireballDistTracker = 0;
                AttackTimer = 0;
                AttackType += 1;
                NPC.velocity *= 0.1f; //stunt velocity so it stops flying up
            }
        }

        void FireSpew(float timer)
        {
            NPC.velocity *= 0.9f;

            Animation = 2;
            if (timer == 1)
            {
                NPC.velocity.X = 0;
                NPC.velocity.Y = -2;
            }

            float shootTime = Main.hardMode ? (NPC.downedGolemBoss ? PostGolemShootTime : HardmodeShootTime) : PreHardmodeShootTime;
            float shootStart = Main.hardMode ? (NPC.downedGolemBoss ? PostGolemShootStart : HardmodeShootStart) : PreHardmodeShootStart;
            float shootInterval = Main.hardMode ? (NPC.downedGolemBoss ? PostGolemShootInterval : HardmodeShootInterval) : PrehardmodeShootInterval;
            int shootCount = Main.hardMode ? (NPC.downedGolemBoss ? PostGolemShootCount : HardmodeShootCount) : PrehardmodeShootCount;
            if (timer >= shootStart && timer % shootInterval == 0 && timer <= shootStart + shootTime)
            {
                ShootFireballs(timer, shootTime, shootStart, shootCount);
            }

            float endTime = Main.hardMode ? (NPC.downedGolemBoss ? shootTime + shootStart + 30 : shootTime + shootStart + 30) : shootTime + shootStart + 60;

            if (timer >= endTime)
            {
                AttackTimer = 0;
                AttackType += 1;
            }
        }

        private void ShootFireballs(float timer, float shootTime, float shootStart, float shootCount)
        {
            for (int i = 0; i < 20; i++) //spew dust every shot
            {
                Dust.NewDustPerfect(NPC.Center, DustID.Torch, Main.rand.NextVector2Circular(10f, 10f), Scale: 2f);
            }
            SoundEngine.PlaySound(SoundID.Item20, NPC.Center); //fire cast

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            Vector2 position = NPC.Center;
            float knockback = 5f;
            int type = ModContent.ProjectileType<Projectiles.Flames.BatafireFire>();
            //rotate starting down diagonally and then turning up
            float trajectory = (MathF.PI / 4) - MathF.PI / 2 * ((timer - shootStart) / shootTime);
            if (NPC.downedGolemBoss)
                trajectory = (MathF.PI / 2.5f) - (MathF.PI / 2.5f + MathF.PI / 2.5f) * ((timer - shootStart) / shootTime);
            float shootSpeed = Main.hardMode ? (NPC.downedGolemBoss ? PostGolemShootSpeed : HardmodeShootSpeed) : PrehardmodeShootSpeed;
            Vector2 projVel = new Vector2(MathF.Cos(trajectory), MathF.Sin(trajectory)) * shootSpeed;
            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projVel, type, projDamage / 2, knockback, Main.myPlayer);
            projVel.X *= -1;
            Projectile.NewProjectile(NPC.GetSource_FromAI(), position, projVel, type, projDamage / 2, knockback, Main.myPlayer);
        }

        void DiveFireballs()
        {
            if (!Main.hardMode || Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            bool golem = NPC.downedGolemBoss;
            float distPerFireball = golem ? 200 : 350;
            float offsetAmt = DiveFireballDistTracker % distPerFireball;
            float fireballGrav = .2f;
            Vector2 dashDeltaPos = (NPC.position - NPC.oldPosition);
            int fireballCount = (int)(DiveFireballDistTracker / distPerFireball);
            float fireballYVel = -16;
            for (int i = 0; i < fireballCount; i++)
            {
                BatafireFire.SpawnWithGravity(NPC.GetSource_FromAI(), NPC.Center - dashDeltaPos.SafeNormalize(Vector2.Zero) * (offsetAmt + i), new Vector2(0, fireballYVel), projDamage / 2, fireballGrav);

            }
            DiveFireballDistTracker %= distPerFireball;
            DiveFireballDistTracker += dashDeltaPos.Length();
        }
        void IdleFireballBurst()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient || !Main.hardMode)
            {
                return;
            }
            SoundEngine.PlaySound(SoundID.Item100, NPC.Center);

            int fireballCount = 8;
            if (NPC.downedGolemBoss)
            {
                fireballCount += 2;
            }
            if (Main.expertMode)
            {
                fireballCount += 3;
            }
            float fireballGrav = NPC.downedGolemBoss ? .1f : 0f;
            float shootSpeed = NPC.downedGolemBoss ? 10 : 15f;
            for (int i = 0; i < fireballCount; i++)
            {
                float angle = Utils.Remap(i, 0, fireballCount, 0, MathF.Tau);
                Vector2 vel = angle.ToRotationVector2() * shootSpeed;
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel, ModContent.ProjectileType<BatafireFire>(), projDamage / 2, 0f, -1, fireballGrav);

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

            PreGolemHardmodeCondition PreGolemCondition = new();
            IItemDropRule HardmodePreGolem = new LeadingConditionRule(PreGolemCondition);

            PostGolemHardmodeCondition PostGolemCondition = new();
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
            if (Animation == 0)
            {
                startFrame = 0;
                endFrame = 5;
            }
            else if (Animation == 1)
            {
                startFrame = 6;
                endFrame = 8;
            }
            else if (Animation == 2)
            {
                startFrame = 9;
                endFrame = 11;
            }
            else //invalid animation so set to 0
            {
                Animation = 0;
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
                    Vector2 speed = new((float)Math.Cos(MathHelper.ToRadians(i * 45)) * 20, (float)Math.Sin(MathHelper.ToRadians(i * 45)) * 20);

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

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            position = NPC.Bottom + Vector2.UnitY * 10;
            return true;
        }
        public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
        {
            boundingBox = NPC.Hitbox;
        }
    }
}