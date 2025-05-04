using KirboMod.Projectiles.NewWhispy.NewWhispyWind;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class Puff : NewWhispySpiralingWind
    {
        public static void GetAIValues(int index, int projCount, float radius, int duration, out float ai0, out float ai1, out float ai2, out Vector2 velocity, float randNumberFrom0ToTauDeterminedOutsideLoop)
        {
            ai0 = -MathF.Tau / duration;
            ai1 = (float)index / projCount;
            ai1 += randNumberFrom0ToTauDeterminedOutsideLoop;
            ai2 = duration;
            velocity = Vector2.Zero;
            velocity.X = radius;
        }
        public override string Texture => "KirboMod/Projectiles/NewWhispy/NewWhispyWind/NewWhispyWindSmall";
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.DamageType = DamageClass.Magic;
        }
        static int ApplyFalloff(int currentDmg)
        {
            int result = (int)(currentDmg * 0.8f);
            if(result <= 5)
            {
                return 0;
            }
            return result;
        }
        public override void AI()
        {
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud);
            }
            if (Timer == 0)
            {
                int dmg = Projectile.damage;
                for (int i = 0; i < 50; i++)//50 is an arbitrary value just so it doesn't result in an infinite loop somehow
                {
                    dmg = ApplyFalloff(dmg);
                    if (dmg <= 0)
                    {
                        Projectile.penetrate = i;
                        Projectile.maxPenetrate = i;
                        break;
                    }
                }
                SpiralPivot = Projectile.Center;
            }
            Timer++;
            if (Projectile.frameCounter++ >= 2)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Type])
                {
                    Projectile.frame = 0;
                }
            }
            float rotationAmount = RotationPerFrame * Timer + NormalizedRotationOffset * MathF.Tau;
            float progress = Timer / TimeLeftUntilDeath;
            Vector2 offset = new Vector2(MathF.Cos(rotationAmount), MathF.Sin(rotationAmount)) * progress * SpiralRadius;
            Projectile.Center = SpiralPivot + offset;
            if (Timer > TimeLeftUntilDeath)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud);
                }
                Projectile.Kill();
            }
            Projectile.scale = (float)Projectile.penetrate / Projectile.maxPenetrate;
        }

        private void SetupPenetrateAmount()
        {
            int dmg = Projectile.damage;
            for (int i = 0; i < 50; i++)//50 is an arbitrary value just so it doesn't result in an infinite loop somehow
            {
                dmg = ApplyFalloff(dmg);
                if (dmg <= 0)
                {
                    Projectile.penetrate = i;
                    Projectile.maxPenetrate = i;
                    break;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = ApplyFalloff(Projectile.damage);
        }
    }
}