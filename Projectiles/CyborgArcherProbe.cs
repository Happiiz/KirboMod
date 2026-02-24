using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
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
			Projectile.width = 1;
			Projectile.height = 1;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 2;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			Vector2 projshoot = new Vector2(Projectile.ai[0], Projectile.ai[1]) - Projectile.Center; //get distance

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
        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D tex = TextureAssets.Projectile[Type].Value;
			float distFromCenter = 60f;
			int drawCount = 4;
			Vector2 center = Projectile.Center;
			Vector2 screenPos = Main.screenPosition;
			Vector2 targetPos = new Vector2(Projectile.ai[0], Projectile.ai[1]);
			Color color = Projectile.GetAlpha(lightColor);
            for (int i = 0; i < drawCount; i++)
			{
				Vector2 posOffset = Utils.Remap(i, 0, drawCount, 0, MathF.Tau, false).ToRotationVector2() * distFromCenter;
				Vector2 worldPos = center + posOffset;
				float rotation = (worldPos - targetPos).ToRotation();
				Vector2 drawPos = worldPos - screenPos;
				Main.EntitySpriteDraw(tex, drawPos, null, color, rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None);
			}
			return false;
        }
        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
    }
}