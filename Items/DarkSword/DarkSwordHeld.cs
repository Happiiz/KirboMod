using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.DarkSword
{
    public class DarkSwordHeld : ModProjectile
    {
        public override string Texture => "KirboMod/Items/DarkSword/DarkSword";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 40;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.Size = new(30);
            Projectile.extraUpdates = 6;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.scale = 1;
        }
        ref float Timer { get => ref Projectile.localAI[0]; }
        ref float TrailCancelCount { get => ref Projectile.localAI[1]; }
        ref float UseTime { get => ref Projectile.ai[0]; }
        ref float SwingAngle { get => ref Projectile.ai[1]; }
        ref float SwingDir { get => ref Projectile.ai[2]; }
        bool Dead { get => Progress > 1; }
        float Progress { get => Utils.GetLerpValue(0, Projectile.ai[0], Projectile.localAI[0]); }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale *= Main.player[Projectile.owner].GetAdjustedItemScale(Main.player[Projectile.owner].HeldItem);
            UseTime *= Projectile.MaxUpdates;
            Projectile.velocity.Normalize();
            Main.instance.LoadProjectile(Type);
        }
        List<Vector2> SparklePoints = new();
        Vector2[] oldRotCenters = new Vector2[40];
        List<float> rotations = new();
        List<Vector2> centers = new();
        static Effect rainbowEffect;
        static float EaseBackOut(float progress)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            return 1 + c3 * MathF.Pow(progress - 1, 3) + c1 * MathF.Pow(progress - 1, 2);
        }
        static float Derivative(float progress)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            float derivative = 3 * c3 * MathF.Pow(progress - 1, 2) + 2 * c1 * (progress - 1);
            return derivative;
        }
        static float AngleLerpLongWay(float curAngle, float targetAngle, float amount)
        {
            float angle;
            if (targetAngle < curAngle)
            {
                float num = targetAngle + (float)Math.PI * 2f;
                angle = ((num - curAngle > curAngle - targetAngle) ? MathHelper.Lerp(curAngle, num, amount) : MathHelper.Lerp(curAngle, targetAngle, amount));
            }
            else
            {
                if (!(targetAngle > curAngle))
                {
                    return curAngle;
                }
                float num = targetAngle - (float)Math.PI * 2f;
                angle = ((targetAngle - curAngle > curAngle - num) ? MathHelper.Lerp(curAngle, targetAngle, amount) : MathHelper.Lerp(curAngle, num, amount));
            }
            return MathHelper.WrapAngle(angle);
        }
        public float VisualRotation { get => Projectile.rotation - MathF.PI / 4; }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Dead)
            {
                Projectile.timeLeft = 100;
                if (TrailCancelCount > Projectile.oldPos.Length)
                    Projectile.Kill();
                TrailCancelCount++;
                return;
            }
            float progress = EaseBackOut(Progress);
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (SwingDir == -1)
                progress = 1 - progress;
            Projectile.rotation += MathF.PI / 4;
            if (SwingAngle >= MathF.PI)
            {
                Projectile.rotation = AngleLerpLongWay(Projectile.rotation - SwingAngle / 2f, Projectile.rotation + SwingAngle / 2f, progress);
            }
            else
            {
                Projectile.rotation = Utils.AngleLerp(Projectile.rotation - SwingAngle / 2f, Projectile.rotation + SwingAngle / 2f, progress);
            }
            if (SwingDir == -1)
                progress = 1 - progress;
            MakePlayerHoldMe();
            Timer++;
            GetInterpolatedPoints(out rotations, out centers);
            for (int i = oldRotCenters.Length - 1; i >= 1; i--)
            {
                oldRotCenters[i] = oldRotCenters[i - 1];
            }
            oldRotCenters[0] = GetArmRotationCenterAdjustForUpdates();
        }
        Vector2 GetArmRotationCenter()
        {
            Player player = Main.player[Projectile.owner];
            return player.RotatedRelativePoint(player.MountedCenter + new Vector2(-4f * player.direction, -2f));
        }
        Vector2 GetArmRotationCenterAdjustForUpdates()
        {
            return GetArmRotationCenter() + GetExtraUpdateAdjustmentOffset(Main.player[Projectile.owner]);
        }
        private void MakePlayerHoldMe()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            if (UseTime - Timer > 1)
                player.SetDummyItemTime(2);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, VisualRotation - MathF.PI / 2 - player.fullRotation);
            Vector2 dirToPlayer = VisualRotation.ToRotationVector2();
            Projectile.position = player.RotatedRelativePoint(player.GetFrontHandPosition(player.compositeFrontArm.stretch, player.compositeFrontArm.rotation)) - Projectile.Size / 2f + GetExtraUpdateAdjustmentOffset(player);
            Projectile.position += dirToPlayer * 170 * Projectile.scale;
        }
        private Vector2 GetExtraUpdateAdjustmentOffset(Player player)
        {
            return Utils.GetLerpValue(0, Projectile.MaxUpdates, Projectile.numUpdates + 1) * (player.oldPosition - player.position);
        }
        void GetInterpolatedPoints(out List<float> rotations, out List<Vector2> centers)
        {
            rotations = new();
            centers = new();
            float lerpAmount = (UseTime / Projectile.MaxUpdates) <= 20 ? 0.33f : 0.5f;
            for (int i = (int)TrailCancelCount; i < Projectile.oldPos.Length; i++)
            {
                for (float j = 0; j < 0.98f; j += lerpAmount)
                {
                    if (Projectile.oldPos[i] == Vector2.Zero && Projectile.oldRot[i] == 0)
                        continue;
                    Vector2 rotCenter = Vector2.Lerp(i == 0 ? GetArmRotationCenterAdjustForUpdates() : oldRotCenters[i - 1], oldRotCenters[i], j);
                    centers.Add(VectorAngleLerp(i == 0 ? Projectile.position : Projectile.oldPos[i - 1], Projectile.oldPos[i], j, rotCenter) + Projectile.Size / 2f - Main.screenPosition);
                    rotations.Add(Utils.AngleLerp(i == 0 ? Projectile.rotation : Projectile.oldRot[i - 1], Projectile.oldRot[i], j));
                }
            }
        }
        static Vector2 VectorAngleLerp(Vector2 vec1, Vector2 vec2, float t, Vector2? center = null)
        {
            center ??= Vector2.Zero;
            vec1 -= center.Value;
            vec2 -= center.Value;
            float magnitude = MathHelper.Lerp(vec1.Length(), vec2.Length(), t);
            float vec1Angle = vec1.ToRotation();
            float vec2Angle = vec2.ToRotation();
            return Utils.AngleLerp(vec1Angle, vec2Angle, t).ToRotationVector2() * magnitude + center.Value;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texBack = ModContent.Request<Texture2D>("KirboMod/Items/RainbowSword/RainbowSwordHeldGlow").Value;
            Texture2D texMain = TextureAssets.Projectile[Type].Value;
            Vector3 hslvec = Main.rgbToHsl(Main.DiscoColor);
            float timeLeft = UseTime - Timer;

            float fade = Utils.GetLerpValue(0, 5, timeLeft, true);
            for (int i = rotations.Count - 1; i >= 1; i--)
            {
                float brightness = centers[i].Distance(centers[i - 1]);
                brightness *= 0.02f;
                (float toMin, float toMax) lightnessRemap = (0.5f, 0.85f);
                if (Main.dayTime)
                {
                    lightnessRemap.toMin = 0.5f;
                    lightnessRemap.toMax = 1;
                    brightness *= 4;
                }
                float lightness = Utils.Remap(i, rotations.Count, 0.1f, lightnessRemap.toMin, lightnessRemap.toMax);
                hslvec.Z = lightness;
                Color col = Main.hslToRgb(hslvec);
                if (!Main.dayTime)
                    col.A = 0;

                brightness *= Utils.GetLerpValue(-Projectile.oldPos.Length, 0, timeLeft, true);
                brightness *= fade;
                Main.EntitySpriteDraw(texBack, centers[i], null, col * brightness * Utils.GetLerpValue(rotations.Count, rotations.Count / 2f, i, true), rotations[i], texBack.Size() / 2, Projectile.scale, SpriteEffects.None);
            }

            if (Dead)
                return false;
            Main.EntitySpriteDraw(texMain, Projectile.Center - Main.screenPosition, null, Color.White * fade, Projectile.rotation, texMain.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - VisualRotation.ToRotationVector2() * 170 * Projectile.scale, Projectile.Center + VisualRotation.ToRotationVector2() * 170 * Projectile.scale, 70, ref a);

        }
    }
}
