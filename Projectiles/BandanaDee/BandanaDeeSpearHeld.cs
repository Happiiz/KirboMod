using KirboMod.Projectiles.BandanaDee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.BandanaDee
{
    public class BandanaDeeSpearHeld : ModProjectile //referenced staff code
    {
        public Projectile owner = Main.projectile[0];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        static float Range => 160;
        static float HitboxWidth => 100;
        public override void AI()
        {
            InitializeRotation();
            owner = Main.projectile.FirstOrDefault(p => p.active && p.owner == Projectile.owner && p.identity == Projectile.ai[1]);
            if (owner == null)//can happen if the minion is desummoned at the right time
            {
                Projectile.Kill();
                return;
            }
            BandanaWaddleDee bandanaDee = owner.ModProjectile as BandanaWaddleDee;
            if (bandanaDee == null)//can happen if the minion is desummoned at the right time
            {
                Projectile.Kill();
                return;
            }
            Vector2 center = owner.Center;
            Projectile.direction = owner.direction;
            Projectile.Center = center;
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
            if (owner.active && owner.whoAmI != -1 && bandanaDee.attacking && bandanaDee.attacktype == 0)
            {
                Vector2 targetVel = center + bandanaDee.Projectile.velocity;

                if (bandanaDee.aggroTarget != null && bandanaDee.aggroTarget.active)
                {
                    targetVel = bandanaDee.aggroTarget.Center - center;
                }
                targetVel.Normalize();
                targetVel *= Range;
                if (targetVel.HasNaNs())
                {
                    targetVel = Vector2.UnitX * owner.direction;
                }

                //probably not needed since this isn't mouse controlled anymore

                //if (targetVel.X != Projectile.velocity.X || targetVel.Y != Projectile.velocity.Y)
                //{
                //    Projectile.netUpdate = true;
                //}

                Projectile.velocity = targetVel;
            }
            else
            {
                Projectile.Kill();
            }
            Projectile.Center = center - Projectile.velocity;
        }

        private void InitializeRotation()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[1] = 1;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            InitializeRotation();

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
                float scaleMultiplier = MathHelper.Lerp(0.8f, 1.5f, t);
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