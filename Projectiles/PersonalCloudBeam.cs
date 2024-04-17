using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class PersonalCloudBeam : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.MinionShot[Type] = true;
        }

		public override void SetDefaults()
		{
			Projectile.width = 15;
			Projectile.height = 15;
			Projectile.friendly = true;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1; //default
		}

		public override void AI()
		{
			Projectile.rotation += 0.2f * (float)Projectile.direction; // rotates projectile
			if (++Projectile.frameCounter >= 2) //changes frames every 2 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 2)
				{
					Projectile.frame = 0;
				}
			}

            if (Projectile.ai[1] == 0)
            {
                Projectile.ai[1] = Projectile.velocity.Length();
            }
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (Helper.ValidHomingTarget(npc, Projectile, false))
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(npc.Center) * Projectile.ai[1], .2f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
			VFX.DrawElectricOrb(Projectile.Center, Vector2.One * 1.3f, Projectile.Opacity, Projectile.rotation);
			return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Utils.CenteredRectangle(Projectile.Center, Projectile.Size * (10 * 1/3)).Intersects(targetHitbox);
        }

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}