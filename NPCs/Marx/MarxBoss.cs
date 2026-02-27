using KirboMod.Configs;
using KirboMod.Dusts.MarxPurpleSmoke;
using KirboMod.Dusts.MarxSparks;
using KirboMod.NPCs.Marx.SpecialFX;
using KirboMod.Projectiles.Marx.GiantBlackHoleOfDoom;
using KirboMod.Projectiles.Marx.IceBomb;
using KirboMod.Projectiles.Marx.VineSeed;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.NPCs.Marx
{
    [AutoloadBossHead]
    public partial class MarxBoss : ModNPC //Nightmare Wizard used as a base
    {
        public static int TeleportFrameDuration => 3;
        static int MassiveLaserChargeupTime => Main.getGoodWorld ? 65 : Main.expertMode ? 80 : 120;
        static int MassiveLaserDuration => 50;
        static int MassiveLaserExtraWaitBeforeTeleport => 1;//NEEDS TO BE MORE THAN 0 OR ELSE NO TELEPORT WILL HAPPEN
        static int MassiveLaserExtraWaitAfterTeleport => 19;
        static int MassiveLaserRecoilSpeed => 70;
        static int CutterMoveDuration => TotalTeleportInOutDuration + 1;
        static int CutterChargeDuration => 53;
        static int CutterRounds => 2;
        static int CutterExtraWaitAfterRound => 20;
        static int CutterExtraWaitAfterAllRounds => 100;
        static int CutterRoundDuration => CutterMoveDuration + CutterChargeDuration + CutterExtraWaitAfterRound;
        static int IceBombMoveDuration => Main.expertMode ? 40 : 80;
        static int IceBombMaxHold => 120;
        static int IceBombSpitDuration => 15;
        static int IceBombExtraWait => 30;
        static float IceBombAimAheadAmount => 7f;
        static int DashFromBelowChaseDuration => 80;
        static int DashFromBelowTelegraphDuration => Main.expertMode ? 25 : 40;
        static int DashFromBelowDashUpDuration => 20;
        static int DashFromBelowDashUpSpeed => 60;
        //I don't remember why there is a +4, but keep it
        static int TeleportFrenzyTpRate => TeleportFrameDuration * (TeleportFrameEnd - TeleportFrameStart + 4);
        static int TeleportFrenzyTpCount => Main.getGoodWorld ? 2 : 6;
        static int VineSeedCount => 10;
        static int VineSeedSpreadX => 2000;
        static int VineSeedRate => 2;
        static int VineSeedStartup => 30;
        static int VineSeedTeleportRate => 100;
        static int VineSeedExtraWait => 200;
        static float VineSeedDropVel => 12;
        static float VineSeedSpawnYOffset => -600;
        static float VineSeedMinFallDist => 100;
        static float VineSeedMaxFallDist => 1200;

        /// <summary>
        /// DO NOT SET DIRECTLY!!! use function ChangeAnimation()
        /// </summary>
        private Animation animation = Animation.Intro;
        private AttackType attacktype = AttackType.Intro;
        private AttackType lastattacktype = AttackType.DecideNext;


        static int DeathAnimDuration => 220;
        public static int IntroDuration => 60;
        ref float AttackTimer { get => ref NPC.ai[0]; }
        ref float DeathCounter { get => ref NPC.ai[1]; }

        Vector2 DeathLocation;

        Vector2 TargetTPPos
        {
            get
            {
                return new Vector2(NPC.ai[2], NPC.ai[3]);
            }
            set
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.ai[2] = value.X;
                    NPC.ai[3] = value.Y;
                }
                else
                {
                    //failsafe value so he doesn't teleport out of existence if something goes wrong
                    NPC.ai[2] = NPC.position.X + NPC.width / 2;
                    NPC.ai[3] = NPC.position.Y + NPC.height / 2;
                }

            }
        }
        public static SoundStyle SplitSFX => new("KirboMod/Sounds/NPC/Marx/BlackholeSnap");
        public static SoundStyle TpSFX => new("KirboMod/Sounds/NPC/Marx/Teleport");
        public override void AI() //constantly cycles each time
        {
            NPC.TargetClosest(false);
            Player player = Main.player[NPC.target];
            NPC.spriteDirection = 1;

            AttackTimer++;
            NPC.TargetClosest(false);
            if (DeathCounter > 0)
            {
                DoDeathAnimation();
            }
            else if (attacktype == AttackType.Intro && AttackTimer <= IntroDuration + 1) //intro
            {
                if (AttackTimer > IntroDuration)
                {
                    EndState();
                }
            }
            else if ((NPC.target < 0 || NPC.target == 255 || player.dead || !player.active) && AttackTimer == 1)
            {
                //don't need to target closest again because already did it this frame
                Despawn();
            }
            else //regular attack
            {
                AttackCycle();
            }

            //if (AttackTimer == 59)
            //{
            //    ShootCutters();
            //}
            NPC.spriteDirection = 1;
        }
        void State_BlackHole()
        {
            if (AttackTimer == 1)
            {
                ChangeAnimation(Animation.Split);
            }
            if (AttackTimer == HorizontalSplitStart)
            {
                SoundEngine.PlaySound(SplitSFX, NPC.Center);
            }
            //a bit of delay
            if (AttackTimer == HorizontalSplitStart + 3)
            {
                SpawnBlackHole();
            }
            if (AttackTimer > HorizontalSplitStart)
            {
                NPC.dontTakeDamage = true;
            }
            else
            {
                NPC.dontTakeDamage = false;
            }
            float endTime = HorizontalSplitStart + 3 + MarxBlackHole.SuckDuration + MarxBlackHole.ScaleUpDuration;
            if (AttackTimer >= endTime)
            {
                if (AttackTimer == endTime)
                {
                    ChangeAnimation(Animation.TeleportIn);
                }
                if (AttackTimer > endTime + TotalTeleportInOutDuration / 2)
                {
                    EndState_DontSetLastAttackType();
                }
            }

        }
        private void AttackCycle()
        {
            switch (attacktype)
            {
                case AttackType.Teleport:
                    State_Teleport();
                    break;
                case AttackType.Cutter:
                    State_Cutter();
                    break;
                case AttackType.Vine:
                    State_Vine();
                    break;
                case AttackType.IceBomb:
                    State_IceBomb();
                    break;
                case AttackType.MassiveLaser:
                    State_MassiveLaser();
                    break;
                case AttackType.BlackHole:
                    State_BlackHole();
                    break;
                case AttackType.Intro:
                    break;
                case AttackType.DashFromBelow:
                    State_DashFromBelow();
                    break;
                case AttackType.TeleportFrenzy:
                    State_TeleportFrenzy();
                    break;
                default:
                    break;
            }
            if (attacktype == AttackType.DecideNext)
            {
                DecideNextState();
            }
            if (animation == Animation.TeleportIn || animation == Animation.TeleportOut)
            {
                NPC.velocity = Vector2.Zero;
            }
            // Main.NewText(attacktype);
        }
        private void State_Vine()
        {

            if (AttackTimer % VineSeedTeleportRate == 0)
            {
                TeleportAboveAheadPlayer(30);
            }

            if (AttackTimer < VineSeedStartup)
            {

            }
            else if (AttackTimer < VineSeedStartup + VineSeedRate * VineSeedCount)
            {
                if ((AttackTimer - VineSeedStartup) % VineSeedRate == 0)
                {

                    Vector2 spawnPos;
                    if (NPC.HasPlayerTarget)
                    {
                        Player plr = Main.player[NPC.target];
                        spawnPos = plr.Center;
                        spawnPos += (plr.position - plr.oldPosition) * 20;
                    }
                    else
                    {
                        spawnPos = NPC.Center;
                    }
                    //PLACEHOLDER 
                    // SoundEngine.PlaySound(IceBombSpitSFX, spawnPos);
                    int seedIndex = (int)(AttackTimer - VineSeedStartup) / VineSeedRate;
                    seedIndex = (int)((seedIndex * Helper.Phi) % VineSeedCount);
                    float range = VineSeedSpreadX / VineSeedCount;
                    range /= 4;
                    spawnPos.X += Utils.Remap(seedIndex, 0, VineSeedCount, -VineSeedSpreadX / 2f, VineSeedSpreadX / 2f);
                    spawnPos.X += Main.rand.NextFloat(-range, range);
                    int vineSeedID = ModContent.ProjectileType<MarxVineSeed>();
                    //Main.rand.NextFloat(-VineSeedSpreadX / 2f, VineSeedSpreadX / 2f);
                    spawnPos.Y += VineSeedSpawnYOffset;
                    bool closeInX = false;
                    bool closeInY = false;
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile pToCheck = Main.projectile[i];
                        if (!pToCheck.active || pToCheck.type != vineSeedID)
                        {
                            continue;
                        }
                        if (MathF.Abs(spawnPos.X - pToCheck.position.X) < range * 2f)
                        {
                            closeInX = true;
                        }

                    }
                    if (!closeInX && (!Main.expertMode || !closeInY))
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPos, new Vector2(0, VineSeedDropVel), vineSeedID, VineSeedDamage, 0f, -1, 0, spawnPos.Y + Main.rand.NextFloat(VineSeedMinFallDist, VineSeedMaxFallDist));
                    }
                }
            }
            else if (AttackTimer < VineSeedStartup + VineSeedRate * VineSeedCount + VineSeedExtraWait)
            {
                if (AttackTimer < VineSeedStartup + VineSeedRate * VineSeedCount + VineSeedExtraWait / 2) //while waiting
                {
                    SoundEngine.PlaySound(MarxAmbientLaugh, NPC.Center);  //do a little laugh
                }
            }
            else
            {
                EndState();
            }

        }
        private void State_MassiveLaser()
        {
            if (AttackTimer == 1)
            {
                NPC.velocity = Vector2.Zero;
                Player plr = Main.player[NPC.target];
                int side = Main.rand.Next(2) * 2 - 1;
                NPC.direction = side;
                side *= 400;
                TeleportToPosAhead(plr.Center + new Vector2(side, 0), 40);
            }
            if (AttackTimer < TotalTeleportInOutDuration)
            {
                return;
            }

            float relativeTimer = AttackTimer - TotalTeleportInOutDuration;
            if (relativeTimer < 20)
            {
                if (relativeTimer == 1)
                {
                    ChangeAnimation(Animation.PuffUp);
                }
                int side = NPC.direction;
                side *= 400;
                Player plr = Main.player[NPC.target];
                MoveTo_LerpDecayVel(plr.Center + new Vector2(side, 0) + plr.velocity * 20, 40f, .1f);
            }
            else
            {
                NPC.velocity *= 0.9f;
            }
            if (relativeTimer < MassiveLaserChargeupTime)
            {
                if (relativeTimer == MathF.Max(MassiveLaserChargeupTime - 65, 0))
                {
                    //default max shoot charge strength value is 4    
                    IncreasingStrengthShake.Add(MassiveLaserChargeupTime, GFXConfig.Instance.MaxLaserChargeShake);
                    SoundEngine.PlaySound(MassiveLaserCharge.WithPitchOffset(0f), NPC.Center);
                }
            }
            else if (relativeTimer < MassiveLaserChargeupTime + MassiveLaserDuration)
            {
                NPC.velocity = new Vector2(NPC.direction * MassiveLaserRecoilSpeed, 0);
                if (relativeTimer == MassiveLaserChargeupTime)
                {
                    ChangeAnimation(Animation.Blast);
                    //default max shoot shake strength value is 10    
                    DecreasingStrengthShake.Add(MassiveLaserDuration, GFXConfig.Instance.MaxLaserShootShakeStrength);
                    LaserColorCorrection.ActivateScreenSaturation(MassiveLaserDuration, false);
                    ShootMassiveLaser();
                }
            }
            else if (relativeTimer < MassiveLaserChargeupTime + MassiveLaserDuration + MassiveLaserExtraWaitBeforeTeleport)
            {
                if (relativeTimer == MassiveLaserChargeupTime + MassiveLaserDuration)
                {
                    TeleportAboveAheadPlayer();
                }
            }
            else if (relativeTimer < MassiveLaserChargeupTime + MassiveLaserDuration + MassiveLaserExtraWaitBeforeTeleport + MassiveLaserExtraWaitAfterTeleport)
            {

            }
            else
            {
                NPC.velocity = Vector2.Zero;
                EndState();
            }
        }
        private void State_Cutter()
        {
            //constants defined outside
            int moveDuration = CutterMoveDuration;
            int chargeDuration = CutterChargeDuration;
            int cutterRounds = CutterRounds;
            int extraWaitAfterRound = CutterExtraWaitAfterRound;
            int extraWaitAfterAllRounds = CutterExtraWaitAfterAllRounds;
            int roundDuration = moveDuration + chargeDuration + extraWaitAfterRound;
            int relativeTimer = (int)(AttackTimer % roundDuration);


            //after all rounds
            if (AttackTimer > roundDuration * cutterRounds)
            {
                NPC.velocity *= .96f;
                if (AttackTimer > roundDuration * cutterRounds - extraWaitAfterRound + extraWaitAfterAllRounds)
                {
                    EndState();
                }
                return;
            }

            //first part of a round
            if (relativeTimer == MathF.Min(1, moveDuration - TotalTeleportInOutDuration))
            {
                // MoveAbovePlayer_LerpDecayVel();
                TeleportAboveAheadPlayer(CutterChargeDuration - 10);
            }
            //second part of a round
            else if (relativeTimer <= moveDuration + chargeDuration)
            {
                NPC.velocity *= .9f;
                if (relativeTimer == moveDuration + 1)
                {
                    ChangeAnimation(Animation.Cutter);
                    SoundEngine.PlaySound(CutterChargeSFX, NPC.Center);
                }
            }
            //third and final part of a round
            else if (relativeTimer <= moveDuration + chargeDuration + extraWaitAfterRound)
            {
                NPC.velocity *= .9f;
                if (relativeTimer == moveDuration + chargeDuration + 1)
                {
                    DecreasingStrengthShake.Add(10, 10);
                    ShootCutters();
                }
            }

        }
        private void State_IceBomb()
        {
            float velDecayAmount = 0.9f;
            if (AttackTimer <= IceBombMoveDuration + IceBombMaxHold)
            {
                IceBombMovement();
                if (AttackTimer == 1)
                {
                    SoundEngine.PlaySound(IceBombChaseSFX, NPC.Center);
                    SoundEngine.PlaySound(IceBombPuffUpSFX, NPC.Center);
                    ChangeAnimation(Animation.PuffUp);
                }

                Player plr = Main.player[NPC.target];
                float distAhead = (plr.Center.X - NPC.Center.X) * -MathF.Sign(plr.velocity.X);
                if (distAhead > MathF.Abs(plr.velocity.X * IceBombAimAheadAmount))
                {
                    //can skip when this time threshold is reached
                    if (AttackTimer >= IceBombMoveDuration + IceBombSpitDuration)
                    {
                        AttackTimer = IceBombMoveDuration + IceBombMaxHold;
                    }
                }
            }
            else if (AttackTimer <= IceBombMoveDuration + IceBombSpitDuration + IceBombMaxHold)
            {
                NPC.velocity *= velDecayAmount;
                if (AttackTimer == IceBombMoveDuration + IceBombMaxHold + 1)
                {
                    SoundEngine.PlaySound(IceBombSpitSFX, NPC.Center);
                    ChangeAnimation(Animation.Spit);
                    MarxIceBomb.SpawnBombsForEveryPlayerAndPlaySFX(NPC, IceBombDamage);
                    NPC.velocity.Y = -30;//recoil from spit
                }

            }
            else if (AttackTimer <= IceBombMoveDuration + IceBombSpitDuration + IceBombExtraWait + IceBombMaxHold)
            {
                NPC.velocity *= velDecayAmount;
            }
            else
            {
                EndState();

            }
        }
        void IceBombMovement()
        {
            Player plr = Main.player[NPC.target];
            float offY = -300;
            float accY = 0.1f;
            float accX = 0.01f;
            float speedY = 40;
            float speedX = 100;
            Vector2 targetPos = plr.Center;
            targetPos.X += plr.velocity.X * IceBombAimAheadAmount;
            targetPos.Y += offY;
            Vector2 deltaPos = targetPos - NPC.Center;
            float dist = MathF.Abs(deltaPos.Y);
            float speedMultY = Utils.GetLerpValue(16, 16 * 6, dist, true);
            speedY *= MathF.Sign(deltaPos.Y);
            speedX *= MathF.Sign(deltaPos.X);
            NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, speedY * speedMultY, accY);
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, speedX, accX);
        }
        private void State_TeleportFrenzy()
        {
            NPC.velocity = Vector2.Zero;
            NPC.damage = 0;
            int tpRate = TeleportFrenzyTpRate;
            int tpCount = TeleportFrenzyTpCount;
            int tpIndex = (int)(AttackTimer / tpRate);
            if (AttackTimer % tpRate == 1)
            {
                Player target = Main.player[NPC.target];
                Vector2 tpPos = target.Center;
                if (tpIndex != tpCount - 1)
                {
                    tpPos += Main.rand.Ring(16 * 11, 16 * 16);
                }
                else
                {
                    tpPos += target.velocity * 20;
                    tpPos -= Vector2.UnitY * 200;
                }
                TeleportTo(tpPos);
            }
            if (AttackTimer > tpCount * tpRate)
            {
                EndState(AttackType.BlackHole, Animation.Idle);
            }
        }
        //thank you chatgpt for part of this code
        private void DecideNextState()
        {

            // Fetch durations for each attack
            GetAttackDurations(out float cutterDuration, out float iceBombDuration, out float blackHoleDuration, out float laserDuration, out float vineDuration, out _, out float dashFromBelowDuration);

            Dictionary<AttackType, float> durations = new()
            {
                { AttackType.Cutter, cutterDuration },
                { AttackType.MassiveLaser, laserDuration },
                { AttackType.IceBomb, iceBombDuration },
                { AttackType.Vine, vineDuration },
                { AttackType.DashFromBelow, dashFromBelowDuration },
            };

            if (Main.expertMode || NPC.GetLifePercent() < 0.6f)
            {
                durations.Add(AttackType.TeleportFrenzy, blackHoleDuration);
            }

            List<KeyValuePair<AttackType, float>> candidates = durations
                .Where(kvp => kvp.Key != lastattacktype)
                .ToList();

            // Build a list of (AttackType, Weight) pairs where weight is 1/duration
            List<(AttackType atkType, float weight)> weightedAttackChances = candidates
                .Select(kvp => (atkType: kvp.Key, weight: 1f / kvp.Value))
                .ToList();

            // Compute total weight
            float totalWeight = weightedAttackChances.Sum(w => w.weight);

            // Build cumulative distribution
            float cumulative = 0f;
            for (int i = 0; i < weightedAttackChances.Count; i++)
            {
                (AttackType atk, float weight) = weightedAttackChances[i];
                cumulative += weight / totalWeight;
                weightedAttackChances[i] = (atk, cumulative);
            }

            // Roll random chance
            float roll = Main.rand.NextFloat();
            foreach ((AttackType atk, float threshold) in weightedAttackChances)
            {
                if (roll <= threshold)
                {
                    // Optionally apply special transitions here
                    attacktype = atk;
                    break;
                }
            }

            if (Main.rand.NextBool(1, Math.Max((int)(NPC.GetLifePercent() * 10), 1))) //gets more common as health goes down (consequent can't be less than antecedent)
                SoundEngine.PlaySound(MarxAmbientLaugh, NPC.Center);

            AttackTimer = 0;
            // Update lastattacktype based on canonical type
            lastattacktype = attacktype;
            return;

            //old attack distribution code

            //DONT SET LASTATTACKTYPE HERE BECAUSE IF YOU DO IT WILL ALWAYS END UP AS DECIDENEXT
            List<AttackType> attacks = new()
            //tp frenzy leads to black hole then dash from below, so don't include any of those in the list
            //massive laser also leads to dash from below
            { AttackType.Cutter, AttackType.IceBomb,  AttackType.TeleportFrenzy, AttackType.MassiveLaser, AttackType.Vine};
            //effectively removing the black hole attack from the options 1/2 of the time
            if (Main.rand.NextBool(2))
            {
                attacks.Remove(AttackType.TeleportFrenzy);
            }
            //don't do big laser too often
            //big laser also leads to dash from below
            if (Main.rand.NextBool(2))
            {
                attacks.Remove(AttackType.MassiveLaser);
            }
            attacks.Remove(lastattacktype);
            lastattacktype = attacktype;
            attacktype = Main.rand.NextFromCollection(attacks);
            //attacktype = AttackType.Cutter;//debug
            // attacktype = AttackType.MassiveLaser;//debug
            // attacktype = AttackType.IceBomb;//debug
            //attacktype = AttackType.Vine; //debug
            AttackTimer = 0;
        }
        private void GetAttackDurations(out float cutterDuration, out float iceBombDuration, out float blackHoleDuration, out float laserDuration, out float vineDuration, out float sum, out float dashFromBelowDuration)
        {
            dashFromBelowDuration = TotalTeleportInOutDuration + DashFromBelowChaseDuration + DashFromBelowTelegraphDuration + DashFromBelowDashUpDuration;
            blackHoleDuration = TeleportFrenzyTpCount * TeleportFrenzyTpRate;
            blackHoleDuration += HorizontalSplitStart + 3 + MarxBlackHole.SuckDuration + MarxBlackHole.ScaleUpDuration;

            cutterDuration = CutterRoundDuration * CutterRounds - CutterExtraWaitAfterRound + CutterExtraWaitAfterAllRounds;
            iceBombDuration = IceBombMoveDuration + IceBombSpitDuration + IceBombExtraWait + IceBombMaxHold / 2;// /2 to kinda average it idk
            laserDuration = MassiveLaserChargeupTime + MassiveLaserDuration + MassiveLaserExtraWaitBeforeTeleport + MassiveLaserExtraWaitAfterTeleport + TotalTeleportInOutDuration;
            vineDuration = VineSeedStartup + VineSeedRate * VineSeedCount + VineSeedExtraWait;
            sum = blackHoleDuration + cutterDuration + iceBombDuration + laserDuration + vineDuration;
        }
        private void State_Teleport()
        {
            NPC.velocity = Vector2.Zero;
            if (AttackTimer == 1)
            {
                TeleportAboveAheadPlayer();
            }
            if (AttackTimer > TotalTeleportInOutDuration)
            {
                EndState_DontSetLastAttackType();
            }
        }
        private void State_DashFromBelow()
        {
            ///add teleport
            Player plr = Main.player[NPC.target];




            if (AttackTimer == 1)
            {
                ChangeAnimation(Animation.ShadowHole);
            }
            if(AttackTimer - 1 == TotalTeleportInOutDuration / 2)
            {
                NPC.Center = plr.Center;
            }
            float chaseDuration = DashFromBelowChaseDuration;
            float riseTelegraphDuration = DashFromBelowTelegraphDuration;
            float dashUpDuration = DashFromBelowDashUpDuration;
            float dashUpSpeed = DashFromBelowDashUpSpeed;
            if (AttackTimer < chaseDuration)
            {
                if (AttackTimer - 1 >= TotalTeleportInOutDuration)
                {
                    Vector2 targetPos = plr.Center;
                    //if (Main.expertMode)
                    {
                        targetPos.X += plr.velocity.X * 30;
                    }
                    NPC.Center = Vector2.Lerp(NPC.Center, targetPos, .09f);
                }
            }
            else if (AttackTimer < chaseDuration + riseTelegraphDuration)
            {
                NPC.velocity = Vector2.Zero;
                Vector2 shadowHolePos = SearchForShadowHolePosition();
                if (AttackTimer == chaseDuration)
                {
                    SoundEngine.PlaySound(ShadowHoleStop, shadowHolePos);
                }
                for (int i = 0; i < 5; i++)
                {
                    Vector2 pos = Main.rand.BetterNextVector2Circular(200);
                    pos.Y *= .5f;
                    pos += shadowHolePos;

                    Dust d = Dust.NewDustPerfect(pos, DustID.Shadowflame);
                    d.noGravity = true;
                    d.noLight = true;
                    d.noLightEmittence = true;
                }
            }
            else if (AttackTimer < chaseDuration + riseTelegraphDuration + dashUpDuration)
            {
                if (AttackTimer == chaseDuration + riseTelegraphDuration)
                {
                    NPC.Center = SearchForShadowHolePosition();
                    SoundEngine.PlaySound(ShadowHoleDash, NPC.Center);
                    float maxWidth = 250;
                    float maxHeight = 50;
                    int dustID = ModContent.DustType<MarxSparks>();
                    for (int i = 0; i < 45; i++)
                    {
                        Vector2 offset = new(Main.rand.NextFloat(-maxWidth / 2, maxWidth / 2), -Main.rand.NextFloat(maxHeight));
                        Vector2 vel = offset * .2f;
                        vel += Main.rand.NextVector2Circular(5, 5);
                        Dust.NewDustPerfect(NPC.Center + offset, dustID, vel, 0, Color.White, 1f);
                    }
                    dustID = ModContent.DustType<PurpleSmoke>();
                    for (int i = 0; i < 10; i++)
                    {
                        Vector2 offset = new(Main.rand.NextFloat(-maxWidth / 2, maxWidth / 2), -Main.rand.NextFloat(maxHeight));
                        Vector2 vel = offset * .2f;
                        vel += Main.rand.NextVector2Circular(5, 5);
                        Dust.NewDustPerfect(NPC.Center + offset, dustID, vel, 0, Color.Purple, 0.35f);
                    }
                    DecreasingStrengthShake.Add();
                }
                NPC.velocity.Y = -dashUpSpeed;
                NPC.velocity.X = 0;
                ChangeAnimation(Animation.Rise);
            }
            else
            {
                NPC.velocity.Y = 0;
                EndState(AttackType.Teleport, Animation.TeleportOut);
            }
        }
        void EndState_DontSetLastAttackType()
        {
            AttackTimer = 0;
            ChangeAnimation(Animation.Idle);
            attacktype = AttackType.DecideNext;
        }
        void EndState()
        {
            AttackTimer = 0;
            ChangeAnimation(Animation.Idle);
            lastattacktype = attacktype;
            attacktype = AttackType.DecideNext;
        }
        void EndState(AttackType nextState, Animation nextAnimation)
        {
            AttackTimer = 0;
            ChangeAnimation(nextAnimation);
            lastattacktype = attacktype;
            attacktype = nextState;
        }
        void MoveAbovePlayer_LerpDecayVel(float yOff = -350, float maxSpeed = 30, float lerpAmount = .15f)
        {
            yOff = -270;
            Vector2 targetPos = new(0, yOff);
            Player plr = Main.player[NPC.target];
            targetPos += plr.Center;
            Vector2 ahead = plr.velocity * 10;
            ahead.X *= 1.3f;
            targetPos += ahead;
            MoveTo_LerpDecayVel(targetPos, maxSpeed, lerpAmount);
        }
        void MoveTo_LerpDecayVel(Vector2 targetPos, float maxSpeed, float lerpAmount)
        {
            Vector2 deltaPos = targetPos - NPC.Center;
            float dist = deltaPos.Length();
            float speedMult = Utils.GetLerpValue(16 * 3, 16 * 10, dist, true);
            Vector2 targetVel = deltaPos.SafeNormalize(Vector2.Zero);
            targetVel *= maxSpeed * speedMult;
            NPC.velocity = Vector2.Lerp(NPC.velocity, targetVel, lerpAmount);
        }
        static void ClampLength(ref Vector2 vector, float length = 300)
        {
            float vecLength = vector.Length();
            if (vecLength < length)
            {
                vector *= length / vecLength;
            }
        }
        Vector2 GetTeleportLocation(int framesAhead)
        {
            if (!NPC.HasValidTarget)
            {
                return Vector2.Zero;
            }
            Player plr = Main.player[NPC.target];
            Vector2 result = new Vector2(0, -250) + plr.velocity * framesAhead;
            ClampLength(ref result);
            result += plr.Center;
            return result;
        }
        void TeleportAboveAheadPlayer(int framesAhead = 20)
        {
            Vector2 tpLocation = GetTeleportLocation(framesAhead);
            TeleportTo(tpLocation);
        }
        void TeleportTo(Vector2 tpLocation)
        {
            TargetTPPos = tpLocation;
            NPC.netUpdate = true;
            ChangeAnimation(Animation.TeleportOut);
        }
        void TeleportToPosAhead(Vector2 tpLocation, int framesAhead = 20)
        {
            TargetTPPos = tpLocation;
            NPC.netUpdate = true;
            if (!NPC.HasValidTarget)
            {
                return;
            }
            Player plr = Main.player[NPC.target];
            TargetTPPos += plr.velocity * framesAhead;
            ChangeAnimation(Animation.TeleportOut);
        }
        void ChangeAnimation(Animation newAnimation)
        {
            //set to -1  instead of 0 because timer increments before it is read
            NPC.frameCounter = -1;
            animation = newAnimation;
        }

        public override bool CheckDead()
        {
            if (DeathCounter <= DeathAnimDuration)
            {
                NPC.active = true;
                NPC.life = 1;

                if (DeathCounter == 0)
                {
                    DeathLocation = NPC.Center; //only set once

                    SoundEngine.PlaySound(MarxDefeat with { MaxInstances = 0 }, NPC.Center);

                    SoundEngine.PlaySound(SoundID.NPCDeath1, NPC.Center);

                    DeathCounter++; //start going up
                }

                return false;
            }
            return true;
        }

        void DoDeathAnimation()
        {
            int slowRotDuration = 100;
            AttackTimer = 0;
            NPC.velocity = Vector2.Zero;

            if (DeathCounter >= 0 && DeathCounter <= DeathAnimDuration)
            {
                NPC.dontTakeDamage = true;

                ChangeAnimation(Animation.Defeat);

                if (DeathCounter < slowRotDuration)
                {
                    NPC.rotation += MathF.PI / (4 * slowRotDuration);
                }
                else
                {
                    NPC.rotation += MathF.Tau / 20;

                    float spinTime = MathF.Tau / 30 * (DeathCounter - 30);
                    NPC.Center = DeathLocation + new Vector2(MathF.Cos(spinTime), MathF.Sin(spinTime)) * (DeathAnimDuration - DeathCounter) * 5;

                    if (DeathCounter % 10 == 0 && DeathCounter != DeathAnimDuration)
                    {
                        BossDeathExplosion();
                    }

                    //if (DeathCounter % 2 == 0)
                    //{
                    //    int randomCell = Main.rand.NextBool(2) ? ModContent.GoreType<WingCell1>() :
                    //        ModContent.GoreType<WingCell2>();

                    //    int g = Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, Main.rand.NextVector2CircularEdge(5, 5),
                    //        randomCell, 1f);
                    //}
                }
            }
            else
            {
                NPC.HideStrikeDamage = true;
                NPC.SimpleStrikeNPC(999999, 1, false, 0, null, false, 0, false);

                BossDeathExplosion();

                //spawn a marx
                int index = -1;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    index = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y,
                                ModContent.NPCType<Townie.MarxTownieDown>());
                }

                if (index != -1)
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, index);
            }

            DeathCounter++; //keep going up
        }

        void BossDeathExplosion()
        {
            for (int i = 0; i < 8; i++)
            {
                // go around in a octogonal pattern
                Vector2 speed = new((float)Math.Cos(MathHelper.ToRadians(i * 45)) * 25, (float)Math.Sin(MathHelper.ToRadians(i * 45)) * 25);

                Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.BoldStar>(), speed, Scale: 3f); //Makes dust in a messy circle
                d.noGravity = true;
            }
            for (int i = 0; i < 20; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
                Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, speed, Main.rand.Next(11, 13), Scale: 2f); //double jump smoke
            }
        }

        void Despawn()
        {
            AttackTimer = 0;
            NPC.velocity.X = 0;
            NPC.velocity.Y -= 1;
            ChangeAnimation(Animation.Rise);
            NPC.EncourageDespawn(60);
        }

        public override void FindFrame(int frameHeight) // animation
        {

            if (NPC.IsABestiaryIconDummy)
            {
                animation = Animation.Idle;
            }

            if (animation == Animation.ShadowHole)
            {
                if (NPC.frameCounter == -1)
                {
                    SoundEngine.PlaySound(TpSFX, NPC.Center);
                }
                if (NPC.frame.Y < frameHeight * TeleportFrameStart)
                {
                    NPC.frameCounter = -1;
                    NPC.frame.Y = frameHeight * TeleportFrameStart;
                }
                NPC.frameCounter++;
                NPC.dontTakeDamage = true;
                int frameIndex = (TeleportFrameStart + (int)NPC.frameCounter / TeleportFrameDuration);
                //drawing code will check if frame Y is out of sheet bounds so it can actually draw the hole
                //transitioning to this from the black hole will
                //start the frame counter value from the one where the tp animation ends
                //because marx already teleports away during the black hole attack
                if (NPC.frameCounter / TeleportFrameDuration == TeleportFrameEnd - TeleportFrameStart)
                {
                    SoundEngine.PlaySound(ShadowHoleAppear, NPC.Center);
                }
                NPC.frame.Y = frameHeight * frameIndex;
                //no need to change frame.Y because next part of code does that

            }

            if (animation == Animation.TeleportOut)
            {
                if (NPC.frameCounter == -1)
                {
                    SoundEngine.PlaySound(TpSFX, NPC.Center);
                }
                if (NPC.frame.Y < frameHeight * TeleportFrameStart)
                {
                    NPC.frameCounter = -1;
                    NPC.frame.Y = frameHeight * TeleportFrameStart;
                }
                NPC.frameCounter++;
                NPC.dontTakeDamage = true;
                int frameIndex = (TeleportFrameStart + (int)NPC.frameCounter / TeleportFrameDuration);
                NPC.frame.Y = frameHeight * frameIndex;
                if (frameIndex >= Main.npcFrameCount[Type])//automatically set to teleport in
                {
                    ChangeAnimation(Animation.TeleportIn);
                    NPC.Center = TargetTPPos;
                    //no need to change frame.Y because next part of code does that
                }
            }

            if (animation == Animation.TeleportIn)
            {
                if (NPC.frameCounter == -1)
                {
                    SoundEngine.PlaySound(TpSFX, NPC.Center);
                }
                if (NPC.frame.Y < frameHeight * TeleportFrameStart)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y = frameHeight * TeleportFrameEnd;
                }

                NPC.frameCounter++;
                NPC.dontTakeDamage = true;
                //-1 to account for frames starting at 0 and not 1
                int teleportFrameCount = TeleportFrameEnd - TeleportFrameStart - 1;
                int frameIndex = (TeleportFrameStart + (teleportFrameCount - (int)NPC.frameCounter / TeleportFrameDuration));
                NPC.frame.Y = frameHeight * frameIndex;
                if (frameIndex < TeleportFrameStart)//automatically set to idle
                {
                    NPC.frame.Y = 0;
                    //sync position after teleporting
                    NPC.netUpdate = true;
                    ChangeAnimation(Animation.Idle);
                }
            }
            if (animation == Animation.Cutter)
            {
                NPC.frameCounter++;
                NPC.dontTakeDamage = false;
                //dummy value so it compiles, but it will always be overwritten
                int frameIndex = 0;
                int cutterThrowFrameCount = CutterThrowFrameEnd - CutterThrowFrameStart + 1;
                int relativeFrameCounter = (int)AttackTimer;
                relativeFrameCounter %= CutterRoundDuration;
                if (relativeFrameCounter < CutterMoveDuration || relativeFrameCounter >= CutterMoveDuration + CutterChargeDuration + cutterThrowFrameCount * CutterThrowFrameDuration
                    || AttackTimer >= CutterRoundDuration * CutterRounds + cutterThrowFrameCount * CutterThrowFrameDuration)
                {
                    //copy of idle framing
                    frameIndex = relativeFrameCounter / IdleFrameDuration;
                    frameIndex %= (IdleFrameEnd + 1);
                    frameIndex += IdleFrameStart;
                }
                else if (relativeFrameCounter < CutterMoveDuration + CutterChargeDuration)
                {
                    frameIndex = CutterChargeFrame;
                }
                else if (relativeFrameCounter < CutterMoveDuration + CutterChargeDuration + cutterThrowFrameCount * CutterThrowFrameDuration)
                {
                    int relativeTimer = (int)NPC.frameCounter - CutterChargeDuration;
                    frameIndex = relativeTimer / CutterThrowFrameDuration;
                    frameIndex += CutterThrowFrameStart;
                    //if(frameIndex > CutterThrowFrameEnd)
                    //{
                    //    frameIndex
                    //}
                }
                NPC.frame.Y = frameHeight * frameIndex;
            }
            if (animation == Animation.PuffUp)
            {
                NPC.frameCounter++;
                int puffUpFrameCount = PuffUpFrameEnd - PuffUpFrameStart + 1;

                int frameIndex = (int)NPC.frameCounter / PuffUpFrameDuration;
                frameIndex %= puffUpFrameCount;
                frameIndex += PuffUpFrameStart;
                NPC.frame.Y = frameHeight * frameIndex;
            }
            //MUST EXECUTE BEFORE IDLE CHECK
            if (animation == Animation.Spit)
            {
                NPC.frameCounter++;
                int frameIndex = (int)NPC.frameCounter / SpitFrameDuration;
                int spitFrameCount = SpitFrameEnd - SpitFrameStart + 1;
                frameIndex += SpitFrameStart;
                if (frameIndex > SpitFrameEnd + 1)
                {
                    ChangeAnimation(Animation.Idle);
                }
                if (frameIndex > SpitFrameEnd)
                {
                    frameIndex = SpitFrameEnd;
                }
                NPC.frame.Y = frameHeight * frameIndex;
                //hang on the last spit frame for an additional frame step

            }
            if (animation == Animation.Idle)
            {
                NPC.frameCounter++;
                NPC.dontTakeDamage = NPC.IsABestiaryIconDummy;
                int frameIndex = (int)NPC.frameCounter / IdleFrameDuration;
                frameIndex %= (IdleFrameEnd + 1);
                frameIndex += IdleFrameStart;
                NPC.frame.Y = frameHeight * frameIndex;
            }

            if (animation == Animation.Rise)
            {
                NPC.dontTakeDamage = false;
                NPC.frameCounter++;
                int frameIndex = (int)NPC.frameCounter / RiseFrameDuration;
                int riseFrames = RiseFrameEnd - RiseFrameStart;
                frameIndex %= (riseFrames + 1);
                frameIndex += RiseFrameStart;
                NPC.frame.Y = frameHeight * frameIndex;
            }
            if (animation == Animation.Blast)
            {
                NPC.dontTakeDamage = false;
                NPC.frameCounter++;
                int frameCount = BigLaserShootLeftFrameEnd - BigLaserShootLeftFrameStart + 1;

                int frameIndex = (int)NPC.frameCounter / BigLaserShootFrameDuration;
                frameIndex %= frameCount;
                if (NPC.direction == 1)
                {
                    frameIndex += BigLaserShootLeftFrameStart;
                }
                else
                {
                    frameIndex += BigLaserShootRightFrameStart - 1;
                }
                NPC.frame.Y = frameIndex * frameHeight;
            }
            if (attacktype == AttackType.TeleportFrenzy)
            {
                NPC.dontTakeDamage = true;
            }
            if (animation == Animation.Defeat)
            {
                NPC.frame.Y = frameHeight * 3;
            }
            NPC.damage = NPC.dontTakeDamage || attacktype != AttackType.DashFromBelow ? 0 : NPC.defDamage;
        }
    }
}