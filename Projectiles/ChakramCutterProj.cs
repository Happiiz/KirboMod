using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class ChakramCutterProj : ModProjectile
	{
	    public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1; //one frame

            //for afterimages
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }
		public override void SetDefaults()
		{
			Projectile.width = 60;
			Projectile.height = 60;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 3600; //1 minute
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.extraUpdates = 1;
			Projectile.usesLocalNPCImmunity = true; //doesn't wait for other projectiles to hit again
			Projectile.localNPCHitCooldown = 10; //time until able to hit npc even if npc has just been struck
		}
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
			return Projectile.ai[0] >= 0 && AIUtils.CheckCircleCollision(targetHitbox, Projectile.Center, 35);
        }
        public override bool PreAI()
        {
			Projectile.ai[0]++;
			if (Projectile.ai[0] < 0)
				Projectile.Center = Main.player[Projectile.owner].Center - Projectile.velocity;
			return Projectile.ai[0] >= 0;
        }
        public override void AI()
		{
			Lighting.AddLight(Projectile.Center, Color.Gold.ToVector3());
			Player player = Main.player[Projectile.owner];
			Projectile.rotation += Projectile.direction * 0.5f;
			Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SpelunkerGlowstickSparkle)];
			dust.scale = 2;
			dust.velocity += Projectile.velocity;
			if (Projectile.ai[0] >= 25)//return
            {
				if(Projectile.ai[0] == 25)
                {
					Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[1] * MathF.PI / 2);
                }
				float speed = 25f; //top speed(original shoot speed)
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

        public static Asset<Texture2D> afterimage;

        public override bool PreDraw(ref Color lightColor)
        {
			if (Projectile.ai[0] < 0)
				return false;
            Main.instance.LoadProjectile(Projectile.type);
            afterimage = ModContent.Request<Texture2D>(Texture);
            Texture2D texture = afterimage.Value;

            // Redraw the projectile with the color not influenced by light
            for (int k = 1; k < Projectile.oldPos.Length; k++) //start at 1 so not ontop of actual ring
            {
                Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);

                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, 1, SpriteEffects.None, 0);
            }

            return true; //draw og
        }
    }
}