using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class GoodMatterOrb : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 1000;
			Projectile.tileCollide = false;
			Projectile.extraUpdates = 2;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;
		}
		public override void AI()
		{
			Player player = Main.player[Projectile.owner]; 

			Projectile.ai[0]++;

			if (Projectile.ai[0] == 1)
            {
				SoundEngine.PlaySound(SoundID.SplashWeak, player.Center);
			}
			if (Projectile.ai[0] < 40)
			{
				Projectile.velocity *= 0.92f;
			}

			if (Projectile.ai[0] == 40 && Projectile.owner == Main.myPlayer)
            {
				Vector2 move = Main.MouseWorld - Projectile.Center;
				move.Normalize();
				move *= 18;
				Projectile.velocity = move; //movemove = player.Center - projectile.Center; //update player position
			}

			if (Projectile.ai[0] == 60) //sixth of a second (extra updates accounted for)
			{
                Projectile.tileCollide = true;
            }

            if (++Projectile.frameCounter >= 10) //changes frames every 5 ticks (extra updates accounted for)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			SoundEngine.PlaySound(SoundID.Item10, Projectile.position); //impact
			return true; //collision
		}
		public override void OnKill(int timeLeft) //when the projectile dies
		{
			for (int i = 0; i < 20; i++)
			{
				Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
				Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, Scale: 1f); //Makes dust in a messy circle
			}
        }

        public static Asset<Texture2D> afterimage;

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            afterimage = ModContent.Request<Texture2D>(Texture);
            Texture2D texture = afterimage.Value;

            // Redraw the projectile with the color not influenced by light
            for (int k = 1; k < Projectile.oldPos.Length; k++) //start at 1 so not ontop of actual projectile
            {
                Vector2 drawOrigin = texture.Size() / 2;
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);

                Color color = Color.White * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.rotation, drawOrigin, 1, SpriteEffects.None, 0);
            }
            return true; //draw og
        }

        public override Color? GetAlpha(Color lightColor)
        {
			return Color.White; //regular color unaffected by light
        }
    }
}