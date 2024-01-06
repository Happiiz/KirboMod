using KirboMod.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace KirboMod.Particles
{
    public class StarScalingDown : Particle
    {
        public StarScalingDown(Vector2 position)
        {
            timeLeft = 100;//duration
            texture = TextureAssets.Projectile[ModContent.ProjectileType<BadStar>()];
            rotation = Main.rand.NextFloat() * MathF.Tau;
            this.position = position;
        }

        public override void Draw()
        {
            float scale = Utils.GetLerpValue(0, 100, timeLeft) * 1.2f + 0.2f;
            Main.EntitySpriteDraw(texture.Value, ScreenPos, null, Color.White, rotation + timeLeft * 0.2f, texture.Size() / 2, scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
        }
    }
}
