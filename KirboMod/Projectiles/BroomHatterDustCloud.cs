using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BroomHatterDustCloud : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dust Cloud");
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 40; 
			Projectile.height = 40; 
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 15;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
		}
		public override void AI()
		{
			//detect direction of movement
			int direction = 1;
			if (Projectile.velocity.X < 0)
			{
				direction = -1;
			}

			Projectile.ai[0]++;
			if (Projectile.ai[0] == 1)
			{
                for (int i = 0; i < 3; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                {
                    Vector2 speed = new Vector2(Main.rand.Next(2,4) * direction, 0);
                    Gore.NewGorePerfect(Projectile.GetSource_FromThis(), direction == -1 ? Projectile.Center - new Vector2(40, 0) : Projectile.Center, 
						speed.RotatedByRandom(MathHelper.ToRadians(35f)), 
						Main.rand.Next(61, 63), Scale: 0.8f); //smoke
                }
            }
		}
    }
}