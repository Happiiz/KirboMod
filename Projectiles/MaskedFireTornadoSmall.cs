using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class MaskedFireTornadoSmall : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
			// DisplayName.SetDefault("Fire Tornado");
		}

		public override void SetDefaults()
		{
			Projectile.width = 64;
			Projectile.height = 90; //2 less so it's the size it would've been if not for extra space at bottom to avoid cutting (ITSIWBINFESABTAC)
			DrawOriginOffsetY = 1;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.timeLeft = 180;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true; //doesn't wait for any other immunity to hit again
			Projectile.localNPCHitCooldown = 5; //time until able to hit npc even if npc has just been struck
			Projectile.ignoreWater = true; //doesn't confine to water physics for it is on a higher realm of existance
		}
		public override void AI()
		{
			//Gravity
			Projectile.velocity.Y = Projectile.velocity.Y + 0.8f;
			if (Projectile.velocity.Y >= 24f)
			{
				Projectile.velocity.Y = 24f;
			}

			//for stepping up tiles
			Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);

			//Animation
			if (++Projectile.frameCounter >= 2) //changes frames every 2 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}

			Projectile.ai[0]++;

			if (Projectile.ai[0] % 2 == 0)
			{
				int dust = Dust.NewDust(Projectile.position + new Vector2(Projectile.width / 2, Projectile.height), 1, 1, DustID.SolarFlare, Projectile.direction * -0.4f, 0f, 200, default, 3f); //dust
				Main.dust[dust].noGravity = true;
			}

			if (Projectile.velocity.X == 0) //stop moving
            {
				Projectile.Kill();
            }

			Lighting.AddLight(Projectile.Center, 0.8f, 0.4f, 0f); //orange light
		}

        public override void Kill(int timeLeft) //when the projectile dies
         {
             for (int i = 0; i < 15; i++)
             {
                 Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
                 Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.SolarFlare, speed * 3, Scale: 1.5f); //Makes dust in a messy circle
             }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false; //dont die
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false; //don't fall through platforms
			return true;
        }

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			target.AddBuff(BuffID.Daybreak, 600); //10 seconds 
		}
    }
}