using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class CleaningBroomDustCloud : ModProjectile
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
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.timeLeft = 60;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
		}
		public override void AI()
		{
			if (Projectile.timeLeft == 59)
			{
                for (int i = 0; i < 6; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                {
                    Gore.NewGorePerfect(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity.RotatedByRandom(MathF.PI / 8) * Main.rand.NextFloat(1f, 2f), 
						Main.rand.Next(61, 63)); //smoke
                }
            }
		}
    }
}