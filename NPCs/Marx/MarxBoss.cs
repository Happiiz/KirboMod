using KirboMod.Projectiles.NightmareLightningOrb;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.NPCs.Marx
{
    //[AutoloadBossHead]
    public partial class MarxBoss : ModNPC //Nightmare Wizard used as a base
    {
        private Animation animation = Animation.TeleportIn;
        private AttackType attacktype = AttackType.Teleport;
        private AttackType lastattacktype = AttackType.None;

        private int phase = 1; //decides what kind of attack cycle

        static int DeathAnimDuration => 360;

        ref float attackTimer { get => ref NPC.ai[0]; }

        ref float deathCounter { get => ref NPC.ai[1]; }

        public override void AI() //constantly cycles each time
        {
            NPC.TargetClosest(false);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction; //face whatever direction

            //Despawn

            if (deathCounter > 0)
            {
                //DoDeathAnimation();
            }
            else if (attackTimer < 60) //intro
            {
                attackTimer++;
            }
            else if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active || Main.dayTime) //Despawn
            {

            }
            else //regular attack
            {
                //AttackCycle();
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


        /*private void AttackCycle()
        {
            Player player = Main.player[NPC.target];

            attackTimer++;
        }*/

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

        public override void FindFrame(int frameHeight) // animation
        {
            if (animation == Animation.Idle)
            {
                NPC.frameCounter++;
                NPC.dontTakeDamage = false;

                if (NPC.frameCounter >= 10)
                {
                    NPC.frame.Y += frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 7)
                {
                    NPC.frame.Y = 0;
                }
            }

            if (animation == Animation.TeleportOut)
            {
                if (NPC.frame.Y < frameHeight * 21)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y = frameHeight * 21;
                }

                NPC.frameCounter++;
                NPC.dontTakeDamage = true;

                if (NPC.frameCounter < 5)
                {
                    NPC.frame.Y = frameHeight * 21;
                }
                else if (NPC.frameCounter < 10)
                {
                    NPC.frame.Y = frameHeight * 22;
                }
                else if (NPC.frameCounter < 15)
                {
                    NPC.frame.Y = frameHeight * 23;
                }
                else //automatically set to teleport in
                {
                    animation = Animation.TeleportIn;
                }
            }

            if (animation == Animation.TeleportIn)
            {
                if (NPC.frame.Y < frameHeight * 21)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y = frameHeight * 23;
                }

                NPC.frameCounter++;
                NPC.dontTakeDamage = true;

                if (NPC.frameCounter < 5)
                {
                    NPC.frame.Y = frameHeight * 23;
                }
                else if (NPC.frameCounter < 10)
                {
                    NPC.frame.Y = frameHeight * 22;
                }
                else if (NPC.frameCounter < 15)
                {
                    NPC.frame.Y = frameHeight * 21;
                }
                else //automatically set to idle
                {
                    NPC.frame.Y = 0;
                    animation = Animation.Idle;
                }
            }
        }
    }
}