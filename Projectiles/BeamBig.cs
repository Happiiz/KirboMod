using Microsoft.Xna.Framework;
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
		
		public override void AI()
		{
			Projectile.rotation += 0.2f * (float)Projectile.direction; // rotates projectile

			if (++Projectile.frameCounter >= 1) //changes frames every 1 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 2)
				{
					Projectile.frame = 0;
				}
			}
			NPC kracko = Main.npc[(int)Projectile.ai[0]]; //use ai 0 to get the kracko this spawned from

            float rotationalOffset = MathHelper.ToRadians(Projectile.ai[2]); //convert degrees to radians
            Projectile.ai[2] += 12; //go up to rotate (4 revolutions)

            //set a point and then make the projectiles rotate around it
            if (Projectile.ai[1] <= 2) //first set
			{
				Projectile.position.X = kracko.Center.X - 29 + (float)Math.Cos(rotationalOffset - (Projectile.ai[1] * 0.5f)) * (Projectile.ai[1] + 1) * 60;
				Projectile.position.Y = kracko.Center.Y - 29 + (float)Math.Sin(rotationalOffset - (Projectile.ai[1] * 0.5f)) * (Projectile.ai[1] + 1) * 60;
			}
			else
			{
				float oppositeOffset = MathHelper.ToRadians(180);
				float ai_1_Reset = Projectile.ai[1] - 3;
                Projectile.position.X = kracko.Center.X - 29 + (float)Math.Cos(oppositeOffset + rotationalOffset - (ai_1_Reset * 0.5f)) * (ai_1_Reset + 1) * 60;
				Projectile.position.Y = kracko.Center.Y - 29 + (float)Math.Sin(oppositeOffset + rotationalOffset - (ai_1_Reset * 0.5f)) * (ai_1_Reset + 1) * 60;
            }

            if (Main.rand.NextBool(5)) // happens 1/5 times
            {
                int dustnumber = Dust.NewDust(Projectile.position, 60, 60, DustID.Electric, 0f, 0f, 200, default, 1f); //dust
                Main.dust[dustnumber].noGravity = true;
            }
        }
		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}