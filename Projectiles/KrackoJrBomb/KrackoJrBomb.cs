using KirboMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;


namespace KirboMod.Projectiles.KrackoJrBomb
{
    internal class KrackoJrBomb : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = .6f;
            Projectile.Size = new Vector2(120);
            Projectile.alpha = 255;
        }
        static float GetExplosionSizeMultiplier()
        {
           if (Main.getGoodWorld)
            {
                return 4;
            }
            if (Main.expertMode)
            {
                return 2;
            }
            return 1;
        }
        static int GetExplosionDuration()
        {
            if (Main.getGoodWorld)
            {
                return 60 * 3;
            }
            if (Main.expertMode)
            {
                return 40;
            }
            return 10;
        }
        Vector2 RndCircleOffset { get => Main.rand.NextVector2Circular(Projectile.width, Projectile.height); }
        Vector2 RndInCircle { get => Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height); }
        Vector2 RndInHitbox { get => Main.rand.NextVector2FromRectangle(Projectile.Hitbox); }
        int TargetPlayerIndex { get => (int)Projectile.ai[0]; }
        ref float Timer { get => ref Projectile.ai[1]; }
        bool Exploding { get => Projectile.ai[2] == 1; set => Projectile.ai[2] = value ? 1 : 0; }
        public override void AI()
        {
            float spawnTime = 6;
            Projectile.rotation += MathF.Cos(Projectile.whoAmI) / 30f - .02f;
            if (!Exploding)
            {
                Projectile.velocity.Y += .3f;
            }
            else
            {
                Projectile.velocity = Vector2.Zero;
            }
            float spawnAnimation = Utils.GetLerpValue(0, spawnTime, Timer, true);
            spawnAnimation = Easings.EaseInOutSine(spawnAnimation);
            Projectile.Opacity = spawnAnimation;
            Projectile.scale = MathHelper.Lerp(.6f, 1, spawnAnimation);
            if (Timer > spawnTime && !Exploding)
            {
                Player target = Main.player[TargetPlayerIndex];
                if (target.Center.Y < Projectile.Center.Y)
                {
                    Exploding = true;
                    Timer = 1000000;
                    Projectile.Hitbox = Utils.CenteredRectangle(Projectile.Center, Projectile.Size * GetExplosionSizeMultiplier());
                    Projectile.netUpdate = true;
                }
            }
            if (Exploding)
            {
                if (Timer > 1000000 + GetExplosionDuration())
                {
                    Projectile.Kill();
                }
                if (Timer % 2 == 0)
                {
                    //bomb sfx
                    SoundEngine.PlaySound(SoundID.Item14 with { MaxInstances = 0, Volume = .6f, PitchVariance = .2f }, Projectile.Center);
                }
                if (Timer % 4 == 0)
                {
                    //tactical shotgun sfx
                    SoundEngine.PlaySound(SoundID.Item38 with { MaxInstances = 0, Volume = .6f, PitchVariance = .2f }, Projectile.Center);
                }
                int max = Main.getGoodWorld ? 4 : 2;
                for (int i = 0; i < max; i++)
                {
                    Vector2 scale = new Vector2(1, 1).RotatedByRandom(.5f) * 1.2f;
                    Vector2 offset = RndCircleOffset / 2f;
                    Sparkle sparkle = new(Projectile.Center + offset, Color.OrangeRed, Vector2.Zero, scale * 2, scale * 2, 3);
                    sparkle.rotation = Main.rand.NextBool() ? 0 : MathHelper.PiOver4;
                    sparkle.Confirm();
                    Ring ring = Ring.EmitRing(Projectile.Center + offset, Color.Lerp(Color.Orange * .7f, Color.Black, Main.rand.NextFloat()));
                    ring.squish = scale;
                    Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromAI(), Projectile.Center + Main.rand.NextVector2Circular(4, 4) * GetExplosionSizeMultiplier(), Main.rand.NextVector2Circular(4, 4), Main.rand.NextFromList(GoreID.Smoke1, GoreID.Smoke2, GoreID.Smoke2), Main.rand.NextFloat() * .3f + .9f);
                    gore.rotation = Main.rand.NextFloat() * MathF.Tau;
                    Dust.NewDustPerfect(Projectile.Center + RndCircleOffset / 3, DustID.Torch, -Vector2.UnitY.RotatedByRandom(1) * (Main.rand.NextFloat() * 4 + 2), 0, default, 3);
                }
            }
            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!Exploding)
            {
                Texture2D tex = TextureAssets.Projectile[Type].Value;
                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, new Vector2(41.5f, 51.5f), Projectile.scale, SpriteEffects.None);
            }
            return false;
        }
        public override void OnKill(int timeLeft)
        {

        }
    }
}
