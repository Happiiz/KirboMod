using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class RangerStarExplode : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 100;
			Projectile.height = 100;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 5;
			Projectile.tileCollide = false;
			Projectile.penetrate = 999;
			Projectile.scale = 1f;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 5;
		}
		public override void AI()
		{
			Projectile.ai[0]++;
			if (Projectile.ai[0] == 1)
			{
				SoundEngine.PlaySound(SoundID.Item14, Projectile.position); //bomb sound

				for (int i = 0; i < 20; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
					Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.LilStar>(), speed); //Makes dust in a messy circle
					d.noGravity = true;
				}
			}
		}
    }
}