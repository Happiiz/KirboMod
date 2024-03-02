using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.NightmareLightningOrb
{
    public class NightmareLightningOrbHoming : ModProjectile//180x180 proj? use circle collision ofc
    {
        const string pathForFiles = "KirboMod/Projectiles/NightmareLightningOrb/";
        public override string Texture => "KirboMod/Projectiles/NightmareLightningOrb/NightmareLightningOrb";
        private class LightningArc
        {
            float timer;
            readonly int lifetime;
            float extra;
            float rotation;
            readonly Asset<Texture2D> texture;
            float startRotation;
            public LightningArc()
            {
                lifetime = 30;
                startRotation = Main.rand.NextFloat() * MathF.Tau;
                rotation = 0;
                extra = Main.rand.NextFloat();
                //texture = ModContent.Request<Texture2D>(pathForFiles + "spark_0" + Main.rand.Next(1, 5));
                texture = ModContent.Request<Texture2D>(pathForFiles + "twirl_0" + Main.rand.Next(1, 4));
            }
            public void Draw(Projectile parentOrb)
            {
                Texture2D texture = this.texture.Value;
                Vector2 origin = texture.Size() / 2;
                float progress = Utils.GetLerpValue(0, lifetime, timer);
                float scale = MathHelper.Lerp(.1f, 0.3f, progress) * parentOrb.scale;
                Vector2 offset = (startRotation).ToRotationVector2() * texture.Width * .35f * scale;
                float opacity = Utils.GetLerpValue(lifetime, lifetime - 10, timer, true) * Utils.GetLerpValue(0, 4, timer, true) * parentOrb.Opacity;
                Color col = new Color(180, 180, 180, 0) * opacity;
                Main.EntitySpriteDraw(texture, offset + parentOrb.Center - Main.screenPosition, null, col, startRotation + rotation, origin, scale, default);
                Main.EntitySpriteDraw(texture, offset + parentOrb.Center - Main.screenPosition, null, col, startRotation - rotation, origin, scale, SpriteEffects.FlipVertically);
            }
            public bool Update()
            {
                rotation += Utils.Remap(MathF.Sin(extra * MathF.Tau), -1, 1, .04f, .12f);
                timer++;
                return timer >= lifetime;
            }
        }
        private class LightningLine
        {
            float timer;
            readonly int lifetime;
            float rotation;
            readonly Asset<Texture2D> texture;
            sbyte spinDirection;
            SpriteEffects fx;
            readonly int startFrame;
            const int maxFrames = 64;
            public LightningLine()
            {
                lifetime = 30;
                rotation = Main.rand.NextFloat() * MathF.Tau + MathF.PI / 2;
                int sparkIndex = Main.rand.Next(5, 6);
                texture = ModContent.Request<Texture2D>(pathForFiles + "spark_0" + sparkIndex);
                fx = (SpriteEffects)Main.rand.Next(3);
                startFrame = Main.rand.Next(maxFrames);
                spinDirection = (sbyte)(Main.rand.NextBool() ? 1 : -1);
            }
            public void Draw(Projectile parentOrb)
            {
                Texture2D texture = this.texture.Value;
                float scale = 0.65f * parentOrb.scale;
                float progress = Utils.GetLerpValue(0, lifetime, timer);
                Rectangle frame = texture.Frame(maxFrames, 1, ((int)MathHelper.Lerp(0, maxFrames, (float)(Main.timeForVisualEffects / 45f)) + startFrame) % maxFrames, 0);
                float rotation = this.rotation + progress * spinDirection;
                Vector2 offset = (rotation).ToRotationVector2() * texture.Height * scale * .55f;

                float opacity = Utils.GetLerpValue(lifetime, lifetime - 4, timer, true) * Utils.GetLerpValue(0, 4, timer, true);
                Color col = Color.Lerp(Color.Yellow, new Color(234, 88, 213), Main.rand.NextFloat());
                col.A = 100;
                Main.EntitySpriteDraw(texture, offset + parentOrb.Center - Main.screenPosition, frame, col * opacity, rotation + MathF.PI / 2, frame.Size() / 2, new Vector2(scale * 2.5f, scale), fx);
            }
            public bool Update()
            {
                timer++;
                return timer >= lifetime;
            }
        }
        private class Twirl
        {
            Vector2 positionOffset;
            readonly Asset<Texture2D> texture;
            int timer;
            float randomNumber;
            readonly int lifetime = 100;
            Color color;
            float scale;
            float rotSpeed;
            byte dir;
            public Twirl(float rotationForPositionOffset)
            {
                randomNumber = Main.rand.NextFloat() * MathF.Tau;
                positionOffset = (rotationForPositionOffset * MathF.Tau).ToRotationVector2() * MathHelper.Lerp(5, 20, Main.rand.NextFloat());
                texture = ModContent.Request<Texture2D>(pathForFiles + "twirl_0" + Main.rand.Next(1, 4));
                color = Main.rand.NextBool() ? Color.DarkRed with { A = 0 } * .5f : new Color(34, 24, 117, 0) * .7f;
                scale = MathHelper.Lerp(0.3f, 0.8f, rotationForPositionOffset);
                rotSpeed = MathHelper.Lerp(-7, 7, Main.rand.NextFloat());
                dir = (byte)Main.rand.Next(0, 3);
            }
            public void Draw(Projectile parentOrb)
            {
                Texture2D texture = this.texture.Value;
                Vector2 origin = texture.Size() / 2;
                float scale = this.scale * parentOrb.scale;
                float progress = Utils.GetLerpValue(0, lifetime, timer);
                float rotation = progress * rotSpeed + randomNumber;
                Vector2 offset = positionOffset.RotatedBy(rotation) * parentOrb.scale;
                float opacity = Utils.GetLerpValue(lifetime, lifetime - 10, timer, true) * Utils.GetLerpValue(0, 10, timer, true);
                Main.EntitySpriteDraw(texture, parentOrb.Center - Main.screenPosition + offset, null, color * opacity, rotation, origin, scale, (SpriteEffects)dir);

            }
            public bool Update()
            {
                timer++;
                return timer >= lifetime;
            }
        }
        static float ScaleUpTime => 20;
        ref float Timer => ref Projectile.localAI[1];
        ref float TargetPlayerIndex => ref Projectile.ai[0];
        static float MaxVelocity => 6;
        float MaxSpeed => Projectile.ai[1];
        static int Duration => 1000;
        static float OrbRadius => 180;
        readonly List<LightningArc> arcs = new();
        readonly List<Twirl> twirls = new();
        readonly List<LightningLine> lines = new();
        public static float GetMaxSpeed(int firerate, int numOrbs, int start, int timer)
        {
            return Utils.Remap(timer - start, firerate * numOrbs, 0 , 1, 6);
        }
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.scale = 0;
            Projectile.Size = new(128);

        }

        public override void AI()
        {
            UpdateParticles();
            Projectile.scale = Easings.EaseInOutSine(Utils.GetLerpValue(0, ScaleUpTime, Timer, true));
            Projectile.Opacity = Utils.GetLerpValue(-1, ScaleUpTime, Timer, true) * Utils.GetLerpValue(Duration, Duration - 10, Timer, true);
            if (Timer >= Duration)
            {
                Projectile.Kill();
                return;
            }
            if (Timer >= ScaleUpTime)
            {
                //homing reach max strength over the course of 100 frames
                //first is -10 so it has an initial boost
                float steerSpeed = Utils.GetLerpValue(-10, 100, Timer - ScaleUpTime, true);
                steerSpeed = Easings.EaseInOutSine(steerSpeed);
                steerSpeed *= .2f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.player[(int)TargetPlayerIndex].Center) * MaxSpeed, steerSpeed);
            }
            Timer++;
        }
        private void UpdateParticles()
        {
            if (Timer == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    arcs.Add(new());
                    lines.Add(new());
                    twirls.Add(new(i / 3f));
                }
            }
            if (Timer % 8 == 0)
            {
                arcs.Add(new());
            }
            if (Timer % 5 == 0)
            {
                lines.Add(new());
            }
            if (Timer % 10 == 0)
            {
                twirls.Add(new(Main.rand.NextFloat()));
            }
            for (int i = 0; i < arcs.Count; i++)
            {
                if (arcs[i].Update())
                {
                    arcs.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < twirls.Count; i++)
            {
                if (twirls[i].Update())
                {
                    twirls.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Update())
                {
                    lines.RemoveAt(i);
                    i--;
                }
            }
        }
        static Color OrbColor
        {
            get
            {
                float random = Main.rand.NextFloat();
                if (random < .2f)
                    return new Color(34, 24, 117, 0) * .7f;
                if (random < .6f)
                    return Color.DarkRed with { A = 0 } * .5f;
                return new Color(100, 24, 152, 0) * .5f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 0; i < twirls.Count; i++)
            {
                twirls[i].Draw(Projectile);
            }
            Texture2D tex = VFX.Circle;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, OrbColor, MathF.Tau * Main.rand.NextFloat(), tex.Size() / 2, Projectile.scale * 5.5f, SpriteEffects.None);
            tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White, MathF.Tau * Main.rand.NextFloat(), tex.Size() / 2, Projectile.scale * .75f, SpriteEffects.None);

            for (int i = 0; i < lines.Count; i++)
            {
                lines[i].Draw(Projectile);
            }
            tex = VFX.RingShine;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, OrbColor * 2, MathF.Tau * Main.rand.NextFloat(), tex.Size() / 2, Projectile.scale * 3.2f, SpriteEffects.None);
            for (int i = 0; i < arcs.Count; i++)
            {
                arcs[i].Draw(Projectile);
            }
            for (int i = 0; i < 9; i++)
            {
                Vector2 offset = (Main.rand.NextFloat() * MathF.Tau).ToRotationVector2() * MathHelper.Lerp(40, 170, Main.rand.NextFloat());
                Vector2 drawPos = Projectile.Center + offset * Projectile.scale - Main.screenPosition;

                int lineCount = Main.rand.Next(3, 7);
                float increment = 1f / lineCount;
                float opacity = increment * .8f + 0.2f;
                opacity *= Projectile.Opacity;
                float spaceBetweenLines = MathF.Tau / lineCount;
                spaceBetweenLines *= .1f;
                for (float j = 0; j < 0.998f; j += increment)
                {
                    DrawSparkleLine(drawPos, opacity, j * MathF.Tau + MathHelper.Lerp(-spaceBetweenLines, spaceBetweenLines, Main.rand.NextFloat()));
                }
            }
            tex = VFX.CircleOutline;
            float t = Utils.Remap(MathF.Sin(Main.GlobalTimeWrappedHourly * 6 - Projectile.identity), .5f, .6f, 0, 1);
            Vector3 hsl = Main.rgbToHsl(OrbColor);
            hsl.Y = 1;
            hsl.Z = .6f;
            Color orbColSaturated = Main.hslToRgb(hsl);
            Color c = Color.Lerp(orbColSaturated, Color.LightGray, t);
            c.A = 0;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, c, MathF.Tau * Main.rand.NextFloat(), tex.Size() / 2, Projectile.scale * 3.2f, SpriteEffects.None);
            return false;
        }
        static void DrawSparkleLine(Vector2 drawPos, float opacity, float rotation)
        {
            Texture2D tex = TextureAssets.Extra[98].Value;
            Vector2 origin = tex.Size() / 2;
            Color col = new Color(255, 80, 128, 0);
            Vector2 scale = new Vector2(MathHelper.Lerp(0.75f, 1.1f, Main.rand.NextFloat()));
            Main.EntitySpriteDraw(tex, drawPos, null, col * opacity, rotation, origin, scale, SpriteEffects.None);
            scale.X *= .5f;
            scale.Y *= .8f;
            Main.EntitySpriteDraw(tex, drawPos, null, new Color(255, 255, 255, 0) * opacity, rotation, origin, scale, SpriteEffects.None);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Helper.CheckCircleCollision(targetHitbox, Projectile.Center, OrbRadius * Projectile.scale);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Slow, 60 * 3);//ankh shields gives immunity. maybe make dedicated debuff instead?
            target.AddBuff(BuffID.Electrified, 60 * 10);//reduce damage of attack to compensate, but don't reduce too much
        }
    }
}
