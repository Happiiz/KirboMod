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
        static int DashFromBelowTelegraphDuration => Main.expertMode ? 30 : 100;
        static int DashFromBelowDashUpDuration => 20;
        static int DashFromBelowDashUpSpeed => 50;
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


        static int DeathAnimDuration => 360;
        public static int IntroDuration => 60;
        ref float AttackTimer { get => ref NPC.ai[0]; }
        ref float DeathCounter { get => ref NPC.ai[1]; }
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
                //DoDeathAnimation();
            }
            else if (attacktype == AttackType.Intro && AttackTimer <= IntroDuration + 1) //intro
            {
                if (AttackTimer > IntroDuration)
                {
                    EndState();
                }
            }
            else if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active) //Despawn
            {

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
                    EndState();
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
                    IncreasingStrengthShake.Add(MassiveLaserChargeupTime, 4, 2f);
                    SoundEngine.PlaySound(MassiveLaserCharge.WithPitchOffset(0f), NPC.Center);
                }
            }
            else if (relativeTimer < MassiveLaserChargeupTime + MassiveLaserDuration)
            {
                NPC.velocity = new Vector2(NPC.direction * MassiveLaserRecoilSpeed, 0);
                if (relativeTimer == MassiveLaserChargeupTime)
                {
                    ChangeAnimation(Animation.Blast);
                    DecreasingStrengthShake.Add(MassiveLaserDuration, 10);
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
        //thank you chatgpt for this code
        private void DecideNextState()
        {
            // Define canonical comparison
            //Name comes from math:
            //"relating to a general rule or standard formula."
            AttackType GetCanonicalType(AttackType type) => type;
            // type == AttackType.DashFromBelow ? AttackType.TeleportFrenzy : type;

            // Fetch durations for each attack
            GetAttackDurations(out float cutterDuration, out float iceBombDuration, out float blackHoleDuration, out float laserDuration, out float vineDuration, out _, out float dashFromBelowDuration);

            Dictionary<AttackType, float> durations = new()
            {
                { AttackType.Cutter, cutterDuration },
                { AttackType.TeleportFrenzy, blackHoleDuration },
                { AttackType.MassiveLaser, laserDuration },
                { AttackType.IceBomb, iceBombDuration },
                { AttackType.Vine, vineDuration },
                { AttackType.DashFromBelow, dashFromBelowDuration },
            };
            //TODO: FIX DASH FROM BELOW??? LIKE FINISH MAKING IT A UNIQUE ATTACK
            // Exclude the last attack, using canonical type to treat some as equivalent
            AttackType lastCanonical = GetCanonicalType(lastattacktype);

            List<KeyValuePair<AttackType, float>> candidates = durations
                .Where(kvp => GetCanonicalType(kvp.Key) != lastCanonical)
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
                    NPC.Center = Vector2.Lerp(NPC.Center, plr.Center, .05f);
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
            Vector2 targetVel = deltaPos / dist;
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
            NPC.damage = (NPC.dontTakeDamage) ? 0 : NPC.defDamage;
        }
    }
}