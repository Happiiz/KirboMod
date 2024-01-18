using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
namespace KirboMod.Items.DarkSword
{
    internal class DarkSwordBeam : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.alpha = 255;
        }

        ref float Timer { get => ref Projectile.ai[0]; }
        public override void AI()
        {
            Timer++;
            if (Timer < 0)
            {
                Player player = Main.player[Projectile.owner];
                Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) - Projectile.velocity;
                return;
            }
            Projectile.Opacity = Utils.GetLerpValue(0, 5, Timer, true);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity *= 0.985f;
            int dustnumber = Dust.NewDust(Projectile.position, 76, 18, ModContent.DustType<Dusts.DarkResidue>(), 0f, 0f, 200, default, 0.8f); //dust
            Main.dust[dustnumber].velocity *= 0.3f;
            Main.dust[dustnumber].noGravity = true;

        }
        public override void OnKill(int timeLeft) //when the projectile dies
        {
            for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
            {
                Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.DarkResidue>(), speed); //Makes dust in a messy circle
                d.noGravity = true;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float unused = 86555;
            Vector2 length = new Vector2(50, 0).RotatedBy(Projectile.rotation);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - length, Projectile.Center + length, 24, ref unused);
        }
        static Asset<Texture2D> glowTexture;
        public override bool PreDraw(ref Color lightColor)
        {
            DrawDarkBeam(Projectile);
            return false;
        }

        public static void DrawDarkBeam(Projectile projectile)
        {
            Texture2D glow = glowTexture.Value;
            projectile.DrawSelf();
            float scale = (float)(Main.timeForVisualEffects * .05f) % 1;
            float opacity = MathHelper.Lerp(0, 20, scale);
            if (opacity > 1)
                opacity = 1;
            opacity *= MathHelper.Clamp(MathHelper.Lerp(5, 0, scale), 0, 1);
            opacity *= projectile.Opacity;
            scale *= .5f;
            scale += .8f;
            Main.EntitySpriteDraw(glow, projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * opacity, projectile.rotation, glow.Size() / 2, scale * projectile.scale, SpriteEffects.None);
        }

        public override void Unload()
        {
            glowTexture = null;
        }
        public override void Load()
        {
            glowTexture = ModContent.Request<Texture2D>("KirboMod/Projectiles/DarkBeamGlow", AssetRequestMode.ImmediateLoad);
        }
    }
}
