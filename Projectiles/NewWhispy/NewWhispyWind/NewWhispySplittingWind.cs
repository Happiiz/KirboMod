using KirboMod.NPCs.NewWhispy;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.NewWhispy.NewWhispyWind
{
    public class NewWhispySplittingWind : ModProjectile
    {
        public override string Texture => "KirboMod/Projectiles/NewWhispy/NewWhispyWind/NewWhispyWindBig";

        public static void GetAIValues(Vector2 projSpawnPos, Vector2 targetPos, float projSpeed, out float ai0, out Vector2 projVelocity)
        {
            projSpawnPos -= targetPos;
            ai0 = projSpawnPos.Length() / projSpeed;
            projVelocity = Vector2.Normalize(projSpawnPos) * -projSpeed;
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.CanHitPastShimmer[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 70;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }
        public override void AI()
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud);

            if (Projectile.frameCounter++ >= 2)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Type])
                {
                    Projectile.frame = 0;
                }
            }
            Projectile.ai[0]--;
            if (Projectile.ai[0] < 0)
            {
                Projectile.Kill();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return Projectile.DrawSelf(lightColor);
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(NewWhispyBoss.AirShotSplitSFX, Projectile.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int projCount = NewWhispyBoss.SplittingWindSplitCount;
                float radius = NewWhispyBoss.SplittingWindRadius;
                int type = ModContent.ProjectileType<NewWhispySpiralingWind>();
                for (int i = 0; i < projCount; i++)
                {
                    //100 is duration in ticks
                    NewWhispySpiralingWind.GetAIValues(i, projCount, radius, 160, out float ai0, out float ai1, out float ai2, out Vector2 velocity);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, type, Projectile.damage, 0, -1, ai0, ai1, ai2);
                }
            }
        }
    }
}
