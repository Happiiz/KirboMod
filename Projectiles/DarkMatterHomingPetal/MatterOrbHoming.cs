using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.DarkMatterHomingOrb
{

    public class MatterOrbHoming : MatterOrb
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.timeLeft = 4000;
        }
        public static void SpawnHomingOrb(IEntitySource source, Vector2 pos, Vector2 vel, int damage, int targetIndex, float homingStrength = -1, float homingMaxVel = -1)
        {
            if (homingStrength < 0)
            {
                homingStrength = DefaultHomingStrength;
            }
            if (homingMaxVel <= 0)
            {
                homingMaxVel = DefaultHomingMaxVel;
            }
            Projectile.NewProjectile(source, pos, vel, ModContent.ProjectileType<MatterOrbHoming>(), damage, 0f, -1, targetIndex, homingStrength, homingMaxVel);
        }
        public static float DefaultHomingStrength => 0.05f;
        public static float DefaultHomingMaxVel => 20f;
        int TargetPlayerIndex => (int)Projectile.ai[0];
        static int DecelerateStartTime => 20;
        static int DecelerateDuration => 30;
        static int HomingStartTime => 40;
        static int HomingDuration => 140;
        static int HomingFadeoutDuration => 30;
        static int TotalHomingDuration => HomingDuration + HomingFadeoutDuration;
        ref float HomingStrength => ref Projectile.ai[1];
        ref float HomingMaxVel => ref Projectile.ai[2];
        static float DecelerateAmount => 0.98f;
        public override bool CanHitPlayer(Player target)
        {
            //if invalid player target index, then can hit any player
            if (TargetPlayerIndex < 0 || TargetPlayerIndex >= Main.maxPlayers)
            {
                return true;
            }
            //otherwise, can only hit the target index
            return target.whoAmI == TargetPlayerIndex;
        }
        public override void AI()
        {
            if(Main.myPlayer != TargetPlayerIndex)
            {
                Projectile.Opacity = 0.2f;
            }

            //why this??? investigate later
            if(Timer < 30)
            {
                Timer = 30;
            }
            if (++Projectile.frameCounter >= 3) //changes frames every 3 ticks 
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
            Timer++;
            //if (Timer > DecelerateStartTime && Timer <= HomingStartTime && Timer < DecelerateStartTime + DecelerateDuration)
            //{
            //    Projectile.velocity *= DecelerateAmount;
            //}
            if (Timer > HomingStartTime && Timer < TotalHomingDuration)
            {
                Vector2 targetVel = Projectile.velocity.SafeNormalize(Vector2.Zero) * HomingMaxVel;
                float homingStrength = Utils.Remap(Timer, HomingStartTime + HomingDuration, HomingStartTime + TotalHomingDuration, HomingStrength, 0f);
                if (TargetPlayerIndex >= 0 && TargetPlayerIndex < Main.maxPlayers)
                {
                    Player targetPlayer = Main.player[TargetPlayerIndex];
                    targetVel = Projectile.DirectionTo(targetPlayer.Center) * HomingMaxVel;
                }
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVel, homingStrength);
            }
            if(Timer >= TotalHomingDuration)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.velocity.SafeNormalize(Vector2.Zero) * HomingMaxVel, 0.05f);
            }
        }
    }
}
