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
            texture = VFX.ring;
            timeLeft = 160;
            squish = Vector2.One;
            scale = 1;
            scaleUpTime = .0001f;
            shineBrightness = 1;
            minScale = .15f;
            fadeInTime = 5;
            fadeOutTime = 5;
            timeLeft = 10 ;
            scaleUpTime = 10;
            maxScale = 1;
        }
        public float maxScale;
        public float minScale;
        public float scaleUpTime;
        public float shineBrightness;
        public override void Update()
        {
            base.Update();
            scale = Utils.GetLerpValue(0, scaleUpTime, Duration - timeLeft, true);
            scale = 1 - scale;
            scale *= scale;
            scale = 1 - scale;
            opacity = Utils.GetLerpValue(0, fadeInTime, Duration - timeLeft, true) * Utils.GetLerpValue(0, fadeOutTime, timeLeft, true);
            scale = MathHelper.Lerp(minScale, maxScale, scale);
        }
        public override void Draw()
        {
            Main.EntitySpriteDraw(texture.Value, position - Main.screenPosition, null, color * opacity, rotation, texture.Size() / 2, squish * scale, SpriteEffects.None);
            Main.EntitySpriteDraw(VFX.ringShine.Value, position - Main.screenPosition, null, Color.White with { A = 0 } * opacity * shineBrightness, rotation, texture.Size() / 2, squish * scale, SpriteEffects.None);
        }
        public static Ring ShotRing(Vector2 position, Color color, Vector2 direction)
        {
            Ring ring = new(position);
            ring.opacity = 1;
            ring.color = color;
            ring.scaleUpTime = 20;
            ring.squish = new Vector2(.5f, 1);
            ring.rotation = direction.ToRotation();
            ring.fadeOutTime = 10;
            ring.timeLeft = 20;
            ring.Confirm();
            return ring;
        }
        public static Ring CutterRing(ModProjectile proj)
        {
            Ring ring = new(proj.Projectile.Center);
            ring.opacity = 1;
            ring.fadeInTime = 5;
            ring.fadeOutTime = 5;
            ring.color = (proj is BadCutter ? Color.Gold : Color.Cyan) * .6f;
            ring.shineBrightness = .6f;
            ring.timeLeft = proj is BadCutter ? 10 : 10;
            ring.scaleUpTime = ring.timeLeft;

            ring.minScale = .5f;
            ring.maxScale = 1;
            ring.squish = new Vector2(1, .5f);
            ring.Confirm(true);
            return ring;
        }
        public static Ring EmitRing(Vector2 position, Color color)
        {
            Ring ring = new(position);
            ring.opacity = 1;
            ring.color = color;
            ring.scaleUpTime = 20;
            ring.rotation = MathF.Tau * Main.rand.NextFloat();
            if ((ring.color.R + ring.color.B + ring.color.G) / 756f < .25f)
                ring.shineBrightness = 0;
            ring.Confirm();
            return ring;
        }
        public static Ring DarkMatterSwordShot(Vector2 position)
        {
            Ring ring = new(position);
            ring.minScale = .1f;
            ring.maxScale = 1;
            ring.scaleUpTime = 10;
            ring.timeLeft = 15;
            ring.fadeInTime = 2;
            ring.fadeOutTime = 5;
            ring.shineBrightness = 0;
            ring.color = Color.Black;
            ring.Confirm();
            return ring;
        }
        public static void DarkMatterSwordSuperShot(Vector2 position)
        {
            DarkMatterSwordShot(position);
            for (int i = 0; i < 2; i++)
            {
                Ring ring = DarkMatterSwordShot(position);
                ring.scaleUpTime = 10;
                ring.timeLeft = 15;
                ring.fadeInTime = 2;
                ring.fadeOutTime = 5;
                ring.minScale = .05f;
                ring.maxScale = .8f;
                ring.shineBrightness = 0;
                ring.squish = new Vector2(Main.rand.NextFloat() * .75f + .25f, Main.rand.NextFloat() *.75f + .25f);
                ring.Confirm();
            }
        }
    }
}
