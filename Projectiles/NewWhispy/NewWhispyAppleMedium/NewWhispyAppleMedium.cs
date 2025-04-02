using KirboMod.NPCs.NewWhispy;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.NewWhispy.NewWhispyAppleMedium
{
    internal class NewWhispyAppleMedium : ModProjectile
    {
        public static int MaxBounces => 5;
        public override string Texture => "KirboMod/Projectiles/NewWhispy/NewWhispyAppleMedium/Apple";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CanHitPastShimmer[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 35;
            Projectile.height = 35;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.penetrate = MaxBounces + 1;
        }
        ref float YPosToBecomeSolid => ref Projectile.ai[1];
        ref float BounceDirection => ref Projectile.ai[2];
        ref float PlayerTarget => ref Projectile.ai[2];//not a mistake.
        public static void GetAIValues(float telegraphTime, float yPosThresholdToBecomeSolid, int targetPlayerIndex, out float ai0, out float ai1, out float ai2)
        {

            ai0 = -telegraphTime;
            ai1 = yPosThresholdToBecomeSolid;
            ai2 = targetPlayerIndex;
        }
        public override bool PreAI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = Projectile.ai[0];
            }
            if (Projectile.ai[0] < 0)
                Projectile.ai[0]++;
            Projectile.scale = Utils.GetLerpValue(Projectile.localAI[0], Projectile.localAI[0] + 10, Projectile.ai[0], true);
            return Projectile.ai[0] >= 0;
        }
        public override void AI()
        {
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 9999;
                SoundEngine.PlaySound(NewWhispyBoss.ObjFallSFX, Projectile.Center);
            }
            Projectile.tileCollide = Projectile.position.Y + Projectile.height / 2 > YPosToBecomeSolid;
            Projectile.velocity.Y += 0.4f;
            Projectile.rotation += Projectile.velocity.X * 0.03f;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ice_Red, Projectile.velocity.X, Projectile.velocity.Y);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return Projectile.DrawSelf(lightColor);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if(Projectile.penetrate == MaxBounces + 1)
            {
                if (PlayerTarget < 0 || PlayerTarget >= Main.maxPlayers)
                {
                    Projectile.Kill();
                    return false;
                }
                BounceDirection = MathF.Sign(Main.player[(int)PlayerTarget].Center.X - Projectile.Center.X);
            }
            Projectile.velocity.X = BounceDirection * 4;
            Projectile.penetrate--;
            Projectile.velocity.Y = -Utils.Remap(Projectile.penetrate, MaxBounces, 0, 15f, 7f);
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            return false;
        }
    }
}
