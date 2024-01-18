using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class CutterBlade : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			Projectile.width = 80;
			Projectile.height = 40;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
            Projectile.extraUpdates = 2;
			Projectile.usesLocalNPCImmunity = true; //allows to have npc immunity frames on its own accord
			Projectile.localNPCHitCooldown = 40; //time until it can damage again regardless if a projectile just struck the target
		}
        ref float Timer { get => ref Projectile.localAI[0]; }
        ref float Acceleration { get => ref Projectile.ai[2]; }
        public override void AI()
		{
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 1.2f);
			Player player = Main.player[Projectile.owner];

            if (Timer == 0)
            {
                Projectile.spriteDirection = MathF.Sign(Projectile.velocity.X);
            }
            if (Timer > 20)
            {
                Projectile.velocity.X -= Acceleration * Projectile.spriteDirection;
            }
            int frameSpeed = 2 * Projectile.MaxUpdates;

            ParticleEffect(frameSpeed);
            if (++Projectile.frameCounter > frameSpeed) //changes frames every 3 ticks 
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                    SoundEngine.PlaySound(SoundID.Run.WithVolumeScale(0.5f), Projectile.Center); //(int) converts it to int
                }

            }
            Timer++;
        }
        private void ParticleEffect(int frameSpeed)
        {
            if (Projectile.alpha != 0 || Timer % 10 != 0)
                return;
            Particles.Ring.CutterRing(this);
        }
        public override bool OnTileCollide(Vector2 oldVelocity) 
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position); //impact
            return true; //kill like normal
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void OnKill(int timeLeft)
		{
			if (Projectile.ai[1] != 1) //checks if not killed by player contact
			{
                for (int i = 0; i < 4; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                    Gore.NewGorePerfect(Projectile.GetSource_FromAI(), Projectile.Center, speed, Main.rand.Next(16, 18));
                }

                for (int i = 0; i < 5; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                    Gore.NewGorePerfect(Projectile.GetSource_FromThis(), Projectile.Center, speed, Main.rand.Next(11, 13), Scale: 0.5f); //smoke
                }
            }
        }
    }
}