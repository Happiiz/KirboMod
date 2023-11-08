using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace KirboMod.Items.DarkSword
{
    internal class DarkSwordWave : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.Size = new(2);
            Projectile.tileCollide = false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 86;
            Vector2 offset = new Vector2(0, 64).RotatedBy(Projectile.rotation);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - offset, Projectile.Center + offset, 66, ref a);
        }
        public override void AI()
        {
            base.AI();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            for (int i = 0; i < 4; i++)
            {
                Main.EntitySpriteDraw(tex, Projectile.Center - Projectile.velocity * i, null, Color.Purple * (1 - i / 4f), Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None);
            }
            for (float i = 0; i < .98f; i += 1/4f)
            {
                Vector2 offset = (i * MathF.Tau).ToRotationVector2() * 4;
                Main.EntitySpriteDraw(tex, Projectile.Center + offset, null, Color.Black * 0.25f, Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None);

            }
            return Projectile.DrawSelf();
        }
    }
}
