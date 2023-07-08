using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BeamBig : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 60;
			Projectile.height = 60;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 120;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
		}
		public NPC Kracko { get => Main.npc[(int)Projectile.ai[0]]; set => Projectile.ai[0] = value.whoAmI; }
		public float RotationOffset { get => Projectile.ai[1]; set => Projectile.ai[1] = value; }
		public float DistanceFromCenter { get => Projectile.ai[2]; set => Projectile.ai[2] = value; }
		public override void AI()
		{
			float rotationSpeedMultipler = Utils.GetLerpValue(120, 90, Projectile.timeLeft, true);
			Projectile.Opacity = Utils.GetLerpValue(120, 110, Projectile.timeLeft, true) * Utils.GetLerpValue(1, 10, Projectile.timeLeft, true);
			Projectile.rotation += 0.2f * Projectile.direction; // rotates projectile
			Projectile.frame++;
			Projectile.frame %= Main.projFrames[Type];
			RotationOffset += (!Main.expertMode ? 0.02f : 0.04f) * rotationSpeedMultipler;//spin twice as fast on expert
			Vector2 offset = RotationOffset.ToRotationVector2() * DistanceFromCenter;
			Projectile.Center = Kracko.Center + offset;
            if (Main.rand.NextBool(5)) // happens 1/5 times
            {
				offset.Normalize();//I don't use offset after this for positioning, so I can normalize it without issues
				offset = offset.RotatedBy(MathF.PI / 2);
				offset *= 4 * Main.rand.NextFloat() + 2;
				Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height) / 2, DustID.Electric, offset);
				dust.noGravity = true;
				dust.alpha = 200;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        { 
			return Projectile.DistanceSQ(targetHitbox.ClosestPointInRect(Projectile.Center)) < 600 && Projectile.Opacity == 1;//circle hitbox and only hit if fully visible
        }
        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White * Projectile.Opacity; // Makes i uneffected by light
		}

        public override bool PreDraw(ref Color lightColor)
        {
			Color[] possibleColors = new Color[] { Color.Yellow, Color.Lime, Color.Cyan };
			Vector2 randomOffset = Main.rand.NextVector2Circular(16, 16);
			Vector2 fatness = Vector2.One;//feel free to mess around with
			Vector2 sparkleScale = Vector2.One;//these values to see what thet change

			fatness *= Projectile.scale;
			sparkleScale *= Projectile.scale;
			randomOffset *= Projectile.scale;
			VFX.DrawPrettyStarSparkle(Projectile.Opacity, Projectile.Center - Main.screenPosition + randomOffset, Color.White, possibleColors[Main.rand.Next(possibleColors.Length)], 1, 0, 1, 1, 2, Main.rand.NextFloat() * MathF.Tau, sparkleScale, fatness);
            return base.PreDraw(ref lightColor);
        }
    }
}