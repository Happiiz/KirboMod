using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BonkersSmash : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Smash");
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 100; 
			Projectile.height = 120; 
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 5;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
		}
		public override void AI()
		{
			Projectile.ai[0]++;
			if (Projectile.ai[0] == 1) //Only do once
			{
                //NPC that spawned this was not in air
                if (Projectile.velocity.Y == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item14, Projectile.position); //bomb sound

                    for (int i = 0; i < 30; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                    {
                        Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                        Gore.NewGorePerfect(Projectile.GetSource_FromThis(), Projectile.Center, speed, Main.rand.Next(61, 63), Scale: 1f); //smoke
                    }
                }
            }
		}
    }
}