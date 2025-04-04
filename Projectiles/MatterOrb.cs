using KirboMod.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Reflection.Metadata;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class MatterOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;

            //for afterimages
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 17; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
        }
        static int BlastSize => 60;
        ref float Timer => ref Projectile.localAI[0];
        ref float BallIndex => ref Projectile.ai[0];
        int BallDelayIndex => SpawnIndexToDelayIndex(Projectile.ai[0]);
        int DarkMatterNPCIndex => (int)Projectile.ai[1];
        ref float DirectionSign => ref Projectile.ai[2];
        static int ThrowDelay => 20;
        static int PerIndexExtraThrowDelay => 8;
        float ThrowTime => ThrowDelay + BallIndex * PerIndexExtraThrowDelay;
        static int FlyDuration => 30;
        float ExplodeTime => ThrowTime + FlyDuration;
        static float ExplodeDuration => 35;
        float DeathTime => ExplodeTime + ExplodeDuration;
        static float LaunchSpeed => 19;
        public static void GetAIValues(float directionSign, int bossWhoAmI, int i, out float ai0, out float ai1, out float ai2, out Vector2 velocity)
        {

            ai0 = i;
            ai1 = bossWhoAmI;
            ai2 = directionSign;
            float maxAngle = 1.5f;
            i = SpawnIndexToDelayIndex(i);
            float angle = Utils.Remap(i, 0, 3, -maxAngle, maxAngle);
            angle -= MathF.PI / 2;
            velocity = new(MathF.Sin(angle), MathF.Cos(angle));
            velocity.X *= directionSign;
        }
        static int SpawnIndexToDelayIndex2(float i)
        {
            if (i == 2)
                return 0;
            if (i == 0)
                return 2;
            if (i == 1)
                return 3;
            if (i == 3)
                return 1;
            return (int)i;
        }

        private static int SpawnIndexToDelayIndex(float i)
        {

            if (i == 1)
                return 3;
            if (i == 3)
                return 1;
            return (int)i;
        }

        public override void AI()
        {
            Timer++;
            NPC darkMatter = Main.npc[DarkMatterNPCIndex];
            if (Timer < ThrowTime)
            {
                float progress = Timer / (ThrowTime - 1);
                if (!darkMatter.active || darkMatter.type != ModContent.NPCType<PureDarkMatter>())
                {
                    Projectile.Kill();
                    return;
                }
                progress = 1 - progress;
                progress *= progress * progress * progress;
                progress = 1 - progress;
                Projectile.Center = darkMatter.Center + Projectile.velocity * progress * 170;
                return;
            }

            float index = BallDelayIndex;
            int delayIndex = BallDelayIndex;
            float relativeTimer = Timer - ThrowTime;
            //-1 if it is 0 or 1, 1 if it is 2 or 3.
            int verticalSide = (delayIndex + 2) / 4 * 2 - 1;
            float startOffset = 0.5f;
            float sineSpeed = (startOffset + MathF.PI) / FlyDuration;
            float maxSpeed = 12f;
            if (index == 0 || index == 3)
            {
                Projectile.velocity.Y -= verticalSide * (LaunchSpeed / (FlyDuration / 2));
            }
            else
            {
                float accel = MathF.Cos(relativeTimer * sineSpeed + (verticalSide * MathF.PI * 0.5f) - startOffset) * maxSpeed;
                Projectile.velocity.Y = accel;
            }
            if (Timer == ThrowTime)
            {
                SoundEngine.PlaySound(PureDarkMatter.PetalThrowSFX, darkMatter.Center);
                if (index == 0 || index == 3)
                {
                    Projectile.velocity = new Vector2(DirectionSign * LaunchSpeed, LaunchSpeed * verticalSide);
                }
                else
                {
                    Projectile.velocity = new Vector2(DirectionSign * LaunchSpeed, MathF.Cos(relativeTimer * sineSpeed + (verticalSide * MathF.PI * 0.5f) - startOffset) * maxSpeed);
                }
            }
            if (Timer > ExplodeTime)
            {
                Projectile.alpha = 255;
                relativeTimer = Timer - ExplodeTime - 1;
                if (relativeTimer % 10 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item14 with {  MaxInstances = 0 }, Projectile.Center);
                }
                if (relativeTimer % 3 == 0)
                {
                   
                    int dustCount = 16;
                    float deviation = 5;
                    int lineDustCount = 4;
                    for (int i = 0; i < lineDustCount; i++)
                    {
                        Vector2 dustPos = Projectile.Center;
                        Vector2 dustVel = Projectile.velocity.Normalized((float)i / lineDustCount * 8f);
                        Dust d = Dust.NewDustPerfect(dustPos + dustVel, ModContent.DustType<Dusts.DarkResidue>(), dustVel, 0, Color.White, 1f);
                        
                    }
                    for (int i = 0; i < dustCount; i++)
                    {
                        float angle = MathF.Tau * ((float)i / dustCount);
                        Vector2 dustVel = new(MathF.Cos(angle), MathF.Sin(angle));
                        Vector2 dustPos = Projectile.Center + dustVel * 30;
                        dustVel *= 6; 
                        dustPos.X += Main.rand.NextFloat(-deviation, deviation);
                        dustVel.X += Main.rand.NextFloat(-deviation, deviation);
                        dustVel.Y += Main.rand.NextFloat(-deviation, deviation);
                        dustPos.Y += Main.rand.NextFloat(-deviation, deviation);
                        Dust d = Dust.NewDustPerfect(dustPos, ModContent.DustType<Dusts.DarkResidue>(), dustVel, 0, Color.White, 1f);
                        d.noGravity = true;
                        d.scale *= 1.3f;
                    }
                }
                //for (int i = 0; i < 2; i++)
                //{
                //    Vector2 gorePos = Projectile.Center;
                //    gorePos.X += Main.rand.NextFloat(-BlastSize, BlastSize);
                //    gorePos.Y += Main.rand.NextFloat(-BlastSize, BlastSize);
                //    Gore.NewGoreDirect(Projectile.GetSource_FromThis(), gorePos, Projectile.velocity, Utils.SelectRandom(Main.rand, GoreID.Smoke1, GoreID.Smoke2, GoreID.Smoke3), 1.2f);
                   
                //}

            }
            if (Timer > DeathTime)
            {
                Projectile.Kill();
            }
        }
        //for explosions, make it several dust circles and several custom sparkle particles, and smoke gore
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (Timer > ExplodeTime)
            {
                hitbox = Utils.CenteredRectangle(Projectile.Center, new Vector2(BlastSize));
            }
        }
        Rectangle GetExplosionDamageHitbox()
        {
            return Utils.CenteredRectangle(Projectile.Center, new Vector2(BlastSize));
        }
        public override bool ShouldUpdatePosition()
        {
            return Timer >= ThrowTime;
        }
        private void AI_Old()
        {
            Projectile.ai[2] += 1 / 60f;
            Projectile.ai[2] %= 3600;
            Player player = Main.player[(int)Projectile.ai[1]]; //chooses player that was already being targeted by npc

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                player = Main.player[Main.myPlayer];
            }

            Vector2 move = player.Center - Projectile.Center;

            Projectile.ai[0]++;

            if (Projectile.ai[0] < 20)
            {
                Projectile.velocity *= 0.95f;
            }
            else if (Projectile.ai[0] == 20)
            {
                Projectile.hostile = true;

                if (move.X > 0) //going right
                {
                    Projectile.velocity.X = 30 + MathF.Cos(Projectile.ai[2] * 2) * 25;
                }
                else //going left
                {
                    Projectile.velocity.X = -30 + MathF.Cos(Projectile.ai[2] * 2) * 25;
                }

                if (Projectile.velocity.Y < 0)
                {
                    Projectile.localAI[2] = 1;
                    Projectile.velocity.Y = -50; //start up
                }
                else
                {
                    Projectile.velocity.Y = 50; //start down
                }
            }
            else if (Projectile.ai[0] > 20)
            {
                if (Projectile.localAI[2] == 1) //go down
                {
                    Projectile.velocity.Y += 4f;

                    Projectile.velocity.X *= 0.98f;
                }
                else //go up
                {
                    Projectile.velocity.Y -= 4f;

                    Projectile.velocity.X *= 0.98f;
                }
            }

            if (++Projectile.frameCounter >= 5) //changes frames every 5 ticks 
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override void OnKill(int timeLeft) //when the projectile dies
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, ModContent.DustType<Dusts.DarkResidue>(), speed, Scale: 1f); //Makes dust in a messy circle
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; //white
        }


        public static Asset<Texture2D> afterimage;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D main = TextureAssets.Projectile[Type].Value;
            int skipCount = 4;
            Rectangle frame = main.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            for (int i = Projectile.oldPos.Length - 1; i >= 0; i -= skipCount)
            {
                int index = i + (int)Timer % skipCount - skipCount;
                if (index < 0 || index >= Projectile.oldPos.Length || Timer - index > ExplodeTime)
                {
                    continue;
                }
                Vector2 oldPos = Projectile.oldPos[index] + Projectile.Size / 2;
                float progress = (float)index / Projectile.oldPos.Length;
                Color c = Color.White;
                c *= 1 - progress;
                Main.EntitySpriteDraw(main, oldPos - Main.screenPosition, frame, c, 0, frame.Size() / 2, 1f, SpriteEffects.None, 0f);
            }
            return Projectile.DrawSelf(Color.White);
        }


    }
}