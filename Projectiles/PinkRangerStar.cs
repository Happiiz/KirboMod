using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class PinkRangerStar : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 36;
			Projectile.height = 36;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 180;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

        }
		public override void AI()
		{
			Lighting.AddLight(Projectile.Center, 0.255f, 0.255f, 0f);
			Projectile.rotation += 0.3f * (float)Projectile.direction; // rotates projectile
            if (Main.rand.NextBool(3)) // happens 1/3 times
            {
				int dustnumber = Dust.NewDust(Projectile.position, 36, 36, DustID.Enchanted_Pink, 0f, 0f, 200, default, 1f); //dust
				Main.dust[dustnumber].velocity *= 0.3f;
			}

			Projectile.ai[0]++;
			if (Projectile.ai[0] == 1) //if ai equal 1
            {
				SoundEngine.PlaySound(SoundID.MaxMana, Projectile.position); //star sound
			}
		}

        public override void OnKill(int timeLeft) //when the projectile dies
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 speed = Main.rand.BetterNextVector2Circular(5f); //circle
                Dust.NewDustPerfect(Projectile.Center, DustID.Enchanted_Pink, speed, Scale: 1.3f); //Makes dust in a messy circle
            }
            for (int i = 0; i < 5; i++)
            {
                Vector2 speed = Main.rand.BetterNextVector2Circular(5f); //circle
                Dust.NewDustPerfect(Projectile.Center, DustID.Confetti_Pink, speed, Scale: 1.6f); //Makes dust in a messy circle
            }
            for (int i = 0; i < 5; i++)
            {
                Vector2 speed = Main.rand.BetterNextVector2Circular(5f); //circle
                Gore gore = Gore.NewGorePerfect(Projectile.GetSource_FromAI(), Projectile.Center, speed, Main.rand.Next(16, 18), 1f);
				gore.scale = 1.5f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			SoundEngine.PlaySound(SoundID.Item10, Projectile.position); //impact

			return true; //collision
		}

        public override Color? GetAlpha(Color lightColor)
        {
			return Color.White; // Makes it uneffected by light
        }
    }
}