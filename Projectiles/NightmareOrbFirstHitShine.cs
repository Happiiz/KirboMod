using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class NightmareOrbFirstHitShine : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_1";//dummy texture
        static int AnimationDuration => 10;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 3;
            Projectile.tileCollide = false;
            Projectile.timeLeft = AnimationDuration;
        }
        
        Vector2 DrawPos { get => new(Projectile.localAI[0], Projectile.localAI[1]); set { Projectile.localAI[0] = value.X; Projectile.localAI[1] = value.Y; } }
        public override bool PreDraw(ref Color lightColor)
        {
            float progress = 1 - (float)Projectile.timeLeft / AnimationDuration;
            float time = AnimationDuration - Projectile.timeLeft;
            float opacity = Utils.GetLerpValue(0, 3, time, true) * Utils.GetLerpValue(AnimationDuration, AnimationDuration - 3, time, true);
            if(DrawPos == Vector2.Zero)
            {
                DrawPos = Projectile.Center - Main.screenPosition;
            }
            Color white = Color.White;
            white.A = 0;
            float scaleX = 1 - progress;
            float scaleY = MathHelper.Lerp(0.5f, 66f, progress);
            float fadeThin = Utils.GetLerpValue(AnimationDuration - 7, AnimationDuration, time, true);
            VFX.DrawPrettyStarSparkle(1f,DrawPos, white, white, 1f, 0f, .2f, 2f, 3f, Projectile.velocity.ToRotation(), new Vector2(scaleX, scaleY), new Vector2(fadeThin, fadeThin));
            return false;
        }
        public static Vector2 GetSpawnVelocity(int direction)
        {
            return new Vector2(-3f, direction);   
        }
    }
}
