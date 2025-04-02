using KirboMod.Items.WhispyWoods;
using KirboMod.Systems;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.NPCs.NewWhispy
{
    [AutoloadBossHead]
    public partial class NewWhispyBoss : ModNPC
    {
        public override string BossHeadTexture =>  "KirboMod/NPCs/NewWhispy/NewWhispyBoss_Head_Boss";
        public const int FightAreaHeight = 448;// maybe set it as 480?
        public const int FightAreaWidth = 16 * 42 + 272 / 2;
        public const int CanopyWidth = FightAreaWidth + 16 * 6;
        static int FireAppleDamage => 60 / 2;
        static int GordoDamage => 40 / 2;
        static int AppleDamage => 40 / 2;
        static int BladoDamage => 46 / 2;
        static int CloseSpikeDamage => 50 / 2;
        static int SpikeDamage => 40 / 2;
        static int WindDamage => 40 / 2;
        static float SplittingWindSpeed => 12f;
        //splitting wind splits into a 7 armed spiral on ftw, 6 armed spiral on expert, and 5 armed spiral on classic
        public static int SplittingWindSplitCount => Main.getGoodWorld ? 7 : Main.expertMode ? 6 : 5;
        public static float SplittingWindRadius => 16 * 4f * SplittingWindSplitCount;
        public static SoundStyle AirShotSFX => new SoundStyle("KirboMod/Sounds/Projectiles/NewWhispy/AirShot");
        public static SoundStyle AirShotSplitSFX => new SoundStyle("KirboMod/Sounds/Projectiles/NewWhispy/AirShotSplit");
        public static SoundStyle ObjFallSFX => (new SoundStyle("KirboMod/Sounds/Projectiles/NewWhispy/WhispyObjFall")) with { MaxInstances = 0, Volume = 0.55f, PitchVariance = 0.2f };
        public enum AIState
        {
            Spawn = 0,
            Change,
            FireApple,
            Blado,
            Gordo,
            Wind,
            Tornado,
            CloseSpikes,
            EvenSpikes,
            SplittingWind,
            AppleSmall,
            AppleMedium
        }
        enum AnimationState
        {
            Regular = 0,
            Angry = 1,
            PuffedUpCheeksAndClosedMouth=2,
        }
        public override string Texture => "KirboMod/NPCs/NewWhispy/NewWhispyBase_Placeholder";
        public override void SetStaticDefaults()
        {
            Main.musicNoCrossFade[MusicLoader.GetMusicSlot(Mod, "Music/Evobyte_K1Boss")] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;

            // Add this in for bosses that have a summon item, requires corresponding code in the item
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new()
            {
                PortraitScale = 1f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 40,
                PortraitPositionXOverride = 50,
                Position = new Vector2(40, 30),
                Scale = 0.8f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetDefaults()
        {
            NPC.immortal = true;//for intro
            NPC.width = 272;
            NPC.height = 448-16 * 4;
            NPC.damage = 30;
            NPC.noTileCollide = false;
            NPC.defense = 10;
            NPC.lifeMax = 2500;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 1992f; // money it drops
            NPC.knockBackResist = 0f;
            Banner = 0;
            BannerItem = Item.BannerToItem(Banner);
            NPC.aiStyle = -1;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.timeLeft = 60;
            NPC.friendly = false;   
            if(Main.netMode != NetmodeID.Server)
            {
                LoadTextures();
            }
            if (!Main.dedServ)//if not dedicated server
            {
                LoadTextures();
                int musicSlot = MusicLoader.GetMusicSlot("KirboMod/Music/Evobyte_K1Boss");
                Music = musicSlot;
                Main.musicFade[musicSlot] = 1;
                Main.musicNoCrossFade[musicSlot] = true;
            }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale *= 1.5f;
            return true;
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            Helper.BossHpScalingForHigherDifficulty(ref NPC.lifeMax, balance);
            //NPC.damage = (int)(NPC.damage * 0.6f);
        }
        float CurrentStateProgress => Timer / (attackStartTime + attackCount * attackRate + attackExtraWaitTime);
        float CurrentShotProgress => (Timer - attackStartTime) % attackRate / attackRate;
        int CurrentShotIndex => ((int)Timer - attackStartTime) / attackRate;
        float CurrentAttackProgress => (Timer - attackStartTime) / ((attackCount - 1) * attackRate);
        int TotalStateDuration => (attackStartTime + attackCount * attackRate + attackExtraWaitTime);
        void DoHealthbarFillAndSnapToTile()
        {
            LoadTextures();
            NPC.damage = 0;
            attackExtraWaitTime = 500;
            Timer++;
            NPC.immortal = true;
            NPC.life = (int)Utils.Remap(Timer, 0, TotalStateDuration - 1, 1, NPC.lifeMax);
            if (WillEndStateThisFrame())
            {
                //should probably set this somewhere else to prevent multiplayer issues making it completely invincible forever.
                //(I did it in state change)
                NPC.immortal = false;
            }
            //NPC.position.Y = (int)(NPC.position.Y / 16) * 16;
            //NPC.position.X = (int)(NPC.position.X / 16) * 16;
        }
        bool CheckShouldShoot(int fireRate, int numberOfShots, int start)
        {
            return (Timer - start) % fireRate == 0 && Timer < (start + fireRate * numberOfShots) && Timer >= start;
        }
        bool CheckShouldShoot()
        {
            return (Timer - attackStartTime) % attackRate == 0
                && Timer < (attackStartTime + attackRate * attackCount)
                && Timer >= attackStartTime;
        }
        bool WillEndStateThisFrame(int fireRate, int numberOfShots, int start, int extraDelay)
        {
            return (Timer >= fireRate * numberOfShots + extraDelay + start);
        }
        bool WillEndStateThisFrame()
        {
            return (Timer >= attackRate * attackCount + attackExtraWaitTime + attackStartTime);
        }
        void TryEndState(int fireRate, int numberOfShots, int start, int extraDelay)
        {
            if (Timer > fireRate * numberOfShots + extraDelay + start)
            {
                State = AIState.Change;
                Timer = 0;
            }
        }
        Vector2 GetRandomPointInThirdOfLeavesForAttacks(int index)
        {
            int playerSide = MathF.Sign(TargetedPlayer.Center.X - NPC.Center.X);
            float range = 35f * 16f * 0.334f;
            float min = range * index;
            float max = range * (index + 1);
            return NPC.Top + new Vector2((Main.rand.NextFloat(min, max) + NPC.width * 0.65f) * playerSide, Main.rand.NextFloat(-10, 10));

        }
        Vector2 GetRandomPointInLeavesForAttacks()
        {
            //if (PhaseIndex == 0)
            {
                int playerSide = MathF.Sign(TargetedPlayer.Center.X - NPC.Center.X);
                return NPC.Top + new Vector2((Main.rand.NextFloat(35 * 16) + NPC.width * 0.65f) * playerSide , Main.rand.NextFloat(-10, 10));
            }
            return NPC.Center;//math this later
        }
        void LeafParticles(Vector2 position, int width, int height, float amountMultiplier = 1)
        {
            int amount = (int)(width * height * 0.05f * amountMultiplier);
            width += 1;
            height += 1;
            for (int i = 0; i < amount; i++)
            {
                Gore.NewGore(NPC.GetSource_FromAI(), position + new Vector2(Main.rand.Next(width), Main.rand.Next(height)), new Vector2(0, 1).RotatedByRandom(1), GoreID.TreeLeaf_Normal, 1);
            }
        }
        Vector2 GetMouthPosition()
        {
            return NPC.Bottom + new Vector2(NPC.spriteDirection * 100, -136);//adjust this later
        }
        static Vector2 ScanDownForTiles(Vector2 from, int maxTilesDown = 10)
        {
            Vector2 result = from;
            for (int i = 0; i < maxTilesDown; i++)
            {
                Point scanPoint = result.ToTileCoordinates();
                if (Main.tile[scanPoint].HasUnactuatedTile && Main.tileSolid[Main.tile[scanPoint].TileType] && !Main.tileSolidTop[Main.tile[scanPoint].TileType])
                {
                    return result;
                }
                result.Y += 16;
            }
            return result;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return State != AIState.Spawn;
        }
        void PlayAirShotSFX()
        {
            SoundEngine.PlaySound(AirShotSFX with { Pitch = -0.8f, PitchVariance = 0.2f }, GetMouthPosition());
        }
        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedWhispyBoss, -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<WhispyBag>())); //only drops in expert

            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert()); //checks if not expert
            LeadingConditionRule masterMode = new LeadingConditionRule(new Conditions.IsMasterMode()); //checks if master mode

            notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.Wood, 1, 20, 20)); //hell yeah

            notExpertRule.OnSuccess(new OneFromRulesRule(1, ItemDropRule.Common(ModContent.ItemType<Items.Weapons.SwishyTree>(), 1)
                , ItemDropRule.Common(ModContent.ItemType<Items.Weapons.GordoItem>(), 1, 300, 300)
                , ItemDropRule.Common(ModContent.ItemType<Items.Weapons.WindPipe>(), 1)
                , ItemDropRule.Common(ModContent.ItemType<Items.Weapons.SwayingBranch>(), 1)));

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<WhispyMask>(), 7));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WhispyTrophy>(), 10)); //drop trophy

            //master mode stuff
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeables.BossRelics.WhispyWoodsRelic>()));

            masterMode.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.WhispyWoods.WhispyPetItem>(), 4));

            // add the rules
            npcLoot.Add(notExpertRule);
            npcLoot.Add(masterMode);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "Whispy Woods";
            potionType = ItemID.LesserHealingPotion;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
          
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 8; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                {
                    // go around in a octogonal pattern
                    Vector2 speed = new Vector2((float)Math.Cos(MathHelper.ToRadians(i * 45)) * 25, (float)Math.Sin(MathHelper.ToRadians(i * 45)) * 25);

                    Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.BoldStar>(), speed, Scale: 3f); //Makes dust in a messy circle
                    d.noGravity = true;
                }
                for (int i = 0; i < 20; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
                    Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, speed, Main.rand.Next(11, 13), Scale: 2f); //double jump smoke
                }
                for (int i = 0; i < 200; i++)
                {
                    int playerSide = MathF.Sign(TargetedPlayer.Center.X - NPC.Center.X);
                    Vector2 spawnPos = NPC.Top + new Vector2((Main.rand.NextFloat(-20 * 16, 54 * 16)) * playerSide, Main.rand.NextFloat(-200, 10));
                    Gore g = Gore.NewGoreDirect(NPC.GetSource_Death(), spawnPos, Vector2.Zero, GoreID.TreeLeaf_Normal);
                    if (Main.expertMode)
                    {
                        SpriteFrame frame = g.Frame;
                        frame.CurrentColumn = (byte)Main.rand.Next(10, 13);
                        g.Frame = frame;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(3f, 3f); //circle
                    Dust d = Dust.NewDustPerfect(NPC.Center, DustID.Dirt, speed * 2, Scale: 1f); //Makes dust in a messy circle
                    d.noGravity = false;
                }
            }
        }
    }
}
