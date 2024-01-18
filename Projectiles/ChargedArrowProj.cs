using KirboMod.Particles;
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
	public class ChargedArrowProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Charged Star Arrow");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }
		public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 18;
			DrawOriginOffsetX = -9;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 120; //2 seconds
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.alpha = 50;
			Projectile.aiStyle = 0;
			Projectile.light = 0.4f;
			Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

			int dustnumber = Dust.NewDust(Projectile.position, 18, 18, DustID.IcyMerman, 0f, 0f, 0, default, 1f); //dust
			Main.dust[dustnumber].velocity *= 0.2f;
			Main.dust[dustnumber].noGravity = true;
		}

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}

        public override void OnKill(int timeLeft) //when the projectile dies
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, DustID.IcyMerman, speed, Scale: 1f); //Makes dust in a messy circle

                Sparkle b = new(Projectile.Center, Color.Blue, Projectile.velocity.RotatedByRandom(Math.PI * 0.75f), new Vector2(0.2f, 0.2f));
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position); //impact
            return true; //collision
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
                Vector2 drawOrigin = new Vector2(8, 8);
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);

                Color color = Color.CornflowerBlue * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, 1, SpriteEffects.None, 0);
            }
            return true; //draw og
        }

        public override void PostDraw(Color lightColor)
        {
            //add glow ball at tip of arrow
            VFX.DrawGlowBallAdditive(Projectile.Center + Projectile.velocity * 0.5f, 1.2f, Color.Blue, Color.White);
        }
    }
}