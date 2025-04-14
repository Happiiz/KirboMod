using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class FlyingPunch : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			//ProjectileID.Sets.TrailCacheLength[Type] = 8;
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 10;
			Projectile.friendly = true;
			Projectile.timeLeft = 7;
			Projectile.tileCollide = false;
			Projectile.penetrate = 2;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.usesLocalNPCImmunity = true; //uses own immunity frames
			Projectile.localNPCHitCooldown = 7; //time before hit again
			Projectile.ownerHitCheck = true;
			Projectile.alpha = 60;
			Projectile.scale = 1.5f;
		}

		public override void AI()
		{
			Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathF.PI * ((1 - Projectile.spriteDirection) / 2); 
		}
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			Projectile.damage = (int)(Projectile.damage * 0.4f);
			KirbPlayer.IncreaseComboCounter(Projectile.owner);
		}

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			SpriteEffects dir = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
            {
				float opacity = i / (float)Projectile.oldPos.Length;
				opacity = Utils.GetLerpValue(0, .5f, i, true) * Utils.GetLerpValue(1f, .5f, opacity, true);
				opacity *= 0.2f; 
				Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition - Projectile.velocity * (i-Projectile.oldPos.Length / 2) / 16f, null, Color.White with { A = 128 } * opacity, Projectile.rotation, texture.Size() / 2, Projectile.scale, dir);
            }
			return false;// Projectile.DrawSelf(Color.White);
        }
    }
}