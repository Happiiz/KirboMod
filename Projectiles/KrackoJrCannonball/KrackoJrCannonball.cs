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

namespace KirboMod.Projectiles.KrackoJrCannonball
{
    internal class KrackoJrCannonball : ModProjectile
    {
        private struct CannonbalTrail
        {
            public int timeLeft;
            public Vector2 position;
            public CannonbalTrail(Vector2 pos)
            {
                timeLeft = 13;
                position = pos;
            }
            public void Draw(Texture2D tex)
            {
                Main.spriteBatch.Draw(tex, position - Main.screenPosition, null, Color.White * Utils.GetLerpValue(0, 13, timeLeft, true), 0, tex.Size() / 2, 1, SpriteEffects.None, 0); ;
            }
            public static void SpawnCloud(KrackoJrCannonball cannonball)
            {
                if (cannonball.trail == null)
                    cannonball.trail = new(16);
                if (cannonball.trail.Count < 13)
                {
                    cannonball.trail.Add(new CannonbalTrail(cannonball.Projectile.Center + cannonball.SpinOffset));
                }
            }
        }
        List<CannonbalTrail> trail = new(16);
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(30, 30);
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 2400;
        }
        public static float GetHitboxOffset()
        {
            return Main.getGoodWorld ? 30 : Main.expertMode ? 20 : 10;
        }
        Vector2 SpinOffset { get => Projectile.ai[0].ToRotationVector2() * GetHitboxOffset(); }
        public override void AI()
        {
            if (Projectile.ai[1] % 2 == 0)
            {
                CannonbalTrail.SpawnCloud(this);
            }
            for (int i = 0; i < trail.Count; i++)
            {
                CannonbalTrail img = trail[i];
                img.timeLeft--;
                if(img.timeLeft <= 0)
                {
                    trail.RemoveAt(i);
                    i--;
                    continue;
                }
                trail[i] = img;
            }
            Projectile.ai[1]++; //timer
            Projectile.ai[0] += MathF.Tau / 8f;//spin speed
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            for (int i = trail.Count - 1; i >= 0; i--)
            {
                trail[i].Draw(tex);
            }
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + SpinOffset, null, Color.White * Projectile.Opacity, 0, tex.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            Vector2 offset = SpinOffset;
            hitbox.X += (int)offset.X;
            hitbox.Y += (int)offset.Y;
        }
    }
}
