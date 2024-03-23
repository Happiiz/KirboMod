using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class Staffproj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 2;//weapon with armor pen
        }
        static float Range => 280;
        static float HitboxWidth => 100;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 center = player.RotatedRelativePoint(player.MountedCenter, true);
            Projectile.direction = player.direction;
            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation;
            player.SetDummyItemTime(2);
            player.itemRotation =
            Projectile.timeLeft = 2;
            Projectile.Center = center;
            Projectile.scale = 1;// Projectile.ai[1];
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 8f)
            {
                Projectile.ai[0] = 0f;
            }
            Projectile.soundDelay--;
            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item1 with { MaxInstances = 0 }, Projectile.Center);
                Projectile.soundDelay = 6;
            }
            if (Main.myPlayer == Projectile.owner)
            {
                if (player.channel && !player.noItems && !player.CCed)
                {
                    Vector2 targetVel = Main.MouseWorld - center;
                    targetVel.Normalize();
                    targetVel *= Range;
                    if (targetVel.HasNaNs())
                    {
                        targetVel = Vector2.UnitX * player.direction;
                    }
                    if (targetVel.X != Projectile.velocity.X || targetVel.Y != Projectile.velocity.Y)
                    {
                        Projectile.netUpdate = true;
                    }
                    Projectile.velocity = targetVel;
                }
                else
                {
                    Projectile.Kill();
                }
            }
            player.ChangeDir(MathF.Sign(Projectile.velocity.X));
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation(); ;
            Projectile.Center = center - Projectile.velocity;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            float staffLength = 1.41f * texture.Width - 10;//sqrt2 * width since texture is square, -10 to compensate for the nubs
            Vector2 rangeVector = Vector2.Normalize(Projectile.velocity) * staffLength;
            for (int i = 0; i < 2; i++)
            {
                float opacity = Utils.GetLerpValue(-1, 1, i);
                Vector2 startPoint = Projectile.Center + Main.rand.NextVector2Circular(16, 16);
                Vector2 endPoint = Projectile.Center + rangeVector.RotateRandom(.2f);
                float rotation = (endPoint - startPoint).ToRotation() + MathF.PI / 4;
                float time = (float)((Main.timeForVisualEffects / Helper.Phi + i * 5) % 10);
                float t = Easings.RemapProgress(0, 5, 5, 10, time);
                t = Easings.EaseInOutSine(t);
                float scaleMultiplier = MathHelper.Lerp(0.8f, 1.5f, t); ;
                t = MathHelper.Lerp(0.1f, 0.5f, t);
                Vector2 drawpos = Vector2.Lerp(startPoint, endPoint, t);
                Main.EntitySpriteDraw(texture, drawpos - Main.screenPosition, null, lightColor * opacity, rotation, texture.Size() / 2, Projectile.scale * scaleMultiplier, SpriteEffects.None);
            }
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 hitboxStart = Projectile.Center - Vector2.Normalize(Projectile.velocity) * 86;
            Vector2 hitboxEnd = Projectile.Center + Projectile.velocity;
            float unused = 2;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), hitboxStart, hitboxEnd, HitboxWidth, ref unused);
        }
    }
}