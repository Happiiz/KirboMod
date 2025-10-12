using KirboMod.Projectiles.Marx.GiantBlackHoleOfDoom;
using Microsoft.Xna.Framework;
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
        public static int TeleportFrameDuration => 5;

        public static int BlackHoleDamage => 80 / 2;

        public static SoundStyle SplitSFX => new("KirboMod/Sounds/NPC/Marx/BlackholeSnap");
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
                if(AttackTimer > IntroDuration)
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
                    break;
                case AttackType.Vine:
                    break;
                case AttackType.IceBomb:
                    break;
                case AttackType.MassiveLaser:
                    break;
                case AttackType.BlackHole:
                    State_BlackHole();
                    break;
                case AttackType.Intro:
                    break;
                case AttackType.DashFromBelow:
                    State_DashFromBelow();
                    break;
                default:
                    break;
            }
            if (attacktype == AttackType.DecideNext)
            {
                DecideNextState();
            }
        }

        private void DecideNextState()
        {
            //DONT SET LASTATTACKTYPE HERE BECAUSE IF YOU DO IT WILL ALWAYS END UP AS DECIDENEXT
            attacktype = AttackType.BlackHole;//debug
        }

        private void State_Teleport()
        {
            if(AttackTimer == 1)
            {
                ChangeAnimation(Animation.TeleportOut);
            }
            if (AttackTimer > 6 * TeleportFrameDuration)
            {
                EndState();
            }
        }

        private void State_DashFromBelow()
        {
            if(AttackTimer == 1)
            {
                ChangeAnimation(Animation.Rise);
            }
            Player plr = Main.player[NPC.target];
            float chaseDuration = 120;
            float riseTelegraphDuration = Main.expertMode ? 30 : 70;
            float dashUpDuration = 40;
            float dashUpSpeed = 50;
            if (AttackTimer < chaseDuration)
            {
                Vector2 dashFromBelowChaseOffset = new(0, 500);
                Vector2 targetOffset = Main.expertMode ? new Vector2(plr.velocity.X * riseTelegraphDuration, 0) : Vector2.Zero;
                NPC.Center = Vector2.Lerp(NPC.Center, plr.Center + dashFromBelowChaseOffset + targetOffset, .2f);
            }
            else if (AttackTimer < chaseDuration + riseTelegraphDuration)
            {
                NPC.velocity = Vector2.Zero;
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Shadowflame);
            }
            else if (AttackTimer < chaseDuration + riseTelegraphDuration + dashUpDuration)
            {
                NPC.velocity.Y = -dashUpSpeed;
                NPC.velocity.X = 0;
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
            if (animation == Animation.TeleportOut)
            {
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

            if (animation == Animation.Idle)
            {
                NPC.frameCounter++;
                NPC.dontTakeDamage = NPC.IsABestiaryIconDummy;
                int frameIndex = (int)NPC.frameCounter / IdleFrameDuration;
                frameIndex %= (IdleFrameEnd + 1);
                frameIndex += IdleFrameStart;
                NPC.frame.Y = frameHeight * frameIndex;
            }

            if(animation == Animation.Rise)
            {
                NPC.dontTakeDamage = false;
                NPC.frameCounter++;
                int frameIndex = (int)NPC.frameCounter / RiseFrameDuration;
                int riseFrames = RiseFrameEnd - RiseFrameStart;
                frameIndex %= (riseFrames + 1);
                frameIndex += RiseFrameStart;
                NPC.frame.Y = frameHeight * frameIndex;
            }
            NPC.damage = NPC.dontTakeDamage ? 0 : NPC.defDamage;
        }
    }
}