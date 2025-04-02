using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace KirboMod.Projectiles.Tornadoes
{
    public abstract class Tornado : ModProjectile
    {
        public override string Texture => "KirboMod/Projectiles/Tornadoes/tornado_0";
        static Asset<Texture2D> tornado1;
        static Asset<Texture2D> tornado2;
        protected const int framesX = 6;
        protected const int framesY = 5;
        protected const int frames = framesX * framesY;
        public ref float Timer => ref Projectile.localAI[0];
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Opacity = 0.8f;
        }

        protected Rectangle GetFrame(int timerOffset = 0)
        {
            int frameIndex = (int)(10000000 - Timer + timerOffset) % frames;
            return TextureAssets.Projectile[Type].Value.Frame(framesX, framesY, frameIndex / framesY % framesX, frameIndex % framesY);
        }

        public override void PostAI()
        {
            Timer+= 1;
        }

        public virtual Color[] SetPalette()
        {
            Color[] palette = { new Color(204, 255, 247), new Color(152, 255, 238), Color.LightCyan };
            return palette;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            tornado1 ??= ModContent.Request<Texture2D>("KirboMod/Projectiles/Tornadoes/tornado_1");
            tornado2 ??= ModContent.Request<Texture2D>("KirboMod/Projectiles/Tornadoes/tornado_2");
            Texture2D[] textures = { TextureAssets.Projectile[Type].Value, tornado1.Value, tornado2.Value };
            int maxheight = Projectile.height;
            int spirals = (int)MathF.Round(maxheight * 0.11333333333f);
            float wobbleSpeed = .1f;
            UnifiedRandom rnd = new UnifiedRandom(Projectile.identity * 1000);
            Color[] palette = SetPalette();
            float scrollSpeed = 4;

            TornadoPass2(textures, spirals, (int)(maxheight * Projectile.scale), wobbleSpeed, rnd, palette, scrollSpeed, lightColor);
            TornadoPass1(textures, spirals, (int)(maxheight * Projectile.scale), wobbleSpeed, rnd, palette, lightColor);
            return false;
        }

        private void TornadoPass2(Texture2D[] textures, int spirals, int maxheight, float wobbleSpeed, UnifiedRandom rnd, Color[] palette, float scrollSpeed, Color lightColor)
        {
            for (int i = 0; i < spirals; i++)
            {
                Rectangle frame = GetFrame(rnd.Next(frames));
                Texture2D t = textures[rnd.Next(textures.Length)];
                scrollSpeed = MathHelper.Lerp(3f, 5f, rnd.NextFloat()) * rnd.NextFloatDirection();
                float normalizedHeightOffset = Utils.GetLerpValue(0, spirals, i + (Timer * scrollSpeed + 100000));
                Vector2 offset = rnd.NextVector2Unit(Timer * MathHelper.Lerp(.1f, .2f, rnd.NextFloat())) * 8f;
                offset.Y -= (i + normalizedHeightOffset) % spirals * (maxheight / spirals);
                float opacity = Easings.RemapProgress(0, maxheight * -0.1f, maxheight * -0.8f, -maxheight * .9f, offset.Y);
                float progress = Utils.GetLerpValue(0, maxheight, -offset.Y);
                float wobbleT = MathF.Sin(rnd.NextFloat() * MathF.Tau + Timer * wobbleSpeed);
                float wobble = MathHelper.Lerp(-.05f, .05f, wobbleT);
                Vector2 scale = new Vector2(MathHelper.Lerp(.5f, 1, progress), .25f);
                scale *= MathHelper.Lerp(.4f, .6f, rnd.NextFloat());
                scale *= (float)(Projectile.width + 32) / frame.Width;
                Main.EntitySpriteDraw(t, Projectile.Center - Main.screenPosition + offset + new Vector2(0, maxheight * .5f), frame, palette[rnd.Next(palette.Length)].MultiplyRGB(lightColor) * opacity * Projectile.Opacity, wobble, frame.Size() / 2, scale, (SpriteEffects)rnd.Next(3));
            }
        }

        private void TornadoPass1(Texture2D[] textures, int spirals, int maxheight, float wobbleSpeed, UnifiedRandom rnd, Color[] palette, Color lightColor)
        {
            for (int i = 0; i < spirals; i++)
            {
                Rectangle frame = GetFrame(rnd.Next(frames));
                Texture2D t = textures[rnd.Next(textures.Length)];
                float progress = Utils.GetLerpValue(0, spirals, i);
                float wobbleT = MathF.Sin(rnd.NextFloat() * MathF.Tau + Timer * wobbleSpeed);
                float wobble = MathHelper.Lerp(-.05f, .05f, wobbleT);
                Vector2 scale = new Vector2(MathHelper.Lerp(1, 2, progress), .5f);
                scale *= MathHelper.Lerp(.3f, .45f, rnd.NextFloat());
                Vector2 offset = rnd.NextVector2Unit(Timer * MathHelper.Lerp(.1f, .2f, rnd.NextFloat())) * 4;
                offset.Y -= i % spirals * (maxheight / spirals);
                scale *= (float)(Projectile.width + 32) / frame.Width;
                Main.EntitySpriteDraw(t, Projectile.Center - Main.screenPosition + offset + new Vector2(0, maxheight * .5f), frame, palette[rnd.Next(palette.Length)].MultiplyRGB(lightColor) * Projectile.Opacity, wobble, frame.Size() / 2, scale, (SpriteEffects)rnd.Next(3));
            }
        }

        public override void Unload()
        {
            tornado1 = null;
            tornado2 = null;
        }
    }
}
