using KirboMod.NPCs;
using KirboMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BioSparkSlashHitbox : ModProjectile, ITrailedProjectile
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Type] = 20;
			ProjectileID.Sets.TrailingMode[Type] = 2;
			Main.projFrames[Projectile.type] = 1;
		}
		const int duration = 20;
		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = duration + 10;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
		}
		public override void AI()
		{
			NPC originNPC = Main.npc[(int)Projectile.ai[0]];
			Projectile.Center = originNPC.Center;

			if (Projectile.timeLeft <= 15)
            {
				Projectile.damage = -1;
				Projectile.Opacity -= .2f;

            }
			Vector2 offset = Helper.RemapEased(Projectile.timeLeft, 30, 10, MathF.PI / 1.5f, -MathF.PI / 1.5f, Easing, false).ToRotationVector2() * 80;
			offset.Y *= .6f;
			offset.X *= originNPC.direction;
			Projectile.rotation = offset.ToRotation() + MathF.PI / 2;
			Projectile.Center += offset;
		}
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
		}
		float WidthFunction(float progress)
        {
			return MathHelper.Lerp(40, 0, progress) * Easings.EaseInOutSine(Utils.GetLerpValue(5,15, Projectile.timeLeft, true));
        }
		Color ColorFunction(float progress)
        {
			return Color.Lerp(Color.White, Color.Cyan, Utils.GetLerpValue(0, 0.5f, progress, true)) * Projectile.Opacity;
        }
		static float Easing(float t)
        {
			t = 1 - t;
			t *= t * t;
			return 1 - t;
        }
        public void AddTrail()
        {
			NPC originNPC = Main.npc[(int)Projectile.ai[0]];
			Projectile.Center = originNPC.Center;
			Vector2 offset;

			for (int i = 0; i < Projectile.oldPos.Length; i++)
			{
				offset = Helper.RemapEased(Projectile.timeLeft + (i * .25f + .25f), 30, 10, MathF.PI / 1.5f, -MathF.PI / 1.1f, Easing, false).ToRotationVector2() * 60;
				offset.Y *= .6f;
				offset.X *= originNPC.direction;
				Projectile.oldRot[i] = offset.ToRotation() + MathF.PI / 2;
				Projectile.oldPos[i] = Projectile.position + offset;
			}
			offset = Helper.RemapEased(Projectile.timeLeft, 30, 10, MathF.PI / 1.5f, -MathF.PI / 1.1f, Easing, false).ToRotationVector2() * 80;
			offset.Y *= .6f;
			offset.X *= originNPC.direction;
			Projectile.rotation = offset.ToRotation() + MathF.PI / 2;
			Projectile.Center += offset;
			TrailSystem.Trail.AddAlphaBlend(Projectile, WidthFunction, ColorFunction);
        }
    }
}