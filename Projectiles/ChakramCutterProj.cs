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
	public class ChakramCutterProj : ModProjectile
	{
	    public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1; //one frame

            //for afterimages
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10; // The length of old position to be recorded
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
			Projectile.extraUpdates = 2;
			Projectile.usesLocalNPCImmunity = true; //doesn't wait for other projectiles to hit again
			Projectile.localNPCHitCooldown = 20; //time until able to hit npc even if npc has just been struck
		}
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
			return Projectile.ai[0] >= 0 && (Helper.CheckCircleCollision(targetHitbox, Projectile.Center, 35) || Helper.CheckCircleCollision(targetHitbox, Projectile.oldPos[0] + Projectile.Size / 2, 35));
        }
        public override bool PreAI()
        {
			if (Projectile.ai[0] <= 0)
				VelLength = Projectile.velocity.Length();
			if (Projectile.ai[0] <= 0)
			{
				Projectile.Center = Main.player[Projectile.owner].Center + Vector2.Normalize(Projectile.velocity).RotatedBy(MathF.PI / 2) * Projectile.ai[1] * 32 - Projectile.velocity;
			}
			Projectile.ai[0]++;
			if(Projectile.ai[0] == 0)
            {
				SoundEngine.PlaySound(SoundID.Item1 with { MaxInstances = 0 }, Projectile.Center);
            }
			return Projectile.ai[0] >= 0;
        }
		ref float VelLength { get => ref Projectile.ai[2]; }
        public override void AI()
		{
			Lighting.AddLight(Projectile.Center, Color.Gold.ToVector3());
			Player player = Main.player[Projectile.owner];
			Projectile.rotation += Projectile.direction * 0.5f;
			Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SpelunkerGlowstickSparkle)];
			dust.scale = 2;
			//if(Main.rand.NextBool())
				dust.velocity += Projectile.velocity * 0.4f;
			if (Projectile.ai[0] >= 60)//return
            {
				if(Projectile.ai[0] == 60)
                {
					Projectile.velocity = Vector2.Normalize( Projectile.velocity).RotatedBy(Projectile.ai[1] * MathF.PI / 2) * VelLength;
                }
				float speed = Helper.RemapEased(Projectile.ai[0], 25, 1000, 25f, 100, Easings.EaseInOutSine); //top speed(original shoot speed)
				float inertia = 10f; //acceleration and decceleration speed

				Vector2 direction = player.Center - Projectile.Center; 																	
				direction.Normalize();
				direction *= speed;
				Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia; 

				Rectangle box = Projectile.Hitbox;
				if (box.Intersects(player.Hitbox)) //if touching player
                {
					Projectile.Kill(); //KILL
                }
				return;
            }
			Projectile.velocity.Normalize();
			Projectile.velocity *= Helper.RemapEased(Projectile.ai[0], 0, 40, VelLength, 0.1f, Easings.EaseInOutSine);
		}

        public static Asset<Texture2D> afterimage;

        public override bool PreDraw(ref Color lightColor)
        {
			if (Projectile.ai[0] < 0)
				return false;
            Main.instance.LoadProjectile(Projectile.type);
            afterimage = ModContent.Request<Texture2D>(Texture);
            Texture2D texture = afterimage.Value;
			Vector2 origin = texture.Size() / 2;
			Vector2 drawPos;

			// Redraw the projectile with the color not influenced by light
			for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
            {
				 drawPos = (Projectile.oldPos[i] - Main.screenPosition) + new Vector2(0f, Projectile.gfxOffY) + Projectile.Size / 2;
				Color color = Color.White * Utils.GetLerpValue(Projectile.oldPos.Length, 0, i);
				Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, origin, 1, SpriteEffects.None, 0);

			}
			drawPos = (Projectile.Center - Main.screenPosition) + new Vector2(0f, Projectile.gfxOffY);
			Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);

			return false; //draw og
        }
    }
}