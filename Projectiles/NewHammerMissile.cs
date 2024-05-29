using KirboMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class NewHammerMissile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Hammer Missile");
			Main.projFrames[Projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			Projectile.width = 42;
			Projectile.height = 26;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.timeLeft = 6000;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;
		
		}
        int TargetIndex { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        ref float InitialVelLength { get => ref Projectile.ai[1]; }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                SoundEngine.PlaySound(SoundID.Item11);
                InitialVelLength = Projectile.velocity.Length();
                TargetIndex = -1;
                Projectile.localAI[0]++;
            }
            float rangeSQ = 1500 * 1500;
            if (!Helper.ValidIndexedTarget(TargetIndex, Projectile, out _))
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Projectile.velocity) * InitialVelLength, 0.1f);
                int closestNPC = -1;
                Vector2 center = Projectile.Center;
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    NPC potentialTarget = Main.npc[i];
                    float distToClosestPointInPotentialTargetHitbox = center.DistanceSQ(potentialTarget.Hitbox.ClosestPointInRect(center));
                    bool notValidTarget = !Helper.ValidHomingTarget(potentialTarget, Projectile);
                    if (notValidTarget || distToClosestPointInPotentialTargetHitbox > rangeSQ)
                        continue;
                    if (!Main.npc.IndexInRange(closestNPC) || center.DistanceSQ(potentialTarget.Hitbox.ClosestPointInRect(center)) < center.DistanceSQ(Main.npc[closestNPC].Hitbox.ClosestPointInRect(center)))
                        closestNPC = i;
                }
                TargetIndex = closestNPC;
                Projectile.localAI[0] = 1;
            }
            if (Helper.ValidIndexedTarget(TargetIndex, Projectile, out NPC target))
            {
                Projectile.localAI[0]++;
                float homingStrength = Helper.RemapEased(Projectile.localAI[0], 1, 20, 0, .01f, Easings.EaseInOutSine);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(target.Center - Projectile.Center) * InitialVelLength, homingStrength);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            //todo make ring flamey dust in regular interval
            if (Main.rand.NextBool(5)) // happens 1/5 times
            {
                int spawnBoxSize = 4;
                Vector2 posOffset = new Vector2(Projectile.width / 2 - spawnBoxSize, Projectile.height / 2 - spawnBoxSize);
                spawnBoxSize *= 2;
                int index = Dust.NewDust(Projectile.position + posOffset, spawnBoxSize, spawnBoxSize, DustID.Smoke, Projectile.velocity.X, Projectile.velocity.Y, 0, default, 2f); //dust
                Main.dust[index].velocity *= 0.3f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.MyTexture(out Vector2 origin, out SpriteEffects fx);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, Projectile.scale, fx);
            return false;
        }
        public override void OnKill(int timeLeft) //when the projectile dies
        {
			for (int i = 0; i < 10; i++)
			{
				Vector2 speed = RandomInCircle(4);
				Dust.NewDustPerfect(Projectile.Center + RandomInCircle(10), DustID.Smoke, speed, Scale: 2f); //Makes dust in a messy circle

                Vector2 speed2 = RandomInCircle(4);

                Dust u = Dust.NewDustPerfect(Projectile.Center + RandomInCircle(10), DustID.Torch, speed2, Scale: 2f); //Makes dust in a messy circle
				u.noGravity = true;
			}

			SoundEngine.PlaySound(SoundID.Item14, Projectile.Center); //explosion
		}
        static Vector2 RandomInCircle(float radius)
        {
            float dist = MathF.Sqrt(Main.rand.NextFloat()) * radius;
            return Main.rand.NextFloat(MathF.Tau).ToRotationVector2() * dist;
        }
    }
}