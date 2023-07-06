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
	public class TripleStarStar : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Star");
			Main.projFrames[Projectile.type] = 1;

            //for drawing afterimages and stuff alike
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

		public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 50;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 500;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.ignoreWater = true;

            //doesn't wait for no one!
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
		}
		public override void AI()
		{
            Player player = Main.player[Projectile.owner];
            Lighting.AddLight(Projectile.Center, 0.255f, 0.255f, 0f);
			Projectile.rotation += 0.3f; // rotates projectile

            int dustnumber = Dust.NewDust(Projectile.position, 50, 50, DustID.BlueTorch, 0f, 0f, 0, Color.White, 1.5f); //dust
            Main.dust[dustnumber].velocity *= 0.2f;
            Main.dust[dustnumber].noGravity = true;

            Projectile.ai[0]++;
			if (Projectile.ai[0] == 1) //if ai equal 1
            {
				SoundEngine.PlaySound(SoundID.Item9, Projectile.position); //star sound
			}

            if (Projectile.ai[0] >= 25)//return
            {
                float speed = 32f; //top speed(original shoot speed)
                float inertia = 10f; //acceleration and decceleration speed

                Vector2 direction = player.Center - Projectile.Center; //start - end 																	
                direction.Normalize();
                direction *= speed;
                Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;

                Rectangle box = Projectile.Hitbox;
                if (box.Intersects(player.Hitbox)) //if touching player
                {
                    Projectile.Kill(); //KILL
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
			return Color.White; // Makes it uneffected by light
        }

        public static Asset<Texture2D> afterimagae;

        public override bool PreDraw(ref Color lightColor)
        {
            afterimagae = ModContent.Request<Texture2D>("KirboMod/Projectiles/TripleStarStarAfterimage");
            Texture2D texture = afterimagae.Value;

            // Redraw the projectile with the color not influenced by light
            for (int k = 1; k < Projectile.oldPos.Length; k++) //start at 1 so not ontop of actual ring
            {
                Vector2 drawOrigin = new Vector2(25, 25);
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);

                Color color = Color.DodgerBlue * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, 1, SpriteEffects.None, 0);
            }

            return true; //draw og
        }
    }
}