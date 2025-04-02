using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod
{
    public class VFX : ModSystem
    {
        public static void DrawGlowBallDiffuse(Vector2 pos, float scaleMultiplier, Color outerColor, Color innerColor)
        {
            Vector2 origin = GlowBall.Size() / 2;
            Main.EntitySpriteDraw(GlowBall, pos - Main.screenPosition, null, outerColor, Main.rand.NextFloat() * MathF.Tau, origin, scaleMultiplier, SpriteEffects.None);
            Main.EntitySpriteDraw(GlowBall, pos - Main.screenPosition, null, innerColor, Main.rand.NextFloat() * MathF.Tau, origin, scaleMultiplier * 0.5f, SpriteEffects.None);
        }
        public static void DrawGlowBallAdditive(Vector2 pos, float scaleMultiplier, Color outerColor, Color innerColor, bool shiny = true)
        {
            Vector2 origin = GlowBall.Size() / 2;
            Main.EntitySpriteDraw(GlowBall, pos - Main.screenPosition, null, outerColor with { A = 0 }, Main.rand.NextFloat() * MathF.Tau, origin, scaleMultiplier, SpriteEffects.None);
            Main.EntitySpriteDraw(GlowBall, pos - Main.screenPosition, null, innerColor with { A = 0 }, Main.rand.NextFloat() * MathF.Tau, origin, scaleMultiplier * 0.5f, SpriteEffects.None);
            if (shiny)
                Main.EntitySpriteDraw(GlowBall, pos - Main.screenPosition, null, Color.White with { A = 0 }, Main.rand.NextFloat() * MathF.Tau, origin, scaleMultiplier * 0.25f, SpriteEffects.None);
        }
        public static void SpawnBlueStarParticle(Projectile proj, Vector2? velocity = null, float scale = 1, Vector2? posOffset = null)
        {
            //16 yellow star gore
            //17 blue star gore
            velocity ??= Vector2.Zero;
            posOffset ??= Vector2.Zero;
            Gore.NewGoreDirect(proj.GetSource_FromThis(), proj.Center + posOffset.Value, velocity.Value, 17, scale);
        }
        public static void SpawnYellowStarParticle(Projectile proj, Vector2? velocity = null, float scale = 1, Vector2? posOffset = null)
        {
            //16 yellow star gore
            //17 blue star gore
            velocity ??= Vector2.Zero;
            posOffset ??= Vector2.Zero;
            Gore.NewGoreDirect(proj.GetSource_FromThis(), proj.Center + posOffset.Value, velocity.Value, 16, scale);
        }
        public static void LoadTextures()
        {

            circle = ModContent.Request<Texture2D>("KirboMod/ExtraTextures/CirclePremultiplied");
            ring = ModContent.Request<Texture2D>("KirboMod/ExtraTextures/RingPremultiplied");
            glowBall = ModContent.Request<Texture2D>("KirboMod/ExtraTextures/GlowBallPremultiplied");
            glowLine = ModContent.Request<Texture2D>("KirboMod/ExtraTextures/GlowLinePremultiplied");
            glowLineCap = ModContent.Request<Texture2D>("KirboMod/ExtraTextures/GlowLineCapPremultiplied");
            ringShine = ModContent.Request<Texture2D>("KirboMod/ExtraTextures/RingShinePremultiplied");
            circleOutline = ModContent.Request<Texture2D>("KirboMod/ExtraTextures/GlowOutlinePremultiplied");
            waddleDooBeam0 = ModContent.Request<Texture2D>("KirboMod/ExtraTextures/WaddleDooBeam0");
            waddleDooBeam1 = ModContent.Request<Texture2D>("KirboMod/ExtraTextures/WaddleDooBeam1");
        }
        public static Asset<Texture2D> glowLine;
        public static Asset<Texture2D> glowLineCap;
        public static Asset<Texture2D> circle;
        public static Asset<Texture2D> ringShine;
        public static Asset<Texture2D> glowBall;
        public static Asset<Texture2D> ring;
        public static Asset<Texture2D> circleOutline;
        public static Asset<Texture2D> waddleDooBeam0;
        public static Asset<Texture2D> waddleDooBeam1;
        public static Texture2D GlowLine { get => glowLine.Value; }
        public static Texture2D GlowLineCap { get => glowLineCap.Value; }
        public static Texture2D Circle { get => circle.Value; }
        public static Texture2D RingShine { get => ringShine.Value; }
        public static Texture2D Ring { get => ring.Value; }
        public static Texture2D GlowBall { get => glowBall.Value; }
        public static Texture2D CircleOutline => circleOutline.Value;
        public static Texture2D WaddleDooBeam0 => waddleDooBeam0.Value;
        public static Texture2D WaddleDooBeam1 => waddleDooBeam1.Value;
        public override void OnWorldLoad()
        {
            LoadTextures();//hopefully this fixes the textures not being loaded??
        }
        public override void Load()
        {
            LoadTextures();
        }
        /// <param name="opacity"></param>
        /// <param name="drawpos">remember to subtract Main.screenPosition</param>
        /// <param name="drawColor">color of the inner cross, most of the time you will want to leave this as white</param>
        /// <param name="shineColor">color of the outer cross</param>
        /// <param name="flareCounter">used as a timer for the fade things</param>
        public static void DrawPrettyStarSparkle(float opacity, Vector2 drawpos, Color drawColor, Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness)
        {
            Texture2D texture = TextureAssets.Extra[98].Value;
            Color bigShineColor = shineColor * opacity;
            bigShineColor.A = 0;
            Vector2 origin = texture.Size() / 2f;
            Color smallShineColor = drawColor * opacity;
            float brightness = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
            Vector2 scaleY = new Vector2(fatness.X * 0.5f, scale.X) * brightness;
            Vector2 scaleX = new Vector2(fatness.Y * 0.5f, scale.Y) * brightness;
            bigShineColor *= brightness;
            smallShineColor *= brightness;
            Main.EntitySpriteDraw(texture, drawpos, null, bigShineColor, MathHelper.PiOver2 + rotation, origin, scaleY, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, drawpos, null, bigShineColor, rotation, origin, scaleX, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, drawpos, null, smallShineColor, MathHelper.PiOver2 + rotation, origin, scaleY * 0.6f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, drawpos, null, smallShineColor, rotation, origin, scaleX * 0.6f, SpriteEffects.None, 0);
        }
        /// <summary>
        /// random electric color
        /// </summary>
        public static Color RndElectricCol { get => Main.rand.NextBool(2, 5) ? Color.Yellow : Color.Cyan; }
        public static void DrawElectricOrb(Projectile proj, Vector2 scale)
        {
            Vector2 randomOffset = Main.rand.NextVector2Circular(4, 4);
            Vector2 fatness = Vector2.One;//feel free to mess around with
            Vector2 sparkleScale = Vector2.One;//these values to see what thet change
            fatness *= scale;
            sparkleScale *= scale;
            randomOffset *= scale;
            float randRot = Main.rand.NextFloat() * MathF.Tau;
            Vector2 randScale = new(Main.rand.NextFloat() + 1f, Main.rand.NextFloat() + 1f);
            randScale *= scale;
            randScale *= 0.15f;
            Main.EntitySpriteDraw(Ring, proj.Center - Main.screenPosition, null, RndElectricCol with { A = 0 } * 0.8f * proj.Opacity, randRot, Ring.Size() / 2, randScale, SpriteEffects.None);
            Main.EntitySpriteDraw(Circle, proj.Center - Main.screenPosition, null, RndElectricCol with { A = 0 } * 0.25f * proj.Opacity, randRot, Circle.Size() / 2, randScale * 1.8f, SpriteEffects.None);
            DrawPrettyStarSparkle(proj.Opacity, proj.Center - Main.screenPosition + randomOffset, new Color(255, 255, 255, 0), RndElectricCol, 1, 0, 1, 1, 2, proj.rotation, sparkleScale, fatness);
        }
        public static void DrawElectricOrb(Vector2 pos, Vector2 scale, float opacity, float rotation)
        {
            Vector2 randomOffset = Main.rand.NextVector2Circular(4, 4);
            Vector2 fatness = Vector2.One;//feel free to mess around with
            Vector2 sparkleScale = Vector2.One;//these values to see what thet change
            fatness *= scale;
            sparkleScale *= scale;
            randomOffset *= scale;
            float randRot = Main.rand.NextFloat() * MathF.Tau;
            Vector2 randScale = new(Main.rand.NextFloat() + 1f, Main.rand.NextFloat() + 1f);
            randScale *= scale;
            randScale *= 0.15f;
            Main.EntitySpriteDraw(Ring, pos - Main.screenPosition, null, RndElectricCol with { A = 0 } * 0.8f * opacity, randRot, Ring.Size() / 2, randScale, SpriteEffects.None);
            Main.EntitySpriteDraw(Circle, pos - Main.screenPosition, null, RndElectricCol with { A = 0 } * 0.25f * opacity, randRot, Circle.Size() / 2, randScale * 1.8f, SpriteEffects.None);
            DrawPrettyStarSparkle(opacity, pos - Main.screenPosition + randomOffset, new Color(255, 255, 255, 0), RndElectricCol, 1, 0, 1, 1, 2, rotation, sparkleScale, fatness);
        }
        public static void DrawWaddleDooBeam(Vector2 pos, float scale, float opacity)
        {
            pos -= Main.screenPosition;
            bool beam0Primary = Main.rand.NextBool();
            Texture2D primaryTex = beam0Primary ? WaddleDooBeam0 : WaddleDooBeam1;
            Texture2D secondaryTex = beam0Primary ? WaddleDooBeam1 : WaddleDooBeam0;
            Color beamOrange = new Color(255, 179, 56, 128);
            Color beamPink = new Color(255, 125, 233, 128);
            bool orangePrimary = Main.rand.NextBool();
            Color primary = orangePrimary ? beamOrange : beamPink;
            Color secondary = orangePrimary ? beamPink : beamOrange;
            Vector2 drawPos = pos + Main.rand.BetterNextVector2Circular(16f * scale);
            Main.EntitySpriteDraw(primaryTex, drawPos, null, primary, Main.rand.NextFloat() * MathF.Tau, primaryTex.Size() / 2f, 1f * scale, SpriteEffects.None);
            drawPos += Main.rand.BetterNextVector2Circular(8f * scale);
            Main.EntitySpriteDraw(secondaryTex, drawPos, null, secondary, Main.rand.NextFloat() * MathF.Tau, secondaryTex.Size() / 2f, 0.75f * scale, SpriteEffects.None);
        }
        public override void Unload()
        {
            //annoying thing we need to do to static fields, free up the memory they are using.
            circle = null;
            glowBall = null;
            ring = null;
            ringShine = null;
            glowLine = null;
            glowLineCap = null;
            waddleDooBeam0 = null;
            waddleDooBeam1 = null;
        }

        public static void DrawProjWithStarryTrail(Projectile proj, Color drawColorMainTrail, Color drawColorSmallInnerTrail, Color drawColorStar, float drawColorMult = 0.2f, byte innerTrailAlpha = 0, byte trailAlpha = 0, byte starAlpha = 0)
        {
            Main.instance.LoadProjectile(proj.type);
            drawColorMainTrail *= proj.Opacity;
            drawColorSmallInnerTrail *= proj.Opacity;
            drawColorStar *= proj.Opacity;
            Texture2D projSprite = TextureAssets.Projectile[proj.type].Value;
            float projSPriteSizeLength = projSprite.Size().Length();
            float scaleMultFromTex = Utils.Remap(projSPriteSizeLength, 27, 120, 1, 2);
            float scaleIncrease = 0.3f;
            Vector2 spinningpoint = new Vector2(0f, 15) * projSPriteSizeLength * .007f;
            Vector2 drawPos = proj.Center + Vector2.Normalize(proj.velocity) * projSprite.Size().Length() / 2 - Main.screenPosition;
            Texture2D starTrailTexture = TextureAssets.Extra[ExtrasID.FallingStar].Value;
            Vector2 starTrailOrigin = new(starTrailTexture.Width / 2f, 10f);
            float timerVar = (float)Main.timeForVisualEffects / 60f;
            float rotation = proj.velocity.ToRotation() + MathHelper.PiOver2;
            drawColorMainTrail *= drawColorMult;
            Main.EntitySpriteDraw(starTrailTexture, drawPos + spinningpoint.RotatedBy(MathF.Tau * timerVar), null, drawColorMainTrail, rotation, starTrailOrigin, scaleMultFromTex * (1.3f + scaleIncrease), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(starTrailTexture, drawPos + spinningpoint.RotatedBy(MathF.Tau * timerVar + MathHelper.TwoPi / 3f), null, drawColorMainTrail, rotation, starTrailOrigin, scaleMultFromTex * (0.9f + scaleIncrease), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(starTrailTexture, drawPos + spinningpoint.RotatedBy(MathF.Tau * timerVar + 4.1887903f), null, drawColorMainTrail, rotation, starTrailOrigin, scaleMultFromTex * (1.1f + scaleIncrease), SpriteEffects.None, 0);
            drawPos = proj.Center + Vector2.Normalize(proj.velocity) * -projSprite.Size().Length() / 4 - Main.screenPosition;
            drawColorSmallInnerTrail.A = innerTrailAlpha;
            for (float i = 0f; i < 1f; i += 0.5f)
            {
                float scale = timerVar % 0.5f / 0.5f;
                scale = (scale + i) % 1f;
                float colorMult = scale * 2f;
                if (colorMult > 1f)
                {
                    colorMult = 2f - colorMult;
                }
                colorMult += +0.2f;
                Color drawColor = drawColorSmallInnerTrail * colorMult;
                if (innerTrailAlpha != 0)
                    drawColor.A = (byte)Math.Clamp(innerTrailAlpha * colorMult, 0, 255);
                Main.EntitySpriteDraw(starTrailTexture, drawPos, null, drawColor, rotation, starTrailOrigin, (0.2f + scale * 0.7f) * scaleMultFromTex, SpriteEffects.None, 0);
            }
            drawPos = proj.Center - Main.screenPosition;
            Main.instance.LoadProjectile(proj.type);
            starTrailTexture = TextureAssets.Projectile[proj.type].Value;
            drawColorStar.A = starAlpha;
            Main.EntitySpriteDraw(starTrailTexture, drawPos, null, drawColorStar, proj.rotation, starTrailTexture.Size() / 2, 1, SpriteEffects.None, 0);
        }

    }

}