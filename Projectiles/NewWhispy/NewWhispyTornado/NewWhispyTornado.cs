using KirboMod.Projectiles.Tornadoes;
using Microsoft.Xna.Framework;
using Terraria;

namespace KirboMod.Projectiles.NewWhispy.NewWhispyTornado
{
    public class NewWhispyTornado : Tornado
    {
        public override string Texture => "KirboMod/Projectiles/Tornadoes/tornado_0";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.hostile = true;
            Projectile.height = 130;
            Projectile.width = 100;
            Projectile.tileCollide = true;
            Projectile.Opacity = 0;
        }
        public static void GetAIVAlues(int timeBeforeRising, out float ai0)
        {
            ai0 = timeBeforeRising;
        }
        public override void PostAI()
        {
            base.PostAI();
            Projectile.ai[0]--;
            Projectile.Opacity += 0.2f;
            if (Projectile.ai[0] > 0)
            {
                Projectile.velocity.Y += .6f;
            }
            else
            {
                Projectile.velocity.Y -= 1f;
                Projectile.tileCollide = false;
            }
            Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
        }
        public override Color[] SetPalette()
        {
            Color[] palette = { new(204, 255, 247), new(152, 255, 238), Color.LightCyan };
            return palette;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Height -= 16;
            hitbox.Width -= 16;
            hitbox.X += 8;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
    }
}
