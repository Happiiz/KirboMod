using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Content;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.SwordAuras
{
    public abstract class SwordSlash : ModProjectile
    {
        public virtual Color[] Palette { get => new Color[3] { new Color(45, 124, 205), new Color(181, 230, 29), new Color(34, 177, 76) }; }
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.TerraBlade2Shot;
        public bool CollidedWithTile { get => Projectile.localAI[1] == 1; set => Projectile.localAI[1] = value ? 1 : 0; }
        public ref float Timer { get => ref Projectile.localAI[0]; }
        public virtual float ScaleMultiplier => 1;
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.alpha = 255;
        }
        public override void AI()
        {
            float num3 = Projectile.ai[1];
            float fadeOutStart = Projectile.ai[1] + 25f;

            Timer += 1f;
            if (CollidedWithTile)
            {
                Timer += 2f;
            }
            Projectile.Opacity = Utils.Remap(Timer, 0f, Projectile.ai[1], 0f, 1f) * Utils.Remap(Timer, num3, fadeOutStart, 1f, 0f);
            if (Timer >= fadeOutStart)
            {
                CollidedWithTile = true;
                Projectile.Kill();
                return;
            }
            Player player = Main.player[Projectile.owner];
            float maybeProgress = Timer / Projectile.ai[1];
            float progress = Utils.Remap(Timer, Projectile.ai[1] * 0.4f, fadeOutStart, 0f, 1f);
            Projectile.direction = (Projectile.spriteDirection = (int)Projectile.ai[0]);
            float progressEased = 1f - (1f - progress) * (1f - progress);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.scale = Utils.Remap(progressEased, 0f, 1f, 1.5f, 1f) * Projectile.ai[2] * ScaleMultiplier;
            Projectile.Opacity = Utils.Remap(Timer, 0f, Projectile.ai[1] * 0.25f, 0f, 1f) * Utils.Remap(Timer, fadeOutStart - 12f, fadeOutStart, 1f, 0f);
            if (Projectile.velocity.Length() > 8f)
            {
                Projectile.velocity *= 0.94f;
                float num12 = Utils.Remap(maybeProgress, 0.7f, 1f, 110f, 110f);
                if (!CollidedWithTile)
                {
                    bool flag2 = false;
                    for (float i = -1f; i <= 1f; i += 0.5f)
                    {
                        Vector2 position3 = Projectile.Center + (Projectile.rotation + i * (MathF.PI / 4f) * 0.25f).ToRotationVector2() * num12 * 0.5f * Projectile.scale;
                        Vector2 position4 = Projectile.Center + (Projectile.rotation + i * (MathF.PI / 4f) * 0.25f).ToRotationVector2() * num12 * Projectile.scale;
                        if (Collision.CanHit(position3, 0, 0, position4, 0, 0))
                        {
                            flag2 = true;
                            break;
                        }
                    }
                    if (!flag2)
                    {
                        CollidedWithTile = true;
                    }
                }
                if (CollidedWithTile && Projectile.velocity.Length() > 8f)
                {
                    Projectile.velocity *= 0.8f;
                }
                if (CollidedWithTile)
                {
                    Projectile.velocity *= 0.88f;
                }
            }
            float num14 = Projectile.rotation + Main.rand.NextFloatDirection() * (MathF.PI / 2f) * 0.9f;
            Vector2 vector3 = Projectile.Center + num14.ToRotationVector2() * 85f * Projectile.scale;
            (num14 + Projectile.ai[0] * ((float)Math.PI / 2f)).ToRotationVector2();
            Color value = Palette[1];
            Lighting.AddLight(Projectile.Center + Projectile.rotation.ToRotationVector2() * 85f * Projectile.scale, value.ToVector3());
            for (int j = 0; j < 3; j++)
            {
                if (Main.rand.NextFloat() < Projectile.Opacity + 0.1f)
                {
                    Dust dust = Dust.NewDustPerfect(vector3, DustID.RainbowMk2, Projectile.velocity * 0.7f, 100, Palette[0] * Projectile.Opacity, Projectile.Opacity);
                    dust.velocity += player.velocity * 0.1f;
                    dust.position -= dust.velocity * 6f;
                    dust.noGravity = true;
                    dust.noLightEmittence = true;

                }
            }
            if (Projectile.damage == 0)
            {
                Timer += 3f;
                Projectile.velocity *= 0.76f;
            }
            if (Timer < 10f && (CollidedWithTile || Projectile.damage == 0))
            {
                Timer += 1f;
                Projectile.velocity *= 0.85f;
                for (int k = 0; k < 4; k++)
                {
                    float num15 = Main.rand.NextFloatDirection();
                    float num16 = 1f - Math.Abs(num15);
                    num14 = Projectile.rotation + num15 * (MathF.PI / 2f) * 0.9f;
                    vector3 = Projectile.Center + num14.ToRotationVector2() * 85f * Projectile.scale;
                    Dust dust = Dust.NewDustPerfect(vector3, DustID.RainbowMk2, Projectile.velocity.RotatedBy(num15 * ((float)Math.PI / 4f)) * 0.2f * Main.rand.NextFloat(), 100, Palette[2], 2 * num16);
                    dust.velocity += player.velocity * 0.1f;
                    dust.noGravity = true;
                    dust.position -= dust.velocity * Main.rand.NextFloat() * 3f;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Asset<Texture2D> texture = TextureAssets.Projectile[Projectile.type];
            Rectangle frame = texture.Frame(1, 4);
            Vector2 origin = frame.Size() / 2f;
            float scale = Projectile.scale;
            SpriteEffects spriteEffects = ((!(Projectile.ai[0] >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None);
            SpriteEffects effects = spriteEffects ^ SpriteEffects.FlipVertically;
            float flareCounter = Utils.Remap(Projectile.localAI[0], 0f, Projectile.ai[1] + 30f, .15f, .75f);
            float opacity = Projectile.Opacity;
            float num2 = 0.975f;
            float light = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3().Length() / MathF.Sqrt(3f);
            light = 0.5f + light * 0.5f;
            light = Utils.Remap(light, 0.2f, 1f, 0f, 1f);
            Color[] palette = Palette;
            Color blue = palette[0]; 
            Color lime = palette[1];
            Color green = palette[2];
            Main.EntitySpriteDraw(texture.Value, drawPos, frame, blue * light * opacity, Projectile.rotation + Projectile.ai[0] * (MathF.PI / 4f) * .12f, origin, scale * 0.95f, spriteEffects, 0f);
            Main.EntitySpriteDraw(texture.Value, drawPos, frame, blue * light * opacity, Projectile.rotation + Projectile.ai[0] * (MathF.PI / 4f) * -.12f, origin, scale * 0.95f, effects, 0f);

            Color color4 = Color.White * opacity * 0.5f;
            color4.A = (byte)(color4.A * (1f - light));
            Color color5 = color4 * light * 0.5f;
            color5.G = (byte)(color5.G * light);
            color5.B = (byte)(color5.R * (0.25f + light * 0.75f));
            Main.EntitySpriteDraw(texture.Value, drawPos, frame, color5 * 0.15f, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, scale, spriteEffects, 0f);
            Main.EntitySpriteDraw(texture.Value, drawPos, frame, color5 * 0.15f, Projectile.rotation + Projectile.ai[0] * -0.01f, origin, scale, effects, 0f);
            float num4 = 2f;
            float num5 = .25f;
            float num6 = 0.25f;
            float num7 = 0.05f;
            Main.EntitySpriteDraw(texture.Value, drawPos, frame, green * light * opacity * 0.3f, Projectile.rotation + Projectile.ai[0] * num5 * num4, origin, scale, spriteEffects, 0f);
            Main.EntitySpriteDraw(texture.Value, drawPos, frame, green * light * opacity * 0.3f, Projectile.rotation + (0f - Projectile.ai[0]) * num5 * num4, origin, scale, effects, 0f);
            Main.EntitySpriteDraw(texture.Value, drawPos, frame, lime * light * opacity * 0.5f, Projectile.rotation + Projectile.ai[0] * num6 * num4, origin, scale * num2, spriteEffects, 0f);
            Main.EntitySpriteDraw(texture.Value, drawPos, texture.Frame(1, 4, 0, 3), Color.White * 0.6f * opacity, Projectile.rotation + Projectile.ai[0] * num7 * num4, origin, scale, spriteEffects, 0f);
            Main.EntitySpriteDraw(texture.Value, drawPos, texture.Frame(1, 4, 0, 3), Color.White * 0.5f * opacity, Projectile.rotation + Projectile.ai[0] * -0.05f, origin, scale * 0.8f, spriteEffects, 0f);
            Main.EntitySpriteDraw(texture.Value, drawPos, texture.Frame(1, 4, 0, 3), Color.White * 0.4f * opacity, Projectile.rotation + Projectile.ai[0] * -0.1f, origin, scale * 0.6f, spriteEffects, 0f);
            for (float i = -9f; i < 9f; i += 1f)
            {
                float num9 = Projectile.rotation + Projectile.ai[0] * i * ((float)Math.PI * -2f) * 0.025f;
                Vector2 drawpos = drawPos + num9.ToRotationVector2() * (texture.Width() * 0.5f - 6f) * scale;
                float num10 = Math.Abs(i) / 9f;
                VFX.DrawPrettyStarSparkle(Projectile.Opacity, drawpos, new Color(255, 255, 255, 0) * opacity * num10, green, flareCounter, 0f, 0.5f, 0.5f, 1f, num9, new Vector2(0f, Utils.Remap(flareCounter, 0f, 1f, 3f, 0f)) * scale, Vector2.One * scale);
            }
            for (float i = -1f; i <= 1f; i += 0.5f)
            {
                if (i != 0f)
                {
                    Vector2 sparklePos = drawPos + (Projectile.rotation + i * (float)Math.PI * 0.75f * flareCounter).ToRotationVector2() * (texture.Width() * 0.5f - 4f) * scale;
                    float starScale = Utils.Remap(Math.Abs(i), 0f, 1f, 1f, 0.5f);
                    VFX.DrawPrettyStarSparkle(Projectile.Opacity, sparklePos, new Color(255, 255, 255, 0) * opacity * 0.5f, green, flareCounter, 0f, 0.5f, 0.5f, 0.75f, (float)Math.PI / 4f, new Vector2(Utils.Remap(flareCounter, 0f, 1f, 4f, 1f)) * scale * starScale, Vector2.One * scale * starScale);
                    VFX.DrawPrettyStarSparkle(Projectile.Opacity, sparklePos, new Color(255, 255, 255, 0) * opacity * 0.5f, green, flareCounter, 0f, 0.5f, 0.5f, 0.75f, 0f, new Vector2(2f, Utils.Remap(flareCounter, 0f, 1f, 4f, 1f)) * scale * starScale, Vector2.One * scale * starScale);
                }
            }
            Vector2 primarySparklePos = drawPos + Projectile.rotation.ToRotationVector2() * (texture.Width() * 0.5f - 4f) * scale;
            VFX.DrawPrettyStarSparkle(Projectile.Opacity, primarySparklePos, new Color(255, 255, 255, 0) * opacity * 0.5f, green, flareCounter, 0f, 0.5f, 0.5f, 1f, (float)Math.PI / 4f, new Vector2(Utils.Remap(flareCounter, 0f, 1f, 4f, 1f)) * scale, Vector2.One * scale * 1.5f);
            VFX.DrawPrettyStarSparkle(Projectile.Opacity, primarySparklePos, new Color(255, 255, 255, 0) * opacity * 0.5f, green, flareCounter, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(2f, Utils.Remap(flareCounter, 0f, 1f, 4f, 1f)) * scale, Vector2.One * scale * 1.5f);
            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.position += Projectile.velocity;
            Projectile.velocity = oldVelocity;
            Projectile.velocity *= 0.01f;
            CollidedWithTile = true;
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float coneLength = 96f * Projectile.scale;
            float maximumAngle = MathF.PI / 2f;
            float coneRotation = Projectile.rotation;
            return targetHitbox.IntersectsConeSlowMoreAccurate(Projectile.Center, coneLength, coneRotation, maximumAngle);
        }
        public static void NewSwordSlash<T>(EntitySource_ItemUse_WithAmmo source, Player player, Vector2 velocity, int damage, float kb, float ai1UnknownParameter) where T : SwordSlash
        {
            float scale = player.GetAdjustedItemScale(player.HeldItem);
            Projectile.NewProjectile(source, player.MountedCenter, velocity, ModContent.ProjectileType<T>(), damage, kb, Main.myPlayer, player.direction * player.gravDir, ai1UnknownParameter, scale);
        }
    }
}
