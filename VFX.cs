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
        public static void LoadTextures()
        {
            Circle = ModContent.Request<Texture2D>("KirboMod/ExtraTextures/CirclePremultiplied").Value;
            Ring = ModContent.Request<Texture2D>("KirboMod/ExtraTextures/RingPremultiplied").Value;
            GlowBall = ModContent.Request<Texture2D>("KirboMod/ExtraTextures/GlowBallPremultiplied").Value;

        }
        public static Texture2D Circle;

        public static Texture2D GlowBall;

        public static Texture2D Ring;

        public override void Load()
        {
            LoadTextures();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="opacity"></param>
        /// <param name="drawpos">remember to subtract Main.screenPosition</param>
        /// <param name="drawColor">color of the inner cross, most of the time you will want to leave this as white</param>
        /// <param name="shineColor">color of the outer cross</param>
        /// <param name="flareCounter">used as a timer for the fade things</param>
        /// <param name="fadeInStart"></param>
        /// <param name="fadeInEnd"></param>
        /// <param name="fadeOutStart"></param>
        /// <param name="fadeOutEnd"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="fatness"></param>
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


        public override void Unload()
        {
            //annoying thing we need to do to static fields, free up the memory they are using.
            Circle = null;
            GlowBall = null;
            Ring = null;
        }
    }

}