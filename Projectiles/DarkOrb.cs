using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class DarkOrb : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 62;
			Projectile.height = 62;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.timeLeft = 120;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			Projectile.scale = 0.5f;
		}

		public override void AI()
		{
			Player player = Main.player[(int)Projectile.ai[1]]; //chooses npc target player

			if (Main.netMode == NetmodeID.SinglePlayer)
            {
				player = Main.player[Main.myPlayer];
			}

            if (Main.rand.NextBool(3)) // happens 1/3 times
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.DarkResidue>(), Projectile.velocity.X * 0.25f, Projectile.velocity.Y * -0.5f, 200, default, 0.8f); //dust
            }

            Projectile.ai[0]++;

			if (Projectile.ai[0] <20) //Grow up darnet!
            {
				Projectile.scale += 0.025f;
			}

			if (Projectile.ai[0] == 20) //Start hurt'in
            {
				Projectile.width = 66;
				Projectile.height = 66;
				SoundEngine.PlaySound(SoundID.Item117, Projectile.Center); //conjure arcanum

				Vector2 move = (player.Center + player.velocity * 5) - Projectile.Center; //aims ahead of player
				Projectile.hostile = true; //hurt
				move.Normalize();
				move *= 20;
				Projectile.velocity = move; //move
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
			if (Projectile.ai[0] > 20)
			{
				Projectile.Kill(); //YESS KILL!
			}
        }

        public override void Kill(int timeLeft)
        {
			for (int i = 0; i < 20; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
				Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 10, 10); //Makes dust in a messy circle
				d.noGravity = true;
			}
		}
    }
}