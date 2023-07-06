using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class MatterOrb : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			Projectile.width = 28;
			Projectile.height = 26;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 240;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
		}
		public override void AI()
		{
			Player player = Main.player[(int)Projectile.ai[1]]; //chooses player that was already being targeted by npc

			if (Main.netMode == NetmodeID.SinglePlayer)
			{
				player = Main.player[Main.myPlayer];
			}

			Vector2 move = player.Center - Projectile.Center; 

			Projectile.ai[0]++;

			if (Projectile.ai[0] < 20)
			{
				Projectile.velocity *= 0.92f;
			}

			if (Projectile.ai[0] == 20)
            {
				Projectile.hostile = true;
				move.Normalize();
				move *= 18;
				Projectile.velocity = move; //movemove = player.Center - projectile.Center; //update player position
			}

			if (++Projectile.frameCounter >= 5) //changes frames every 5 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}
		}
        public override void Kill(int timeLeft) //when the projectile dies
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, ModContent.DustType<Dusts.DarkResidue>(), speed, Scale: 1f); //Makes dust in a messy circle
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
			return new Color(255, 255, 255); //white
        }
    }
}