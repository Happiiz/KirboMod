using KirboMod.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod
{
    public class VFX : ModSystem
    {
        public static Texture2D Circle;

        public static Texture2D GlowBall;

        public static Texture2D Ring;

        public override void Load()
        {
            Circle = ModContent.Request<Texture2D>("KirboMod/ExtraTextures/CirclePremultiplied").Value;

            GlowBall = ModContent.Request<Texture2D>("KirboMod/ExtraTextures/GlowBallPremultiplied").Value;

            Ring = ModContent.Request<Texture2D>("KirboMod/ExtraTextures/RingPremultiplied").Value;
        }

        public static void DrawPrettyStarSparkle(float opacity, SpriteEffects dir, Vector2 drawpos, Color drawColor, Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness)
        {
            Texture2D texture = TextureAssets.Extra[98].Value;
            Color bigShineColor = shineColor * opacity;
            bigShineColor.A = 0;
            Vector2 origin = texture.Size() / 2f;
            Color smallShineColor = drawColor;
            float brightness = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
            Vector2 scaleY = new Vector2(fatness.X * 0.5f, scale.X) * brightness;
            Vector2 scaleX = new Vector2(fatness.Y * 0.5f, scale.Y) * brightness;
            bigShineColor *= brightness;
            smallShineColor *= brightness;
            Main.EntitySpriteDraw(texture, drawpos, null, bigShineColor, MathHelper.PiOver2 + rotation, origin, scaleY, dir, 0);
            Main.EntitySpriteDraw(texture, drawpos, null, bigShineColor, rotation, origin, scaleX, dir, 0);
            Main.EntitySpriteDraw(texture, drawpos, null, smallShineColor, MathHelper.PiOver2 + rotation, origin, scaleY * 0.6f, dir, 0);
            Main.EntitySpriteDraw(texture, drawpos, null, smallShineColor, rotation, origin, scaleX * 0.6f, dir, 0);
        }
    }

}