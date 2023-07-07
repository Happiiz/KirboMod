using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class CyborgArcherProbe : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 2;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			Vector2 projshoot = Main.MouseWorld - Projectile.Center; //get distance

			/*if (player.itemAnimation == 1)
            {
                projshoot.Normalize(); //to one
				projshoot *= 30f; //now thirty

				Projectile.NewProjectile(projectile.Center, projshoot, ModContent.ProjectileType<Projectiles.CyborgArcherArrow>(), projectile.damage, 2, projectile.owner);
            }
			projectile.ai[0]++;*/


			//rotato

			// First, calculate a Vector pointing towards what you want to look at 
			//(PROJSHOOT)
			// Second, use the ToRotation method to turn that Vector2 into a float representing a rotation in radians.
			float chosenRotation = projshoot.ToRotation();
			// Now we can do 1 of 2 things. The simplest approach is to use the rotation value directly
			Projectile.rotation = chosenRotation;
			// A second approach is to use that rotation to turn the npc while obeying a max rotational speed. Experiment until you get a good value.
			//projectile.rotation = projectile.rotation.AngleTowards(chosenRotation, 1f);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
    }
}