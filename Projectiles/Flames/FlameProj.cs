using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static tModPorter.ProgressUpdate;

namespace KirboMod.Projectiles.Flames
{
    public abstract class FlameProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Flames;
    
        protected virtual void FlamethrowerStats() {}
        protected Color smokeColor;
        protected Color startColor;
        protected Color middleColor;
        protected Color endColor;
        protected float startScale = 1;
        protected float endScale = 1;
        protected int dustID;
        protected float dustRadius = 50;
        protected float dustChance = .25f;
        protected int debuffID;
        protected int debuffDuration;
        protected int duration = 60;
        protected int fadeOutDuration = 12;
        protected float whiteInsideOpacity = 0;
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.tileCollide = false;
            Projectile.penetrate = 99;
            Projectile.localNPCHitCooldown = 10;
            Projectile.usesLocalNPCImmunity = true;
            FlamethrowerStats();
        }
        public override void AI()
        {
            Projectile.localAI[0] += 1f;
            int totalDuration = duration + fadeOutDuration;
            if (Projectile.localAI[0] >= totalDuration)
            {
                Projectile.Kill();
            }
            if (Projectile.localAI[0] >= duration)
            {
                Projectile.velocity *= 0.95f;
            }
            int num4 = 50;
            int num5 = num4;
            if (Projectile.localAI[0] < num5 && Main.rand.NextFloat() < dustChance)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(dustRadius, dustRadius) * Utils.Remap(Projectile.localAI[0], 0f, totalDuration, 0.5f, 1f), 4, 4, dustID, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100);
                if (Main.rand.NextBool(4))
                {
                    dust.noGravity = true;
                    dust.scale *= 3f;
                    dust.velocity.X *= 2f;
                    dust.velocity.Y *= 2f;
                }
                else
                {
                    dust.scale *= 1.5f;
                }
                dust.scale *= 1.5f;
                dust.velocity *= 1.2f;
                dust.velocity += Projectile.velocity * 1f * Utils.Remap(Projectile.localAI[0], 0f, duration * 0.75f, 1f, 0.1f) * Utils.Remap(Projectile.localAI[0], 0f, duration * 0.1f, 0.1f, 1f);
                dust.customData = 1;
            }
            if (num4 > 0 && Projectile.localAI[0] >= num4 && Main.rand.NextFloat() < 0.5f)
            {
                Vector2 center = Main.player[Projectile.owner].Center;
                Vector2 vector = (Projectile.Center - center).SafeNormalize(Vector2.Zero).RotatedByRandom(0.19634954631328583) * 7f;
                short num7 = 31;
                Dust dust = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(50f, 50f) - vector * 2f, 4, 4, num7, 0f, 0f, 150, new Color(80, 80, 80));
                dust.noGravity = true;
                dust.velocity = vector;
                dust.scale *= 1.1f + Main.rand.NextFloat() * 0.2f;
                dust.customData = -0.3f - 0.15f * Main.rand.NextFloat();
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            float totalDuration = duration + fadeOutDuration;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Color color;
            float startColorThreshold = 0.35f;
            float middleColorThreshold = 0.7f;
            float endColorThreshold = 0.85f;
            float decrementStep = (Projectile.localAI[0] > duration - 10f) ? 0.175f : 0.2f;
            int verticalFrames = 7;
            float fadeOutProgress = Utils.Remap(Projectile.localAI[0], duration, totalDuration, 1f, 0f);
            float startOffset = Math.Min(Projectile.localAI[0], 20f);
            float progress = Utils.Remap(Projectile.localAI[0], 0f, totalDuration, 0f, 1f);
            float drawScale = Utils.Remap(progress, 0.2f, 0.5f, startScale, endScale);
            float oldDrawScale = Utils.Remap(Utils.GetLerpValue(0, totalDuration, Projectile.localAI[0] - 1), 0.2f, 0.5f, startScale, endScale);
            Rectangle frame = texture.Frame(1, verticalFrames, 0, 3);
            if (progress >= 1f)
            {
                return false;
            }
            for (int i = 0; i < 2; i++)
            {
                for (float j = 1f; j >= 0f; j -= decrementStep)
                {
                    color = (progress < 0.1f) ? Color.Lerp(Color.Transparent, startColor, Utils.GetLerpValue(0f, 0.1f, progress, clamped: true)) :
                        ((progress < 0.2f) ? Color.Lerp(startColor, middleColor, Utils.GetLerpValue(0.1f, 0.2f, progress, clamped: true)) :
                        ((progress < startColorThreshold) ? middleColor :
                        ((progress < middleColorThreshold) ? Color.Lerp(middleColor, endColor, Utils.GetLerpValue(startColorThreshold, middleColorThreshold, progress, clamped: true)) :
                        ((progress < endColorThreshold) ? Color.Lerp(endColor, smokeColor, Utils.GetLerpValue(middleColorThreshold, endColorThreshold, progress, clamped: true)) :
                        ((!(progress < 1f)) ? Color.Transparent :
                        Color.Lerp(smokeColor, Color.Transparent, Utils.GetLerpValue(endColorThreshold, 1f, progress, clamped: true)))))));

                    float fadeFromTrail = (1f - j) * Utils.Remap(progress, 0f, 0.2f, 0f, 1f);
                    Vector2 drawPos = Projectile.Center - Main.screenPosition + Projectile.velocity * -startOffset * j;
                    Color secondaryTrailColor = color * fadeFromTrail;
                    Color drawColor = secondaryTrailColor;
                    drawColor.G /= 2;
                    drawColor.B /= 2;
                    drawColor.A = (byte)Math.Min(secondaryTrailColor.A + 80f * fadeFromTrail, 255f);
                    float rotation = 1f / decrementStep * (j + 1f);
                    float rotationOffsetCw = Projectile.rotation + j * (MathF.PI / 2f) + Main.GlobalTimeWrappedHourly * rotation * 2f;
                    float rotationOffsetCcw = Projectile.rotation - j * (MathF.PI / 2f) - Main.GlobalTimeWrappedHourly * rotation * 2f;
                    float finalDrawScale = MathHelper.Lerp(oldDrawScale, drawScale, j);

                    Color white = new Color(255, 255, 255, 0) * whiteInsideOpacity * Utils.Remap(progress, middleColorThreshold, endColorThreshold, 1, 0) * Utils.GetLerpValue(0, .1f, Projectile.localAI[0], true) * fadeFromTrail * .4f;
                    switch (i)
                    {
                        case 0:
                            VFX.DrawGlowBallDiffuse(drawPos + Main.screenPosition, finalDrawScale * 2, drawColor, Color.Transparent);
                            Main.EntitySpriteDraw(texture, drawPos + Projectile.velocity * (0f - startOffset) * decrementStep * 0.5f, frame, drawColor * fadeOutProgress * 0.25f, rotationOffsetCw + MathF.PI / 4f, frame.Size() / 2f, finalDrawScale, SpriteEffects.None);
                            Main.EntitySpriteDraw(texture, drawPos, frame, drawColor * fadeOutProgress, rotationOffsetCcw, frame.Size() / 2f, finalDrawScale, SpriteEffects.None);
                            Main.EntitySpriteDraw(texture, drawPos, frame, white, rotationOffsetCcw, frame.Size() / 2f, finalDrawScale * .5f, SpriteEffects.None);
                            Main.EntitySpriteDraw(texture, drawPos, frame, white, rotationOffsetCcw, frame.Size() / 2f, finalDrawScale * .25f, SpriteEffects.None);
                            break;
                        case 1:
                            Main.EntitySpriteDraw(texture, drawPos + Projectile.velocity * (0f - startOffset) * decrementStep * 0.2f, frame, secondaryTrailColor * fadeOutProgress * 0.25f, rotationOffsetCw + MathF.PI / 2f, frame.Size() / 2f, finalDrawScale * 0.75f, SpriteEffects.None);
                            Main.EntitySpriteDraw(texture, drawPos, frame, secondaryTrailColor * fadeOutProgress, rotationOffsetCcw + MathF.PI / 2f, frame.Size() / 2f, finalDrawScale * 0.75f, SpriteEffects.None);

                            Main.EntitySpriteDraw(texture, drawPos, frame, white, rotationOffsetCcw + MathF.PI / 2f, frame.Size() / 2f, finalDrawScale * 0.19f, SpriteEffects.None);
                            Main.EntitySpriteDraw(texture, drawPos, frame, white, rotationOffsetCcw + MathF.PI / 2f, frame.Size() / 2f, finalDrawScale * 0.37f, SpriteEffects.None);
                            break;

                    }
                }
            }
            return false;
        }
        static int DebuffDurationMultiplier { get => Main.masterMode ? 3 : Main.expertMode ? 2 : 1; }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(debuffID, debuffDuration * DebuffDurationMultiplier);
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(debuffID, debuffDuration * DebuffDurationMultiplier);
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float baseHitboxSize = Projectile.friendly ? 40 : 20;
            float progress = Utils.Remap(Projectile.localAI[0], 0f, duration + fadeOutDuration, 0f, 1f);
            float drawScale = Utils.Remap(progress, 0.2f, 0.5f, startScale, endScale);
            projHitbox = Utils.CenteredRectangle(Projectile.Center, new Vector2(baseHitboxSize * drawScale));
            if (Projectile.hostile && Projectile.localAI[0] >= duration - 2)
            {
                return false;
            }
            return targetHitbox.Intersects(projHitbox);
        }
    }

}

