using KirboMod.Projectiles.NewWhispy.NewWhispyAppleMedium;
using KirboMod.Projectiles.NewWhispy.NewWhispyBlado;
using KirboMod.Projectiles.NewWhispy.NewWhispyFireAppleProj;
using KirboMod.Projectiles.NewWhispy.NewWhispyGordo;
using KirboMod.Projectiles.NewWhispy.NewWhispySpikes;
using KirboMod.Projectiles.NewWhispy.NewWhispyTornado;
using KirboMod.Projectiles.NewWhispy.NewWhispyWind;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AttackStats = (KirboMod.NPCs.NewWhispy.NewWhispyBoss.AIState state, short attackStartTime, short attackRate, short attackCount, short attackExtraWaitTime);

namespace KirboMod.NPCs.NewWhispy
{
    public partial class NewWhispyBoss : ModNPC
    {
        short attackRate;
        short attackExtraWaitTime;
        short attackCount;
        short attackStartTime;
        ref float Timer => ref NPC.ai[0];
        AIState State { get => (AIState)NPC.ai[1]; set => NPC.ai[1] = (float)value; }
        ref float StateSwitchCount => ref NPC.ai[2];
        Player TargetedPlayer => Main.player[NPC.target];
        ref float PhaseIndex => ref NPC.localAI[0];
        AnimationState AnimState { get => (AnimationState)NPC.localAI[1]; set => NPC.localAI[1] = (float)value; }
        ref float AnimationTimer => ref NPC.localAI[2];
        bool ShouldBeInPhase2 => NPC.GetLifePercent() < .5f && Main.expertMode;

        public override void AI()
        {
            FindFrame(1);
            NPC.TargetClosest();
            NPC.spriteDirection = NPC.direction;
            Player player = Main.player[NPC.target];
            if (player.dead)
            {
                NPC.alpha += 10;
                if (NPC.alpha >= 255)
                {
                    NPC.active = false;
                }
                return;
            }
            if (State == AIState.Change)
            {
                State_Change();
            }
            else if (State != AIState.Spawn)
            {
                NPC.damage = 30;
                NPC.immortal = false;
            }
            switch (State)
            {
                case AIState.Spawn:
                    DoHealthbarFillAndSnapToTile();
                    break;
                case AIState.FireApple:
                    State_FireApple();
                    break;
                case AIState.Blado:
                    State_Blado();
                    break;
                case AIState.Gordo:
                    State_Gordo();
                    break;
                case AIState.Wind:
                    State_Wind();
                    break;
                case AIState.Tornado:
                    State_Tornado();
                    break;
                case AIState.CloseSpikes:
                    State_CloseSpikes();
                    break;
                case AIState.EvenSpikes:
                    State_EvenSpikes();
                    break;
                case AIState.SplittingWind:
                    State_SplittingWind();
                    break;
                //APPLES BOUNCE, GORDOS DONT
                case AIState.AppleMedium:
                    State_AppleMedium();
                    break;
                case AIState.AppleSmall:
                    break;
            }
            Timer++;
            TryEndState(attackRate, attackCount, attackStartTime, attackExtraWaitTime);
        }


        void State_FireApple()
        {
            if (CheckShouldShoot() && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 spawnPos = GetRandomPointInLeavesForAttacks();
                Vector2 spawnVelocity = new Vector2(0, -30).RotatedByRandom(1);
                float ai0 = NewWhispyFireApple.TimeToReachYPoint(spawnPos.Y, spawnPos.Y, NewWhispyFireApple.YAccel, spawnVelocity.Y);
                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), spawnPos, spawnVelocity, ModContent.ProjectileType<NewWhispyFireApple>(), FireAppleDamage, 0, Main.myPlayer, ai0);
            }
        }
        void State_Blado()
        {
            SetAngryAnimationValues();
            if (CheckShouldShoot() && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 spawnPos = GetRandomPointInThirdOfLeavesForAttacks(CurrentShotIndex % 3);
                Vector2 spawnVelocity = new(0, 0.1f);
                NewWhispyBlado.GetAIValues(30f, NPC.Center.Y, NPC.direction, out float ai0, out float ai1, out float ai2);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPos, spawnVelocity, ModContent.ProjectileType<NewWhispyBlado>(), BladoDamage, 0, -1, ai0, ai1, ai2);
            }
        }

        private void SetAngryAnimationValues()
        {
            if (Timer > attackStartTime - 15)
            {
                AnimState = AnimationState.Angry;
            }
            if (WillEndStateThisFrame() || CurrentShotIndex != 0)
            {
                AnimState = AnimationState.Regular;
            }
        }

        void State_Gordo()
        {
            SetAngryAnimationValues();
            if (CheckShouldShoot() && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 spawnPos = GetRandomPointInThirdOfLeavesForAttacks(CurrentShotIndex % 3);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPos, default, ModContent.ProjectileType<NewWhispyGordo>(), GordoDamage, 0, -1, -30);
            }
        }
        void State_Wind()
        {

            if (CheckShouldShoot())
            {
                PlayAirShotSFX();
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 mouthPos = GetMouthPosition();
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        Player p = Main.player[i];
                        if(!p.active || p.dead)
                        {
                            continue;
                        }
                        NewWhispyWind.GetAIValues(i, out float ai0);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), mouthPos, new Vector2(NPC.spriteDirection * 5, 0), ModContent.ProjectileType<NewWhispyWind>(), WindDamage, 0, -1, ai0);
                    }
                }
            }
            SetSpitAnimationValues();
        }

        private void SetSpitAnimationValues()
        {
            int animationDuration = (int)MathF.Min(15, 2 * attackRate / 3);
            int relativeTimer = ((int)Timer - attackStartTime + animationDuration) % attackRate;
            if (relativeTimer < animationDuration && relativeTimer > 0 && Timer - attackStartTime < attackCount * attackRate - attackRate)
            {
                AnimState = AnimationState.PuffedUpCheeksAndClosedMouth;
            }
            else
            {
                AnimState = AnimationState.Regular;
            }
            if (WillEndStateThisFrame())
            {
                AnimState = AnimationState.Regular;
            }
        }

        void State_Tornado()
        {
            if (CheckShouldShoot())
            {
                PlayAirShotSFX();
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 mouthPos = GetMouthPosition();
                    int shotIndex = CurrentShotIndex;
                    NewWhispyTornado.GetAIVAlues((attackCount - CurrentShotIndex - 1) * 16 + 14, out float ai0);
                    float speed = Utils.Remap(shotIndex, 0, attackCount - 1, 4.75f, 2.5f, false);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), mouthPos, new Vector2(NPC.spriteDirection * speed, 0), ModContent.ProjectileType<NewWhispyTornado>(), WindDamage, 0, -1, ai0);
                }
            }
            SetSpitAnimationValues();
        }
        void State_CloseSpikes()
        {
            if (CheckShouldShoot() && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 projPos = NPC.Bottom;
                projPos.Y -= 32;
                projPos.X += (CurrentAttackProgress * NPC.spriteDirection * 140) + 170 * NPC.spriteDirection;
                projPos = ScanDownForTiles(projPos);
                float rotation = MathHelper.Lerp(0, .5f, CurrentAttackProgress) * NPC.spriteDirection;
                NewWhispySpike.SpawnRoot(NPC.GetSource_FromAI(), projPos, new Vector2(0, -1).RotatedBy(rotation), CloseSpikeDamage, 5);
            }
        }
        void State_EvenSpikes()
        {
            if (CheckShouldShoot() && Main.netMode != NetmodeID.MultiplayerClient)
            {
                //NEW ATTACKS TO IMPLEMENT:
                //IF FAR AWAY, SPAWN ROOTS UNDER PLAYER AND  IN A POSITION THAT FORCES THE PLAYER BACK TOWARDS THE BOSS
                //DO FIRE APPLE ATTACK IF EVERY PLAYER IS ABOVE WHISPY IN MP, AND ALSO ADD ANOTHER INSTANCE WHICH FIRES ONE FIRE APPLE TOWARDS EVERY PLAYER + ANOTHER ONE PREDICTIVELY
                //FIRE APPLE STORE PLAYER INDEX SO IT DETONATES ON THE RIGHT PLAYER
                //-1 INDEX FOR ANY PLAYER
                //SPAWN 1 WIND GUST FOR EVERY PLAYER, MAKE WIND GUSTS ONLY COLLIDE WITH THE TARGET PLAYER, OTHERWISE ALSO MAKE IT TRANSPARENT

                float spikeSpacing = Main.expertMode ? 120 : 180;
                int spikeAmount = 8;
                int spikeLength = 7;
                int spikeTelegraphTime = Main.expertMode ? 70 : 100;
                IEntitySource source = NPC.GetSource_FromAI();
                float totalSpikeCoverageX = (spikeAmount - 1) * (spikeSpacing) + ContentSamples.ProjectilesByType[ModContent.ProjectileType<NewWhispySpike>()].width;
                bool centerSpikesOnPlayer = NPC.Distance(TargetedPlayer.Center) > totalSpikeCoverageX;
                for (int i = 0; i < spikeAmount; i++)
                {
                    Vector2 projPos = NPC.Bottom;
                    projPos.Y -= 32;
                    if (centerSpikesOnPlayer)
                    {
                        projPos.X = TargetedPlayer.Center.X;
                        projPos.X -= totalSpikeCoverageX / 2;
                    }
                    projPos.X += (190 + i * spikeSpacing) * NPC.direction;
                    projPos = ScanDownForTiles(projPos);

                    NewWhispySpike.SpawnRoot(source, projPos, new Vector2(0, -1), SpikeDamage, spikeLength, spikeTelegraphTime + i * 5);
                }
                if (Main.expertMode)
                {
                    SetAngryAnimationValues();

                    Vector2 spawnPos = NPC.Top;
                    spawnPos.X += NPC.direction * (NPC.width / 2 + 30);
                    Vector2 spawnVelocity = new(0, 0.1f);
                    NewWhispyBlado.GetAIValues(30f, NPC.Center.Y, NPC.direction, out float ai0, out float ai1, out float ai2);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPos, spawnVelocity, ModContent.ProjectileType<NewWhispyBlado>(), BladoDamage, 0, -1, ai0, ai1, ai2);
                }
            }
        }
        void State_SplittingWind()
        {
            if (CheckShouldShoot())
            {
                PlayAirShotSFX();
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 mouthPos = GetMouthPosition();
                    Vector2 projTargetPos = NPC.Center;
                    //projTargetPos.X += NPC.direction * NPC.width / 2 + NPC.direction * (16 * 10 + (CurrentShotIndex * 20));
                    projTargetPos.X = NPC.position.X + NPC.width / 2 + NPC.direction * 16 * 35;
                    NewWhispySplittingWind.GetAIValues(mouthPos, projTargetPos, SplittingWindSpeed, out float ai0, out Vector2 projVelocity);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), mouthPos, projVelocity, ModContent.ProjectileType<NewWhispySplittingWind>(), WindDamage, 0f, -1, ai0);
                    float waveEdgeX = NPC.position.X + NPC.width / 2 + NPC.direction * 16 * 50;
                    SpawnRootWave(waveEdgeX, NPC.direction, 1f, 8, 7, 40, -0.5f, 0.3f);
                }
            }
            SetSpitAnimationValues();
        }
        void State_AppleMedium()
        {
            SetAngryAnimationValues();
            if (CheckShouldShoot() && Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Vector2 spawnPos = GetRandomPointInLeavesForAttacks();
                    Player p = Main.player[i];
                    if (!p.active || p.dead)
                    {
                        continue;
                    }
                    //if the player is above the spawn position, or if it is out of horizontal, don't spawn      
                    if (p.Center.Y < spawnPos.Y)
                    {
                        continue;
                    }
                    int playerSide = MathF.Sign(p.Center.X - NPC.Center.X);
                    //if the player is not on the same side as whispy is facing, don't spawn
                    if (playerSide != NPC.direction)
                    {
                        continue;
                    }
                    //if the player isn't above the canopy, don't spawn
                    if (p.Center.X - NPC.Center.X > CanopyWidth)
                    {
                        continue;
                    }
                    spawnPos.X = p.Center.X;
                    spawnPos.Y -= 32;//shift it upwards
                    Vector2 spawnVelocity = new(0, 0.01f);//don't make it 0
                    NewWhispyAppleMedium.GetAIValues(30f, NPC.Center.Y, p.whoAmI, out float ai0, out float ai1, out float ai2);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPos, spawnVelocity, ModContent.ProjectileType<NewWhispyAppleMedium>(), AppleDamage, 0, -1, ai0, ai1, ai2);
                }
            }
        }
        void ShootFireAppleAt(Player plr)
        {
            Vector2 spawnPos = NPC.Top;
            spawnPos.Y += NPC.direction * 250;
            float shootSpeed = 70;
            Vector2 spawnVelocity = shootSpeed * Vector2.Normalize(plr.Center - spawnPos);
            float time = Vector2.Distance(plr.Center, spawnPos) / shootSpeed;
            spawnVelocity = Utils.FactorAcceleration(spawnVelocity, time, new Vector2(0, NewWhispyFireApple.YAccel), 0);
            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), spawnPos, spawnVelocity, ModContent.ProjectileType<NewWhispyFireApple>(), FireAppleDamage, 0, Main.myPlayer, 40);
            Utils.ChaseResults results = Utils.GetChaseResults(spawnPos, shootSpeed, plr.Center, plr.velocity);

            if (results.InterceptionHappens)
            {
                results.ChaserVelocity = Utils.FactorAcceleration(results.ChaserVelocity, results.InterceptionTime, new Vector2(0, NewWhispyFireApple.YAccel), 0);
                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), spawnPos, results.ChaserVelocity, ModContent.ProjectileType<NewWhispyFireApple>(), FireAppleDamage, 0, Main.myPlayer, 40);
            }
        }
        void SpawnRootWave(float targetX, float directionSign, float xLimiter, int spikeLength, int spikeAmount, float spikeSpacing, float startAngle = 0, float endAngle = 0)
        {
            int spikeTelegraphTime = 50;
            IEntitySource source = NPC.GetSource_FromAI();
            float totalSpikeCoverageX = (spikeAmount - 1) * (spikeSpacing) + ContentSamples.ProjectilesByType[ModContent.ProjectileType<NewWhispySpike>()].width;
            for (int i = 0; i < spikeAmount; i++)
            {
                Vector2 projPos = NPC.Bottom;
                projPos.Y -= 32;
                projPos.X = targetX;
                projPos.X += totalSpikeCoverageX * 1.5f * directionSign;
                projPos.X += (190 + i * spikeSpacing) * -directionSign;
                projPos = ScanDownForTiles(projPos);
                float angle = Utils.Remap(i, 0, spikeAmount - 1, startAngle, endAngle);
                Vector2 direction = new(MathF.Sin(angle) * -directionSign, -MathF.Cos(angle));
                NewWhispySpike.SpawnRoot(source, projPos, direction, SpikeDamage, spikeLength, spikeTelegraphTime + i * 10);
            }
        }

        void State_Change()
        {
            if (State != AIState.Spawn)
            {
                NPC.immortal = false;
            }
            //PARAM ORDER:
            //state, start, fireRate, numberOfShots, extraDelay
            List<AttackStats> attackStatsList = new(20);
            attackStatsList.AddRange([
                new(AIState.Gordo, 30, 30, 3, 10),
                new(AIState.CloseSpikes, 30, 20, 4, 10),
                new(AIState.AppleMedium, 0, 20, 3, 0),
                new(AIState.CloseSpikes, 0, 20, 4, 0),
                new(AIState.Wind, 20, 20, 3, 10) ]);
            if (Main.expertMode)
            {
                attackStatsList.Add(new(AIState.Tornado, 10, 20, 4, 0));
            }
            attackStatsList.AddRange(
            [
                new(AIState.Gordo, (short)(Main.expertMode ? 0 : 30), 15, 3, 50),
                new(AIState.EvenSpikes, 0, 1, 1, (short)(Main.expertMode ? 100 : 120)),
                new(AIState.Wind, 20, 20, 1, 0),
                new(AIState.CloseSpikes, 30, 10, 4, 0) ]);
            if (Main.expertMode)
            {
                attackStatsList.Add(new(AIState.SplittingWind, 15, 100, 1, 10));
            }
            attackStatsList.Add(new(AIState.Wind, 20, 20, 3, 10));
            if (Main.expertMode)
            {
                attackStatsList.Add(new(AIState.Tornado, 50, 20, 4, 0));
            }
            attackStatsList.Add(new(AIState.CloseSpikes, 0, 10, 4, 0));
            if (Main.expertMode)
            {
                attackStatsList.Add(new(AIState.SplittingWind, 15, 100, 1, 10));
            }

            AttackStats[] attackStats = attackStatsList.ToArray();
            //attackStats = [new(AIState.SplittingWind, 10, 90, 2, 10), new(AIState.Gordo, 2, 30, 1, 2)]; //DEBUG CODE
            if (TargetedPlayer.Center.Y < NPC.Top.Y)
            {
                attackStats = [new(AIState.FireApple, 20, 2, 30, 60)];
                StateSwitchCount--;

            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player p = Main.player[i];
                    if (p.active && !p.dead && (p.Center.Y < NPC.Top.Y || p.Bottom.Y > NPC.Center.Y + 16 * 12))
                    {
                        ShootFireAppleAt(p);
                    }
                }
            }
            if (MathF.Abs(TargetedPlayer.Center.X - NPC.Center.X) > 16 * 42 + NPC.width / 2)
            {
                attackStats = [new(AIState.Wind, 20, 18, 3, 20)];
                SpawnRootWave(TargetedPlayer.Center.X, NPC.direction, 1f, 9, 8, 40);
            }
            State = attackStats[(int)StateSwitchCount % attackStats.Length].state;
            attackStartTime = attackStats[(int)StateSwitchCount % attackStats.Length].attackStartTime;
            attackCount = attackStats[(int)StateSwitchCount % attackStats.Length].attackCount;
            attackRate = attackStats[(int)StateSwitchCount % attackStats.Length].attackRate;
            attackExtraWaitTime = attackStats[(int)StateSwitchCount % attackStats.Length].attackExtraWaitTime;
            if (Main.getGoodWorld)
            {
                attackStartTime = 0;
            }
            if (State == AIState.Gordo)
            {
                if (Main.expertMode)
                {
                    State = AIState.Blado;
                }
                else
                {
                    attackRate = (short)((2 * attackRate) / 3);
                    attackCount *= 2;
                }
            }


            if (Main.expertMode && State == AIState.Wind)
            {
                attackCount++;
            }
            StateSwitchCount++;
            Timer = 0;
            if (ShouldBeInPhase2 && PhaseIndex < 1)
            {
                PhaseIndex = 1;
            }
        }
    }
}
