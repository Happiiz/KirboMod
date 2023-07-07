using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class GoodDarkLaser : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Dark Laser");
            Main.projFrames[Projectile.type] = 5;
		}
		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			DrawOffsetX = -60;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.timeLeft = 90;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.alpha = 128;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.usesLocalNPCImmunity = true; //uses own immunity timers for each npc
            Projectile.localNPCHitCooldown = 10;
        }
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

            if (Main.rand.NextBool(5)) // happens 1/5 times
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.DarkResidue>(), Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 200, default, 0.8f); //dust
            }

            if (++Projectile.frameCounter >= 3) //changes frames every 3 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			//target.AddBuff(BuffID.OnFire, 600);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}