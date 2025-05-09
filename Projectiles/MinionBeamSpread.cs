using KirboMod.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class MinionBeamSpread : ModProjectile
    {
        private float Timer
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] += 16 * 30;
        }

        public override string Texture => "KirboMod/NothingTexture";

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.WhipSettings.RangeMultiplier = 0.5f;
            Projectile.WhipSettings.Segments = 8;
            Projectile.extraUpdates = 3;
        }
        public static int OffsetLength => 10;
        public override void AI()
        {
            WaddleDooMinion dooOwner = Main.projectile.FirstOrDefault(p => p.identity == (int)Projectile.ai[0]).ModProjectile as WaddleDooMinion;
            if (dooOwner == null || !dooOwner.Projectile.active || dooOwner.aggroTarget == null)
            {
                Projectile.Kill();
                return;
            }
            float timeToFlyOut = WaddleDooMinion.AttackDuration * Projectile.MaxUpdates;
            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2f;//leaving it like this incase vanilla code uses it
            Timer += 1f;
            Projectile.Center = dooOwner.Projectile.Center + Vector2.UnitX * OffsetLength * dooOwner.Projectile.spriteDirection - Projectile.velocity + dooOwner.Projectile.velocity;
            Projectile.spriteDirection = (!(Vector2.Dot(Projectile.velocity, Vector2.UnitX) < 0f)) ? 1 : (-1);
            if (Timer % (4 * Projectile.MaxUpdates) == 1 && Timer < timeToFlyOut - (4 * Projectile.MaxUpdates))
            {
                if(Timer + (4 * Projectile.MaxUpdates) >= timeToFlyOut - (4 * Projectile.MaxUpdates))
                {
                    SoundEngine.PlaySound(WaddleDoo.BeamAttackEnd, Projectile.Center);
                }
                else
                {
                    SoundEngine.PlaySound(WaddleDoo.BeamAttackLoop, Projectile.Center);
                }
            }
            if (Timer >= timeToFlyOut)
            {
                Projectile.Kill();
                return;
            }
        }
        public override void PostDraw(Color lightColor)
        {
            List<Vector2> list = new();
            FillWhipControlPoints(list);
            for (int i = 0; i < list.Count; i++)
            {
                Vector2 pos = list[i];

                //if (pos.Distance(prevDrawnPos) > prevDrawnRadius)
                {
                    float prevDrawnRadius = Utils.Remap(i, 0, list.Count - 1, 0.5f, 1f);
                    VFX.DrawWaddleDooBeam(pos, prevDrawnRadius, 1f);
                }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Projectile.WhipPointsForCollision.Clear();
            FillWhipControlPoints(Projectile.WhipPointsForCollision);
            for (int m = 0; m < Projectile.WhipPointsForCollision.Count; m++)
            {
                Point point = Projectile.WhipPointsForCollision[m].ToPoint();
                projHitbox.Location = new Point(point.X - projHitbox.Width / 2, point.Y - projHitbox.Height / 2);
                if (projHitbox.Intersects(targetHitbox))
                {
                    return true;
                }
            }
            return false;
        }
        void GetWhipSettings(out float timeToFlyOut, out int segments, out float rangeMultiplier)
        {
            timeToFlyOut = WaddleDooMinion.AttackDuration * Projectile.MaxUpdates;
            segments = 10;
            rangeMultiplier = 0.5f;
        }
        void FillWhipControlPoints(List<Vector2> controlPoints)
        {
            WaddleDooMinion dooOwner = Main.projectile.FirstOrDefault(p => p.identity == (int)Projectile.ai[0]).ModProjectile as WaddleDooMinion;
            if (dooOwner == null || !dooOwner.Projectile.active || dooOwner.aggroTarget == null)
            {
                Projectile.Kill();
                return;
            }
            GetWhipSettings(out float timeToFlyOut, out int segments, out float rangeMultiplier);
            float num = Timer / timeToFlyOut;
            float num2 = 0.5f;
            float num3 = 1f + num2;
            float num4 = (float)Math.PI * 10f * (1f - num * num3) * -Projectile.spriteDirection / segments;
            float num5 = num * num3;
            float num6 = 0f;
            if (num5 > 1f)
            {
                num6 = (num5 - 1f) / num2;
                num5 = MathHelper.Lerp(1f, 0f, num6);
            }
            float whatTheHellIsThis = Timer - 1f;
            whatTheHellIsThis = WaddleDooMinion.AttackDuration * 2 * num;
            float num8 = Projectile.velocity.Length() * whatTheHellIsThis * num5 * rangeMultiplier / segments;
            float num9 = 1f;
            Vector2 startPosition = Projectile.Center + Vector2.UnitX * OffsetLength * dooOwner.Projectile.spriteDirection;
            Vector2 vector = startPosition;
            float num10 = 0f - (float)Math.PI / 2f;
            Vector2 vector2 = vector;
            float num11 = 0f + (float)Math.PI / 2f + (float)Math.PI / 2f * Projectile.spriteDirection;
            Vector2 vector3 = vector;
            float num12 = 0f + (float)Math.PI / 2f;
            controlPoints.Add(startPosition);
            for (int i = 0; i < segments; i++)
            {
                float num13 = i / (float)segments;
                float num14 = num4 * num13 * num9;
                Vector2 vector4 = vector + num10.ToRotationVector2() * num8;
                Vector2 vector5 = vector3 + num12.ToRotationVector2() * (num8 * 2f);
                Vector2 vector6 = vector2 + num11.ToRotationVector2() * (num8 * 2f);
                float num15 = 1f - num5;
                float num16 = 1f - num15 * num15;
                Vector2 value = Vector2.Lerp(vector5, vector4, num16 * 0.9f + 0.1f);
                Vector2 vector7 = Vector2.Lerp(vector6, value, num16 * 0.7f + 0.3f);
                Vector2 spinningpoint = startPosition + (vector7 - startPosition) * new Vector2(1f, num3);
                float num17 = num6;
                num17 *= num17;
                Vector2 item = spinningpoint.RotatedBy(Projectile.rotation + 4.712389f * num17 * Projectile.spriteDirection, startPosition);
                controlPoints.Add(item);
                num10 += num14;
                num12 += num14;
                num11 += num14;
                vector = vector4;
                vector3 = vector5;
                vector2 = vector6;
            }
        }
    }
}