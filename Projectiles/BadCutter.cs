using KirboMod.NPCs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BadCutter : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			Projectile.width = 80;
			Projectile.height = 40; //30 less than sprite
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.tileCollide = false;
			Projectile.penetrate = 1;
		}
		static bool TimeFromMRUV(float targetSpace ,float initialSpace, float initialVel, float acceleration, out float t1, out float t2)
        {
			float delta = initialVel * initialVel + 2 * acceleration * (initialSpace - targetSpace);
			if (delta < 0)
			{
				t1 = float.NaN;
				t2 = float.NaN;
				return false;
			}
			delta = MathF.Sqrt(delta);
			t1 = (-initialVel + delta) / acceleration;
			t2 = (-initialVel - delta) / acceleration;
			return true;
        }
		
		static float GetCutterYVelocity(Vector2 target, Vector2 initialSpace, float maxSpeed, float acceleration)
        {
			maxSpeed *= MathF.Sign(target.X - initialSpace.X);
			acceleration *= MathF.Sign(target.X - initialSpace.X);
			int timeToStartDecelerating = 20;
			initialSpace.X += timeToStartDecelerating * maxSpeed;
			if(!TimeFromMRUV(target.X, initialSpace.X, maxSpeed, acceleration, out float t1, out float t2))
            {
				return 0;
            }
			float time = MathF.Min(t2,t1);//both are negative for some reason so use min instead of max
			time = MathF.Abs(time);
            time += timeToStartDecelerating;
			return (target.Y - initialSpace.Y) / time;
        }
		static float GetTimeToGoBackToKibble(SirKibble kib, float maxSpeed, float acceleration)
        {
			Vector2 initialSpace = kib.NPC.Center;
			int timeToStartDecelerating = 20;
			initialSpace.X += timeToStartDecelerating * maxSpeed;
			if (!TimeFromMRUV(kib.NPC.Center.X, initialSpace.X, maxSpeed, acceleration, out float t1, out float t2))
			{
				return 100;
			}
			float time = MathF.Min(t2, t1);//both are negative for some reason so use min instead of max
			time = MathF.Abs(time);
			time += timeToStartDecelerating;
			return time;
		}
		public static void ShootBadCutter(NPC kibble, float maxSpeed = 20, float acceleration = .25f, int damage = 40)
        {
			if(Main.netMode == NetmodeID.MultiplayerClient)
            {
				return;
            }
			SirKibble kib = (SirKibble)kibble.ModNPC;
			Vector2 velocity = new Vector2(maxSpeed * kibble.spriteDirection, 0);
			velocity.Y = GetCutterYVelocity(Main.player[kibble.target].MountedCenter, kibble.Center, maxSpeed, acceleration);
			Projectile.NewProjectile(kibble.GetSource_FromAI(), kibble.Center, velocity, ModContent.ProjectileType<BadCutter>(), damage / 2, 0, -1, kibble.whoAmI, maxSpeed, acceleration);
			float maxUpdates = ContentSamples.ProjectilesByType[ModContent.ProjectileType<BadCutter>()].MaxUpdates;
			kib.TimeWhenCutterBladeReachesKibbleAgain = GetTimeToGoBackToKibble(kib, maxSpeed * maxUpdates, acceleration * maxUpdates);
			kib.MostRecentCutterYVelocity = velocity.Y * kib.TimeWhenCutterBladeReachesKibbleAgain + kib.NPC.Center.Y;
		}
		bool ReturnedToKibble { get => Projectile.ai[0] == 255 && MathF.Sign(Projectile.velocity.X) != Projectile.spriteDirection; set
			{
				if (value)
                {
					Projectile.ai[0] = 255;
                }
			} 
		}
		int KibbleIndex { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
		ref float Timer { get => ref Projectile.localAI[0]; }
		ref float MaxSpeed { get => ref Projectile.ai[1]; }
		ref float Acceleration { get => ref Projectile.ai[2]; }
		NPC Kibble { get => Main.npc[KibbleIndex]; }
		int frame = 0;
		public override void AI()
		{
			Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 1.2f);

			if (Timer == 0)
            {
                Projectile.spriteDirection = MathF.Sign(Projectile.velocity.X);
            }
            if (Timer > 20 && !ReturnedToKibble)
            {
                Projectile.velocity.X -= Acceleration * Projectile.spriteDirection;
            }
            if (!ReturnedToKibble && Kibble.Hitbox.Intersects(Projectile.Hitbox) && MathF.Sign(Projectile.velocity.X) != Projectile.spriteDirection)
            {
                ReturnedToKibble = true;
            }
            if (ReturnedToKibble)
            {
                if (Projectile.alpha > 255)
                {
                    Projectile.Kill();
                }
                Projectile.alpha += 40;
                Projectile.damage = -1;
                Projectile.velocity *= .7f;
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
			if (Timer % 4 != 3)
				return;
			Particles.Ring.CutterRing(this);

        }
		public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;

	}
}