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
using KirboMod.Particles;
using KirboMod.Systems;

namespace KirboMod.Items.RainbowSword
{
    internal class RainbowSwordHeld : ModProjectile, ITrailedHeldProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 100;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.Size = new(30);
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.scale = 1;
            Projectile.hide = true;
            Projectile.extraUpdates = 7;
            Projectile.noEnchantmentVisuals = true;
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

        }
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
        float GetSparkleVel(Vector2 hitPoint)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 origin = player.RotatedRelativePoint(player.GetFrontHandPosition(player.compositeFrontArm.stretch, player.compositeFrontArm.rotation));
            return Derivative(Progress) * Utils.Remap(hitPoint.Distance(origin) * 0.02f, 0, 20, 2, 20, false);
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
            if (Projectile.localAI[2] == 0)
            {
                Projectile.scale *= Main.player[Projectile.owner].GetAdjustedItemScale(Main.player[Projectile.owner].HeldItem);
                UseTime *= Projectile.MaxUpdates;
                Projectile.velocity.Normalize();
                Main.instance.LoadProjectile(Type);
                Projectile.localAI[2] = 1;
            }
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
            Vector2 lightStart, lightEnd;
            AddLight(player, out lightStart, out lightEnd);
            if(progress < 1)
            SparklesFromSwing(progress, lightStart, lightEnd);
        }
        Vector2 GetOrigin()
        {
            Player player = Main.player[Projectile.owner];
            return player.RotatedRelativePoint(player.GetFrontHandPosition(player.compositeFrontArm.stretch, player.compositeFrontArm.rotation));
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
        public override bool PreDraw(ref Color lightColor)
        {

            LoadShaderIfNeeded();
            if (Dead)
                return false;
            float timeLeft = UseTime - Timer;
            float fade = Utils.GetLerpValue(0, 5, timeLeft, true);
            Texture2D texMain = TextureAssets.Projectile[Type].Value;
            SetShaderParams();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, rainbowEffect, Main.GameViewMatrix.TransformationMatrix);
            Main.EntitySpriteDraw(texMain, Projectile.Center - Main.screenPosition, null, Color.White * fade, Projectile.rotation, texMain.Size() / 2, Projectile.scale, SpriteEffects.None);
            //Main.EntitySpriteDraw(texBack, Projectile.Center - Main.screenPosition, null, Main.DiscoColor with {A = 0} * 0.5f * fade, Projectile.rotation, texBack.Size() / 2, Projectile.scale, SpriteEffects.None);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - VisualRotation.ToRotationVector2() * 170 * Projectile.scale, Projectile.Center + VisualRotation.ToRotationVector2() * 170 * Projectile.scale, 70, ref a))
            {
                HitEffect(targetHitbox);
                return true;
            }
            return false;
        }
        private static void LoadShaderIfNeeded()
        {
            if (rainbowEffect == null)
            {                                                                
                rainbowEffect = ModContent.Request<Effect>("KirboMod/Items/RainbowSword/RainbowSwordHeldShader", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            }
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
            Sparkle spalrkle = new Sparkle(
                targetHitbox.Center.ToVector2() - centerVel * 5 + Main.rand.NextVector2Circular(1000, 1000) / 80,
                Main.hslToRgb(hue, 1, 0.5f),
                extraVel,
                new Vector2(1, 1.5f) * 1.6f);
            spalrkle.RotateToVel();
            spalrkle.Confirm();
        }
        private void SparklesFromSwing(float progress, Vector2 lightStart, Vector2 lightEnd)
        {
            float sparkleChance = MathHelper.Lerp(0.5f, 1, Main.gfxQuality);
            {
                if (Main.rand.NextFloat() > sparkleChance)
                    return;
                // float extraProgress = Utils.GetLerpValue(0, 15, i) / UseTime;

                Vector2 spawnPos = (Projectile.rotation - MathF.PI / 4).ToRotationVector2() * 140 * Projectile.scale + Projectile.Center;
                Vector2 origin = GetOrigin();
                float backFactor = Main.rand.NextFloat();
                backFactor = 1 - (1 - backFactor) * (1 - backFactor);
                Vector2 spawnPosOffset = Vector2.Lerp(origin, spawnPos, MathHelper.Lerp(0.5f, 1f, backFactor));
                Sparkle sparkle = new(spawnPos + (spawnPosOffset - spawnPos), Main.DiscoColor, VisualRotation.ToRotationVector2().RotatedBy(SwingDir * MathF.PI / 2).RotatedByRandom(0.3f) * GetSparkleVel(spawnPos/*, -extraProgress*/) * backFactor, new Vector2(1, 1.5f) * 1.5f, Vector2.One * 1.5f, 30);
                sparkle.RotateToVel();
                sparkle.Confirm();
            }
        }
        private void AddLight(Player player, out Vector2 lightStart, out Vector2 lightEnd)
        {
            lightStart = Projectile.Center - (Projectile.rotation - MathF.PI / 4).ToRotationVector2() * 170;
            lightEnd = Projectile.Center + (Projectile.rotation - MathF.PI / 4).ToRotationVector2() * 170;
            if (Main.gfxQuality == 1)
            {
                Vector2 toBladeTip = VisualRotation.ToRotationVector2() * 100;
                for (float i = 0; i < 1; i += 0.02f)
                {

                    Lighting.AddLight(Projectile.Center + Vector2.Lerp(toBladeTip, -toBladeTip, i), Main.DiscoColor.ToVector3() * 3f);
                }
            }
            Lighting.AddLight(player.Center, Main.DiscoColor.ToVector3() * 4);
            lightEnd = (lightEnd - lightStart) * 0.8f * Projectile.scale + lightStart;
        }
        public override void Unload()
        {
            rainbowEffect = null;
        }
        static void SetShaderParams()
        {
            rainbowEffect.Parameters["s"].SetValue(1);
            rainbowEffect.Parameters["l"].SetValue(0.5f);
            rainbowEffect.Parameters["uOpacity"].SetValue(1);
            rainbowEffect.Parameters["gradientScale"].SetValue(2f);
            rainbowEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 3f);
        }

        public void AddTrail()
        {
            float timeLeft = UseTime - Timer;

            float fade = Utils.GetLerpValue(0, 5, timeLeft, true);
            if (!HeldProjTrailSystem.IsDarkEnvironment(Main.player[Projectile.owner], out byte spaceAlpha))
            {
                Color color = Main.DiscoColor with { A = spaceAlpha };
                HeldProjTrailSystem.Trail.AddAlphaBlend(Projectile, 270, color * fade, color * fade);
            }
            else
            {
                HeldProjTrailSystem.Trail.AddAdditive(Projectile, 270, Main.DiscoColor * fade, Main.DiscoColor * fade);
            }
        }
    }
}
