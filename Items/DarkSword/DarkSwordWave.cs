using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.DarkSword
{
    internal class DarkSwordWave : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.Size = new(20);
            Projectile.tileCollide = false;
            Projectile.scale = 2;
            Projectile.friendly = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.scale *= 1.9f;
            Projectile.timeLeft = 10 * 60 * Projectile.MaxUpdates;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 86;
            Vector2 offset = 64 * (Projectile.rotation + MathF.PI * 0.5f).ToRotationVector2() * Projectile.scale;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - offset, Projectile.Center + offset, 132, ref a);
        }
        public override void AI()
        {
            Projectile.ai[0]++;
            if(Projectile.ai[0] < 0)
            {
                Player player = Main.player[Projectile.owner];
                Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) - Projectile.velocity;
                return;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Opacity += .111f;

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Color[] palette = new Color[] { Color.Black, new Color(39, 0, 65), new Color(86,0,144), new Color(152, 0, 255) };
            for (int i = 0; i < 4; i++)
            {
                float opacity = Utils.GetLerpValue(0, 3, Projectile.ai[0] - i, true);
                Main.EntitySpriteDraw(tex, Projectile.Center - Vector2.Normalize(Projectile.velocity) * 48 * i - Main.screenPosition, null, palette[i] * opacity, Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None);
            }
            for (float i = 0; i < .98f; i += 1/4f)
            {
                Vector2 offset = (i * MathF.Tau + (float)Main.timeForVisualEffects / 20f).ToRotationVector2() * 6;
                Main.EntitySpriteDraw(tex, Projectile.Center + offset - Main.screenPosition, null, palette[3] * Projectile.Opacity, Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None);

            }
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.Black * Projectile.Opacity, Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None);

            return false;
        }
    }
}
