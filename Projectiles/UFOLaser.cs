using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using SoundType = Terraria.Audio.SoundType;

namespace KirboMod.Projectiles
{
	public class UFOLaser : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
			// DisplayName.SetDefault("UFO Laser"); //make display name otherwise UFO would be spaced out
		}

		public override void SetDefaults()
		{
			Projectile.width = 9;
			Projectile.height = 9;
			DrawOffsetX = -32;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.extraUpdates = 2;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.aiStyle = 0;
			Projectile.timeLeft = 120 * Projectile.MaxUpdates;
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + MathF.PI;
			Vector2 offset = Vector2.Normalize(Projectile.velocity) * 20;//spawn at the end of the laser
            for (int i = 0; i < Projectile.MaxUpdates; i++)
            {
				Dust.NewDustPerfect(Projectile.Center - Projectile.velocity * i * .5f - offset, DustID.TheDestroyer, Vector2.Zero).noGravity = true;
				Dust.NewDustPerfect(Projectile.Center - Projectile.velocity * i - offset, DustID.TheDestroyer, Vector2.Zero).noGravity = true;

			}
			Lighting.AddLight(Projectile.position, 0.2f, 0f, 0f); //red
        }

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}