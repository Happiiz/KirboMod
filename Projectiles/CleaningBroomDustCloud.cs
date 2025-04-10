using Microsoft.Xna.Framework;
using System;
using Terraria;
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
		static int ProjDuration => 60;
		public override void SetDefaults()
		{
			Projectile.width = 40; 
			Projectile.height = 40; 
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.timeLeft = ProjDuration;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}
		public override void AI()
		{
			if (Projectile.timeLeft == ProjDuration - 1)
			{
                for (int i = 0; i < 6; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                {
                   Gore g = Gore.NewGorePerfect(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity.RotatedByRandom(MathF.PI / 8) * Main.rand.NextFloat(1f, 2f), 
						Main.rand.Next(61, 63)); //smoke
					g.rotation = Main.rand.NextFloat(MathF.Tau);
                }
			}
			Projectile.tileCollide = false;
			if (Projectile.timeLeft % 2 == 0)
			{
				Projectile.position -= new Vector2(1);
				Projectile.width += 2;
				Projectile.height += 2;
			}
		}
    }
}