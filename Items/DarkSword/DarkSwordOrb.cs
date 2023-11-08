using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Items.DarkSword
{
    internal class DarkSwordOrb : ModProjectile
    {
        public override string Texture => "KirboMod/Projectiles/GoodDarkOrb";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.penetrate = 30;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        ref float Timer { get => ref Projectile.ai[0]; }
        public override void AI()
        {
            Timer++;
            Projectile.alpha -= 51;
            if(Timer > 1200)
                Projectile.Kill();
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.DarkResidue>(), Projectile.velocity.X * 0.3f, Projectile.velocity.Y * 0.3f, 200, default, 0.8f); //dust
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if(oldVelocity.Y != Projectile.velocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            if(oldVelocity.X != Projectile.velocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            Projectile.penetrate--;
            return false;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
            {
                Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 10, 10); //Makes dust in a messy circle
                d.noGravity = true;
            }
        }
    }
}
