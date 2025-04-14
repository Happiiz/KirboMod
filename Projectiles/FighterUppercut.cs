using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class FighterUppercut : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = Projectile.width;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true; //uses own immunity frames
            Projectile.localNPCHitCooldown = 10000; //time before hit again
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public virtual int AnimationDuration => 10;
        public virtual int DecelerateDuration => 3;
        ref float Timer => ref Projectile.localAI[0];
        bool MakePlayerInvincible => Projectile.ai[1] == 1;
        ref float DrawCounter => ref Projectile.localAI[2];
        public virtual Color EndColor => Color.Blue with { A = 128 };
        public virtual Color StartColor => Color.OrangeRed with { A = 128 };
        public virtual Color InnerStartColor => new Color(255, 255, 255, 0);
        public virtual Color InnerEndColor => new Color(255,255,255,0);
        public virtual float HighSpeed => 34;
        public virtual float LowSpeed => 20;
        public virtual float YMult => 3f;
        public static void GetAIValues(Player player, float fractionNeededForIframes, out float ai1)
        {
            KirbPlayer kplr = player.GetModPlayer<KirbPlayer>();
            ai1 = 0;
            if(kplr.fighterComboCounter / (float)KirbPlayer.MaxFighterComboCounter > fractionNeededForIframes)
            {
                ai1 = 1;
            }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.localAI[0]++;
            if (Timer > AnimationDuration)
            {
                Projectile.damage = -1;
                if (Timer > AnimationDuration * 2)
                {
                    Projectile.Kill();
                }
                return;
            }
            Projectile.spriteDirection = Projectile.direction; //look in direction
            float riseProgress = MathF.Min(1f, Timer / AnimationDuration);
            float angle = Utils.AngleLerp(0f, -MathF.PI / 2, riseProgress);
            Vector2 dir = new(MathF.Cos(angle), MathF.Sin(angle));
            dir.X *= player.direction;
            dir.Y *= YMult;
            float decelerate = Utils.Remap(Timer, AnimationDuration - DecelerateDuration, AnimationDuration, 1f, 0.2f);
            float speed = MakePlayerInvincible ? HighSpeed : LowSpeed;
            Vector2 deltaPos = dir * speed * decelerate;
            player.position += deltaPos;//setting velocity was giving some issues related to wings
            if(Timer == AnimationDuration)
            {
                player.velocity = deltaPos;
            }
            Projectile.rotation = dir.ToRotation() + MathF.PI / 2;
            Projectile.Center = player.MountedCenter + new Vector2(player.direction * 12, 0);
            if (MakePlayerInvincible)
            {
                ClampIframes(player);
            }
        }
        void AtLeastOneIframes()
        {
            Player player = Main.player[Projectile.owner];
            player.immuneTime = (int)MathF.Max(player.immuneTime, 1);
            for (int i = 0; i < player.hurtCooldowns.Length; i++)
            {
                player.hurtCooldowns[i] = (int)MathF.Max(player.hurtCooldowns[i], 1);
            }
        }
        private static void ClampIframes(Player player)
        {
            int min = 60;
            player.immune = true;
            player.immuneTime = (int)MathF.Max(player.immuneTime, min);
            for (int i = 0; i < player.hurtCooldowns.Length; i++)
            {
                player.hurtCooldowns[i] = (int)MathF.Max(player.hurtCooldowns[i], min);
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawCounter++;//draw behind stuff for layering the inner part above the player, but the outer part below the player
            Texture2D comet = TextureAssets.Extra[91].Value;
            float interpolatedAfterimagesCount = 3;
            float opacityMult = .75f;
            Vector2[] positions = new Vector2[Projectile.oldPos.Length + 1];
            Vector2 drawOffset = -Main.screenPosition + Projectile.Size / 2;
            int afterimageCancelCount = (int)MathF.Max(0, Timer - AnimationDuration + 1);
            for (int i = 1; i < positions.Length; i++)
            {
                positions[i] = Projectile.oldPos[i - 1] + drawOffset;
            }
            positions[0] = Projectile.position + drawOffset;

            float[] rotations = new float[Projectile.oldRot.Length + 1];
            for (int i = 1; i < positions.Length; i++)
            {
                rotations[i] = Projectile.oldRot[i - 1];
            }
            rotations[0] = Projectile.rotation;
            if (DrawCounter % 2 == 1)
            {
                for (int i = positions.Length - 1; i >= 1 + afterimageCancelCount; i--)
                {
                    for (int j = 0; j < interpolatedAfterimagesCount; j++)
                    {
                        float subProgress = (j / interpolatedAfterimagesCount) / Projectile.oldPos.Length;
                        float progress = 1 - (float)(i) / Projectile.oldPos.Length;
                        progress += subProgress;
                        float scale = MathHelper.Lerp(.75f, 1f, progress) + 0.2f;
                        float opacity = MathHelper.Lerp(.25f, 1f, progress);
                        Color currentCol = HSVLerp(EndColor, StartColor, progress);
                        Vector2 currentPos = positions[i];
                        float t = (j / interpolatedAfterimagesCount);
                        currentPos += (positions[i - 1] - positions[i]) * t;
                        opacity *= opacityMult;
                        float drawRot = Utils.AngleLerp(rotations[i], rotations[i - 1], t);
                        Main.EntitySpriteDraw(comet, currentPos, null, currentCol * opacity, drawRot, comet.Size() / 2, Projectile.scale * scale, SpriteEffects.None);
                    }

                }
            }
            else
            {
                int inner = positions.Length;
                for (int i = 1 + afterimageCancelCount; i < inner; i++)
                {
                    for (int j = 0; j < interpolatedAfterimagesCount; j++)
                    {
                        float subProgress = (j / interpolatedAfterimagesCount) / inner;
                        float progress = 1 - (float)(i) / inner;
                        progress += subProgress;
                        float scale = MathHelper.Lerp(.25f, 0.95f, progress);
                        float opacity = MathHelper.Lerp(-0.25f, 1f, progress);
                        Vector2 currentPos = positions[i];
                        float t = (j / interpolatedAfterimagesCount);
                        currentPos += (positions[i - 1] - positions[i]) * t;
                        opacity *= opacityMult;
                        float drawRot = Utils.AngleLerp(rotations[i], rotations[i - 1], t);
                        Color currentCol = HSVLerp(InnerEndColor, InnerStartColor, progress);
                        Main.EntitySpriteDraw(comet, currentPos, null, currentCol * opacity, drawRot, comet.Size() / 2, Projectile.scale * scale, SpriteEffects.None);
                    }
                }
            }
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life <= 0)
            {
                AtLeastOneIframes();
            }
        }

        //this hsv lerp code was mostly made by chatgpt because I didn't feel like writing code for it
        static Color HSVLerp(Color c1, Color c2, float progress)
        {
            progress = MathHelper.Clamp(progress, 0f, 1f);
            RGBToHSV(c1, out float h1, out float s1, out float v1);
            RGBToHSV(c2, out float h2, out float s2, out float v2);
            float dh = ((h2 - h1 + 1.5f) % 1f) - 0.5f;
            float h = (h1 + dh * progress + 1f) % 1f;
            float s = MathHelper.Lerp(s1, s2, progress);
            float v = MathHelper.Lerp(v1, v2, progress);
            Color c= HSVToRGB(h, s, v);
            c.A = (byte)MathHelper.Lerp(c1.A, c2.A, progress);
            return c;
        }
        static void RGBToHSV(Color color, out float h, out float s, out float v)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;
            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));
            v = max;
            float delta = max - min;
            if (max == 0f)
            {
                s = 0f;
                h = 0f;
            }
            else
            {
                s = delta / max;
                if (delta == 0f) h = 0f;
                else if (max == r) h = (g - b) / delta + (g < b ? 6f : 0f);
                else if (max == g) h = (b - r) / delta + 2f;
                else h = (r - g) / delta + 4f;
                h /= 6f;
            }
        }
        static Color HSVToRGB(float h, float s, float v)
        {
            h = (h % 1f + 1f) % 1f;
            float r, g, b;
            if (s == 0f)
            {
                r = g = b = v;
            }
            else
            {
                float sector = h * 6f;
                int i = (int)Math.Floor(sector);
                float f = sector - i;
                float p = v * (1f - s);
                float q = v * (1f - s * f);
                float t = v * (1f - s * (1f - f));
                switch (i % 6)
                {
                    case 0: r = v; g = t; b = p; break;
                    case 1: r = q; g = v; b = p; break;
                    case 2: r = p; g = v; b = t; break;
                    case 3: r = p; g = q; b = v; break;
                    case 4: r = t; g = p; b = v; break;
                    case 5: r = v; g = p; b = q; break;
                    default: r = g = b = 0f; break;
                }
            }
            return new Color(r, g, b);
        }
    }
}