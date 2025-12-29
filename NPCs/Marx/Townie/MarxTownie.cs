using KirboMod.Projectiles.Marx;
using KirboMod.Systems;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace KirboMod.NPCs.Marx.Townie
{
    //Load NPC head icon
    [AutoloadHead]
    public class MarxTownie : ModNPC
    {
        public int hint = -1; //start at -1 so when it increases for the first time it starts at 0

        bool wrathfulGodsEnabled = ModLoader.HasMod("NoxusBoss");

        #region WOTG checks

        //Holding off on adding project references to this mod for now so just skips inspection for these properties

        //[JITWhenModsEnabled("NoxusBoss")]
        //public bool AvatarDown => NoxusBoss.Core.World.WorldSaving.BossDownedSaveSystem.HasDefeated<NoxusBoss.Content.NPCs.Bosses.Avatar.SecondPhaseForm.AvatarOfEmptiness>();

        //[JITWhenModsEnabled("NoxusBoss")]
        //public bool RiftEclipseOn => NoxusBoss.Core.World.GameScenes.RiftEclipse.RiftEclipseManagementSystem.RiftEclipseOngoing;

        //[JITWhenModsEnabled("NoxusBoss")]
        //public bool SolynAround => NPC.AnyNPCs(ModContent.NPCType<NoxusBoss.Content.NPCs.Friendly.Solyn>());

        //[JITWhenModsEnabled("NoxusBoss")]
        //public bool NamelessDown => NoxusBoss.Core.World.WorldSaving.BossDownedSaveSystem.HasDefeated<NoxusBoss.Content.NPCs.Bosses.NamelessDeity.NamelessDeityBoss>();

        #endregion WOTG checks

        private static Profiles.StackedNPCProfile NPCProfile;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 26;

            NPCID.Sets.ExtraFramesCount[Type] = 10;
            NPCID.Sets.AttackFrameCount[Type] = 5;
            //TODO: FIX MARX NOT ATTACKING WHEN ENEMY IS TOO CLOSE?? IDK IF IT'S FROM PRETTYSAFE BEING TOO LOW OR TOO HIGH
            //CHECK IF NO OTHER PLACES SET THIS
            NPCID.Sets.DangerDetectRange[Type] = 400;
            NPCID.Sets.PrettySafe[Type] = 0;
            NPCID.Sets.AttackType[Type] = 0;
            NPCID.Sets.AttackTime[Type] = 28;
            NPCID.Sets.AttackAverageChance[Type] = 1;
            NPCID.Sets.NeedsExpertScaling[Type] = true;

            NPCID.Sets.ShimmerTownTransform[Type] = true;

            NPCID.Sets.FaceEmote[Type] = ModContent.EmoteBubbleType<MarxTownieEmote>();

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new()
            {
                Velocity = 1f,
                Direction = -1
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            NPC.Happiness
                .SetBiomeAffection<CorruptionBiome>(AffectionLevel.Love)
                .SetBiomeAffection<CrimsonBiome>(AffectionLevel.Love)
                .SetBiomeAffection<HallowBiome>(AffectionLevel.Like)
                .SetBiomeAffection<JungleBiome>(AffectionLevel.Like)
                .SetBiomeAffection<ForestBiome>(AffectionLevel.Hate)
                .SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Love)
                .SetNPCAffection(NPCID.ArmsDealer, AffectionLevel.Like)
                .SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Like)
                .SetNPCAffection(NPCID.Nurse, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Guide, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Dryad, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.SantaClaus, AffectionLevel.Hate);

            //different profiles for different textures
            NPCProfile = new Profiles.StackedNPCProfile(
                new Profiles.DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture)),
                new Profiles.DefaultNPCProfile(Texture + "_Shimmer", NPCHeadLoader.GetHeadSlot(HeadTexture))
            );
        }

        public override void SetDefaults()
        {

            NPC.width = 36;
            NPC.height = 36;
            DrawOffsetY = 32;
            NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.defense = MarxBoss.Defense;
            NPC.lifeMax = MarxBoss.MaxHP;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath6; //ghastly sound (For playing up the "-has left!" effect)
            NPC.knockBackResist = 0.8f;
            AnimationType = NPCID.Guide;
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.damage = 0;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            //uses AddRange to add multiple things instead of Add for simplicity
            bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,

				//bestiary description
				new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.KirboMod.NPCs.Bestiary.MarxTownie")),

            ]);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.netMode != NetmodeID.Server && NPC.life <= 0) //use smoke to signal "escape" instead of "death"
            {
                for (int i = 0; i < 10; i++)
                {
                    Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Main.rand.Next(11, 14), Main.rand.NextFloat() * 0.5f + 0.5f);
                }
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_SpawnNPC)
            {
                MarxSpawningSystem.UnlockedMarx = true; //secure the deal
            }
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            return MarxSpawningSystem.UnlockedMarx && !NPC.AnyNPCs(ModContent.NPCType<MarxBoss>())
                && !NPC.AnyNPCs(ModContent.NPCType<MarxPrelude>()) && !MarxSpawningSystem.MarxActive;
        }

        public override ITownNPCProfile TownNPCProfile()
        {
            return NPCProfile;
        }

        public override List<string> SetNPCNameList()
        {
            return default; //use type name
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new();

            for (int i = 0; i < 10; i++) //more common than others
            {
                chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.D1"));
                chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.D2"));
                chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.D3"));
                chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.D4"));
                chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.D5"));
            }

            if (Main.bloodMoon || Main.eclipse || Main.pumpkinMoon || Main.snowMoon)
            {
                for (int i = 0; i < 20; i++) //much more common
                {
                    chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DMoon"));
                }
            }

            if (NPC.IsShimmerVariant)
            {
                for (int i = 0; i < 10; i++)
                {
                    chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DMarxolor"));
                }
            }

            if (DownedBossSystem.downedMarxBoss)
            {
                for (int i = 0; i < 5; i++) //a bit more common
                {
                    chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DPostMarx"));
                }
            }
            else if (NPC.downedGolemBoss)
            {
                for (int i = 0; i < 5; i++) //a bit more common
                {
                    chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DPreMarx"));
                }
            }

            chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DShutTheHellUp"));
            chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DReference"));
            chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DKirbyTransformation"));

            if (Main.hardMode)
            {
                chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DKirby1"));
                chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DKirby2"));
                chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DKirby3"));
                chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DKirby4"));
            }

            if (Main.LocalPlayer.wings == 45) //celestial starboard
            {
                chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DKirby5"));
            }

            string chosenChat = chat;

            if (wrathfulGodsEnabled) //only display these lines when wotg is installed
            {
                //if (AvatarDown)
                //{
                //    chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.D1Hater2"));
                //    chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DAOEDowned"));
                //}
                //else
                //{
                //    if (RiftEclipseOn)
                //    {
                //        for (int i = 0; i < 20; i++) //much more common
                //        {
                //            chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DRift"));
                //        }
                //    }

                //    if (SolynAround)
                //    {
                //        for (int i = 0; i < 10; i++) //common
                //        {
                //            chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.D1Hater"));
                //        }
                //    }
                //}

                //if (NamelessDown)
                //{
                //    for (int i = 0; i < 10; i++) //common
                //    {
                //        chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DNamelessDowned"));
                //    }
                //}
            }

            //set to true when, well, talking to Marx
            Main.LocalPlayer.GetModPlayer<KirbPlayer>().talkedToMarx = true;

            return chosenChat;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        { // What the chat buttons are when you open up the chat UI
            button = "Help";
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {
            if (firstButton)
            {
                GetHint();

                if (hint > 16)  //restart if too high
                {
                    hint = -1;
                    GetHint(); //try again
                }
            }
        }

        private void GetHint()
        {
            Player player = Main.LocalPlayer;

            string? chosenText = null;

            hint++;

            #region Help Text

            while (chosenText == null)
            {
                if (hint == 0)
                {
                    chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HStarbits");
                }
                else if (hint == 1)
                {
                    chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HWeapons");
                }
                else if (hint == 2)
                {
                    chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HBosses");
                }
                else if (hint == 3)
                {
                    if (!DownedBossSystem.downedWhispyBoss)
                        chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HWhispyWoods");
                    else
                        hint++;
                }
                else if (hint == 4)
                {
                    if (!DownedBossSystem.downedKrackoBoss)
                        chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HKracko");
                    else
                        hint++;
                }
                else if (hint == 5)
                {
                    if (!DownedBossSystem.downedKingDededeBoss)
                        chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HKingDedede");
                    else
                        hint++;
                }
                else if (hint == 6)
                {
                    if (NPC.downedBoss2)
                        chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HMidbosses");
                    else
                        hint++;
                }
                else if (Main.hardMode)
                {
                    if (hint == 7)
                    {
                        chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HDreamMatter");
                    }
                    else if (hint == 8)
                    {
                        chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HRareStones");
                    }
                    else if (hint == 9)
                    {
                        chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HEvoWeapons");
                    }
                    else if (hint == 10)
                    {
                        if (!DownedBossSystem.downedNightmareBoss)
                            chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HNightmare");
                        else
                            hint++;
                    }
                    else if (NPC.downedPlantBoss)
                    {
                        if (hint == 11)
                        {
                            chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HHeartMatter");
                        }
                        else if (hint == 12)
                        {
                            if (!DownedBossSystem.downedDarkMatterBoss)
                                chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HDarkMatter");
                            else
                                hint++;
                        }
                        else if (hint == 13)
                        {
                            chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HRainbowSword");
                        }
                        else if (NPC.downedGolemBoss)
                        {
                            if (hint == 14)
                            {
                                if (!DownedBossSystem.downedMarxBoss)
                                    chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HMarx");
                                else
                                    hint++;
                            }
                            else if (NPC.downedMoonlord)
                            {
                                if (hint == 15)
                                {
                                    if (!DownedBossSystem.downedZeroBoss)
                                        chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HZero");
                                    else
                                        hint++;
                                }
                                else if (hint == 16)
                                {
                                    chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HPostZero");
                                }
                                else
                                {
                                    hint = 0;
                                }
                            }
                            else
                            {
                                hint = 0;
                            }
                        }
                        else
                        {
                            hint = 0;
                        }
                    }
                    else
                    {
                        hint = 0;
                    }
                }
                else
                {
                    hint = 0;
                }
            }

            Main.npcChatText = chosenText;

            #endregion Help Text
        }

        public override LocalizedText DeathMessage => Language.GetText("Mods.KirboMod.NPCs.MarxTownie.Leave");

        public override bool CanGoToStatue(bool toKingStatue) => true;


        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {

            damage = 30;
            knockback = 7f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 0;
            randExtraCooldown = 0;

        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = 0;//blank proj
            attackDelay = 28; //7 fps
        }
        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 25f;
            randomOffset = 0f;
            gravityCorrection = 30;
            ShootBallStupidWorkaround(multiplier);

        }
        void ShootBallStupidWorkaround(float shootSpeed)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            int unused = 0;
            int atkDelay = -1;
            TownNPCAttackProj(ref unused, ref atkDelay);
            if (NPC.localAI[3] != atkDelay - 1)
            {
                return;
            }

            float num119 = -1f;
            float num2 = -1f;
            int leftTargetIndex = -1;
            int num13 = 0;
            int rightTargetIndex = -1;
            Vector2 Center = NPC.Center;
            bool flag5 = false;
            for (int m = 0; m < Main.maxNPCs; m++)
            {
                if (!Main.npc[m].active || Main.npc[m].friendly || Main.npc[m].damage <= 0 || (!Main.npc[m].noTileCollide && !Collision.CanHit(Center, 0, 0, Main.npc[m].Center, 0, 0)) || !NPCLoader.CanHitNPC(Main.npc[m], NPC))
                {
                    continue;
                }
                bool validTarget = Main.npc[m].CanBeChasedBy(this);
                flag5 = true;
                float num36 = Main.npc[m].Center.X - NPC.Center.X;
                if (num36 < 0f && (num119 == -1f || num36 > num119))
                {
                    num119 = num36;
                    if (validTarget)
                    {
                        leftTargetIndex = m;
                    }
                }
                if (num36 > 0f && (num2 == -1f || num36 < num2))
                {
                    num2 = num36;
                    if (validTarget)
                    {
                        rightTargetIndex = m;
                    }
                }
            }
            if (flag5)
            {
                num13 = ((num119 == -1f) ? 1 : ((num2 != -1f) ? (num2 < 0f - num119).ToDirectionInt() : (-1)));
            }

            Vector2 projSpawnPos = NPC.Center + new Vector2(NPC.spriteDirection * 16, -10f);
            int projDamage = GetBallDamage();
            Vector2 shootVel = -Vector2.UnitY;
            if (NPC.spriteDirection == 1 && rightTargetIndex != -1)
            {
                shootVel = GetShootVelocity(rightTargetIndex, shootSpeed, projSpawnPos);
            }
            if (NPC.spriteDirection == -1 && leftTargetIndex != -1)
            {
                shootVel = GetShootVelocity(leftTargetIndex, shootSpeed, projSpawnPos);
            }
            else
            {
                //attempt to shoot without matching direction to target
                if (rightTargetIndex != -1)
                {
                    shootVel = GetShootVelocity(rightTargetIndex, shootSpeed, projSpawnPos);
                }
                else if(leftTargetIndex != -1)
                {
                    shootVel = GetShootVelocity(leftTargetIndex, shootSpeed, projSpawnPos);
                }
            }
            if (shootVel.HasNaNs())
            {
                //failsafe
                shootVel = new(NPC.spriteDirection * 20f, -1f);
            }
            float kb = 0f;
            TownNPCAttackStrength(ref unused, ref kb);
            int num69 = Projectile.NewProjectile(NPC.GetSource_FromThis(), projSpawnPos, shootVel, ModContent.ProjectileType<MarxBall>(), projDamage, kb, -1);
            Main.projectile[num69].npcProj = true;
            Main.projectile[num69].noDropItem = true;

        }
        Vector2 GetShootVelocity(int targetIndex, float shootSpeed, Vector2 projSpawnPos)
        {
            NPC target = Main.npc[targetIndex];
            Vector2 targetPos = target.Center;
            Vector2 targetVel = target.velocity;
            Vector2 shootVel = projSpawnPos.DirectionTo(targetPos) * shootSpeed;
            Utils.ChaseResults results = Utils.GetChaseResults(projSpawnPos, shootSpeed, targetPos, targetVel);
            float timeToIntercept = projSpawnPos.Distance(targetPos) / shootSpeed;
            if (results.InterceptionHappens)
            {
                shootVel = results.ChaserVelocity;
                timeToIntercept = results.InterceptionTime;
            }
            //shootVel += Utils.FactorAcceleration(shootVel, timeToIntercept, MarxBall.Gravity, 0);
            return CalculateInterceptVelocityIterative(projSpawnPos, targetPos, targetVel, shootSpeed, MarxBall.Gravity.Y, 5);
        }
        public static Vector2 CalculateInterceptVelocityIterative(Vector2 start, Vector2 target, Vector2 targetVelocity, float launchSpeed, float gravity, int iterations = 3)
        {
            Vector2 predictedTarget = target + targetVelocity;
            for (int i = 0; i < iterations; i++)
            {
                // Recalculate the velocity needed to hit the current prediction
                Vector2 launchVelocity = CalculateLaunchVelocity(start, predictedTarget, launchSpeed, gravity);

                // If launchVelocity is zero (no solution), break early
                if (launchVelocity == Vector2.Zero)
                    break;

                // Estimate travel time using actual velocity vector
                float time = (predictedTarget - start).Length() / launchVelocity.Length();

                // Predict target's future position using that time
                predictedTarget = target + targetVelocity * time;
            }
            // After iterations, calculate final velocity to predicted position
            Vector2 result = CalculateLaunchVelocity(start, predictedTarget, launchSpeed, gravity);

            if (result == Vector2.Zero)
            {
                predictedTarget = target + targetVelocity * (start.Distance(target) / launchSpeed);
                return start.DirectionTo(predictedTarget) * launchSpeed;
            }
            return result;
        }

        static Vector2 CalculateLaunchVelocity(Vector2 start, Vector2 target, float launchSpeed, float gravity)
        {
            Vector2 delta = target - start;
            float dx = delta.X;
            float dy = delta.Y;

            // Quadratic formula coefficients
            float g = gravity;
            float v = launchSpeed;
            float v2 = v * v;

            float A = (g * dx * dx) / (2f * v2);
            float B = dx;
            float C = A - dy;

            float discriminant = B * B - 4f * A * C;

            if (discriminant < 0f)
            {
                // No real solution: target is out of range
                return Vector2.Zero;
            }

            float sqrtDiscriminant = (float)System.Math.Sqrt(discriminant);

            // Two possible solutions for k (slope of velocity vector)
            float k1 = (-B + sqrtDiscriminant) / (2f * A);
            float k2 = (-B - sqrtDiscriminant) / (2f * A);

            // Pick the k that gives lower time (i.e., flatter arc = smaller |k|)
            float k = System.Math.Abs(k1) < System.Math.Abs(k2) ? k1 : k2;

            // Now recover vx and vy
            float vx = v / (float)System.Math.Sqrt(1f + k * k);
            vx *= dx < 0f ? -1f : 1f; // Fix direction based on dx
            float vy = k * vx;

            return new Vector2(vx, vy);
        }
        private int GetBallDamage()
        {
            float damageMult = 1;
            int unused = 0;
            if (NPC.combatBookWasUsed)
            {
                damageMult += 0.2f;
            }
            if (NPC.combatBookVolumeTwoWasUsed)
            {
                damageMult += 0.2f;
            }
            if (NPC.downedBoss1)
            {
                damageMult += 0.1f;
            }
            if (NPC.downedBoss2)
            {
                damageMult += 0.1f;
            }
            if (NPC.downedBoss3)
            {
                damageMult += 0.1f;
            }
            if (NPC.downedQueenBee)
            {
                damageMult += 0.1f;
            }
            if (Main.hardMode)
            {
                damageMult += 0.4f;
            }
            if (NPC.downedQueenSlime)
            {
                damageMult += 0.15f;
            }
            if (NPC.downedMechBoss1)
            {
                damageMult += 0.15f;
            }
            if (NPC.downedMechBoss2)
            {
                damageMult += 0.15f;
            }
            if (NPC.downedMechBoss3)
            {
                damageMult += 0.15f;
            }
            if (NPC.downedPlantBoss)
            {
                damageMult += 0.15f;
            }
            if (NPC.downedEmpressOfLight)
            {
                damageMult += 0.15f;
            }
            if (NPC.downedGolemBoss)
            {
                damageMult += 0.15f;
            }
            if (NPC.downedAncientCultist)
            {
                damageMult += 0.15f;
            }
            NPCLoader.BuffTownNPC(ref damageMult, ref unused);
            float kb = 0f;
            int damage = 0;
            TownNPCAttackStrength(ref damage, ref kb);

            if (Main.expertMode)
            {
                damage = (int)(damage * Main.GameModeInfo.TownNPCDamageMultiplier);
            }
            damage = (int)(damage * damageMult);
            return damage;
        }

        public override int? PickEmote(Player closestPlayer, List<int> emoteList, WorldUIAnchor otherAnchor)
        {
            int food = Main.rand.NextFromList<int>(EmoteID.ItemCookedFish, EmoteID.ItemSoup, EmoteID.PartyCake, EmoteID.Hungry);

            if (!Main.afterPartyOfDoom)
            {
                for (int i = 0; i < 5; i++) //more likely
                {
                    emoteList.Add(food);
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    emoteList.Add(EmoteID.EmoteEating);
                }
            }

            int reaction = EmoteID.EmoteHappiness;

            if (otherAnchor.entity is NPC { type: NPCID.SantaClaus }) //checks another NPC it's conversating with
            {
                return EmoteID.EmotionAnger;
            }
            else if (Main.bloodMoon || Main.eclipse || Main.pumpkinMoon || Main.snowMoon)
            {
                reaction = EmoteID.WeatherRainbow;
            }
            else if (otherAnchor.entity is Player)
            {
                reaction = Main.rand.NextFromList<int>(EmoteID.EmoteWink, EmoteID.EmotionLove, EmoteID.EmoteNote, EmoteID.EmoteSilly);

                if (DownedBossSystem.downedMarxBoss)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        emoteList.Add(EmoteID.EmotionCry);
                    }
                }
            }

            for (int i = 0; i < 5; i++) //more likely
            {
                emoteList.Add(reaction);
            }

            return null;
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