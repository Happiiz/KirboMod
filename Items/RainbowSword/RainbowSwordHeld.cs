using Terraria;
using Terraria.ModLoader;
using System;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.Audio;

namespace KirboMod.Items.RainbowSword
{
    internal class RainbowSwordHeld : ModProjectile
    {

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
            Projectile.extraUpdates = 0;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.scale = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
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
            LoadShaderIfNeeded();
            Projectile.scale *= Main.player[Projectile.owner].GetAdjustedItemScale(Main.player[Projectile.owner].HeldItem);
            UseTime *= Projectile.MaxUpdates;
            Projectile.velocity.Normalize();
            Main.instance.LoadProjectile(Type);
        }
        List<Rectangle> hitboxes = new();
        List<Vector2> SparklePoints = new();
        List<Rectangle> GetInterpolatedHitboxes()
        {
            List<float> rotations = new();
            List<Vector2> centers = new();
            SparklePoints = new();

            for (int i = (int)TrailCancelCount; i < Projectile.oldPos.Length; i++)
            {
                for (float j = 0; j < 0.99; j += 0.066666f)
                {
                    if (Projectile.oldPos[i] == Vector2.Zero && Projectile.oldRot[i] == 0)
                        continue;
                    Vector2 rotCenter = Vector2.Lerp(i == 0 ? GetArmRotationCenterAdjustForUpdates() : oldRotCenters[i - 1], oldRotCenters[i], j);
                    Vector2 newcenter = VectorAngleLerp(i == 0 ? Projectile.position : Projectile.oldPos[i - 1], Projectile.oldPos[i], j, rotCenter) + Projectile.Size / 2f;
                    centers.Add(newcenter);
                    float newRot = Utils.AngleLerp(i == 0 ? Projectile.rotation : Projectile.oldRot[i - 1], Projectile.oldRot[i], j);
                    rotations.Add(newRot);

                    SparklePoints.Add((newRot - MathF.PI / 4).ToRotationVector2() * 140 * Projectile.scale + newcenter);

                }
            }

            List<Rectangle> result = new();
            for (int i = 0; i < centers.Count; i++)
            {
                for (int j = -100; j < 151; j+= 25)
                {
                    result.Add(Utils.CenteredRectangle((rotations[i] - MathF.PI / 4).ToRotationVector2() * j * Projectile.scale + centers[i], Vector2.One * 40));
                }
            }
            return result;
        }
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
        float GetSparkleVel(Vector2 hitPoint)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 origin = player.RotatedRelativePoint(player.GetFrontHandPosition(player.compositeFrontArm.stretch, player.compositeFrontArm.rotation));
            return Derivative(Progress) * Utils.Remap(hitPoint.Distance(origin) * 0.02f, 0, 20, 2, 20, false);
        }
        float GetSparkleVelAdv(Vector2 hitPoint, float extraProgress)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 origin = player.RotatedRelativePoint(player.GetFrontHandPosition(player.compositeFrontArm.stretch, player.compositeFrontArm.rotation));
            return Derivative(Progress + extraProgress) * Utils.Remap(hitPoint.Distance(origin) * 0.02f, 0, 20, 2, 20, false);
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
            ProjectileDeflection();
            Timer++;
            Vector2 lightStart, lightEnd;
            AddLight(player, out lightStart, out lightEnd);
            SparklesFromSwing(progress, lightStart, lightEnd);


            for (int i = oldRotCenters.Length - 1; i >= 1; i--)
            {
                oldRotCenters[i] = oldRotCenters[i - 1];
            }
            oldRotCenters[0] = GetArmRotationCenterAdjustForUpdates();
        }
        private void ProjectileDeflection()
        {
            Projectile projToCheck;
            const float distance = 400 * 400;
            Projectile me = Projectile;
            hitboxes = GetInterpolatedHitboxes();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                projToCheck = Main.projectile[i];
                if (!projToCheck.hostile || !projToCheck.active || projToCheck.damage < 1 || projToCheck.DistanceSQ(me.Center) > distance || projToCheck.width >= 30 || projToCheck.height >= 30 || projToCheck.type == ProjectileID.FairyQueenLance)
                    continue;
                Rectangle hitboxToCheck = new Rectangle((int)(projToCheck.position.X + (projToCheck.velocity.X * (projToCheck.extraUpdates + 1))), (int)(projToCheck.position.Y + projToCheck.velocity.Y * (projToCheck.extraUpdates + 1)), projToCheck.width, projToCheck.height);
                if (Colliding(default,hitboxToCheck).Value || Colliding(default,projToCheck.Hitbox).Value)
                {
                    HitEffect(projToCheck.Hitbox);
                    Vector2 origin = GetOrigin();
                    projToCheck.velocity += Vector2.Normalize(Projectile.Center - origin).RotatedBy(MathF.PI / 2 * SwingDir) * GetSparkleVel(projToCheck.Hitbox.Center());
                    projToCheck.hostile = false;
                    projToCheck.friendly = true;
                    projToCheck.localNPCHitCooldown = 10;
                    projToCheck.usesLocalNPCImmunity = true;
                }
               
            }
        }
        private void SparklesFromSwing(float progress, Vector2 lightStart, Vector2 lightEnd)
        {
            for (int i = 0; i < MathF.Min(15,SparklePoints.Count - 1) && progress < 1; i++)
            {
                float extraProgress = Utils.GetLerpValue(0, 15, i) / UseTime;
                Vector2 spawnPos = SparklePoints[i];
                Vector2 origin = GetOrigin();
                float backFactor = Main.rand.NextFloat();
                backFactor = 1 - (1 - backFactor) * (1 - backFactor);
                Vector2 spawnPosOffset = Vector2.Lerp(origin, spawnPos, MathHelper.Lerp(0.5f, 1f, backFactor));
                Sparkle sparkle = Sparkle.NewSparkle(spawnPos + (spawnPosOffset - spawnPos), Main.DiscoColor, new Vector2(1, 1.5f) * 1.5f, Vector2.Normalize(SparklePoints[i + 1] - SparklePoints[i]).RotatedBy(SwingDir * MathF.PI).RotatedByRandom(0.3f) * GetSparkleVelAdv(spawnPos, -extraProgress) * backFactor, 30, Vector2.One * 1.5f, null, 1, 0, 0.95f);
                sparkle.Rotation = sparkle.Velocity.ToRotation() + MathF.PI / 2f;
            }
        }
        private void AddLight(Player player, out Vector2 lightStart, out Vector2 lightEnd)
        {
            lightStart = Projectile.Center - (Projectile.rotation - MathF.PI / 4).ToRotationVector2() * 170;
            lightEnd = Projectile.Center + (Projectile.rotation - MathF.PI / 4).ToRotationVector2() * 170;
            float dist = lightStart.Distance(lightEnd);
            for (int i = 0; i < hitboxes.Count; i+= 1)
            {
                Lighting.AddLight(hitboxes[i].Center(), Main.DiscoColor.ToVector3() * 3f);
            }

            Lighting.AddLight(player.Center, Main.DiscoColor.ToVector3() * 4);
            lightEnd = (lightEnd - lightStart) * 0.8f * Projectile.scale + lightStart ;
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
        Vector2 GetOrigin()
        {
            Player player = Main.player[Projectile.owner];
            return player.RotatedRelativePoint(player.GetFrontHandPosition(player.compositeFrontArm.stretch, player.compositeFrontArm.rotation));
        }
        static void SetShaderParams()
        {
            rainbowEffect.Parameters["s"].SetValue(1);
            rainbowEffect.Parameters["l"].SetValue(0.5f);
            rainbowEffect.Parameters["uOpacity"].SetValue(1);
            rainbowEffect.Parameters["gradientScale"].SetValue(2f);
            rainbowEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 3f);
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
            return  Utils.GetLerpValue(0, Projectile.MaxUpdates, Projectile.numUpdates + 1) * (player.oldPosition - player.position);
        }
        Vector2[] oldRotCenters = new Vector2[40];
        void GetInterpolatedPoints(out List<float> rotations, out List<Vector2> centers)
        {
            rotations = new();
            centers = new();
            for (int i = (int)TrailCancelCount; i < Projectile.oldPos.Length; i++)
            {
                for (float j = 0; j < 0.98f; j+= 0.05f)
                {
                    if (Projectile.oldPos[i] == Vector2.Zero && Projectile.oldRot[i] == 0)
                        continue;
                    Vector2 rotCenter = Vector2.Lerp(i == 0 ? GetArmRotationCenterAdjustForUpdates() : oldRotCenters[i - 1], oldRotCenters[i], j);
                    centers.Add(VectorAngleLerp (i == 0 ? Projectile.position : Projectile.oldPos[i - 1], Projectile.oldPos[i], j,rotCenter) + Projectile.Size / 2f - Main.screenPosition);
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
        static Effect rainbowEffect;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texBack = ModContent.Request<Texture2D>("KirboMod/Items/RainbowSword/RainbowSwordHeldGlow").Value;
            Texture2D texMain = TextureAssets.Projectile[Type].Value;
            Vector3 hslvec = Main.rgbToHsl(Main.DiscoColor);
            GetInterpolatedPoints(out List<float> rotations, out var centers);
            float timeLeft = UseTime - Timer;
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
                Main.EntitySpriteDraw(texBack, centers[i], null, col * brightness * Utils.GetLerpValue(rotations.Count, rotations.Count / 2f, i, true), rotations[i], texBack.Size() / 2, Projectile.scale, SpriteEffects.None);
            }

            if (Dead)
                return false;
            SetShaderParams();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, rainbowEffect, Main.GameViewMatrix.TransformationMatrix);
            float fade = Utils.GetLerpValue(0, 5, timeLeft, true);
            Main.EntitySpriteDraw(texMain, Projectile.Center - Main.screenPosition, null, Color.White * fade, Projectile.rotation, texMain.Size() / 2, Projectile.scale, SpriteEffects.None);
            //Main.EntitySpriteDraw(texBack, Projectile.Center - Main.screenPosition, null, Main.DiscoColor with {A = 0} * 0.5f * fade, Projectile.rotation, texBack.Size() / 2, Projectile.scale, SpriteEffects.None);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            //HitboxPreview();
            return false;
        }

        private void HitboxPreview()
        {
            hitboxes = GetInterpolatedHitboxes();
            for (int i = 0; i < hitboxes.Count; i++)
            {
                RectView(hitboxes[i]);
            }
        }

        public static void RectView(Rectangle rect)
        {
            AABBLineVisualizer(new Vector2(rect.X + rect.Width / 2, rect.Y), new Vector2(rect.X + rect.Width / 2, rect.Y + rect.Height), rect.Width);
        }
        public static void AABBLineVisualizer(Vector2 lineStart, Vector2 lineEnd, float lineWidth)
        {
            Texture2D blankTexture = Terraria.GameContent.TextureAssets.Extra[195].Value;
            Vector2 texScale = new Vector2((lineStart - lineEnd).Length(), lineWidth) * 0.00390625f;//1/256, texture is 256x256
            Main.EntitySpriteDraw(blankTexture, (lineStart) - Main.screenPosition, null, Color.Red * 0.25f, (lineEnd - lineStart).ToRotation(), new Vector2(0, 128), texScale, SpriteEffects.None);
        }
        private static void LoadShaderIfNeeded()
        {
            if (rainbowEffect == null)
            {                                                                
                rainbowEffect = ModContent.Request<Effect>("KirboMod/Items/RainbowSword/RainbowSwordHeldShader", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {            
         
            for (int i = 0; i < hitboxes.Count; i++)
            {
                if (targetHitbox.Intersects(hitboxes[i]))
                {
                    HitEffect(targetHitbox);
                    return true;
                }
            }

            
            return false;
        }

        private void HitEffect(Rectangle targetHitbox)
        {
            SoundEngine.PlaySound((Main.rand.NextBool() ? SoundID.Item67 : SoundID.Item68) with { MaxInstances = 0, Volume = 0.4f }, Projectile.Center);
            for (int j = 0; j < 10; j++)
            {
                SparkleFromHit(targetHitbox);
            }
        }

        private void SparkleFromHit(Rectangle targetHitbox)
        {
            Vector2 extraVel = Main.rand.NextVector2Circular(1000, 1000) / 200;
            extraVel += Vector2.Normalize(targetHitbox.Center.ToVector2() - GetOrigin()).RotatedBy(SwingDir * MathF.PI / 2) * GetSparkleVel(targetHitbox.Center());
            Vector2 centerVel = extraVel;
            extraVel = extraVel.RotatedByRandom(0.3f);
            extraVel *= MathHelper.Lerp(0.9f, 1.5f, Main.rand.NextFloat());

            float hue = Utils.Remap((centerVel - extraVel).ToRotation(), -MathF.PI, MathF.PI, 0, 1);
            Sparkle spalrkle = Sparkle.NewSparkle(targetHitbox.Center.ToVector2() - centerVel * 5 + Main.rand.NextVector2Circular(1000, 1000) / 80, Main.hslToRgb(hue, 1, 0.5f), new Vector2(1, 1.5f) * 1.6f, extraVel, 30, Vector2.One * 1.6f, null, 1, 0, 0.93f);
            spalrkle.Rotation = spalrkle.Velocity.ToRotation() + MathF.PI / 2f;
        }

        public override void Unload()
        {
            rainbowEffect = null;
        }
    }
}
