using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Drawing.Drawing2D;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class DragonFireFire : ModProjectile
    {
        int startTime;
        public override void SetStaticDefaults()
		{

        }
		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			DrawOffsetX = -10;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 35;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.usesIDStaticNPCImmunity = true;
			Projectile.idStaticNPCHitCooldown = 4;

            startTime = Projectile.timeLeft;
        }

        public override void AI()
		{
            //Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.rotation += MathF.PI / 4;

            if (Main.rand.NextBool(5)) // happens 1/5 times
			{
                Dust flames = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(50, 50), 
                    DustID.PinkTorch, Projectile.velocity, newColor: Color.Magenta, Scale: 2f); //dust
                flames.noGravity = true;
			}

            Lighting.AddLight(Projectile.Center, 1, 0, 1);

			Projectile.ai[0] = startTime - Projectile.timeLeft; //size depends on time left

			if (Projectile.timeLeft <= 5) //fade when close to death
			{
				Projectile.alpha += 51;
			}
        }

		float Scale()
		{
            return Projectile.ai[0] / 7 < 1.5f ? Projectile.ai[0] / 7 : 1.5f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D flames = TextureAssets.Projectile[85].Value;

            Rectangle frame = flames.Frame(1, 7, 0, 3);
            Vector2 origin = frame.Size() / 2;

            Color color = Color.Fuchsia * Projectile.Opacity;
            Color color2 = Color.White * Projectile.Opacity;

			float scale = Scale();

            //glow
            VFX.DrawGlowBallAdditive(Projectile.Center, scale * 2, Color.Magenta, Color.Fuchsia);

            //purple
            Main.EntitySpriteDraw(flames, Projectile.Center - Main.screenPosition, frame, color, Projectile.rotation,
                origin, scale, SpriteEffects.None);
			//white (on top)
            Main.EntitySpriteDraw(flames, Projectile.Center - Main.screenPosition, frame, color2, Projectile.rotation,
                origin, scale * 0.75f, SpriteEffects.None);

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) //make hitbox into a growing circle
        {
            float scale = Scale();

            return Utils.IntersectsConeFastInaccurate(targetHitbox, Projectile.Center, scale * 50, 0, 360);
        }

        public override bool? CanHitNPC(NPC target) //can hit only if there's a line of sight
        {
            return Collision.CanHit(Projectile, target);
        }
        public override bool CanHitPvp(Player target) //can hit only if there's a line of sight
        {
            return Collision.CanHit(Projectile, target);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //stop projectile
            Projectile.velocity *= 0.1f;

            Projectile.timeLeft = 5;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			target.AddBuff(BuffID.ShadowFlame, 600);
            Projectile.damage = (int)(Projectile.damage * 0.8f); //reduce
        }
    }
}