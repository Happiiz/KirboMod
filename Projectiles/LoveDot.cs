using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class LoveDot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;

            //for afterimages
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.scale = 1;
            Projectile.usesLocalNPCImmunity = true; //shares immunity frames with proj of same type
            Projectile.localNPCHitCooldown = 7; //time before hit again
        }
        Vector2 SpawnPos { get => new(Projectile.ai[0], Projectile.ai[1]); }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 8) //changes frames every 3 ticks 
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
            if (Projectile.Opacity == 0)
            {
                Projectile.Kill();
            }
            if (Projectile.localAI[0] > 60)//how many updates you want it to last
            {
                Projectile.Opacity -= 1f / Projectile.oldPos.Length;
            }
            float progress = Projectile.localAI[0] / 60;
            float rotationalOffset = Projectile.ai[2];
            progress = -progress * 0.5f * progress + 2 * progress;
            //basically I want a distance function + a constant changing angle
            Vector2 oldPos = Projectile.Center;
            Projectile.Center = (rotationalOffset + Projectile.localAI[0] * 0.04f).ToRotationVector2() * progress * 400 + SpawnPos;
             Projectile.scale = oldPos.Distance(Projectile.Center) / 40;
            Projectile.localAI[0] += 3;

        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity; // Makes it uneffected by light
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            // Redraw the projectile with the color not influenced by light
            for (int k = Projectile.oldPos.Length - 1; k >= 0; k--)
            {
                for (float i = 0; i < 1; i += .34f)
                {
                    Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
                    Vector2 lerpedPos = k == 0 ? Vector2.Lerp(Projectile.position, Projectile.oldPos[0], i) : Vector2.Lerp(Projectile.oldPos[k - 1], Projectile.oldPos[k], i);
                    lerpedPos -= -drawOrigin + new Vector2(0f, Projectile.gfxOffY) + Main.screenPosition;
                    Color color = (Projectile.GetAlpha(lightColor) * Projectile.Opacity) * (1 - (float)k / Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, lerpedPos, null, color * 0.75f, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
                }
            }
            return Projectile.Opacity == 1; //draw og
        }

    }
}