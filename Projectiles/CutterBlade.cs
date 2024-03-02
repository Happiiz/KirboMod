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
			Projectile.width = 40;
			Projectile.height = 40;
            DrawOffsetX = -20;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
            Projectile.extraUpdates = 2;
			Projectile.usesLocalNPCImmunity = true; //allows to have npc immunity frames on its own accord
			Projectile.localNPCHitCooldown = 40; //time until it can damage again regardless if a projectile just struck the target
        }
        ref float Timer { get => ref Projectile.localAI[0]; }
        ref float Direction { get => ref Projectile.ai[0]; } 
        ref float Acceleration { get => ref Projectile.ai[1]; }
        ref float EffectCooldown { get => ref Projectile.ai[2]; } //made so effect doesn't stack a bunch when cutter is bouncing rapidly (set to 20 by item code)

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
                if (Direction == 1)
                {
                    Projectile.velocity.X -= Acceleration;

                    if (Projectile.velocity.X < -10)
                    {
                        Projectile.velocity.X = -10;
                    }
                }
                else
                {
                    Projectile.velocity.X += Acceleration;

                    if (Projectile.velocity.X > 10)
                    {
                        Projectile.velocity.X = 10;
                    }
                }
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

            EffectCooldown++;
        }
        private void ParticleEffect(int frameSpeed)
        {
            if (Projectile.alpha != 0 || Timer % 10 != 0 || EffectCooldown < 20)
                return;
            Particles.Ring.CutterRing(this);
        }
        public override bool OnTileCollide(Vector2 oldVelocity) 
        {
            //SoundEngine.PlaySound(SoundID.Item10, Projectile.position); //impact

            if (Projectile.velocity.X != oldVelocity.X) //bounce
            {
                Projectile.velocity.X = -oldVelocity.X;
                Timer = 20; //skip ahead

                //switch direction it's going
                if (Projectile.velocity.X < 0 && Direction == -1)
                {
                    Direction = 1;
                }
                if (Projectile.velocity.X >= 0 && Direction == 1)
                {
                    Direction = -1;
                }

                EffectCooldown = 0;
            }

            if (Projectile.velocity.Y != oldVelocity.Y) //bounce
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false; 
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                Gore.NewGorePerfect(Projectile.GetSource_FromAI(), Projectile.Center, speed, Main.rand.Next(16, 18));
            }

            for (int i = 0; i < 5; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                Gore.NewGorePerfect(Projectile.GetSource_FromThis(), Projectile.Center, speed, Main.rand.Next(11, 13), 1f); //smoke
            }
        }
    }
}