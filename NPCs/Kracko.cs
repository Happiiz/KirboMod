using KirboMod.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
    public partial class Kracko : ModNPC
    {

        private int animation = 0;

        private KrackoAttackType attacktype = KrackoAttackType.SpinningBeamOrbs; //decides the attack
        private int doodelay = 0;
        private float sweepheight = 0;
        private float sweepX = 0;
        private float sweepY = 0;
        float beamCurvingAngleMultiplier = 0.05f;
        int numberOfBeamsPerSpiral = 70;
        private sbyte attackDirection = 1;
        private KrackoAttackType lastattacktype = KrackoAttackType.Sweep; //sets last attack type
        private bool transitioning = false; //checks if going through expert mode exclusive phase
        private bool frenzy = false; //checks if going in frenzy mode in expert mode


        public override void AI() //constantly cycles each time
        {
            Player player = Main.player[NPC.target];

            if (NPC.ai[1] <= 60 || (NPC.ai[0] >= 60 && NPC.ai[0] < 90 && frenzy == true)) //be harmless upon spawn (or when moving during frenzy
            {
                NPC.damage = 0;
            }
            else
            {
                if (Main.expertMode == false)
                {
                    NPC.damage = 30;
                }
                else
                {
                    NPC.damage = 60;
                }
            }
            NPC.ai[1]++;

            if (NPC.life <= NPC.lifeMax * 0.25 && Main.expertMode == true) //transition
            {
                transitioning = true;
            }
            //DESPAWNING
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
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
            else if (transitioning == true && (NPC.ai[2] > 180) == false) //TRANSITION TO PHASE 2
            {
                NPC.ai[2]++;
                NPC.velocity *= 0.0001f; //stop

                if (NPC.ai[2] < 180)
                {
                    NPC.rotation += MathHelper.ToRadians(4f); //rotate (in degrees)

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
            Vector2 distance = player.Center - NPC.Center; //start - end
            Vector2 distanceOverPlayer = player.Center + new Vector2(0, -200) - NPC.Center;
            Vector2 distanceBelowPlayer = player.Center + new Vector2(0, 200) - NPC.Center;

            Vector2 distanceDiagonalUpOfPlayer = player.Center + new Vector2(400 * attackDirection, -400) - NPC.Center;

            Vector2 distanceLeftOfPlayer = player.Center + new Vector2(-400, 0) - NPC.Center;
            Vector2 distanceRightOfPlayer = player.Center + new Vector2(400, 0) - NPC.Center;

            NPC.ai[0]++;

            if (NPC.ai[0] == (!frenzy ? 15 : 60)) //changes depending or not in frenzy
            {
                if (doodelay == 1) //summon (multiplayer syncing stuff is because spawning npc)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int index = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, Mod.Find<ModNPC>("WaddleDoo").Type, 0, 0, 1, 0, 0, NPC.target);

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
            float attackEnd = 0;
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
            if (NPC.ai[0] >= attackEnd) //end          
                ResetVarsForNextAttack();
        }

        void AttackDecideNext()
        {
            //10 attack delay during frenzy, 70 during frenzy
            if (NPC.ai[0] >= (!frenzy ? 70 : 10)) //choose attack randomly(faster is frenzying)
            {
                attackDirection = (sbyte)(Main.rand.NextBool() ? 1 : -1); //right or left
                List<KrackoAttackType> possibleAttacks = new() { KrackoAttackType.SpinningBeamOrbs, KrackoAttackType.Sweep, KrackoAttackType.Dash };
                if (NPC.GetLifePercent() < (Main.expertMode ? 0.75f : 0.4f))
                    possibleAttacks.Add(KrackoAttackType.Lightning);
                possibleAttacks.Remove(lastattacktype);
                possibleAttacks.TrimExcess();
                attacktype = possibleAttacks[Main.rand.Next(possibleAttacks.Count)];
                lastattacktype = attacktype;
                NPC.ai[0] = 0;
                NPC.netUpdate = true;
            }
        }

        private float AttackSweep(Player player)
        {
            float speed = 20f;
            float inertia = 5f;
            float moveStart = 40;
            float moveEnd = 180;
            float sweepStart = 180;
            float sweepEnd = 280;
            float sweepDepth = 14;
            float sweepHorizontalSpeed = 14f;
            float attackEnd = 290;
            float sweepProgress = Utils.GetLerpValue(sweepStart, sweepEnd, NPC.ai[0]);
            if (NPC.ai[0] >= moveStart && NPC.ai[0] < moveEnd) //go to top left or right
            {
                if (NPC.ai[0] >= moveEnd - 1) //stop
                {
                    NPC.velocity *= 0.0001f;
                }
                else //move
                {
                    Vector2 targetPos = player.Center + new Vector2(650 * attackDirection, -400);
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
                Vector2 vel = new(-attackDirection, MathF.Cos(sweepProgress * MathF.PI));
                vel.Y *= sweepDepth;
                vel.X *= sweepHorizontalSpeed;
                float easingThing = Utils.GetLerpValue(sweepStart, sweepStart + 10, NPC.ai[0], true) * Utils.GetLerpValue(sweepEnd, sweepEnd - 10, NPC.ai[0], true);
                vel *= easingThing;
                NPC.Center += vel;
            }

            return attackEnd;
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
                if (NPC.ai[0] == beamStart || (NPC.ai[0] - beamStart) % 34 == 0)// 34 is sound length
                {
                    SoundEngine.PlaySound(SoundID.Item93 with { MaxInstances = 0 }, NPC.Center);
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
            float dashEnd = 200f;
            float dashDuration = dashEnd - dashStart;
            if (frenzy)
            {
                dashDuration = MathF.Round(dashDuration * 0.75f);
                dashEnd = dashStart + dashDuration;
            }
            float dashDistanceY = 900;
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
                NPC.velocity = new Vector2(dashDistanceX / 2 * attackDirection, dashDistanceY) * easingMultiplier;
            }
            else if (dashProgress < 0.66f)
            {
                easingMultiplier = Utils.GetLerpValue(0, 0.5f, (dashProgress - 0.33f) * 3, true) * Utils.GetLerpValue(1, 0.5f, (dashProgress - 0.33f) * 3, true);
                NPC.velocity = new Vector2(dashDistanceX * -attackDirection, 0) * easingMultiplier;
            }
            else if (dashProgress < 1)
            {
                easingMultiplier = Utils.GetLerpValue(0, 0.5f, (dashProgress - 0.66f) * 3, true) * Utils.GetLerpValue(1, 0.5f, (dashProgress - 0.66f) * 3, true);
                NPC.velocity = new Vector2(dashDistanceX / 2 * attackDirection, -dashDistanceY) * easingMultiplier;
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
            float speed = 15f;
            float inertia = 5f;
            float thunderSlideSpeed = 10;
            float moveStart = 0;
            float moveEnd = 100;
            float attackStart = 100;
            float attackEnd = 200;
            float attackDuration = attackEnd - attackStart;
            if (frenzy)
            {
                attackStart -= 40;
                moveEnd -= 40;
                thunderSlideSpeed = 10;
                speed *= 2;
                attackDuration = MathF.Round(attackDuration * 0.6f);
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
                    Vector2 targetPos = Main.player[NPC.target].Center + new Vector2(400 * attackDirection, -400);
                    NPC.velocity = (NPC.velocity * (inertia - 1) + GetDistanceDiagonalUpOfPlayerNormalizedMulipliedBySpeed(speed)) / inertia; //go above left of player
                    if (NPC.DistanceSQ(targetPos) < 20)//cancel movement early if reached target pos
                    {
                        NPC.ai[0] = moveEnd;
                    }
                }

            }
            if (NPC.ai[0] >= attackStart) //for 120 ticks dash across screen(if on it)
            {
                NPC.velocity.X = -attackDirection * thunderSlideSpeed * Utils.GetLerpValue(attackStart - 1, attackStart + 9, NPC.ai[0], true) * Utils.GetLerpValue(attackEnd + 1, attackEnd - 12, NPC.ai[0], true);
                if (NPC.ai[0] % 4 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {//												offset down a bit so it lookslike it's coming out below kracko
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 100, 0, 5.5f, ModContent.ProjectileType<KrackoLightning>(), 25 / 2, 2f, Main.myPlayer, 0, 0);
                    SoundEngine.PlaySound(SoundID.Thunder, NPC.Center);
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
                        Projectile spawnedProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BeamBig>(), 20 / 2, 8f, Main.myPlayer, NPC.whoAmI, j * (MathF.Tau / numberOfSpirals) - i * beamCurvingAngleMultiplier, i * 70 + 100);
                        spawnedProj.direction = Main.rand.NextBool() ? 1 : -1;
                    }
                }
            }
        }

        Vector2 GetDistanceDiagonalUpOfPlayerNormalizedMulipliedBySpeed(float speed)
        {
            Vector2 distanceDiagonalRightOfPlayer = Main.player[NPC.target].Center + new Vector2(400 * attackDirection, -400) - NPC.Center;
            distanceDiagonalRightOfPlayer.Normalize();
            return distanceDiagonalRightOfPlayer * speed;
        }
        // This npc uses an additional texture for drawing
        public static Asset<Texture2D> EyeTexture;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            EyeTexture = ModContent.Request<Texture2D>("KirboMod/NPCs/KrackoEyeBase");
            Texture2D pupil = ModContent.Request<Texture2D>("KirboMod/NPCs/KrackoEyePupil").Value;
            Texture2D eyelid = ModContent.Request<Texture2D>("KirboMod/NPCs/KrackoEyeAngryEyelid").Value;
            Player player = Main.player[NPC.target];
            bool drawEyelid = (transitioning && NPC.ai[2] > 0 && NPC.ai[2] <= 180) || frenzy;
            float offsetLength = 6.5f;
            Vector2 pupilOffset = Vector2.Normalize(player.Center - NPC.Center) * offsetLength;//the multipier is just what looks good
            if ((transitioning == true && NPC.ai[2] > 0 && NPC.ai[2] <= 180))
            {
                pupilOffset = Vector2.Zero;
            }
            Texture2D eye = EyeTexture.Value;
            spriteBatch.Draw(eye, NPC.Center - Main.screenPosition, null, new Color(255, 255, 255), 0, new Vector2(29, 29), 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(pupil, NPC.Center - Main.screenPosition + pupilOffset, null, Color.White, 0, pupil.Size() / 2, 1, SpriteEffects.None, 0);
            if (drawEyelid)
                spriteBatch.Draw(eyelid, NPC.Center - Main.screenPosition + new Vector2(0, -4), null, Color.White, 0, eyelid.Size() / 2, 1, SpriteEffects.None, 0);
        }
       
    }
}
