using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class ParosolHitbox : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 90;
			Projectile.height = 36;
			Projectile.friendly = true;
			Projectile.minion = true;
			Projectile.timeLeft = 3;
			Projectile.tileCollide = false;
			Projectile.penetrate = 999;
			Projectile.scale = 1f;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 4;
		}
		public override void AI()
		{
		    /*if (++projectile.frameCounter >= 15) //changes frames every 15 ticks 
			{
				projectile.frameCounter = 0;
				if (++projectile.frame >= Main.projFrames[projectile.type])
				{
					projectile.frame = 0;
				}
			}*/
		}
        public override void OnKill(int timeLeft) //when the projectile dies
        {
			/*for (int i = 0; i < 10; i++)
			{
				Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
				Dust d = Dust.NewDustPerfect(projectile.position, DustID.Enchanted_Gold, speed * 3, Scale: 1f); //Makes dust in a messy circle
			}*/
		}

        public override Color? GetAlpha(Color lightColor)
        {
			// Makes it uneffected by light
			return default;
        }

        /*public override bool PreDrawExtras(SpriteBatch spriteBatch)
        {
            return base.PreDrawExtras(spriteBatch);
        }*/
    }
}