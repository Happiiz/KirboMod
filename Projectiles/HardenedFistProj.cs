using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class HardenedFistProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{ 

		}

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = true;
			Projectile.timeLeft = 7;
			Projectile.tileCollide = false;
			Projectile.penetrate = 3;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.usesLocalNPCImmunity = true; //uses own immunity frames
			Projectile.localNPCHitCooldown = 7; //time before hit again
            Projectile.ownerHitCheck = true;
            Projectile.alpha = 60;
		}
        ref float InitialVelLength => ref Projectile.localAI[0];
		public override void AI()
		{
            if(InitialVelLength == 0)
            {
                InitialVelLength = Projectile.velocity.Length();
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
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
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition - Projectile.velocity * (i - Projectile.oldPos.Length / 2) / 16f, null, Color.White * opacity, Projectile.rotation, texture.Size() / 2, Projectile.scale, dir);
            }
            return false;// Projectile.DrawSelf(Color.White);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.5f);
            if (Projectile.penetrate == ContentSamples.ProjectilesByType[Type].penetrate)
            {
                KirbPlayer.IncreaseComboCounter(Projectile.owner);
            }
        }
        public override bool? CanCutTiles() //only cut if player can "see" projectile (Hasn't gone through a wall)
        {
            Player player = Main.player[Projectile.owner];

            if (Collision.CanHit(player, Projectile))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}