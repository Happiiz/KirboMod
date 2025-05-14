using KirboMod.Projectiles;
using KirboMod.Projectiles.Lightnings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
    public partial class Kracko : ModNPC
    {
        private int animation = 0;
        private KrackoAttackType attacktype = KrackoAttackType.DecideNext; //decides the attack
        private int doodelay = 0;
        const float beamCurvingAngleMultiplier = 0.05f;
        const int numberOfBeamsPerSpiral = 30;
        bool attackDirection = false;
        private int AttackDirection { get => attackDirection ? 1 : -1; set => attackDirection = value <= 1; }
        private KrackoAttackType nextAttackType = KrackoAttackType.Dash; //sets last attack type
        private bool transitioning = false; //checks if going through expert mode exclusive phase
        private bool frenzy = false; //checks if going in frenzy mode in expert mode


        public override void AI() //constantly cycles each time
        {
            if (NPC.ai[0] >= 60 && NPC.ai[0] < 90 && frenzy) //be harmless upon spawn (or when moving during frenzy)
            {
                NPC.damage = 0;
            }
            else
            {
                NPC.damage = NPC.defDamage;
            }

            NPC.ai[1]++; //keeps track of delay before attacking

            if (NPC.GetLifePercent() <= 0.5f && Main.expertMode) //transition
            {
                transitioning = true;
            }
            //DESPAWNING
            if (NPC.target < 0 || NPC.target >= 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest(false);

                NPC.velocity.Y = NPC.velocity.Y - 0.2f;
                NPC.ai[0] = 0;

                if (NPC.timeLeft > 60)
                {
                    NPC.timeLeft = 60;
                    return;
                }
            }
            else if (NPC.ai[1] <= 60) //attack delay
            {
                NPC.damage = 0;
                NPC.velocity *= 0.01f;
            }
            else if (transitioning == true && NPC.ai[2] <= 180) //TRANSITION TO PHASE 2
            {
                NPC.ai[2]++;
                NPC.velocity *= 0.0001f; //stop
                NPC.dontTakeDamage = true;

                if (NPC.ai[2] < 180)
                {
                    float transitionProgress = Utils.GetLerpValue(0, 180, NPC.ai[2]);
                    NPC.rotation = Easings.EaseInOutSine(transitionProgress) * 6 * MathF.Tau; //rotate (in degrees)

                    if (NPC.ai[2] % 5 == 0) //every multiple of 5
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(20f, 20f); //circle
                            Dust d = Dust.NewDustPerfect(NPC.Center + speed, DustID.Cloud, speed, Scale: 2f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }
                    }
                }
                else
                {
                    NPC.rotation = 0;
                    NPC.dontTakeDamage = false;
                    transitioning = false;
                    frenzy = true;
                }

                NPC.TargetClosest(true);
                NPC.ai[0] = 0; //no attack pattern cycle
            }
            else //regular attack
            {
                NPC.TargetClosest(true);
                AttackPattern();
            }
        }

        private void AttackPattern()
        {
            Player player = Main.player[NPC.target];
            NPC.ai[0]++;
            float attackEnd = float.MaxValue;
            switch (attacktype)
            {
                case KrackoAttackType.DecideNext:
                    AttackDecideNext();
                    break;
                case KrackoAttackType.SpinningBeamOrbs:
                    attackEnd = AttackBeam();
                    break;
                case KrackoAttackType.Sweep:
                    attackEnd = AttackSweep(player);
                    break;
                case KrackoAttackType.Dash:
                    attackEnd = AttackDash();
                    break;
                case KrackoAttackType.Lightning:
                    attackEnd = AttackLightning();
                    break;
            }
            AttackSpawnDoo();

            if (NPC.ai[0] >= attackEnd) //end          
                ResetVarsForNextAttack();
        }
        private void AttackSpawnDoo()
        {
            if (NPC.ai[0] == (!frenzy ? 15 : 60)) //changes depending or not in frenzy
            {
                int dooThreshold = 3;
                int maxDoos = 2;// won't summon any more if there are more than this alive
                if (Main.getGoodWorld)
                {
                    maxDoos = int.MaxValue;//infinite!!!
                }
                else if (Main.expertMode)
                {
                    maxDoos = 4;
                }
                int curAmountOfDoos = 0;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if(npc.active && npc.ModNPC is WaddleDoo)
                    {
                        WaddleDoo doo = (WaddleDoo)npc.ModNPC;
                        if (doo.SpawnedFromKracko)
                        {
                            curAmountOfDoos++;
                        }
                    }
                }
                if (doodelay > dooThreshold && curAmountOfDoos < maxDoos && attacktype != KrackoAttackType.SpinningBeamOrbs) //summon (multiplayer syncing stuff is because spawning npc)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int index = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WaddleDoo>(), 0, 0, 1, 0, 0, NPC.target);

                        if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: index);
                        }
                    }
                    doodelay = 0; //reset
                }
                else
                {
                    doodelay += 1; //next turn will summon 
                }
            }
        }
        void AttackDecideNext()
        {
            //choose attack randomly
            if (NPC.ai[0] == 1)
            {
                attacktype = nextAttackType;
                attackDirection = Main.rand.NextBool(); //right or left (true for right, false for left)
                List<KrackoAttackType> possibleAttacks = new() { KrackoAttackType.SpinningBeamOrbs, KrackoAttackType.Sweep, KrackoAttackType.Dash };
                if (NPC.GetLifePercent() < (Main.expertMode ? 0.75f : 0.5f))
                {
                    possibleAttacks.Add(KrackoAttackType.Lightning);
                }
                possibleAttacks.Remove(attacktype);
                nextAttackType = possibleAttacks[Main.rand.Next(possibleAttacks.Count)];
                NPC.netUpdate = true;
            }
            //10 attack delay during frenzy, 70 during frenzy
            if (NPC.ai[0] >= (!frenzy ? 70 : 10)) 
            {
                NPC.ai[0] = 0;
            }
        }
        private float AttackSweep(Player player)
        {
            float speed = 20f;
            float inertia = 5f;
            float moveStart = 40;
            float moveEnd = 180;
            float sweepStart = 180;
            float sweepEnd = 270;
            float sweepY = 1700;
            float sweepX = 1700;
            float sweepDuration = sweepEnd - sweepStart;

            if (Main.expertMode)
            {
                sweepDuration *= 0.85f;
                speed *= 1.15f;
                inertia *= 1.15f;
                if (frenzy)
                {
                    speed *= 1.15f;
                    inertia *= 1.15f;
                    sweepDuration *= 0.85f;
                }
                sweepEnd = sweepStart + sweepDuration;
            }
            sweepY /= sweepDuration;
            sweepX /= sweepDuration;
                                                                                                      //compensate for it losing overall distance travelled by accelerating and decelerating
            Vector2 targetPos = player.Center + new Vector2(sweepX * sweepDuration * 0.5f * AttackDirection - (AttackDirection * sweepX * 5), -400);
            float sweepProgress = Utils.GetLerpValue(sweepStart, sweepEnd, NPC.ai[0]);
            if (NPC.ai[0] >= moveStart && NPC.ai[0] < moveEnd) //go to top left or right
            {
                if (NPC.ai[0] >= moveEnd - 1) //stop
                {
                    NPC.velocity *= 0.0001f;
                }
                else //move
                {
                    Vector2 distanceDiagonalOfPlayer = targetPos - NPC.Center;
                    distanceDiagonalOfPlayer.Normalize();
                    distanceDiagonalOfPlayer *= speed;
                    NPC.velocity = (NPC.velocity * (inertia - 1) + distanceDiagonalOfPlayer) / inertia;
                    if (NPC.DistanceSQ(targetPos) < 20 && NPC.ai[0] < sweepStart - 20)
                    {
                        NPC.ai[0] = sweepStart - 20;//if has reached the target position, start the sweep up to 20 frames earlier than usual
                        NPC.velocity *= 0.0001f;
                    }
                }
            }
            if (sweepProgress >= 0 && sweepProgress <= 1)
            {
                Vector2 vel = new(-AttackDirection, MathF.Cos(sweepProgress * MathF.PI));
                vel.Y *= sweepY;
                vel.X *= sweepX;
                float easingThing = Utils.GetLerpValue(sweepStart, sweepStart + 10, NPC.ai[0], true) * Utils.GetLerpValue(sweepEnd, sweepEnd - 10, NPC.ai[0], true);
                vel *= easingThing;
                NPC.Center += vel;
            }

            return sweepEnd;
        }
        float AttackBeam()
        {
            Vector2 distanceOverPlayer = Main.player[NPC.target].Center + new Vector2(0, -200) - NPC.Center;
            float speed = 15f;
            float inertia = 5f;
            float moveStart = 0;
            float moveStop = 70;
            float beamStart = 80;
            float beamEnd = 380;
            if (frenzy)
            {
                speed = 30;
                inertia = 3;
                moveStart = 0;
                moveStop = 50;
                beamStart = 50;
                beamEnd = 350;
            }
            if (NPC.ai[0] >= moveStart && NPC.ai[0] < moveStop)
            {
                if (NPC.ai[0] >= moveStop - 1) //stop
                {
                    NPC.velocity *= 0.0001f;
                }
                else //move
                {
                    distanceOverPlayer.Normalize();
                    distanceOverPlayer *= speed;
                    NPC.velocity = (NPC.velocity * (inertia - 1) + distanceOverPlayer) / inertia; //go above player
                }
            }
            if (NPC.ai[0] >= beamStart && NPC.ai[0] < beamEnd) //120 ticks 
            {
                SpawnBeam(numberOfBeamsPerSpiral, beamStart);
                if (NPC.ai[0] == beamStart/* || (NPC.ai[0] - beamStart) % 34 == 0*/)// 34 is sound length
                {
                    SoundEngine.PlaySound(ElecOrbsSFX with { Volume = .4f}, NPC.Center);
                    //SoundEngine.PlaySound(SoundID.Item93 with { MaxInstances = 0 }, NPC.Center);
                }
            }
            return beamEnd;
        }

        private void ResetVarsForNextAttack()
        {
            attacktype = KrackoAttackType.DecideNext;
            NPC.velocity = Vector2.Zero;
            NPC.ai[0] = 0;
            NPC.netUpdate = true;
        }
        float AttackDash()
        {
            Vector2 targetPos = Main.player[NPC.target].Center + new Vector2(0, -200);
            Vector2 distanceOnTopOfPlayer = targetPos - NPC.Center;
            float speed = frenzy ? 30 : 20f;
            float inertia = 5f;
            float moveStart = 0f;
            float moveEnd = 100;
            float dashStart = 100;
            float dashEnd = 280;
            float dashDuration = dashEnd - dashStart;
            if (frenzy)
            {
                dashDuration = MathF.Round(dashDuration * 0.85f);
                dashEnd = dashStart + dashDuration;
            }
            float dashDistanceY = 1400;
            float dashDistanceX = 9000;
            dashDistanceX /= dashDuration;
            dashDistanceY /= dashDuration;
            float dashProgress = Utils.GetLerpValue(dashStart, dashEnd, NPC.ai[0]);
            if (NPC.ai[0] >= moveStart && NPC.ai[0] < moveEnd) //go to right
            {
                if (NPC.ai[0] >= moveEnd - 1) //stop
                {
                    NPC.velocity *= 0.0001f;
                }
                else //move
                {
                    distanceOnTopOfPlayer.Normalize();
                    distanceOnTopOfPlayer *= speed;
                    NPC.velocity = (NPC.velocity * (inertia - 1) + distanceOnTopOfPlayer) / inertia;
                }
                if (NPC.Center.DistanceSQ(targetPos) < 20)
                {
                    NPC.velocity *= 0.0001f;//cancel the movement early if reaches top of player
                    NPC.ai[0] = moveEnd;
                }
            }
            if (dashProgress < 0)
                return dashEnd;
            float easingMultiplier;
            if (dashProgress < 0.33f)
            {
                easingMultiplier = Utils.GetLerpValue(0, 0.5f, dashProgress * 3, true) * Utils.GetLerpValue(1, 0.5f, dashProgress * 3, true);
                NPC.velocity = new Vector2(dashDistanceX / 2 * AttackDirection, dashDistanceY) * easingMultiplier;
            }
            else if (dashProgress < 0.66f)
            {
                easingMultiplier = Utils.GetLerpValue(0, 0.5f, (dashProgress - 0.33f) * 3, true) * Utils.GetLerpValue(1, 0.5f, (dashProgress - 0.33f) * 3, true);
                NPC.velocity = new Vector2(dashDistanceX * -AttackDirection, 0) * easingMultiplier;
            }
            else if (dashProgress < 1)
            {
                easingMultiplier = Utils.GetLerpValue(0, 0.5f, (dashProgress - 0.66f) * 3, true) * Utils.GetLerpValue(1, 0.5f, (dashProgress - 0.66f) * 3, true);
                NPC.velocity = new Vector2(dashDistanceX / 2 * AttackDirection, -dashDistanceY) * easingMultiplier;
            }
            else
            {
                NPC.velocity = Vector2.Zero;
            }
            NPC.rotation = NPC.velocity.X * 0.01f;
            return dashEnd;
        }
        float AttackLightning()
        {
            float speed = 25f;
            float inertia = 5f;
            float thunderSlideSpeed = 13;
            float moveStart = 0;
            float moveEnd = 80;
            float attackStart = 80;
            float attackEnd = 180;
            float attackDuration = attackEnd - attackStart;
            float thunderYHeightOffset = -400;
            float thunderXOffset = 650;
            if (frenzy)
            {
                thunderYHeightOffset -= 60;
                thunderSlideSpeed *= 1.5f;
                thunderXOffset += 60;
                speed *= 2;
                attackDuration = MathF.Round(attackDuration * 0.65f);
                attackEnd = attackStart + attackDuration;
            }
            if (NPC.ai[0] >= moveStart && NPC.ai[0] < attackStart)
            {
                for (int i = 0; i < 6; i++) //make bursts of electricity for warning
                {
                    Vector2 dustSpeed = Main.rand.NextVector2Circular(5f, 5f); //circle
                    Dust d = Dust.NewDustPerfect(NPC.position + new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(0, NPC.height)), DustID.Electric, dustSpeed); //Makes dust in a messy circle
                    d.noGravity = true;
                }
            }
            if (NPC.ai[0] >= moveStart && NPC.ai[0] < moveEnd)
            {
                if (NPC.ai[0] >= moveEnd - 1) //stop
                {
                    NPC.velocity = Vector2.Zero;
                }
                else //move
                {
                    Vector2 targetPos = Main.player[NPC.target].Center + new Vector2(thunderXOffset * AttackDirection, thunderYHeightOffset);
                    Vector2 distanceDiagonalRightOfPlayer = targetPos - NPC.Center;
                    distanceDiagonalRightOfPlayer.Normalize();
                    distanceDiagonalRightOfPlayer *= speed;
                    NPC.velocity = (NPC.velocity * (inertia - 1) + distanceDiagonalRightOfPlayer) / inertia; //go above left of player
                    //if (NPC.DistanceSQ(targetPos) < 20)//cancel movement early if reached target pos
                    //{
                    //    NPC.ai[0] = moveEnd;
                    //}
                }

            }
            if (NPC.ai[0] >= attackStart) //for 120 ticks dash across screen(if on it)
            {
                float easingMultiplier = Utils.GetLerpValue(attackStart - 1, attackStart + 9, NPC.ai[0], true) * Utils.GetLerpValue(attackEnd + 1, attackEnd - 12, NPC.ai[0], true);
                int fireRate = frenzy ? 2 : 4;
                NPC.velocity.Y = 0;
                NPC.velocity.X = -AttackDirection * thunderSlideSpeed * easingMultiplier;
                if (NPC.ai[0] % fireRate == 0)
                {//												offset down a bit so it lookslike it's coming out below kracko
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 spawnVel = new Vector2(0, 16);
                        LightningProj.GetSpawningStats(spawnVel, out float ai0, out float ai1);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + Main.rand.Next(-20,21) + NPC.velocity.X * 2, NPC.Center.Y + 84 + Main.rand.Next(-20, 21), spawnVel.X, spawnVel.Y, ModContent.ProjectileType<KrackoLightning>(), 25 / 2, 2f, Main.myPlayer, ai0, ai1);
                    }
                    SoundStyle sound = new SoundStyle("Terraria/Sounds/Thunder_" + Main.rand.Next(4));
                    SoundEngine.PlaySound(sound with { MaxInstances = 0, Volume = .4f}, NPC.Center);

                }
            }

            return attackEnd;
        }
        void SpawnBeam(int numberOfBeams, float timeToCheck)
        {
            float numberOfSpirals = frenzy ? 4 : 2;
            numberOfSpirals += Main.getGoodWorld ? 2 : 0;//add 2 on for the worthy
            if (NPC.ai[0] == timeToCheck && Main.netMode != NetmodeID.MultiplayerClient) //inital
            {
                for (float i = 0; i < numberOfBeams; i++) //i decides the offset
                {                 
                    for (float j = 0; j < numberOfSpirals; j++)
                    {
                       Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BeamBig>(), 40 / 2, 8f, Main.myPlayer, NPC.whoAmI, j * (MathF.Tau / numberOfSpirals) - i * beamCurvingAngleMultiplier, i * 70 + 100);
                    }
                }
            }
        }
        static Asset<Texture2D> eyeBase;
        static Asset<Texture2D> pupil;
        static Asset<Texture2D> eyelid;
        static Asset<Texture2D> spikes;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float darkness = DarknessProgressForThunder;
            if (attacktype == KrackoAttackType.Lightning)
            {
                VFX.DrawGlowBallDiffuse(NPC.Center + new Vector2(0, 84), 4f, VFX.RndElectricCol * darkness, Color.White);
            }
            float scaleMult = 1f;
            Player player = Main.player[NPC.target];
            bool drawEyelid = (transitioning && NPC.ai[2] > 0 && NPC.ai[2] <= 180) || frenzy;
            float offsetLength = 6.5f;
            Vector2 pupilOffset = Vector2.Normalize(player.Center - NPC.Center) * offsetLength;//the multipier is just what looks good
         
            if ((transitioning == true && NPC.ai[2] > 0 && NPC.ai[2] <= 180))
            {
                pupilOffset = Vector2.Zero;
            }
            if (NPC.IsABestiaryIconDummy)
            {
                scaleMult = 0.6f;
                pupilOffset = (Main.MouseScreen - NPC.Center);//the multipier is just what looks good
                if(pupilOffset.Length() > offsetLength)
                {
                    pupilOffset.Normalize();
                    pupilOffset *= offsetLength;
                }
            }
            pupilOffset /= 2;
            pupilOffset = pupilOffset.Floor() * 2 + Vector2.One;
            pupilOffset *= scaleMult;
            Texture2D texture = TextureAssets.Npc[Type].Value;
            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, GetAlpha(Color.White).Value, NPC.rotation, NPC.frame.Size() / 2, scaleMult, SpriteEffects.None, 0f);
            spriteBatch.Draw(spikes.Value, NPC.Center - screenPos, NPC.frame, new Color(255, 255, 255), NPC.rotation, NPC.frame.Size() / 2, scaleMult, SpriteEffects.None, 0f);
            spriteBatch.Draw(eyeBase.Value, NPC.Center - screenPos, null, new Color(255, 255, 255), 0, eyeBase.Size() / 2, scaleMult, SpriteEffects.None, 0f);
            spriteBatch.Draw(pupil.Value, NPC.Center - screenPos + pupilOffset, null, Color.White, 0, pupil.Size() / 2, scaleMult, SpriteEffects.None, 0);
            if (drawEyelid)
                spriteBatch.Draw(eyelid.Value, NPC.Center - screenPos, null, Color.White, 0, eyelid.Size() / 2, scaleMult, SpriteEffects.None, 0);
           
            return false;
        }
        public override void Load()
        {
            eyeBase = ModContent.Request<Texture2D>("KirboMod/NPCs/KrackoEyeBase");
            pupil = ModContent.Request<Texture2D>("KirboMod/NPCs/KrackoEyePupil");
            eyelid = ModContent.Request<Texture2D>("KirboMod/NPCs/KrackoEyeAngryEyelid");
            spikes = ModContent.Request<Texture2D>("KirboMod/NPCs/KrackoSpikes");
        }
        public override void Unload()
        {
            eyeBase = null;
            pupil = null;
            eyelid = null;
            spikes = null;
        }
    }
}
