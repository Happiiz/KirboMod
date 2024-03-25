using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class CyborgArcherArrow : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Laser Arrow");
		}
		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			DrawOriginOffsetX = -9;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 40; //40 seconds
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.aiStyle = 0;
			Projectile.light = 0.4f;
			Projectile.ignoreWater = true;

			//Doesn't wait for any immunity cooldown
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

			int dustnumber = Dust.NewDust(Projectile.position, 20, 20, DustID.Firework_Red, 0f, 0f, 0, default, 1f); //dust
			Main.dust[dustnumber].velocity *= 0.0f;
			Main.dust[dustnumber].noGravity = true;
		}

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
            {
                Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Firework_Red, speed * 2, Scale: 1f); //Makes dust in a messy circle
                d.noGravity = true;
            }

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Item10.WithVolumeScale(0.5f), Projectile.position); //impact
            return true; //collision
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; //make it unaffected by light
        }
	}
}