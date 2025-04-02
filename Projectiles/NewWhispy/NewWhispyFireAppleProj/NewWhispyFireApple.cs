using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.NewWhispy.NewWhispyFireAppleProj
{
    public class NewWhispyFireApple : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
        }
        public static float YAccel => .3f;
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.width = 35;
            Projectile.height = 35;
            Projectile.timeLeft = 400;
            Projectile.Opacity = 0.25f;//quick fade in
        }
        public static float TimeToReachYPoint(float fromY, float toY, float accelY, float initialVelY)
        {
            bool hasSolution = Utils.SolveQuadratic(accelY * .5f, initialVelY, fromY - toY, out float result1, out float result2);
            if (!hasSolution)
            {
                return 99999f;
            }
            float time = MathF.Max(result2, result1);
            return time;
        }
        public static void LaunchAppleTo(NPC npc, Vector2 from, Vector2 target, float initialVelY, int damage)
        {
            float timeToReach = TimeToReachYPoint(from.Y, target.Y, YAccel, initialVelY);
            float xVel = (target.X - from.X) / timeToReach;
            Projectile.NewProjectile(npc.GetSource_FromAI(), from, new Vector2(xVel, initialVelY), ModContent.ProjectileType<NewWhispyFireApple>(), damage, timeToReach, 0);
        }
        public override void AI()
        {
            Projectile.Opacity += 0.25f;//quick fadein
            if (Projectile.frameCounter++ >= 1)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                Projectile.frame %= Main.projFrames[Type];
            }
            Projectile.velocity.Y += YAccel;
            if (Projectile.localAI[0]++ > Projectile.ai[0] && Projectile.ai[0] != 0)
            {
                Projectile.Kill();
            }
            foreach (Player player in Main.ActivePlayers)
            {
                if(player.Hitbox.Intersects(Utils.CenteredRectangle(Projectile.Center, new Vector2(120))))
                {
                    Projectile.Kill();
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.rotation -= MathF.PI / 2;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            VFX.DrawGlowBallAdditive(Projectile.Center, 2, Color.White, default, false);
            return Projectile.DrawSelf();
        }
        public override void OnKill(int timeLeft)
        {
            BOOM();
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Stoned, 60);

            target.AddBuff(BuffID.OnFire, 60 * 20);//20s this attack should be very punishing as its the one ensuring the player stays within fighting zone
        }
        private void BOOM()
        {
            SoundEngine.PlaySound(SoundID.Item14 with { MaxInstances = 0 }, Projectile.Center);
            Projectile.Hitbox = Utils.CenteredRectangle(Projectile.Center, new Vector2(128));
            Projectile.Damage();
            for (int i = 0; i < 40; i++)
            {
                float dist = MathF.Sqrt(Main.rand.NextFloat());
                dist *= 12;
                float angle = Main.rand.NextFloat(MathF.Tau);
                Vector2 offset = angle.ToRotationVector2() * dist;
                Dust d = Dust.NewDustPerfect(Projectile.Center + offset, DustID.RainbowMk2, offset, 0, default, 2);
                d.color = Color.Lerp(Color.Red, Color.Yellow, Main.rand.NextFloat());
                d.noGravity = true;
            }
            for (int i = 0; i < 10; i++)
            {
                float dist = MathF.Sqrt(Main.rand.NextFloat());
                dist *= 5;
                float angle = Main.rand.NextFloat(MathF.Tau);
                Vector2 offset = angle.ToRotationVector2() * dist;
                Dust d = Dust.NewDustPerfect(Projectile.Center + offset, DustID.Smoke, offset, 0, default, 2);
                d.color = Color.Black;
                d.fadeIn = 30;
                d.noGravity = true;
            }
        }
    }
}
