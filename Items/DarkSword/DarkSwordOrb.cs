using KirboMod.Dusts;
using KirboMod.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.DarkSword
{
    internal class DarkSwordOrb : ModProjectile
    {
        public override string Texture => "KirboMod/Projectiles/GoodDarkOrb";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
        }
        ref float Timer { get => ref Projectile.ai[0]; }
        public override void AI()
        {
            Timer++;
            if (Timer < 0)
            {
                Player player = Main.player[Projectile.owner];
                Projectile.Center = player.RotatedRelativePoint(player.MountedCenter);
                return;
            }
            Projectile.Opacity += .334f;
            if (Timer > 1200)
                Projectile.Kill();
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<DarkResidue>(), Projectile.velocity.X * 0.3f, Projectile.velocity.Y * 0.3f, 200, default, 0.8f); //dust
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawDarkOrb(Projectile);
            return false;
        }

        public static void DrawDarkOrb(Projectile projectile)
        {
            projectile.DrawSelf();
            float time = (float)Main.timeForVisualEffects * .025f;
            Vector2 drawPos = projectile.Center - Main.screenPosition;
            for (int i = 0; i < 3; i++)
            {
                float offsetTime = (time + i * .3333f);
                int index = (int)(offsetTime + (i + 1) * 12043 + projectile.whoAmI * 21);//random jargon for a distinct number for each ring 
                offsetTime %= 1;
                float brightness = MathHelper.Clamp(MathHelper.Lerp(1.5f, 0, offsetTime), 0, 1);
                brightness *= MathHelper.Clamp(MathHelper.Lerp(0, 4, offsetTime), 0, 1);
                brightness *= projectile.Opacity;
                float rotation = (index % 2 * 2 - 1) * (index + offsetTime * 3f);
                float minSquish = MathHelper.Lerp(.3f, -.1f, (index * .1f) % 1);
                float squish = MathHelper.Lerp(1, minSquish, offsetTime);
                Vector2 scale = new Vector2(squish, 1);
                scale *= projectile.scale;
                scale *= .7f;
                Color col = Color.Purple * brightness;
                Main.EntitySpriteDraw(VFX.RingShine, drawPos, null, col, -rotation, VFX.RingShine.Size() / 2, scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
                VFX.DrawPrettyStarSparkle(brightness, drawPos, Color.DarkGray with { A = 0 } * .7f, col, 1, 0, 1, 2, 3, rotation, scale * 2.5f, scale * 2.5f);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (oldVelocity.Y != Projectile.velocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            if (oldVelocity.X != Projectile.velocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            //Projectile.penetrate--;
            return false;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
            {
                Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 10, 10); //Makes dust in a messy circle
                d.noGravity = true;
            }
            if (Projectile.penetrate == 1)
                return;
            Projectile.Hitbox = Utils.CenteredRectangle(Projectile.Center, Projectile.Size * 3);
            ParticleEffect();
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
            ParticleEffect();
            Projectile.Damage();
        }

        void ParticleEffect()
        {
            for (int i = 0; i < 4; i++)
            {
                Ring ring = new Ring(Projectile.Center);
                ring.color = Color.Purple;
                ring.scaleUpTime = 20;
                ring.timeLeft = 20;
                ring.fadeOutTime = 10;
                ring.minScale = .8f + i * .3f;
                ring.shineBrightness = .7f;
                ring.maxScale = 1.2f + i * .5f;
                ring.Confirm();
            }
            Vector2 scale = new Vector2(4);
            Sparkle.EyeShine(Projectile.Center, Color.Purple, scale, scale, 30);
            Sparkle.NewSparkle(Projectile.Center, Color.Purple, scale, Vector2.Zero, 30, scale);
            scale *= .5f;
            Array.ForEach(Sparkle.EyeShine(Projectile.Center, Color.Purple, scale, scale, 30), s => { s.rotation += MathF.PI / 4f; s.velocity = s.velocity.RotatedBy(MathHelper.PiOver4); });
        }
    }
}
