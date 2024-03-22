using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BodyIce : ModProjectile
	{
		Vector2 randomOffset = new Vector2(0, 0);
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
			
		}

		public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.friendly = true;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.alpha = 100;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 30;//high iframes because stacking hits
			Projectile.ArmorPenetration = 1000;//low damage so ignores def to compensate otherwise balance hell
		}

		public override void AI()
		{
			NPC bodyToAttachTo = Main.npc[(int)Projectile.ai[0]];

			if (Projectile.ai[1] == 0) //only when zero
			{
				randomOffset = new Vector2(Main.rand.Next(0, bodyToAttachTo.width), Main.rand.Next(0, bodyToAttachTo.height));

            }
			Projectile.ai[1]++; //no longer zero

            if (bodyToAttachTo.active && !bodyToAttachTo.dontTakeDamage)
            {
				//random position on body
                Projectile.Center = bodyToAttachTo.position + randomOffset + bodyToAttachTo.netOffset;
                Projectile.gfxOffY = bodyToAttachTo.gfxOffY;
            }
			else
			{
				Projectile.Kill();
			}
        }

        public override void OnKill(int timeLeft)
        {
			for (int i = 0; i < 3; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Flake>(), speed * 2, default, Scale: 0.5f); //Makes dust in a messy circle
				d.noGravity = true;
			}
		}
    }
}