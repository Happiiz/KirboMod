using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class MetalFistProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }
        public static int MaxPenetrate => 5;
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.timeLeft = 10; //a bit more than previous to account for extra update
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
            Projectile.penetrate = MaxPenetrate;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true; //uses own immunity frames
            Projectile.localNPCHitCooldown = 7; //time before hit again
            Projectile.ownerHitCheck = true;
            Projectile.alpha = 30;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.penetrate == MaxPenetrate)
            {
                KirbPlayer.IncreaseComboCounter(Projectile.owner);
            }
            Projectile.damage = (int)(Projectile.damage * 0.7f);
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