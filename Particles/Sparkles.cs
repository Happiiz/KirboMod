using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
namespace KirboMod.Particles
{
    public class Sparkle : Particle
    {
        readonly static Texture2D sparkleTexture = TextureAssets.Extra[98].Value;
        public Vector2 fatness;
        public new Vector2 scale;
        public bool drawHorizontalAxis;
        public byte additiveAmount;
        public Sparkle(Vector2 position, Color color, Vector2? velocity = null, Vector2? scale = null, Vector2? fatness = null, int duration = 40)
        {
            additiveAmount = (byte)(Main.dayTime ? 100 : 0);
            scale ??= Vector2.Zero;
            fatness ??= scale;
            velocity ??= Vector2.Zero;
            drawHorizontalAxis = true;
            this.color = color;
            timeLeft = duration;
            this.scale = scale.Value;
            this.fatness = fatness.Value;
            this.opacity = 1;
            this.velocity = velocity.Value;
            this.position = position;
            this.rotation = 0;
        }
        public static Sparkle[] EyeShine(Vector2 pos, Color color, Vector2? scale = null, Vector2? fatness = null, int duration = 40, float opacity =1)
        {
            scale ??= new Vector2(1.5f, 1);
            fatness ??= new Vector2(1.5f, 1);
            Ring ring = Ring.EmitRing(pos, color);
            ring.maxScale = 1.6f;
            ring.fadeOutTime = 7;
            ring.minScale = .5f;
            ring.squish = scale.Value;
            ring.rotation = 0;
            Sparkle[] result = new Sparkle[4];
            for (int i = 0; i < 4; i++)
            {
                Vector2 velocity = (i * MathF.Tau * .25f).ToRotationVector2() * scale.Value * 10;
                Sparkle sparkle = new(pos, color, velocity, scale, fatness, duration);
                sparkle.friction = .7f;
                sparkle.scaleAcceleration = -.3f;
                sparkle.fadeOutTime = 5;
                sparkle.RotateToVel();
                sparkle.drawHorizontalAxis = false;
                sparkle.Confirm();
                result[i] = sparkle;
            }
            return result;
        }
        public override void Draw()
        {
            Vector2 drawpos = position - Main.screenPosition;
            Color bigShineColor = color;
            bigShineColor.A = (byte)(additiveAmount * opacity);
            Vector2 origin = sparkleTexture.Size() / 2f;
            float brightness = MathF.Sin(timeLeft * 0.2f) * 0.25f + 0.75f;
            Vector2 scaleX = new Vector2(fatness.X * 0.5f, scale.X) * brightness;
            Vector2 scaleY = new Vector2(fatness.Y * 0.5f, scale.Y) * brightness;
            bigShineColor *= brightness;
            Color smallShineColor = Color.White;
            smallShineColor.A = 0;
            smallShineColor *= brightness;
            if(drawHorizontalAxis)
                Main.EntitySpriteDraw(sparkleTexture, drawpos, null, bigShineColor * opacity, MathHelper.PiOver2 + rotation, origin, scaleX, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(sparkleTexture, drawpos, null, bigShineColor * opacity, rotation, origin, scaleY, SpriteEffects.None, 0);
            if(drawHorizontalAxis)
                Main.EntitySpriteDraw(sparkleTexture, drawpos, null, smallShineColor * opacity, MathHelper.PiOver2 + rotation, origin, scaleX * 0.6f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(sparkleTexture, drawpos, null, smallShineColor * opacity, rotation, origin, scaleY * 0.6f, SpriteEffects.None, 0);
        }
        public void RotateToVel(bool add90deg = true)
        {
            rotation = velocity.ToRotation();
            if (add90deg)
                rotation += MathF.PI / 2;
        }
        public override void Update()
        {
           base.Update();
           velocity *= .95f;
        }
        public static Sparkle NewSparkle(Vector2 position, Color color, Vector2 scale, Vector2 velocity, int duration, Vector2 fatness)
        {
            Sparkle sparkle = new(position, color, velocity, scale, fatness, duration);
            sparkle.friction = .98f;
            sparkle.Confirm();
            return sparkle;
        }
    }
}
