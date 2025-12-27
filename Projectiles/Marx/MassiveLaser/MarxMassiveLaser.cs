using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Marx.MassiveLaser
{
    public class MarxMassiveLaser : ModProjectile
    {
        static bool[] playersHit;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CanHitPastShimmer[Type] = true;
            Main.projFrames[Type] = 6;
            //increased distance before the laser is culled
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = LaserLength;
        }
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.height = 150 * 2;
            Projectile.width = 182 * 2;
            Projectile.tileCollide = false;
            Projectile.scale = 1f / 5f;
            Projectile.penetrate = -1;
            playersHit = new bool[Main.maxPlayers];
        }
        //debug value, change be later
        static int LaserLength => 16 * 400;
        ref float Timer => ref Projectile.localAI[0];
        ref float LaserDuration => ref Projectile.ai[0];
        public override void AI()
        {
            FrameCounting();
            Timer++;
            Projectile.scale = Utils.GetLerpValue(0f, 5f, Timer, true) * Utils.GetLerpValue(LaserDuration, LaserDuration - 5, Timer, true);
            Projectile.scale = Easings.EaseOutSquare(Projectile.scale) * 3f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        private void FrameCounting()
        {
            if (++Projectile.frameCounter >= 1)
            {
                Projectile.frame++;
                Projectile.frame %= Main.projFrames[Type];
                Projectile.frameCounter = 0;
            }
        }
        static void AABBLineVisualizer(Vector2 lineStart, Vector2 lineEnd, float lineWidth)
        {
            Texture2D blankTexture = Terraria.GameContent.TextureAssets.Extra[ExtrasID.RainbowRodTrailColorGradient].Value;
            Vector2 texScale = new Vector2((lineStart - lineEnd).Length(), lineWidth) * 0.00390625f;//1/256, texture is 256x256
            Main.EntitySpriteDraw(blankTexture, (lineStart) - Main.screenPosition, null, Color.Red, (lineEnd - lineStart).ToRotation(), new Vector2(0, 128), texScale, SpriteEffects.None);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            int frameHeight = tex.Height / Main.projFrames[Type];
            int frameY = (Main.projFrames[Type] - Projectile.frame - 1) * frameHeight;
            Rectangle bodyFrame = new(0, frameY, 910, frameHeight);
            Rectangle capFrame = new(910, frameY, tex.Width - 910, frameHeight);
            Vector2 bodyOrigin = new(0, bodyFrame.Height / 2f);
            Vector2 capOrigin = new(capFrame.Width / 2f, capFrame.Height / 2f);
            Vector2 dir = -Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Projectile.rotation = dir.ToRotation();
            float laserLength = LaserLength;
            int segments = Math.Max(0, (int)MathF.Ceiling(laserLength / (bodyFrame.Width * Projectile.scale)));
            Vector2 start = Projectile.Center - Main.screenPosition;
            Color drawCol = Color.White;
            drawCol.A = 200;
            float cullScale = 2f * 1.5f;
            Main.EntitySpriteDraw(tex, start, capFrame, drawCol, Projectile.rotation, capOrigin, Projectile.scale, SpriteEffects.FlipHorizontally);
            for (int i = 0; i < segments; i++)
            {
                Vector2 pos = start + dir * (capFrame.Width / 2f * Projectile.scale + i * bodyFrame.Width * Projectile.scale);
                //cull individual segments for performance
                if (pos.X > Main.screenWidth + bodyFrame.Width * cullScale || pos.X < -bodyFrame.Width * cullScale || pos.Y > Main.screenHeight + bodyFrame.Height * cullScale || pos.Y < -bodyFrame.Height * cullScale)
                {
                    continue;
                }
                Main.EntitySpriteDraw(tex, pos, bodyFrame, drawCol, Projectile.rotation, bodyOrigin, Projectile.scale, SpriteEffects.None);
            }
            Vector2 capPos = start + dir * (segments * bodyFrame.Width * Projectile.scale + capFrame.Width * Projectile.scale);
            Main.EntitySpriteDraw(tex, capPos, capFrame, drawCol, Projectile.rotation, capOrigin, Projectile.scale, SpriteEffects.None);


            //  GetLaserCollisionParams(out start, out Vector2 end, out float width);
            // AABBLineVisualizer(start, end, width);
            return false;
        }

        private void LaserDraw_Old()
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            int frameHeight = tex.Height / Main.projFrames[Type];
            int frameY = (Main.projFrames[Type] - Projectile.frame - 1) * frameHeight;
            Rectangle bodyFrame = new(0, frameY, 182, frameHeight);
            Rectangle capFrame = new(182, frameY, tex.Width - 182, frameHeight);
            Vector2 bodyOrigin = new(0, bodyFrame.Height / 2f);
            Vector2 capOrigin = new(capFrame.Width / 2f, capFrame.Height / 2f);
            Vector2 dir = -Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Projectile.rotation = dir.ToRotation();
            float laserLength = LaserLength;
            int segments = Math.Max(0, (int)MathF.Ceiling(laserLength / (bodyFrame.Width * Projectile.scale)));
            Vector2 start = Projectile.Center - Main.screenPosition;
            Color drawCol = Color.White;
            drawCol.A = 200;
            float cullScale = 2f * 1.5f;
            Main.EntitySpriteDraw(tex, start, capFrame, drawCol, Projectile.rotation, capOrigin, Projectile.scale, SpriteEffects.FlipHorizontally);
            for (int i = 0; i < segments; i++)
            {
                Vector2 pos = start + dir * (capFrame.Width / 2f * Projectile.scale + i * bodyFrame.Width * Projectile.scale);
                //cull individual segments for performance
                if (pos.X > Main.screenWidth + bodyFrame.Width * cullScale || pos.X < -bodyFrame.Width * cullScale || pos.Y > Main.screenHeight + bodyFrame.Height * cullScale || pos.Y < -bodyFrame.Height * cullScale)
                {
                    continue;
                }
                Main.EntitySpriteDraw(tex, pos, bodyFrame, drawCol, Projectile.rotation, bodyOrigin, Projectile.scale, SpriteEffects.None);
            }
            Vector2 capPos = start + dir * (segments * bodyFrame.Width * Projectile.scale + capFrame.Width * Projectile.scale);
            Main.EntitySpriteDraw(tex, capPos, capFrame, drawCol, Projectile.rotation, capOrigin, Projectile.scale, SpriteEffects.None);
        }

        void GetLaserCollisionParams(out Vector2 start, out Vector2 end, out float width)
        {
            width = 150 * Projectile.scale;
            start = Projectile.Center;
            end = Projectile.Center - Projectile.velocity.Normalized(LaserLength);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if(Timer < 5 || Timer > LaserDuration - 5)
            {
                return false;
            }
            float colPoint = 0;
            GetLaserCollisionParams(out Vector2 start, out Vector2 end, out float width);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref colPoint);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            playersHit[target.whoAmI] = true;
        }
        public override bool CanHitPlayer(Player target)
        {
            return !playersHit[target.whoAmI];
        }
    }
}
