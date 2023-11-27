using KirboMod.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace KirboMod.Particles
{
    public class Ring : Particle
    {
        public Ring(Vector2 position)
        {
            this.position = position;
            texture = VFX.Ring;
            timeLeft = 160;
            squish = Vector2.One;
            scale = 1;
        }
        public int scaleUpTime;
        public Vector2 squish;
        public override void Update()
        {
            base.Update();
            scale = Utils.GetLerpValue(0, scaleUpTime, timeLeft, true);
            scale = 1 - scale;
            scale *= scale;
            scale = 1 - scale;
            scale = MathHelper.Lerp(.5f, 2, scale);
        }
        public override void Draw()
        {
            Main.EntitySpriteDraw(texture, position - Main.screenPosition, null, color * opacity, rotation, texture.Size() / 2, squish * scale, SpriteEffects.None);
        }
        public static Ring DarkMatterSwordShot(Vector2 position)
        {
            Ring ring = new(position);
            ring.scaleUpTime = 30;
            ring.timeLeft = 50;
            return ring;
        }
        public static void DarkMatterSwordSuperShot(Vector2 position)
        {
            DarkMatterSwordShot(position);
            for (int i = 0; i < 2; i++)
            {
                Ring ring = DarkMatterSwordShot(position);
                ring.squish = new Vector2(Main.rand.NextFloat() * .75f + .25f, Main.rand.NextFloat() *.75f + .25f);
            }
        }
    }
}
