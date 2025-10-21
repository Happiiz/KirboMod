using KirboMod.Projectiles.Marx.GiantBlackHoleOfDoom;
using KirboMod.Projectiles.Marx.IceBomb;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.NPCs.Marx
{
    [AutoloadBossHead]
    public partial class MarxBoss : ModNPC //Nightmare Wizard used as a base
    {
        private Animation animation = Animation.Intro;
        private AttackType attacktype = AttackType.Intro;
        private AttackType lastattacktype = AttackType.DecideNext;

        private int phase = 1; //decides what kind of attack cycle

        static int DeathAnimDuration => 360;

        public static int IntroDuration => 60;
        ref float AttackTimer { get => ref NPC.ai[0]; }

        ref float DeathCounter { get => ref NPC.ai[1]; }
        public static int TeleportFrameDuration => 3;

        public static int BlackHoleDamage => 80 / 2;

        public static SoundStyle SplitSFX => new("KirboMod/Sounds/NPC/Marx/BlackholeSnap");
        public static SoundStyle TpSFX => new("KirboMod/Sounds/NPC/Marx/Teleport");
        public override void AI() //constantly cycles each time
        {
            NPC.TargetClosest(false);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction; //face whatever direction

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
            if (AttackTimer > HorizontalSplitStart + 3 + MarxBlackHole.SuckDuration + MarxBlackHole.ScaleUpDuration)
            {
                EndState(AttackType.DashFromBelow, Animation.Rise);
            }

        }
        /*void AttackDecideNext()
        {
            List<AttackType> possibleAttacks = new() { };

            possibleAttacks.Remove(lastattacktype);

            attacktype = possibleAttacks[Main.rand.Next(possibleAttacks.Count)];
            lastattacktype = attacktype;
            NPC.netUpdate = true;
        }*/


        private void AttackCycle()
        {
            Player player = Main.player[NPC.target];

            switch (attacktype)
            {
                case AttackType.Teleport:
                    State_Teleport();
                    break;
                case AttackType.Cutter:
                    State_Cutter();
                    break;
                case AttackType.Vine:
                    DecideNextState();
                    break;
                case AttackType.IceBomb:
                    State_IceBomb();
                    break;
                case AttackType.MassiveLaser:
                    DecideNextState();
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
        }
        static int CutterMoveDuration => 60;
        static int CutterChargeDuration => 40;
        static int CutterRounds => 2;
        static int CutterExtraWaitAfterRound => 20;
        static int CutterExtraWaitAfterAllRounds => 100;
        static int CutterRoundDuration => CutterMoveDuration + CutterChargeDuration + CutterExtraWaitAfterRound;
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
            if (relativeTimer <= moveDuration)
            {
                MoveAbovePlayer_LerpDecayVel();
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
        static int IceBombMoveDuration => Main.expertMode ? 40 : 80;
        static int IceBombMaxHold => 120;
        static int IceBombSpitDuration => 15;
        static int IceBombExtraWait => 30;
        static float IceBombAimAheadAmount => 7f;
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
                    MarxIceBomb.SpawnBombsForEveryPlayerAndPlaySFX(NPC, 100);
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
            NPC.damage = 0;
            int tpRate = TeleportFrameDuration * (TeleportFrameEnd - TeleportFrameStart + 4);
            int tpCount = 10;
            int tpIndex = (int)(AttackTimer / tpRate);
            if (AttackTimer % tpRate == 1)
            {
                ChangeAnimation(Animation.TeleportOut);
            }
            if (AttackTimer % tpRate == 4)
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
                    NPC.netUpdate = true;
                }
                NPC.Center = tpPos;
            }
            if (AttackTimer > tpCount * tpRate)
            {
                EndState(AttackType.BlackHole, Animation.Idle);
            }
        }

        private void DecideNextState()
        {
            //DONT SET LASTATTACKTYPE HERE BECAUSE IF YOU DO IT WILL ALWAYS END UP AS DECIDENEXT
            List<AttackType> attacks = new()
            //tp frenzy leads to black hole then dash from below, so don't include any of those in the list
            //massive laser also leads to dash from below
            { AttackType.Cutter, AttackType.IceBomb, AttackType.TeleportFrenzy  };
            //effectively removing the black hole attack from the options 1/3 of the time
            if (Main.rand.NextBool(3))
            {
                attacks.Remove(AttackType.TeleportFrenzy);
            }
            //don't do big laser too often
            //big laser also leads to dash from below
            if (Main.rand.NextBool(3))
            {
                attacks.Remove(AttackType.MassiveLaser);
            }
            attacks.Remove(lastattacktype);
            lastattacktype = attacktype;
            attacktype = Main.rand.NextFromCollection(attacks);
            attacktype = AttackType.TeleportFrenzy;
            // attacktype = AttackType.IceBomb;//debug
            AttackTimer = 0;
        }

        private void State_Teleport()
        {
            if (AttackTimer == 1)
            {
                ChangeAnimation(Animation.TeleportOut);
            }
            if (AttackTimer > 6 * TeleportFrameDuration)
            {
                EndState();
            }
        }
        static int DashFromBelowChaseDuration => 120;
        static int DashFromBelowTelegraphDuration => Main.expertMode ? 70 : 120;
        static int DashFromBelowDashUpDuration => 20;
        static int DashFromBelowDashUpSpeed => 50;
        private void State_DashFromBelow()
        {
            if (AttackTimer == 1)
            {
                ChangeAnimation(Animation.ShadowHole);
            }
            Player plr = Main.player[NPC.target];
            float chaseDuration = DashFromBelowChaseDuration;
            float riseTelegraphDuration = DashFromBelowTelegraphDuration;
            float dashUpDuration = DashFromBelowDashUpDuration;
            float dashUpSpeed = DashFromBelowDashUpSpeed;
            if (AttackTimer < chaseDuration)
            {
                
                NPC.Center = Vector2.Lerp(NPC.Center, plr.Center, .2f);
            }
            else if (AttackTimer < chaseDuration + riseTelegraphDuration)
            {
                NPC.velocity = Vector2.Zero;
                for (int i = 0; i < 5; i++)
                {
                    Vector2 pos = Main.rand.BetterNextVector2Circular(200);
                    pos.Y *= .5f;
                    pos += SearchForShadowHolePosition();

                    Dust d= Dust.NewDustPerfect(pos, DustID.Shadowflame);
                    d.noGravity = true;
                    d.noLight = true;
                    d.noLightEmittence = true;
                }
            }
            else if (AttackTimer < chaseDuration + riseTelegraphDuration + dashUpDuration)
            {
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
        /*static void ClampLength(ref Vector2 vector, float length = 300)
        {
            length = 300;
            float vecLength = vector.Length();
            if (vecLength < length)
            {
                vector *= length / vecLength;
            }
        }*/

        /*Vector2 GetTeleportLocation(int framesAhead)
        {
            Player plr = Main.player[NPC.target];
            Vector2 result = new Vector2(0, -100) + plr.velocity * framesAhead;
            ClampLength(ref result);
            return result;
        }*/

        /*public override bool CheckDead()
        {
            if (DeathCounter < 360)
            {
                NPC.active = true;
                NPC.life = 1;
                DeathCounter += 1; //go up
                return false;
            }
            return true;
        }*/

        /*private void DoDeathAnimation()
        {
            attackTimer = 0; //don't attack
            NPC.dontTakeDamage = true;
            NPC.damage = 0;
            NPC.rotation = 0;

            DeathCounter++; //go up

            if (DeathCounter < 120)
            {
                animation = 9;
            }
            else if (DeathCounter < 240)
            {
                animation = 10;
            }
            else if (DeathCounter < 360)
            {
                animation = 11;
            }
            else
            {
                NPC.HideStrikeDamage = true;
                NPC.SimpleStrikeNPC(999999, 1, false, 0, null, false, 0, false);
            }
        }*/
        void ChangeAnimation(Animation newAnimation)
        {
            //set to -1 instead of 0 because timer increments before it is read
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
                NPC.frameCounter++;
                NPC.dontTakeDamage = true;
            }

            if (animation == Animation.TeleportOut)
            {
                if(NPC.frameCounter == -1)
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

            if (attacktype == AttackType.TeleportFrenzy)
            {
                NPC.dontTakeDamage = true;
            }
            NPC.damage = (NPC.dontTakeDamage) ? 0 : NPC.defDamage;
        }
    }
}