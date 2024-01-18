using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Stars
{
    public abstract class StarryProj : ModProjectile
    {
        protected virtual void StarStats() { }
        protected Color trailColor = Color.Blue with { A = 0 } * .1f;
        protected Color innerColor = Color.White with { A = 0 } * .5f;
        protected int dustID = DustID.Enchanted_Gold;
        public override bool PreAI()
        {
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 20 + Main.rand.Next(40);
                SoundEngine.PlaySound(SoundID.Item9 with { MaxInstances = 0 }, Projectile.position);
            }
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
            }
            //this is for fade in
            Projectile.alpha += (int)(25f * Projectile.localAI[0]);
            if (Projectile.alpha > 200)
            {
                Projectile.alpha = 200;
                Projectile.localAI[0] = -1f;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
                Projectile.localAI[0] = 1f;
            }
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * Projectile.direction;
            Vector2 screenSize = new Vector2(Main.screenWidth, Main.screenHeight);
            if (Projectile.Hitbox.Intersects(Utils.CenteredRectangle(Main.screenPosition + screenSize / 2f, screenSize + new Vector2(400f))) && Main.rand.NextBool(6))
            {
                int starGoreID = Utils.SelectRandom(Main.rand, 16, 17, 17, 17);
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity * 0.2f, starGoreID);
            }
            if (Main.rand.NextBool(10))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustID, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, default, 1.2f);
            }
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 projVel = Projectile.velocity;
            Texture2D projTexture = TextureAssets.Projectile[Type].Value;
            Vector2 spinOffset = new Vector2(0f, -projTexture.Size().Length()) / 8.90224690738f;//this number is thas the offset for the fallen star texture is 4, and I want to automatically adjust it for other stars
            Texture2D trailTexture = TextureAssets.Extra[91].Value;
            Vector2 trailOrigin = new Vector2(trailTexture.Width / 2f, 10f);
            float time = (float)Main.timeForVisualEffects / 60f;
            Vector2 drawPos = Projectile.Center + projVel * 0.5f + new Vector2(0f, Projectile.gfxOffY) - Main.screenPosition;
            float rotation = projVel.ToRotation() + MathF.PI / 2;
            Main.EntitySpriteDraw(trailTexture, drawPos + spinOffset.RotatedBy(MathF.Tau * time), null, trailColor, rotation, trailOrigin, 1.5f, SpriteEffects.None);
            Main.EntitySpriteDraw(trailTexture, drawPos + spinOffset.RotatedBy(MathF.Tau * time + MathF.Tau / 3f), null, trailColor, rotation, trailOrigin, 1.1f, SpriteEffects.None);
            Main.EntitySpriteDraw(trailTexture, drawPos + spinOffset.RotatedBy(MathF.Tau * time + MathF.PI * .75f), null, trailColor, rotation, trailOrigin, 1.3f, SpriteEffects.None);
            drawPos = Projectile.Center - projVel * 0.5f + new Vector2(0f, Projectile.gfxOffY) - Main.screenPosition;
            for (float i = 0f; i < 1f; i += 0.5f)
            {
                float scale = time % 0.5f / 0.5f;
                scale = (scale + i) % 1f;
                float opacity = scale * 2f;
                if (opacity > 1f)
                {
                    opacity = 2f - opacity;
                }
                Main.EntitySpriteDraw(trailTexture, drawPos, null, innerColor * opacity, rotation + MathF.PI / 2f, trailOrigin, 0.3f + scale * 0.5f, SpriteEffects.None);
            }

            //draw  main proj here
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10);//fallen star impact sound
            Color dustColor = Color.CornflowerBlue;
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 58, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 150, default, 0.8f);
            }
            for (float i = 0f; i < 1f; i += 0.125f)
            {
                Dust.NewDustPerfect(Projectile.Center, 278, Vector2.UnitY.RotatedBy(i * MathF.Tau + Main.rand.NextFloat() * 0.5f) * (4f + Main.rand.NextFloat() * 4f), 150, dustColor).noGravity = true;
            }
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                Dust.NewDustPerfect(Projectile.Center, 278, Vector2.UnitY.RotatedBy(i * MathF.Tau + Main.rand.NextFloat() * 0.5f) * (2f + Main.rand.NextFloat() * 3f), 150, Color.Gold).noGravity = true;
            }
            Vector2 screenSize = new Vector2(Main.screenWidth, Main.screenHeight);
            if (Projectile.Hitbox.Intersects(Utils.CenteredRectangle(Main.screenPosition + screenSize / 2f, screenSize + new Vector2(400f))))
            {
                for (int i = 0; i < 7; i++)
                {
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * Projectile.velocity.Length(), Utils.SelectRandom(Main.rand, 16, 17, 17, 17, 17, 17, 17, 17));
                }
            }
        }
    }
}
