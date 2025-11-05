using KirboMod.Projectiles;
using KirboMod.Projectiles.DarkMatterHomingOrb;
using KirboMod.Projectiles.Lightnings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.NPCs.PureDarkMatterRematch
{
    [AutoloadBossHead]
    public class PureDarkMatterRematch : ModNPC
    {
        public override string BossHeadTexture => "KirboMod/NPCs/PureDarkMatter_Head_Boss";
        public override string Texture => "KirboMod/NPCs/PureDarkMatter";
        ref float Timer => ref NPC.ai[0];
        AtkType CurrentAttackType { get => (AtkType)NPC.ai[1]; set => NPC.ai[1] = (float)value; }
        AtkType LastAttackType { get => (AtkType)NPC.ai[2]; set => NPC.ai[2] = (float)value; }
        ref float Phase => ref NPC.localAI[0];
        //phase 1 is 0
        bool Phase2 { get => Phase >= 1; set => Phase = value ? 1 : Phase; }
        static int LaserDamage => 100 / 2;
        static int PetalDamage => 100 / 2;
        static int BeamDamage => 100 / 2;
        enum AtkType : byte
        {
            DecideNext = 0,
            Petals,//1
            Dash,//2
            Lasers,//3
            Spin,//4
            Beams,//5
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dark Matter");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.CanHitPastShimmer[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new()
            {
                Hide = true,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifiers);
            string musicPath = "KirboMod/Music/Photonic0_DarkMatterRematch_WithLoopMetadata";
            int musicSlot = MusicLoader.GetMusicSlot(musicPath);
            Music = musicSlot;
            Main.musicFade[musicSlot] = 1;
            Main.musicNoCrossFade[musicSlot] = true;
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true; //immune to all buffs that aren't whips
        }
        public override void SetDefaults()
        {
            NPC.width = 130;
            NPC.height = 130;
            NPC.damage = 100;
            NPC.noTileCollide = true;
            NPC.defense = 86;
            NPC.lifeMax = 80000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 16;
            NPC.boss = true;
            NPC.trapImmune = true;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            if (!Main.dedServ)//if not dedicated server
            {
                string musicPath = "KirboMod/Music/Photonic0_DarkMatterRematch_WithLoopMetadata";
                int musicSlot = MusicLoader.GetMusicSlot(musicPath);
                Music = musicSlot;
                Main.musicFade[musicSlot] = 1;
                Main.musicNoCrossFade[musicSlot] = true;
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            Helper.BossHpScalingForHigherDifficulty(ref NPC.lifeMax, balance);
            //NPC.damage = (int)(NPC.damage * 0.6);
        }
        public override void AI() //constantly cycles each time
        {
            if (!NPC.HasPlayerTarget)
            {
                NPC.TargetClosest();
            }
            if (!NPC.HasPlayerTarget)
            {
                FleeAndDespawn();
                return;
            }
            Player plr = Main.player[NPC.target];
            if (!plr.active || plr.dead)
            {
                FleeAndDespawn();
                return;
            }
            AttackPattern();
        }

        private void AttackPattern()
        {

            NPC.spriteDirection = NPC.direction;

            switch (CurrentAttackType)
            {
                case AtkType.Petals:
                    AttackPetals();
                    break;
                case AtkType.Dash:
                    AttackDash();
                    break;
                case AtkType.Lasers:
                    AttackLasers();
                    break;
                case AtkType.Spin:
                    AttackSpin();
                    break;
                case AtkType.Beams:
                    AttackBeams();
                    break;
            }
            Timer++;
            if (CurrentAttackType == AtkType.DecideNext)
            {
                AttackDecideNext();
            }
        }
        //circle around loosely while firing predictive beams
        float BeamCircleDist => 500;
        float BeamCircleRate => 0.03f;
        float BeamShootSpeed => 17f;
        float BeamStartup => 30f;
        float BeamCount => 12;
        float BeamRate => 17;
        float BeamExtraWait => 30;
        float BeamCirclingMaxMoveSpeed => 33f;
        float BeamCircleInertia => 30;
        static float BeamPostAttackMaxYOffset => 400;
        static float BeamPostAttackXDist => 400;
        private void AttackBeams()
        {
            Player plr = Main.player[NPC.target];
            Vector2 plrPos = plr.Center;
            Vector2 targetPos = plrPos + (Timer * BeamCircleRate + MathF.PI * .5f).ToRotationVector2() * BeamCircleDist;
            PredictiveAimWithFailsafe(NPC.Center, BeamShootSpeed, plrPos, (plr.position - plr.oldPosition), out Vector2 projVel);

            if (Timer < BeamStartup)
            {
                NPC.rotation = projVel.ToRotation() + MathF.PI;
                targetPos = plr.Center - new Vector2(NPC.direction * 400, -200);
                MoveTo(targetPos, 50, 0.12f);
            }
            else if (Timer < BeamStartup + BeamCount * BeamRate)
            {
                NPC.rotation = projVel.ToRotation() + MathF.PI;
                float relativeTimer = Timer - BeamStartup;
                if (relativeTimer % BeamRate == 0)
                {
                    SoundEngine.PlaySound(DarkMatter.DarkMatter.DarkBeamShoot, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projVel, ModContent.ProjectileType<AngledDarkBeam>(), BeamDamage, 0f);
                    }
                }
            }
            else if (Timer < BeamStartup + BeamCount * BeamRate + BeamExtraWait)
            {
                float relativeTimer = Timer - BeamStartup - BeamCount * BeamRate;
                float yOffset = Utils.Remap(relativeTimer, 0, BeamExtraWait, -BeamPostAttackMaxYOffset, BeamPostAttackMaxYOffset);
                float xOffset = -NPC.direction * BeamPostAttackXDist;
                targetPos = plr.Center + new Vector2(xOffset, yOffset);
                NPC.rotation = projVel.ToRotation() + MathF.PI;
            }
            else
            {
                EndState();
            }
            MoveToPosUsingInertiaThing(targetPos, BeamCirclingMaxMoveSpeed, BeamCircleInertia);
        }
        void MoveToPosUsingInertiaThing(Vector2 targetPos, float maxSpeed, float inertia)
        {

            Vector2 move = targetPos - NPC.Center; //move above
            move = move.SafeNormalize(Vector2.Zero);
            move *= maxSpeed;
            NPC.velocity = (NPC.velocity * (inertia - 1) + move) / inertia;
        }
        private void FleeAndDespawn()
        {
            NPC.velocity.Y -= 2;
            NPC.timeLeft = 29;
            NPC.EncourageDespawn(30);
        }

        private void AttackDecideNext()
        {
            List<AtkType> possibleAttacks = new() { AtkType.Petals, AtkType.Dash, AtkType.Lasers, AtkType.Beams };
            if (Main.rand.NextBool())
            {
                possibleAttacks.Add(AtkType.Spin);
            }
            possibleAttacks.Remove(LastAttackType);
            CurrentAttackType = possibleAttacks[Main.rand.Next(possibleAttacks.Count)];
            LastAttackType = CurrentAttackType;
            //CurrentAttackType = AtkType.Dash;
            NPC.netUpdate = true;
            UpdatePhase();
            Timer = 0;
        }

        private void UpdatePhase()
        {
            if (NPC.GetLifePercent() < .5f)
            {
                Phase2 = true;
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            UpdatePhase();
        }
        private void AttackSpin()
        {
            //idk??? I guess just do what regular dark matter does but faster
            Player player = Main.player[NPC.target];
            float speed = 52f;
            float inertia = 10f;

            Vector2 move = player.Center + new Vector2(0, -200) - NPC.Center; //move above

            float rotationSpeed = Main.expertMode ? 2.2f : 3.1f;
            float start = 50;
            float stayStillDuration = 60;
            float rotationDuration = 720f / rotationSpeed;//2 full spins
            NPC.direction = -1;

            if (NPC.ai[0] < start)
            {
                NPC.rotation = MathHelper.ToRadians(90);

                move.Normalize();
                move *= speed;
                NPC.velocity = (NPC.velocity * (inertia - 1) + move) / inertia;
            }
            else if (NPC.ai[0] < start + stayStillDuration)
            {
                if (NPC.ai[0] < start + stayStillDuration - (360f * 2f / 30f)) //two full rotations
                {
                    NPC.rotation -= MathHelper.ToRadians(30);
                }
                NPC.velocity *= 0.01f;
            }
            else if (NPC.ai[0] < start + stayStillDuration + rotationDuration)
            {

                NPC.rotation -= MathHelper.ToRadians(rotationSpeed);

                Vector2 velocity = NPC.rotation.ToRotationVector2() * 32;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int damage = 100 / 2;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - velocity, -velocity, ModContent.ProjectileType<AngledDarkBeam>(), damage, 4, Main.myPlayer);
                }

                if (NPC.ai[0] % 3 == 0)
                {
                    //PlayBeamSFX();
                }
                SoundEngine.PlaySound(SoundID.Item158, NPC.Center); //zapinator
            }
            else if (NPC.ai[0] > start + stayStillDuration + rotationDuration + 30) //cooldown of 30 frames after finishing attack
            {
                EndState();
            }
        }
        static int LasersStartup => 35;
        static int LasersCount => 3;
        int LasersRate => Phase2 ? 70 : 80;
        int LasersExtraWait => (Phase2 ? 20 : 25) - (Main.getGoodWorld ? 5 : 0);
        private void AttackLasers()
        {
            Player player = Main.player[NPC.target];
            Vector2 targetPos = player.Center;
            float distToPlayer = NPC.Distance(targetPos);
            float chaseSpeed = Utils.Remap(distToPlayer, 100, 15000, 0, 150);
            NPC.rotation = (NPC.Center - targetPos).ToRotation();
            NPC.direction = MathF.Sign(player.Center.X - NPC.Center.X);
            NPC.spriteDirection = NPC.direction;

            if (Timer < LasersStartup)
            {
                targetPos = player.Center + new Vector2(-NPC.direction * 700, -150);
                chaseSpeed = 60f;
                MoveTo(targetPos, chaseSpeed, 0.2f);

            }
            else if (Timer < LasersStartup + LasersRate * LasersCount)
            {
                MoveTo(targetPos, chaseSpeed, 0.1f);
                if ((Timer - LasersStartup) % LasersRate == 0)
                {
                    ShootLasers(player);
                }
            }
            else if (Timer < LasersStartup + LasersCount * LasersRate + LasersExtraWait)
            {
                MoveTo(targetPos, 100f, 0.005f);
            }
            else
            {
                EndState();
            }

        }
        Vector2 SafeDirectionTo(Vector2 target)
        {
            return (target - NPC.Center).SafeNormalize(Vector2.Zero);
        }
        private void ShootLasers(Player player)
        {
            SoundEngine.PlaySound(PureDarkMatter.LaserSFX, NPC.Center);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            int laserCount = 11;
            float spreadY = 9f;
            float maxVelX = 12;
            float minVelX = 7;
            float midVelX = (minVelX + maxVelX) / 2;
            int halfLaserCountCeil = (int)((laserCount + 0.5f) / 2);
            Vector2 toPlayer = SafeDirectionTo(player.Center);
            int type = ModContent.ProjectileType<DarkMatterLaser>();
            //type = ProjectileID.BulletDeadeye;//debug
            for (int i = 0; i < laserCount; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    //check if middle laser would be a repeat and overlap
                    if (i % 2 == 1 && i == laserCount / 2 && j == 0)
                    {
                        continue;
                    }
                    float shootSpeed = Utils.GetLerpValue(0, halfLaserCountCeil, i, true) * Utils.GetLerpValue(laserCount - 1, halfLaserCountCeil, i, true);
                    shootSpeed = MathHelper.Lerp(j == 0 ? minVelX : maxVelX, midVelX, shootSpeed);
                    float angle = Utils.Remap(i, 0, laserCount - 1, -spreadY, spreadY);
                    Vector2 velocity = new Vector2(shootSpeed, angle).RotatedBy(toPlayer.ToRotation());
                    LightningProj.GetSpawningStats(velocity, out float ai0, out float ai1);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, type, LaserDamage, 0f, -1, ai0, ai1); ;
                }
            }
        }
        static bool PredictiveAimWithFailsafe(Vector2 chaserPosition, float chaserSpeed, Vector2 runnerPosition, Vector2 runnerVelocity, out Vector2 chaserVelocity)
        {
            Utils.ChaseResults results = Utils.GetChaseResults(chaserPosition, chaserSpeed, runnerPosition, runnerVelocity);
            if (results.InterceptionHappens)
            {
                chaserVelocity = results.ChaserVelocity;
            }
            else
            {
                chaserVelocity = runnerVelocity;
            }

            return results.InterceptionHappens;
        }
        int DashStartup => Main.getGoodWorld ? 40 : Main.expertMode ? 50 : 60 - (Phase2 ? 10 : 0);
        static int DashCount => 5;
        int DashRate => 80 - (Main.getGoodWorld ? 30 : Main.expertMode ? 15 : 0) - (Phase2 ? 10 : 0);
        static int DashDuration => 40;
        static int DashPetalSpawnRate => 3;
        int DashPetalCount => Phase2 ? 1 : 0;
        static int DashPetalShootSpeed => 5;
        static float DashSpeed => 32;
        static float DashHomingStrength => 0f;
        static int DashExtraWait => 20;
        static int DashLightningBurstProjCount => 11;
        static float DashLightningShootSpeed => 10f;
        static float DashDecelerateRate => 0.9f;
        //maybe a burst of lightning on the spot the dash started?
        //do a homing curved dash
        private void AttackDash()
        {
            Player player = Main.player[NPC.target];
            Utils.ChaseResults chaseResults = Utils.GetChaseResults(NPC.Center, DashSpeed, player.Center, (player.position - player.oldPosition));
            Vector2 targetSpeed = SafeDirectionTo(player.Center) * DashSpeed;
            if (chaseResults.InterceptionHappens)
            {
                targetSpeed = chaseResults.ChaserVelocity;
            }
            targetSpeed = targetSpeed.SafeNormalize(Vector2.UnitY) * DashSpeed;
            if (Timer < DashStartup)
            {
                NPC.rotation = targetSpeed.ToRotation() + MathF.PI;
                MoveTo(player.Center + new Vector2(400 * -NPC.direction, -400), 50f, 0.1f);
            }
            else if (Timer < DashStartup + DashCount * DashRate)
            {
                float relativeTimer = Timer - DashStartup;
                if (relativeTimer % DashRate == 0)
                {
                    // DashLightningBurst();
                    SoundEngine.PlaySound(PureDarkMatter.DashSFX, NPC.Center);
                    NPC.velocity = targetSpeed;
                }

                NPC.velocity = NPC.velocity.MoveTowards(targetSpeed, DashHomingStrength).SafeNormalize(Vector2.Zero) * DashSpeed;

                relativeTimer %= DashRate;
                if (relativeTimer % DashPetalSpawnRate == 0 && relativeTimer < DashPetalSpawnRate * DashPetalCount)
                {
                    SpawnPetalsDuringDash();
                }
                if (relativeTimer < DashDuration)
                {
                    NPC.rotation = NPC.velocity.ToRotation() + MathF.PI;
                }
                else
                {
                    NPC.velocity *= DashDecelerateRate;
                    float targetAngle = (player.Center - NPC.Center).ToRotation() + MathF.PI;
                    NPC.rotation = NPC.rotation.AngleLerp(targetAngle, 0.1f);
                }
            }
            else if (Timer < DashStartup + DashCount * DashRate + DashExtraWait)
            {
                MoveTo(player.Center + new Vector2(-NPC.direction * 200, 0), 80f, 0.03f);
            }
            else
            {
                EndState();
            }
            float velocityCapToFixWeirdDashBug = DashSpeed;
            if (NPC.velocity.LengthSquared() > velocityCapToFixWeirdDashBug * velocityCapToFixWeirdDashBug)
            {
                NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero) * velocityCapToFixWeirdDashBug;
            }

        }

        private void SpawnPetalsDuringDash()
        {
            SoundEngine.PlaySound(PureDarkMatter.PetalThrowSFX with { MaxInstances = 0 }, NPC.Center);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            int petalCountInBurst = 2;
            for (int i = 0; i < petalCountInBurst; i++)
            {
                foreach (Player plr in Main.ActivePlayers)
                {
                    float angle = Utils.Remap(i, 0, petalCountInBurst, 0, MathF.Tau) + MathF.PI * .5f;
                    angle += NPC.velocity.ToRotation();
                    Vector2 vel = angle.ToRotationVector2() * DashPetalShootSpeed;
                    MatterOrbHoming.SpawnHomingOrb(NPC.GetSource_FromAI(), NPC.Center, vel, PetalDamage, plr.whoAmI);
                }
            }
        }

        void DashLightningBurst()
        {
            SoundEngine.PlaySound(PureDarkMatter.LaserSFX, NPC.Center);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            for (int i = 0; i < DashLightningBurstProjCount; i++)
            {
                float angle = Utils.Remap(i, 0, DashLightningBurstProjCount, 0, MathF.Tau);
                Vector2 spawnVel = angle.ToRotationVector2() * DashLightningShootSpeed;
                LightningProj.GetSpawningStats(angle, out float ai0, out float ai1);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, spawnVel, ModContent.ProjectileType<DarkMatterLaser>(), LaserDamage, 0f, -1, ai0, ai1);
            }
        }
        //fire homing ones in a clockwork pattern like EoL phase 2
        static int PetalsStartup => 30;
        int PetalsCount => (Main.getGoodWorld ? 17 : Main.expertMode ? 14 : 10) + (Phase2 ? 10 : 0);
        int PetalsRate => Phase2 ? 2 : 3;
        int PetalsExtraWait => 200 - (Main.getGoodWorld ? 60 : Main.expertMode ? 40 : 0) - (Phase2 ? 15 : 0);
        static float PetalShootSpeed => 20;
        static float PetalHomingStrength => 0.02f;
        static float PetalHomingMaxVel => 25;
        private void AttackPetals()
        {
            Player player = Main.player[NPC.target];
            Vector2 targetPos = player.Center;
            Vector2 toTarget = NPC.DirectionTo(targetPos);
            Vector2 moveTarget = targetPos - toTarget * 450;
            NPC.direction = MathF.Sign(player.Center.X - NPC.Center.X);
            NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.spriteDirection == -1 ? 0f : MathF.PI;
            if (Timer < PetalsStartup)
            {
                moveTarget = player.Center - new Vector2(NPC.direction * 400, -200);
                MoveTo(moveTarget, 50, 0.12f);
            }
            else if (Timer < PetalsStartup + PetalsCount * PetalsRate)
            {
                NPC.velocity *= 0.9f;
                if (Timer == PetalsStartup)
                {
                    SoundEngine.PlaySound(PureDarkMatter.PetalThrowSFX, NPC.Center);
                }
                if ((Timer - PetalsStartup) % PetalsRate == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int petalIndex = (int)(Timer - PetalsStartup) / PetalsRate;
                        float angle = Utils.Remap(petalIndex, 0, PetalsCount, 0, MathF.Tau);
                        Vector2 petalVel = angle.ToRotationVector2() * PetalShootSpeed;
                        foreach (Player plr in Main.ActivePlayers)
                        {
                            petalVel.X *= MathF.Sign(plr.position.X - NPC.position.X);
                            MatterOrbHoming.SpawnHomingOrb(NPC.GetSource_FromAI(), NPC.Center, petalVel, PetalDamage, plr.whoAmI, PetalHomingStrength, PetalHomingMaxVel);
                        }
                    }
                }
            }
            else if (Timer < PetalsStartup + PetalsCount * PetalsRate + PetalsExtraWait)
            {
                MoveTo(moveTarget, 30, 0.13f);
            }
            else
            {
                EndState();
            }
        }
        static Vector2 AngleLerp(Vector2 v1, Vector2 v2, float t)
        {
            if (v1.HasNaNs() || v2.HasNaNs())
            {
                return Vector2.Zero;
            }
            float mag1 = v1.Length();
            float mag2 = v2.Length();
            if (mag1 == 0 && mag2 == 0)
                return Vector2.Zero;
            float mag = MathHelper.Lerp(mag1, mag2, t);
            Vector2 dir1 = mag1 > 0 ? v1 / mag1 : Vector2.Zero;
            Vector2 dir2 = mag2 > 0 ? v2 / mag2 : Vector2.Zero;
            if (dir1 == Vector2.Zero)
                return dir2 * mag;
            if (dir2 == Vector2.Zero)
                return dir1 * mag;
            float dot = Vector2.Dot(dir1, dir2);
            dot = MathHelper.Clamp(dot, -1.0f, 1.0f); // Avoid precision issues
            float angle = (float)Math.Acos(dot);

            if (Math.Abs(angle) < 1e-5)
            {
                return Vector2.Lerp(v1, v2, t);
            }
            Vector2 resultDir =
                (float)Math.Sin((1 - t) * angle) / (float)Math.Sin(angle) * dir1 +
                (float)Math.Sin(t * angle) / (float)Math.Sin(angle) * dir2;
            Vector2 result = resultDir * mag;
            if (result.HasNaNs())
            {
                return Vector2.Zero;
            }
            return result;
        }


        private void EndState()
        {
            LastAttackType = CurrentAttackType;
            CurrentAttackType = AtkType.DecideNext;
        }

        void MoveTo(Vector2 targetPos, float maxVel, float lerpSmoothingT)
        {
            float dist = NPC.Center.Distance(targetPos);
            if (!float.IsNormal(dist))
            {
                dist = 16 * 5;
            }
            maxVel *= Utils.GetLerpValue(16, 16 * 5, dist, true);
            Vector2 directionToTarget = (targetPos - NPC.Center).SafeNormalize(Vector2.Zero);
            NPC.velocity = Vector2.Lerp(NPC.velocity, directionToTarget * maxVel, lerpSmoothingT);
        }
        public override void OnKill()
        {
            NPC.NewNPC(NPC.GetSource_Death(), (int)((NPC.width / 2 + NPC.position.X) / 16), (int)((NPC.height / 2 + NPC.position.Y) / 16), ModContent.NPCType<Zero>());
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = TextureAssets.Npc[Type].Value;
            //if (!Main.gamePaused)
            //{
            //    FindFrame(tex.Height);
            //}
            drawColor = NPC.GetNPCColorTintedByBuffs(drawColor);
            drawColor = NPC.GetAlpha(drawColor);
            Vector2 drawPos = NPC.Center - screenPos;
            float rotation = NPC.rotation;
            SpriteEffects fx = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (NPC.spriteDirection != -1)
            {
                rotation += MathF.PI;
            }
            spriteBatch.Draw(tex, drawPos, NPC.frame, drawColor, rotation, NPC.frame.Size() / 2, NPC.scale, fx, 0f);
            return false;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 80; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                {
                    Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
                    Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 4, 10, Scale: 2); //Makes dust in a messy circle
                    d.noGravity = true;
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                    Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 2, 2); //Makes dust in a messy circle
                    d.noGravity = false;
                }
            }
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter < 8.0)
            {
                NPC.frame.Y = 0; //idle
            }
            else if (NPC.frameCounter < 16.0)
            {
                NPC.frame.Y = frameHeight;
            }
            else if (NPC.frameCounter < 24.0)
            {
                NPC.frame.Y = frameHeight * 2;
            }
            else if (NPC.frameCounter < 32.0)
            {
                NPC.frame.Y = frameHeight * 3;
            }
            else
            {
                NPC.frameCounter = 0.0; //reset
            }
        }

    }
}
