using KirboMod.NPCs;
using KirboMod.NPCs.DarkMatter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
			Projectile.width = 70;
			Projectile.height = 70;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.timeLeft = 120;
			Projectile.tileCollide = false;
			Projectile.penetrate = 1;
		}
		static int TimeBeforeShoot => 37;
		float Scale { get => Utils.GetLerpValue(0, TimeBeforeShoot - 10, Projectile.ai[0], true); }
		bool Predictive { get => Projectile.ai[2] == 1; }
		public override void AI()
		{
			Player player = Main.player[(int)Projectile.ai[1]]; //chooses npc target player

			if (Main.netMode == NetmodeID.SinglePlayer)
            {
				player = Main.player[Main.myPlayer];
			}

            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.DarkResidue>(), Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 200, default, 0.8f); //dust

            Projectile.ai[0]++;
			Projectile.scale = Easings.EaseInOutSine(Scale);
			float shootSpeed = 30;
			if (Projectile.ai[0] == TimeBeforeShoot) //Start hurtin'
            {
				SoundEngine.PlaySound(DarkMatter.OrbShoot, Projectile.Center); 

				Vector2 move = (player.Center + player.velocity * 10) - Projectile.Center; //aims ahead of player
				if (Predictive)
				{
					Utils.ChaseResults results = Utils.GetChaseResults(Projectile.Center, shootSpeed, player.Center, player.velocity);
					if (results.InterceptionHappens)
					{
						move = results.ChaserVelocity;
					}
				}
                move.Normalize();
                move *= shootSpeed;
                Projectile.hostile = true; //hurt

                Projectile.velocity = move; //move
			}
		}
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
			if (Projectile.ai[0] > 20)
			{
				Projectile.Kill(); //YESS KILL!
			}
        }

        public override void OnKill(int timeLeft)
        {
			for (int i = 0; i < 20; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
				Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 10, 10); //Makes dust in a messy circle
				d.noGravity = true;
			}
		}

		//todo: add draw effect to remaining dark orbs
        public override bool PreDraw(ref Color lightColor)
        {
			//         Main.instance.LoadProjectile(Projectile.type);
			//         image = ModContent.Request<Texture2D>("KirboMod/Projectiles/DarkOrb");
			//         Texture2D texture = image.Value;

			//float size = Projectile.ai[0] / 20 < 1 ? Projectile.ai[0] / 20 : 1; //keep growing until proj ai0 = 20

			//         Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, 0, new Vector2(45, 45), size, SpriteEffects.None);
			Items.DarkSword.DarkSwordOrb.DrawDarkOrb(Projectile);
            return false;
        }
    }
}