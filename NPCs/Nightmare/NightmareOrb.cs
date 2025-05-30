using KirboMod.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
    public partial class NightmareOrb : ModNPC
    {
        public override string HeadTexture => "KirboMod/NPCs/Nightmare/NightmareOrb_Head_Boss";
        public override string Texture => "KirboMod/NPCs/Nightmare/NightmareOrb";
        Vector2 GetTargetPosOffset(float changeRate = 0.05f)
        {
            int moveType = AttacksPerformedSinceSpawn / 2 % 3;
            Vector2 offset = new(MathF.Sin(NPC.ai[0] * changeRate * 2 + MathF.PI) * 250, MathF.Sin(NPC.ai[0] * changeRate) * 250);
            if (moveType == 1)
            {
                offset.X = 0;
            }
            else if (moveType == 2)
            {
                offset.Y = 0;
                offset.X += MathF.CopySign(100, NPC.Center.X - Main.player[NPC.target].Center.X);//100 is horizontal offset to player
            }
            offset.X += MathF.CopySign(500, NPC.Center.X - Main.player[NPC.target].Center.X);//500 is horizontal offset to player
            return offset;
        }
        /// <summary>
        /// these values are already divided by 2
        /// </summary>
        static Dictionary<NightmareOrbAtkType, int> dmgPerAtkType = new()
        {
            { NightmareOrbAtkType.SlashBeam, 60 / 2},
            { NightmareOrbAtkType.SingleStar, 40 / 2},
            { NightmareOrbAtkType.TripleStar, 40 / 2},
            { NightmareOrbAtkType.HomingStar, 50 / 2}
        };
        private NightmareOrbAtkType AttackType { get => (NightmareOrbAtkType)NPC.ai[2]; set => NPC.ai[2] = (int)value; }
        int AttacksPerformedSinceSpawn { get => (int)NPC.ai[1]; set => NPC.ai[1] = value; }
        public bool frenzy { get => NPC.ai[3] == 1f; set => NPC.ai[3] = value ? 1f : 0f; }
        static int DashSFXTimeOffset => 80;
        public override void AI() //constantly cycles each time
        {
            Player player = Main.player[NPC.target];

            NPC.TargetClosest(true);
            {
                if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active || Main.dayTime == true)
                {
                    NPC.ai[0] = 0;

                    NPC.velocity.Y = NPC.velocity.Y - 0.4f;
                    if (NPC.timeLeft > 60)
                    {
                        NPC.timeLeft = 60;
                        return;
                    }
                }
                else //regular attack stuff
                {
                    //checks if should go frenzy (expert mode special phase)
                    if (Main.expertMode && NPC.GetLifePercent() <= 0.4f && AttackType == NightmareOrbAtkType.DecideNext)
                    {
                        frenzy = true;
                    }

                    DecideNextAttack();
                    AttackPattern();
                }
            }
        }
        NightmareOrbAtkType[] GetAttackOrder()
        {
            return new NightmareOrbAtkType[]
                {
                NightmareOrbAtkType.SingleStar,
                NightmareOrbAtkType.HomingStar,
                NightmareOrbAtkType.SlashBeam,
                NightmareOrbAtkType.Dash,
                NightmareOrbAtkType.TripleStar,
                NightmareOrbAtkType.TripleStar,
                NightmareOrbAtkType.Dash
                };
        }
        void DecideNextAttack()
        {

            if (AttackType == NightmareOrbAtkType.DecideNext)
            {
                NightmareOrbAtkType[] atkOrder = GetAttackOrder();
                NightmareOrbAtkType nextAtkType = atkOrder[AttacksPerformedSinceSpawn % atkOrder.Length];
                if(nextAtkType == NightmareOrbAtkType.Dash)
                {
                    CheckToPlayDashSFX();
                }
                if (NPC.ai[0] >= 30)//time between attacks
                {
                    AttackType = nextAtkType;
                }
            }
        }
        private void AttackPattern()
        {
            Player player = Main.player[NPC.target];

            NPC.ai[0]++;
            if(AttackType == NightmareOrbAtkType.Spawn)
            {
                Intro();
                return;
            }
            if (AttackType != NightmareOrbAtkType.Dash && AttackType != NightmareOrbAtkType.Spawn)
            {
                MainMovement(player);
            }
            if (AttackType == NightmareOrbAtkType.SingleStar)
            {
                AttackSingleStar(player);
            }
            else if (AttackType == NightmareOrbAtkType.SlashBeam)
            {
                AttackSlashBeam(player);
            }
            else if (AttackType == NightmareOrbAtkType.TripleStar)
            {
                AttackTripleStar();
            }
            else if (AttackType == NightmareOrbAtkType.HomingStar)
            {
                AttackHomingStar();
            }
            else if (AttackType == NightmareOrbAtkType.Dash)
            {
                AttackDash();
            }
        }

        private void Intro()
        {
            float velY = Helper.RemapEased(NPC.ai[0], 0, 40, -10, 0, Easings.EaseOutSquare);
            NPC.velocity.Y = velY;
            if (NPC.ai[0] >= 60)
            {
                NPC.dontTakeDamage = false;
            }
            if (NPC.ai[0] >= 400)
            {
                EndAttack();
            }
        }

        private void MainMovement(Player player)
        {
            Vector2 move = player.Center - NPC.Center + GetTargetPosOffset(0.05f);
            float magnitude = move.Length();
            float inertia = 20f;
            float speed = 30;
            if (magnitude > speed)
            {
                move *= speed / magnitude;//sets the move vector's magnitude to be speed
            }
            NPC.velocity = (inertia * NPC.velocity + move) / (inertia + 1);
            magnitude = NPC.velocity.Length();
            if (magnitude > speed)
            {
                NPC.velocity *= speed / magnitude;//sets npc's velocity vector's magnitude to be speed
            }
        }
        private void AttackSlashBeam(Player player)
        {
            //>attempts to make code better
            //>actually make it worse
            //>wires.png
            const int startTime = 40;
            float fireRate = GetValueMultipliedDependingOnPhaseAndDifficulty(40, 0.85f, 0.85f);
            int currentSlashBeamIndex = (int)(NPC.ai[0] - startTime) / (int)fireRate;
            Vector2 playerDistance = player.Center - NPC.Center;
            Vector2 playerXOffset = player.Center + new Vector2(playerDistance.X <= 0 ? 650 : -650, 0); //go in front of player
            int[] yOffsetsForSlashBeam = new int[6] { -2, 1, -1, 2, 0, 0 };
            Vector2 targetPos = playerXOffset + new Vector2(0, yOffsetsForSlashBeam[currentSlashBeamIndex]) * 120;
            int relativeTimer = (int)((int)(NPC.ai[0] - startTime) % fireRate);
            targetPos.X += Utils.Remap(relativeTimer, 0, fireRate / 3, 100, 0) * NPC.direction;
            NPC.velocity = NPC.DirectionTo(targetPos) * Utils.Remap(NPC.Distance(targetPos), 10, 50, 0, 45);
            if (relativeTimer == 10) //shoot
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(NPC.direction * 20, 0), ModContent.ProjectileType<NightSlash>(), dmgPerAtkType[AttackType], 0f, Main.myPlayer, 0, 0);
                }
                PlaySlashBeamSFX();
            }
            if (NPC.ai[0] >= startTime + fireRate * 5)//5 is number of waves
            {
                EndAttack();
            }
        }
        private void AttackTripleStar()
        {
            int startTime = frenzy ? 40 : 60;
            int fireRate = frenzy ? 20 : 35;
            fireRate = GetValueDividedDependingOnPhaseAndDifficulty(fireRate);
            int numberOfShots = 4;
            if ((NPC.ai[0] - startTime) % fireRate == 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    //Straight
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, NPC.direction * 15, 0, ModContent.ProjectileType<BadStar>(), dmgPerAtkType[AttackType], 0f, Main.myPlayer, 0, 0);

                    //Down
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, NPC.direction * 10, 5, ModContent.ProjectileType<BadStar>(), dmgPerAtkType[AttackType], 0f, Main.myPlayer, 0, 0);

                    //Up
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, NPC.direction * 10, -5, ModContent.ProjectileType<BadStar>(), dmgPerAtkType[AttackType], 0f, Main.myPlayer, 0, 0);
                }
                PlaySpreadShotSFX();
            }
            if (NPC.ai[0] > startTime + fireRate * numberOfShots)
            {
                EndAttack();
            }
        }
        private void AttackHomingStar()
        {
            int startTime = frenzy ? 40 : 60;
            int fireRate = frenzy ? 40 : 60;
            int numberOfShots = 3;
            if ((NPC.ai[0] - startTime) % fireRate == 0 && (NPC.ai[0] - startTime) <= numberOfShots * fireRate) //homing stars go behind it
            {
                PlayHomingStarsSpawnSFX();
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2[] shotDirections = new Vector2[] { new(-20, 80), new(-80, -120), new(-80, 120), new(-20, -80) };
                    int starsSpawned = 0;
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        Player possibleTarget = Main.player[i];
                        if (possibleTarget.dead || !possibleTarget.active)
                            continue;
                        for (int j = 0; j < shotDirections.Length; j++)
                        {
                            Vector2 projVel = shotDirections[j] with { X = NPC.direction * shotDirections[j].X };
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projVel * 2, ModContent.ProjectileType<HomingNightStar>(), dmgPerAtkType[AttackType], 0f, Main.myPlayer, NPC.whoAmI, i, starsSpawned);
                            starsSpawned++;
                        }
                    }
                }
            }
            if (NPC.ai[0] > startTime + (numberOfShots + 1) * fireRate)
            {
                EndAttack();
            }
        }
        private void AttackSingleStar(Player player)
        {
            Vector2 shootVel = player.Center - NPC.Center;
            shootVel.Normalize(); //reduces it to a value of 1
            shootVel *= 15f; //projectile speed
            shootVel *= Main.expertMode ? 1.25f : 1;
            shootVel *= frenzy ? 1.25f : 1;
            int fireRate = frenzy ? 12 : 15;
            int startTime = 49;
            int numberOfShots = frenzy ? 13 : 8;
            if ((NPC.ai[0] - startTime) % fireRate == 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, shootVel, ModContent.ProjectileType<BadStar>(), dmgPerAtkType[AttackType], 0f, Main.myPlayer, 0, 0);
                }
                PlayStarThrowSFX();
            }
            if (NPC.ai[0] > startTime + numberOfShots * fireRate)
            {
                EndAttack();
            }
        }


        float DashXOffset()
        {
            float dashTime = GetDashTime();
            if (NPC.ai[0] < dashTime / 1.8f)
                return MathHelper.Lerp(100, 900, Easings.EaseInOutSine(Utils.GetLerpValue(0, dashTime, NPC.ai[0]))) * NPC.direction;
            if (NPC.ai[0] < dashTime)
                return 900 * NPC.direction;
            return NPC.direction;//don't set t 0 otherwise it wil just stay still
        }
        void CheckToPlayDashSFX()
        {
            int dashTime = (int)GetDashTime();
            if (NPC.ai[0] == dashTime - DashSFXTimeOffset)
            {
                PlayDashChargeSFXSlow();
            }
        }
        private void AttackDash()
        {

            Player player = Main.player[NPC.target];
            Vector2 targetPos = player.Center - new Vector2(DashXOffset(), 0);
            float dashTime = GetDashTime();
            float dashSpeed = NPC.ai[0] < dashTime ? 40 : 100;
            float lerpAmount = Utils.Remap(NPC.ai[0], dashTime, dashTime + 15, 0.08f, 0.00f);
            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.Center.DirectionTo(targetPos) * dashSpeed, lerpAmount);                                                                                                                                //4 is padding
            if ((MathF.Sign(player.Center.X - (NPC.Center.X - NPC.direction * 100)) != MathF.Sign(NPC.velocity.X) && NPC.ai[0] > dashTime + 4) || NPC.ai[0] > 300)
            {
                EndAttack();
                return;
            }
            CheckToPlayDashSFX();
            if (NPC.ai[0] == (int)dashTime)
            {
                PlayDashSFX();
            }
            if (dashSpeed <= 40)
            {
                return;
            }

            if ((int)NPC.ai[0] % 5 == 4)
            {
                for (float i = 0; i < MathF.Tau; i += MathF.Tau / 20f)
                {
                    Vector2 offset = i.ToRotationVector2() * 40;
                    offset.X *= 0.5f;
                    offset = offset.RotatedBy(NPC.velocity.ToRotation());
                    Dust dust = Dust.NewDustPerfect(NPC.Center + offset, DustID.RainbowMk2, offset * 0.1f);
                    dust.scale *= 0.75f;
                    SetDashDustStats(dust.dustIndex);
                }
            }
            for (int i = 0; i < 3; i++)
            {
                int index = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.RainbowMk2);
                SetDashDustStats(index);
            }

        }

        private float GetDashTime()
        {
            return GetValueMultipliedDependingOnPhaseAndDifficulty(70, 0.85f, 0.85f) * 1.8f;
        }

        private void SetDashDustStats(int index)
        {
            Dust dust = Main.dust[index];
            dust.color = Color.Lerp(Color.Purple, Color.Blue, Main.rand.NextFloat());
            dust.velocity += NPC.velocity;
            dust.scale *= 4.5f;
            dust.noGravity = true;
            dust = Dust.CloneDust(dust);
            if (dust.dustIndex != 6000)
            {
                dust.color = Color.White with { A = 0 };
                dust.scale *= 0.65f;
            }
        }

        int GetValueMultipliedDependingOnPhaseAndDifficulty(float value, float expertMultiplier = 1.2f, float frenzyMultiplier = 1.2f)
        {
            value *= Main.expertMode ? expertMultiplier : 1;
            value *= frenzy ? frenzyMultiplier : 1;
            return (int)value;
        }
        int GetValueDividedDependingOnPhaseAndDifficulty(float value, float expertDivisor = 1.2f, float frenzyDivisor = 1.2f)
        {
            value /= Main.expertMode ? expertDivisor : 1;
            value /= frenzy ? frenzyDivisor : 1;
            return (int)value;
        }
        private void EndAttack(int delayBeforeNextAttack = 0)
        {
            NPC.ai[0] = -delayBeforeNextAttack;
            AttackType = NightmareOrbAtkType.DecideNext;
            NPC.netUpdate = true;
            AttacksPerformedSinceSpawn++;
        }
    }
}
