using KirboMod.NPCs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class JoeBlast : ModProjectile //whos joe?
	{
		public override void SetStaticDefaults()
		{
			
		}
		public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 50;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = (int)(KnuckleJoe.BlastRange / KnuckleJoe.BlastVelocityPublic);
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

			if (Main.rand.NextBool(2)) // happens 1/2 times
			{
				int dustnumber = Dust.NewDust(Projectile.position, 50, 50, DustID.BlueTorch, 0f, 0f, 200, Color.White, 1f); //dust
				Main.dust[dustnumber].noGravity = true;
			}
		}

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
			if (Main.expertMode)
			{
				target.AddBuff(BuffID.OnFire, 120);
			}
			else
			{
				if (Main.rand.NextBool(3)) //1/3
				{
					target.AddBuff(BuffID.OnFire, 120);
				}
			}
		
	    }

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}