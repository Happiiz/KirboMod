using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class CyborgArcherLaser : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Red Laser");
		}
		public override void SetDefaults()
		{
			Projectile.width = 5;
			Projectile.height = 5;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 200; //200 seconds
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.ignoreWater = true;
			Projectile.alpha = 255;
			//Doesn't wait for npc immunity frames
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
			Projectile.extraUpdates = 100; //additional updates per tick (make object move twice for example)
		}
		public override void AI()
		{
			if (Projectile.localAI[0] == 0)
			{
                SoundEngine.PlaySound(SoundID.Item158, Main.player[Projectile.owner].Center); //zapinator
            }
			if (Projectile.localAI[0] != 0)
			{
				Projectile.localAI[0] += Projectile.Distance(Projectile.oldPosition);
			}
			else
			{
				Projectile.localAI[0] += 0.0001f;
			}

			if (Projectile.localAI[0] > 64)
			{
				int dustCount = 18;
				int type = ModContent.DustType<Dusts.CyborgArcherLaser>();
				if (Projectile.localAI[0] < 10000)
				{
					for (int i = 0; i < dustCount; i++)
					{
						float progress = (float)i / dustCount;
						float angle = progress * MathF.Tau;
						Vector2 offset = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
						offset.X *= 0.5f;
						offset = offset.RotatedBy(Projectile.velocity.ToRotation());
						offset *= 10;
						Dust d = Dust.NewDustPerfect(Projectile.Center + offset * 0.1f - Projectile.velocity * .5f, type, offset, 0, Color.White, 2f);
						d.noGravity = true;
					}
					Projectile.localAI[0] += 10000;
				}
				for (int i = 0; i < 4; i++)
				{
					Vector2 position = Projectile.position;
					position -= Projectile.velocity * ((float)i * 0.25f);
					int deez = Dust.NewDust(position, 10, 10, type);
					//int deez = Dust.NewDust(position, 1, 1, DustID.RedTorch);
					Main.dust[deez].position = position;
					//Main.dust[deez].rotation += MathF.Tau * Main.rand.NextFloat();
					Main.dust[deez].position.X += Projectile.width / 2;
					Main.dust[deez].position.Y += Projectile.height / 2;
					Main.dust[deez].scale = 1.50f;
					Main.dust[deez].velocity *= 0f;
					Main.dust[deez].noGravity = true;
				}
			}
		}
		public override void ModifyDamageHitbox(ref Rectangle hitbox)
		{
			hitbox = Utils.CenteredRectangle(Projectile.Center, new Vector2(16));
		}
		public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; //make it unaffected by light
        }
	}
}