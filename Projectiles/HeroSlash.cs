using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class HeroSlash : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            //for afterimages
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

		public override void SetDefaults()
		{
            //hitbox is changed with hook ModifyDamageHitbox
			Projectile.width = 11;
			Projectile.height = 11;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.timeLeft = 110;
			Projectile.tileCollide = true;
			Projectile.penetrate = 3;
            Projectile.extraUpdates = 2;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1; 
		}
		public override void AI()
		{
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Main.rand.NextBool(Projectile.MaxUpdates))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.BetterNextVector2Circular(10), DustID.MagicMirror);
                dust.scale *= 1.5f;
                dust.velocity *= 0.5f;
                dust.velocity += Projectile.velocity;
            }
            Projectile.Opacity = Utils.GetLerpValue(0, Projectile.MaxUpdates * 10, Projectile.timeLeft, true);
		}

        public override Color? GetAlpha(Color lightColor)
        {
            //unaffected by light, but can change opacity aswell
            return Color.White * Projectile.Opacity;
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
            for (int k = 1; k < Projectile.oldPos.Length; k++) //start at 1 so not ontop of actual projectile
            {
                Vector2 drawOrigin = texture.Size() / 2;
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + Projectile.Size / 2;

                Color color = Color.White * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * 0.5f;
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, 1, SpriteEffects.None, 0);
            }

            return Projectile.DrawSelf(Color.White); //draw og
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.75f); //reduce
        }

        public override bool OnTileCollide(Vector2 oldVelocity) //if touching a tile (will kill it)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(6f, 6f); //circle
                Dust.NewDustPerfect(Projectile.Center, DustID.MagicMirror,
                                speed, Scale: 2f);
            }

            return true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Rectangle newHitbox = Utils.CenteredRectangle(Projectile.Center, new Vector2(28));

            return targetHitbox.Intersects(newHitbox);
        }
    }
}