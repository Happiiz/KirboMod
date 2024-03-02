using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public abstract class SwordAura : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.TrueExcalibur;
        public abstract float BaseScale { get; }
        public abstract float ScaleIncrease { get; }
        public abstract Color[] Palette { get; }
        public static Projectile NewAura<T>(Player player, EntitySource_ItemUse_WithAmmo source, int dmg, float kb, Item item) where T : SwordAura
        {
            Projectile proj = Projectile.NewProjectileDirect(source, player.Center, new Vector2(player.direction, 0), ModContent.ProjectileType<T>(), dmg, kb, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax, player.GetAdjustedItemScale(item));
            NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI);
            return proj;
        }
        public override void AI()
        {
            Projectile.localAI[0] += 1f;
            Player player = Main.player[Projectile.owner];
            float progress = Projectile.localAI[0] / Projectile.ai[1];
            float whichSide = Projectile.ai[0];
            float velToRot = Projectile.velocity.ToRotation();
            float realRotation = (float)Math.PI * whichSide * progress + velToRot + whichSide * (float)Math.PI + player.fullRotation;
            Projectile.rotation = realRotation;
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) - Projectile.velocity;
            Projectile.scale = BaseScale + progress * ScaleIncrease;
            Projectile.scale *= Projectile.ai[2];
            float rotationWithDeviation = Projectile.rotation + Main.rand.NextFloatDirection() * ((float)Math.PI / 2f) * 0.7f;
            Vector2 edgePosition = Projectile.Center + rotationWithDeviation.ToRotationVector2() * 85f * Projectile.scale;
            Vector2 vector8 = (rotationWithDeviation + Projectile.ai[0] * (MathF.PI / 2f)).ToRotationVector2();
            Color value = Palette[1];
            Color value2 = Palette[2];
            Lighting.AddLight(Projectile.Center, value2.ToVector3());
            if (Main.rand.NextFloat() * 2f < Projectile.Opacity)
            {
                Color dustCol = Color.Lerp(value2, value, Utils.Remap(progress, 0f, 0.6f, 0f, 1f));
                dustCol = Color.Lerp(dustCol, Color.White, Utils.Remap(progress, 0.6f, 0.8f, 0f, 0.5f));
                Dust dust = Dust.NewDustPerfect(Projectile.Center + rotationWithDeviation.ToRotationVector2() * (Main.rand.NextFloat() * 80f * Projectile.scale + 20f * Projectile.scale), DustID.RainbowMk2, vector8 * 1f, 100, Color.Lerp(dustCol, Color.White, Main.rand.NextFloat() * 0.3f), 1.6f);
                dust.fadeIn = 0.4f + Main.rand.NextFloat() * 0.15f;
                dust.noGravity = true;
                dust = Dust.CloneDust(dust);
                dust.scale *= 0.75f;
                dust.color = Color.White with { A = 0 };
            }
            if (Main.rand.NextFloat() < Projectile.Opacity)
            {
                Dust dust = Dust.NewDustPerfect(edgePosition, DustID.RainbowMk2, vector8 * 3f, 100, Palette[0] * Projectile.Opacity, 1.2f);
                dust.velocity += player.velocity * 0.1f;
                dust.velocity += new Vector2(player.direction, 0f);
                dust.position -= dust.velocity * 6f;
                dust.noGravity = true;
                dust = Dust.CloneDust(dust);
                dust.scale *= 0.75f;
                dust.color = Color.White with { A = 0 };
            }

            Projectile.scale *= 1;
            if (progress > 1)
            {
                Projectile.Kill();
            }
        }
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(16);
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesOwnerMeleeHitCD = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Asset<Texture2D> texture = TextureAssets.Projectile[Projectile.type];
            Rectangle frame = texture.Frame(1, 4);
            Vector2 origin = frame.Size() / 2f;
            float num = Projectile.scale * 1.1f;
            SpriteEffects effects = ((!(Projectile.ai[0] >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None);
            float progress = Projectile.localAI[0] / Projectile.ai[1];
            float remappedProgress = Utils.Remap(progress, 0f, 0.6f, 0f, 1f) * Utils.Remap(progress, 0.6f, 1f, 1f, 0f);
            float num4 = 0.975f;
            float lightingMultiplier = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3().Length() / (float)Math.Sqrt(3.0);
            lightingMultiplier = 0.5f + lightingMultiplier * 0.5f;
            lightingMultiplier = Utils.Remap(lightingMultiplier, 0.2f, 1f, 0f, 1f);
            Color blue = Palette[2];//new Color(45, 124, 205);//blue
            Color lime = Palette[0];//new Color(181, 230, 29);//yellowlime
            Color green = Palette[1];//new Color(34, 177, 76);//green
            Color whiteOverlay = Color.White * remappedProgress * 0.5f;
            whiteOverlay.A = (byte)(whiteOverlay.A * (1f - lightingMultiplier));
            Color idfkWhatThisIs = whiteOverlay * lightingMultiplier * 0.5f;
            idfkWhatThisIs.G = (byte)(idfkWhatThisIs.G * lightingMultiplier);
            idfkWhatThisIs.B = (byte)(idfkWhatThisIs.R * (0.25f + lightingMultiplier * 0.75f));
            Main.spriteBatch.Draw(texture.Value, drawPos, frame, blue * lightingMultiplier * remappedProgress, Projectile.rotation + Projectile.ai[0] * ((float)Math.PI / 4f) * -1f * (1f - progress), origin, num * 0.95f, effects, 0f);
            Main.spriteBatch.Draw(texture.Value, drawPos, frame, idfkWhatThisIs * 0.15f, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, num, effects, 0f);
            Main.spriteBatch.Draw(texture.Value, drawPos, frame, green * lightingMultiplier * remappedProgress * 0.3f, Projectile.rotation, origin, num, effects, 0f);
            Main.spriteBatch.Draw(texture.Value, drawPos, frame, lime * lightingMultiplier * remappedProgress * 0.5f, Projectile.rotation, origin, num * num4, effects, 0f);
            Main.spriteBatch.Draw(texture.Value, drawPos, texture.Frame(1, 4, 0, 3), Color.White * 0.6f * remappedProgress, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, num, effects, 0f);
            Main.spriteBatch.Draw(texture.Value, drawPos, texture.Frame(1, 4, 0, 3), Color.White * 0.5f * remappedProgress, Projectile.rotation + Projectile.ai[0] * -0.05f, origin, num * 0.8f, effects, 0f);
            Main.spriteBatch.Draw(texture.Value, drawPos, texture.Frame(1, 4, 0, 3), Color.White * 0.4f * remappedProgress, Projectile.rotation + Projectile.ai[0] * -0.1f, origin, num * 0.6f, effects, 0f);
            for (float i = 0f; i < 12f; i += 1f)
            {
                float sparkleRotation = Projectile.rotation + Projectile.ai[0] * (i - 2f) * ((float)Math.PI * -2f) * 0.025f + Utils.Remap(progress, 0f, 1f, 0f, (float)Math.PI / 4f) * Projectile.ai[0];
                Vector2 drawpos = drawPos + sparkleRotation.ToRotationVector2() * (texture.Width() * 0.5f - 6f) * num;
                float progressOnDrawingGlowyEdge = i / 12f;
                VFX.DrawPrettyStarSparkle(Projectile.Opacity, drawpos, new Color(255, 255, 255, 0) * remappedProgress * progressOnDrawingGlowyEdge, green, progress, 0f, 0.5f, 0.5f, 1f, sparkleRotation, new Vector2(0f, Utils.Remap(progress, 0f, 1f, 3f, 0f)) * num, Vector2.One * num);
            }
            Vector2 drawpos2 = drawPos + (Projectile.rotation + Utils.Remap(progress, 0f, 1f, 0f, (float)Math.PI / 4f) * Projectile.ai[0]).ToRotationVector2() * (texture.Width() * 0.5f - 4f) * num;
            VFX.DrawPrettyStarSparkle(Projectile.Opacity, drawpos2, new Color(255, 255, 255, 0) * remappedProgress * 0.5f, green, progress, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(2f, Utils.Remap(progress, 0f, 1f, 4f, 1f)) * num, Vector2.One * num * 1.5f);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float coneLength = 95 * Projectile.scale;
            float maximumAngle = MathF.PI / 2.5f;
            float coneRotation = Projectile.rotation - 0.6f * Projectile.direction;
            return targetHitbox.IntersectsConeSlowMoreAccurate(Projectile.Center, coneLength, coneRotation, maximumAngle) && Projectile.localAI[0] > 1;
        }
    }
}
