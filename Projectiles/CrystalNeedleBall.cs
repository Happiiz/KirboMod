using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class CrystalNeedleBall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 102;
            Projectile.height = 102;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true; //wait for no one
            Projectile.localNPCHitCooldown = 5;
        }
        public override void AI()
        {

            Projectile.ai[0]++;
            if (Projectile.ai[0] % 10 == 0 && Projectile.velocity.Y == 0) //every 10 ticks and on the ground
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Bottom.X, Projectile.Bottom.Y - 16, Projectile.velocity.X * 0.01f, Projectile.velocity.Y * 0.01f, ModContent.ProjectileType<CrystalTrap>(), Projectile.damage * 3 / 4, 1, Projectile.owner, 0, 0, 0);
            }

            //Gravity
            Projectile.velocity.Y = Projectile.velocity.Y + 0.6f;
            if (Projectile.velocity.Y >= 16f)
            {
                Projectile.velocity.Y = 16f;
            }

            //Rotation
            Projectile.rotation += Projectile.velocity.X * 0.02f;

            if (Projectile.velocity.Y == 0)
            {
                Projectile.velocity.X *= 0.992f; //slow down
            }

            if (Main.rand.NextBool(3)) // happens 1/3 times
            {
                int dustnumber = Dust.NewDust(Projectile.position, 102, 102, ModContent.DustType<Dusts.RainbowSparkle>(), 0f, 0f, 200, default, 1f); //dust
                Main.dust[dustnumber].velocity *= 0.3f;
            }

            //for stepping up tiles
            Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
        }

        public override void OnKill(int timeLeft) //when the projectile dies
        {
            //crystal
            for (int i = 0; i < 30; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center, 91, speed * 3, 0, new Color(Main.rand.Next(0, 255), Main.rand.Next(0, 255), Main.rand.Next(0, 255)), Scale: 1.5f); //Makes dust in a messy circle
            }
            //crystal clutter projectiles
            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Projectile.direction * 10f, Main.rand.Next(-10, 0), ModContent.ProjectileType<Projectiles.CrystalClutter>(), Projectile.damage, 10f, Projectile.owner, 0, 0);
            }

            SoundEngine.PlaySound(SoundID.Item27, Projectile.Center); //crystal break
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X) //bounce
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            return false; //dont die
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false; //don't fall through platforms
            return true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; // Makes it uneffected by light
        }
    }
}