using KirboMod.NPCs;
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
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 3f;
		}
		public float startingTimeLeft = 300;//have this so I can customize the duration
		public NPC Kracko { get => Main.npc[(int)Projectile.ai[0]]; set => Projectile.ai[0] = value.whoAmI; }
		public float RotationOffset { get => Projectile.ai[1]; set => Projectile.ai[1] = value; }
		public float DistanceFromCenter { get => Projectile.ai[2]; set => Projectile.ai[2] = value; }
		public override void AI()
		{
			if (Kracko.type != ModContent.NPCType<Kracko>() || !Kracko.active)
			{
				for (int i = 0; i < 20; i++)
				{
					Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(40, 40), DustID.Electric);
				}
				Projectile.Kill();
				return;
			}
			if (Projectile.timeLeft > startingTimeLeft)
				Projectile.timeLeft = (int)startingTimeLeft;
			float rotationSpeedMultipler = Utils.GetLerpValue(startingTimeLeft - 20, startingTimeLeft - 50, Projectile.timeLeft, true);
			Projectile.Opacity = Utils.GetLerpValue(startingTimeLeft + 1, startingTimeLeft - 10, Projectile.timeLeft, true) * Utils.GetLerpValue(1, 10, Projectile.timeLeft, true);
			Projectile.rotation += 0.2f * Projectile.direction; // rotates projectile
			Projectile.frame++;
			Projectile.frame %= Main.projFrames[Type];
			RotationOffset += (!Main.expertMode ? 0.012f : 0.024f) * rotationSpeedMultipler;//spin faster on expert, just enough to complete 1 rotation, 2 on expert

			Vector2 offset = RotationOffset.ToRotationVector2() * DistanceFromCenter;
			Projectile.Center = Kracko.Center + offset;
            if (Main.rand.NextBool(2)) // happens 1/2 times
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
			return Projectile.DistanceSQ(targetHitbox.ClosestPointInRect(Projectile.Center)) < 600 && Projectile.timeLeft < startingTimeLeft - 50 && Projectile.Opacity == 1;//circle hitbox and only hit if fully visible
        }
        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White * Projectile.Opacity; // Makes i uneffected by light
		}
        Color RndElectricCol { get => (Main.rand.NextBool(2, 5) ? Color.Yellow : Color.Cyan) * Projectile.Opacity; }
        public override bool PreDraw(ref Color lightColor)
        {
			Color[] possibleColors = [Color.Yellow, Color.Cyan, Color.Cyan];
			Vector2 randomOffset = Main.rand.NextVector2Circular(4, 4);
			Vector2 fatness = Vector2.One;//feel free to mess around with
			Vector2 sparkleScale = Vector2.One;//these values to see what thet change

			fatness *= Projectile.scale;
			sparkleScale *= Projectile.scale;
			randomOffset *= Projectile.scale;
			float randRot = Main.rand.NextFloat() * MathF.Tau;
			Vector2 randScale = new(Main.rand.NextFloat() + 1f, Main.rand.NextFloat() + 1f);
			randScale *= Projectile.scale;
			randScale *= 0.15f;
		
			Main.EntitySpriteDraw(VFX.Ring, Projectile.Center - Main.screenPosition, null, RndElectricCol with { A = 0} * 0.8f, randRot, VFX.Ring.Size() / 2, randScale, SpriteEffects.None);
			Main.EntitySpriteDraw(VFX.Circle, Projectile.Center - Main.screenPosition, null, RndElectricCol with { A = 0 } * 0.25f, randRot, VFX.Circle.Size() / 2, randScale * 1.8f, SpriteEffects.None);
			VFX.DrawPrettyStarSparkle(Projectile.Opacity, Projectile.Center - Main.screenPosition + randomOffset, new Color(255, 255, 255, 0), possibleColors[Main.rand.Next(possibleColors.Length)], 1, 0, 1, 1, 2, Projectile.rotation, sparkleScale, fatness);
			return false;
        }
    }
}