using KirboMod.NPCs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class NightCrownEffect : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 3;
			// DisplayName.SetDefault("Purple Slash");
		}

		public override void SetDefaults()
		{
			Projectile.width = 38;
			Projectile.height = 56;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.timeLeft = 15;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
		}

		public override void AI()
		{
			if(Projectile.soundDelay == 0)
			{
				Projectile.soundDelay = 99999;
				NightmareWizard.PlayTeleportSoundEffect(Main.player[Projectile.owner].Center);
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

        public override Color? GetAlpha(Color lightColor)
        {
			return Color.White;
        }
    }
}