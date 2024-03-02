using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class MasterSwing : ModProjectile
	{
		int swing = 1;
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			Projectile.width = 300;
			Projectile.height = 300;
            DrawOffsetX = -300;
            DrawOriginOffsetY= -300;
			Projectile.friendly = true;
			Projectile.usesLocalNPCImmunity = true; //uses own immunity frames
			Projectile.localNPCHitCooldown = 5; //time before hit again
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true; //check if owner has line of sight to hit
        }

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			Vector2 distance = new Vector2(0, 0);

			if (Main.myPlayer == player.whoAmI)
			{
				distance = Main.MouseWorld - player.Center;
				distance.Normalize();
				distance *= 120f;

				Projectile.Center = player.Center + distance;
				Projectile.velocity = distance * 0.001f; //very small
			}
			Projectile.rotation = Projectile.velocity.ToRotation();

			//animation
			Projectile.frameCounter++; //go up by 1 each tick (1/60 of a second)
            if (Projectile.frameCounter < 2)
            {
                Projectile.frame = 0;
            }
            else if (Projectile.frameCounter < 4)
            {
                Projectile.frame = 1;
            }
            else if (Projectile.frameCounter < 6)
            {
                Projectile.frame = 2;
            }
            else
            {
                Projectile.frame = 3;
            }
            if (Projectile.frameCounter >= 8)
            {
                swing *= -1; //reverse
                Projectile.frameCounter = 0;
            }

            //death
            if (!player.active || player.dead || !player.channel) //done/can't attack
            {
                Projectile.Kill(); //kill projectile
            }

            int d = Dust.NewDust(Projectile.position - new Vector2(100, 100), Projectile.width + 200, Projectile.height + 200, 
                DustID.SolarFlare, Projectile.velocity.X * 400f, Projectile.velocity.Y * 400f, Scale: 2f); //dust
            Main.dust[d].noGravity = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame - 1); //back one frame

			SpriteEffects direction = swing == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            
            if (Projectile.frame == 0) //if main sprite is at the start of a swing, then make afterimage still behind
            {
                frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, 3); //last frame

                if (direction == SpriteEffects.None)
                {
                    direction = SpriteEffects.FlipVertically;
                }
                else
                {
                    direction = SpriteEffects.None;
                }
            }

            //after image
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.Yellow * 0.5f, Projectile.rotation, frame.Size() / 2
				, 1f, direction);

            //restate
            frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame); 

            direction = swing == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically; 

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, frame.Size() / 2
				, 1f, direction);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            for (int i = 0; i < 8; i++)
            {
                Vector2 direction = target.Center - player.Center;

                Vector2 speed = Main.rand.NextVector2Unit(direction.ToRotation() - MathF.PI / 8, MathF.PI / 4) * Main.rand.Next(10, 20);

                Dust d = Dust.NewDustPerfect(target.Center, DustID.SolarFlare, speed, Scale: 2f);
                d.noGravity = true;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return targetHitbox.IntersectsConeSlowMoreAccurate(Projectile.Center, 270, Projectile.rotation, MathF.PI * 0.775f);
        }

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}